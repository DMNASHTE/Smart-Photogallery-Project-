
from flask import Flask, request, jsonify
from face_engine import FaceEngine
from config import MYSQL, MODEL_NAME, PHOTO_ROOT
from datetime import datetime
from urllib.parse import quote
import mysql.connector
import numpy as np
import os

app = Flask(__name__)
_engine = None


def get_engine():
    global _engine
    if _engine is None:
        _engine = FaceEngine()
    return _engine


def get_db_connection():
    return mysql.connector.connect(**MYSQL)


@app.route("/health", methods=["GET"])
def health():
    connection = None

    try:
        connection = get_db_connection()

        cursor = connection.cursor()
        cursor.execute("SELECT DATABASE()")
        database_name = cursor.fetchone()[0]
        cursor.close()

        return jsonify({
            "status": "ok",
            "service": "SmartPhotoGallery AI",
            "models_loaded": _engine is not None,
            "database_status": "connected",
            "database_name": database_name
        })

    except mysql.connector.Error as exc:
        return jsonify({
            "status": "error",
            "service": "SmartPhotoGallery AI",
            "database_status": "disconnected",
            "error": str(exc)
        }), 503

    finally:
        if connection is not None and connection.is_connected():
            connection.close()

@app.route("/analyze", methods=["POST"])
def analyze():
    body = request.get_json(silent=True) or {}
    path = body.get("image_path", "")

    if not path or not os.path.isfile(path):
        return jsonify({"error": "Provide an existing local image_path"}), 400

    try:
        faces = get_engine().process_image(path)

        return jsonify({
            "model": MODEL_NAME,
            "face_count": len(faces),
            "faces": faces
        })

    except Exception as exc:
        return jsonify({"error": str(exc)}), 500


@app.route("/compare", methods=["POST"])
def compare():
    body = request.get_json(silent=True) or {}

    try:
        score = get_engine().cosine_similarity(
            body["embedding1"],
            body["embedding2"]
        )

        return jsonify({"similarity": score})

    except Exception as exc:
        return jsonify({"error": str(exc)}), 400


# NEW API: Register a person
@app.route("/register", methods=["POST"])
def register_person():

    body = request.get_json(silent=True) or {}

    person_name = body.get("person_name", "").strip()
    designation = body.get("designation", "").strip()
    image_path = body.get("image_path", "").strip()

    if not person_name:
        return jsonify({"error": "Person name is required"}), 400

    if len(person_name) > 200:
        return jsonify({"error": "Person name must be 200 characters or less"}), 400

    if len(designation) > 200:
        return jsonify({"error": "Designation must be 200 characters or less"}), 400

    if not image_path or not os.path.isfile(image_path):
        return jsonify({"error": "Valid reference photo path is required"}), 400

    connection = None
    cursor = None

    try:
        # Detect and extract face embedding
        faces = get_engine().process_image(image_path)

        if len(faces) == 0:
            return jsonify({"error": "No face detected in reference photo"}), 400

        if len(faces) > 1:
            return jsonify({
                "error": "Reference photo must contain exactly one face"
            }), 400

        embedding = np.asarray(
            faces[0]["embedding"],
            dtype=np.float32
        )

        embedding_bytes = embedding.tobytes()

        # Connect to MySQL
        connection = get_db_connection()
        cursor = connection.cursor()

        # Save person details
        cursor.execute(
            """
            INSERT INTO persons
            (PersonName, Designation, ReferencePhotoPath)
            VALUES (%s, %s, %s)
            """,
            (person_name, designation or None, image_path)
        )

        person_id = cursor.lastrowid

        # Save face embedding
        cursor.execute(
            """
            INSERT INTO personfaces
            (PersonId, Embedding, ModelName)
            VALUES (%s, %s, %s)
            """,
            (person_id, embedding_bytes, MODEL_NAME)
        )

        connection.commit()

        return jsonify({
            "status": "success",
            "message": "Person registered successfully",
            "person_id": person_id,
            "person_name": person_name,
            "designation": designation,
            "model": MODEL_NAME
        }), 201

    except Exception as exc:

        if connection is not None:
            connection.rollback()

        return jsonify({"error": str(exc)}), 500

    finally:

        if cursor is not None:
            cursor.close()

        if connection is not None and connection.is_connected():
            connection.close()


IMAGE_EXTENSIONS = {".jpg", ".jpeg", ".png", ".bmp"}


def get_archive_events():
    """Find year/event folders. Nested image folders remain part of their event."""
    if not os.path.isdir(PHOTO_ROOT):
        raise RuntimeError("Photo archive folder not found: " + PHOTO_ROOT)

    for year_name in sorted(os.listdir(PHOTO_ROOT)):
        year_path = os.path.join(PHOTO_ROOT, year_name)

        if not os.path.isdir(year_path) or not year_name.isdigit():
            continue

        for event_name in sorted(os.listdir(year_path)):
            event_path = os.path.join(year_path, event_name)

            if os.path.isdir(event_path):
                yield year_name, event_name, event_path


def get_archive_images(event_path):
    """Return every supported image inside an event, including nested folders."""
    for current_root, directories, filenames in os.walk(event_path):
        directories.sort()

        for filename in sorted(filenames):
            extension = os.path.splitext(filename)[1].lower()

            if extension in IMAGE_EXTENSIONS:
                yield os.path.join(current_root, filename)

@app.route("/test-route", methods=["GET"])
def test_route():
    return jsonify({
        "message": "Correct app.py is running",
        "scanner": "Bulk scanner file loaded"
    })
@app.route("/scan-archive", methods=["POST"])


def scan_archive():
    connection = None
    cursor = None

    report = {
        "status": "started",
        "archive": PHOTO_ROOT,
        "events_found": 0,
        "photos_found": 0,
        "photos_indexed": 0,
        "photos_skipped": 0,
        "faces_saved": 0,
        "errors": []
    }

    try:
        if not os.path.isdir(PHOTO_ROOT):
            return jsonify({
                "error": "Photo archive folder not found",
                "path": PHOTO_ROOT
            }), 400

        connection = get_db_connection()
        cursor = connection.cursor()

        engine = get_engine()

        for year_name, event_name, event_path in get_archive_events():

            event_relative = os.path.relpath(
                event_path, PHOTO_ROOT
            ).replace("\\", "/")

            # Find or create the event.
            cursor.execute(
                "SELECT EventId FROM events WHERE RelativePath = %s",
                (event_relative,)
            )
            event_row = cursor.fetchone()

            if event_row:
                event_id = event_row[0]
            else:
                cursor.execute(
                    """
                    INSERT INTO events
                    (YearName, EventName, RelativePath, LastScannedAt)
                    VALUES (%s, %s, %s, NOW())
                    """,
                    (year_name, event_name, event_relative)
                )
                event_id = cursor.lastrowid

            connection.commit()
            report["events_found"] += 1

            for image_path in get_archive_images(event_path):
                report["photos_found"] += 1

                relative_path = os.path.relpath(
                    image_path, PHOTO_ROOT
                ).replace("\\", "/")

                filename = os.path.basename(image_path)
                image_url = "Photo.ashx?path=" + quote(
                    relative_path, safe=""
                )

                try:
                    file_stat = os.stat(image_path)
                    file_size = file_stat.st_size
                    modified_at = datetime.fromtimestamp(
                        file_stat.st_mtime
                    )

                    cursor.execute(
                        """
                        SELECT PhotoId, FileSize, ModifiedAt, IsFaceIndexed
                        FROM photos
                        WHERE RelativePath = %s
                        """,
                        (relative_path,)
                    )

                    existing = cursor.fetchone()
                    photo_id = None

                    if existing:
                        photo_id = existing[0]
                        old_size = existing[1]
                        old_modified = existing[2]
                        old_indexed = existing[3]

                        unchanged = (
                            old_size == file_size
                            and old_modified is not None
                            and abs(
                                (old_modified - modified_at).total_seconds()
                            ) < 1
                        )

                        if unchanged and old_indexed:
                            report["photos_skipped"] += 1
                            continue

                    # Detect faces before replacing any previous face records.
                    detected_faces = engine.process_image(image_path)

                    if existing:
                        # Remove old matches before replacing old face records.
                        cursor.execute(
                            "DELETE FROM facematches WHERE PhotoId = %s",
                            (photo_id,)
                        )

                        cursor.execute(
                            "DELETE FROM photofaces WHERE PhotoId = %s",
                            (photo_id,)
                        )

                        cursor.execute(
                            """
                            UPDATE photos
                            SET EventId = %s,
                                FileName = %s,
                                ImageUrl = %s,
                                FileSize = %s,
                                ModifiedAt = %s,
                                IsFaceIndexed = 0
                            WHERE PhotoId = %s
                            """,
                            (
                                event_id, filename, image_url,
                                file_size, modified_at, photo_id
                            )
                        )
                    else:
                        cursor.execute(
                            """
                            INSERT INTO photos
                            (
                                EventId, FileName, RelativePath,
                                ImageUrl, FileSize, ModifiedAt,
                                IsFaceIndexed
                            )
                            VALUES (%s, %s, %s, %s, %s, %s, 0)
                            """,
                            (
                                event_id, filename, relative_path,
                                image_url, file_size, modified_at
                            )
                        )

                        photo_id = cursor.lastrowid

                    # Store one embedding for each detected face.
                    for face in detected_faces:
                        embedding_bytes = np.asarray(
                            face["embedding"],
                            dtype=np.float32
                        ).tobytes()

                        cursor.execute(
                            """
                            INSERT INTO photofaces
                            (PhotoId, FaceIndex, Embedding, ModelName)
                            VALUES (%s, %s, %s, %s)
                            """,
                            (
                                photo_id,
                                face["face_index"],
                                embedding_bytes,
                                MODEL_NAME
                            )
                        )

                    cursor.execute(
                        """
                        UPDATE photos
                        SET IsFaceIndexed = 1
                        WHERE PhotoId = %s
                        """,
                        (photo_id,)
                    )

                    connection.commit()

                    report["photos_indexed"] += 1
                    report["faces_saved"] += len(detected_faces)

                except Exception as photo_error:
                    connection.rollback()

                    report["errors"].append({
                        "photo": relative_path,
                        "error": str(photo_error)
                    })

            cursor.execute(
                """
                UPDATE events
                SET LastScannedAt = NOW()
                WHERE EventId = %s
                """,
                (event_id,)
            )
            connection.commit()

        report["status"] = "completed"
        report["error_count"] = len(report["errors"])

        return jsonify(report), 200

    except Exception as exc:
        if connection is not None:
            connection.rollback()

        return jsonify({
            "status": "failed",
            "error": str(exc),
            "report": report
        }), 500

    finally:
        if cursor is not None:
            cursor.close()

        if connection is not None and connection.is_connected():
            connection.close()


# Starting SFace cosine similarity threshold.
MATCH_THRESHOLD = 0.363


@app.route("/match-persons", methods=["POST"])
def match_persons():

    connection = None
    cursor = None

    report = {
        "status": "started",
        "registered_faces": 0,
        "archive_faces": 0,
        "matches_found": 0,
        "matches_inserted": 0,
        "matches_updated": 0,
        "threshold": MATCH_THRESHOLD,
        "errors": []
    }

    try:
        connection = get_db_connection()
        cursor = connection.cursor()

        # Load registered person embeddings.
        cursor.execute(
            """
            SELECT PersonId, Embedding
            FROM personfaces
            WHERE ModelName = %s
            """,
            (MODEL_NAME,)
        )

        registered_faces = cursor.fetchall()
        report["registered_faces"] = len(registered_faces)

        # Load all indexed archive face embeddings.
        cursor.execute(
            """
            SELECT PhotoFaceId, PhotoId, Embedding
            FROM photofaces
            WHERE ModelName = %s
            ORDER BY PhotoFaceId
            """,
            (MODEL_NAME,)
        )

        archive_faces = cursor.fetchall()
        report["archive_faces"] = len(archive_faces)

        if not registered_faces:
            return jsonify({
                "status": "error",
                "message": "No registered face embeddings found."
            }), 400

        if not archive_faces:
            return jsonify({
                "status": "error",
                "message": "No archive face embeddings found. Scan photos first."
            }), 400

        matched_photo_ids = set()

        # Compare each registered person against every archive face.
        for person_id, registered_blob in registered_faces:

            registered_embedding = np.frombuffer(
                bytes(registered_blob),
                dtype=np.float32
            )

            if registered_embedding.size != 128:
                report["errors"].append(
                    "Invalid registered embedding for PersonId "
                    + str(person_id)
                )
                continue

            for photo_face_id, photo_id, archive_blob in archive_faces:

                archive_embedding = np.frombuffer(
                    bytes(archive_blob),
                    dtype=np.float32
                )

                if archive_embedding.size != 128:
                    continue

                similarity = get_engine().cosine_similarity(
                    registered_embedding,
                    archive_embedding
                )

                if similarity < MATCH_THRESHOLD:
                    continue

                report["matches_found"] += 1
                matched_photo_ids.add(photo_id)

                # Check for an existing match to avoid duplicates.
                cursor.execute(
                    """
                    SELECT FaceMatchId
                    FROM facematches
                    WHERE PersonId = %s AND PhotoFaceId = %s
                    LIMIT 1
                    """,
                    (person_id, photo_face_id)
                )

                existing_match = cursor.fetchone()

                if existing_match:
                    cursor.execute(
                        """
                        UPDATE facematches
                        SET Similarity = GREATEST(Similarity, %s)
                        WHERE FaceMatchId = %s
                        """,
                        (similarity, existing_match[0])
                    )

                    report["matches_updated"] += 1

                else:
                    cursor.execute(
                        """
                        INSERT INTO facematches
                        (PersonId, PhotoId, PhotoFaceId, Similarity)
                        VALUES (%s, %s, %s, %s)
                        """,
                        (
                            person_id,
                            photo_id,
                            photo_face_id,
                            similarity
                        )
                    )

                    report["matches_inserted"] += 1

        connection.commit()

        report["matched_photos"] = len(matched_photo_ids)
        report["status"] = "completed"

        return jsonify(report), 200

    except Exception as exc:

        if connection is not None:
            connection.rollback()

        return jsonify({
            "status": "failed",
            "error": str(exc),
            "report": report
        }), 500

    finally:

        if cursor is not None:
            cursor.close()

        if connection is not None and connection.is_connected():
            connection.close()



@app.route("/search-person", methods=["POST"])
def search_person():

    connection = None
    cursor = None

    try:
        body = request.get_json(silent=True) or {}
        search_text = body.get("query", "").strip()

        if not search_text:
            return jsonify({
                "status": "error",
                "message": "Enter a person name or designation."
            }), 400

        connection = get_db_connection()
        cursor = connection.cursor(dictionary=True)

        search_pattern = "%" + search_text + "%"

        cursor.execute(
            """
            SELECT
                p.PersonId,
                p.PersonName,
                p.Designation,
                ph.PhotoId,
                ph.FileName,
                ph.RelativePath,
                e.YearName,
                e.EventName,
                MAX(fm.Similarity) AS Similarity,
                MAX(fm.Reviewed) AS Reviewed

            FROM persons p

            INNER JOIN facematches fm
                ON p.PersonId = fm.PersonId

            INNER JOIN photos ph
                ON fm.PhotoId = ph.PhotoId

            INNER JOIN events e
                ON ph.EventId = e.EventId

            WHERE
                p.PersonName LIKE %s
                OR COALESCE(p.Designation, '') LIKE %s

            GROUP BY
                p.PersonId,
                p.PersonName,
                p.Designation,
                ph.PhotoId,
                ph.FileName,
                ph.RelativePath,
                e.YearName,
                e.EventName

            ORDER BY
                e.YearName DESC,
                e.EventName,
                ph.FileName
            """,
            (search_pattern, search_pattern)
        )

        results = cursor.fetchall()

        return jsonify({
            "status": "success",
            "query": search_text,
            "total_photos": len(results),
            "results": results
        }), 200

    except Exception as exc:

        return jsonify({
            "status": "failed",
            "error": str(exc)
        }), 500

    finally:

        if cursor is not None:
            cursor.close()

        if connection is not None and connection.is_connected():
            connection.close()


@app.route("/sync-deleted-photos", methods=["POST"])
def sync_deleted_photos():
    connection = None
    cursor = None

    try:
        # Never clean the database if the archive drive is unavailable.
        if not os.path.isdir(PHOTO_ROOT):
            return jsonify({
                "status": "error",
                "message": "Photo archive folder is unavailable. No records deleted."
            }), 503

        root = os.path.realpath(PHOTO_ROOT)

        connection = get_db_connection()
        cursor = connection.cursor()

        cursor.execute(
            "SELECT PhotoId, RelativePath FROM photos"
        )

        photo_records = cursor.fetchall()
        deleted_count = 0
        skipped_count = 0

        for photo_id, relative_path in photo_records:

            if not relative_path:
                skipped_count += 1
                continue

            normalized = relative_path.replace("\\", "/").lstrip("/")

            full_path = os.path.realpath(
                os.path.join(root, *normalized.split("/"))
            )

            # Do not allow paths outside the photo archive.
            if os.path.commonpath([root, full_path]) != root:
                skipped_count += 1
                continue

            if not os.path.isfile(full_path):

                # Delete dependent records first.
                cursor.execute(
                    "DELETE FROM facematches WHERE PhotoId = %s",
                    (photo_id,)
                )

                cursor.execute(
                    "DELETE FROM photofaces WHERE PhotoId = %s",
                    (photo_id,)
                )

                cursor.execute(
                    "DELETE FROM photos WHERE PhotoId = %s",
                    (photo_id,)
                )

                deleted_count += 1

        connection.commit()

        return jsonify({
            "status": "success",
            "checked_photos": len(photo_records),
            "deleted_photos": deleted_count,
            "skipped_records": skipped_count,
            "message": "Deleted photo records synchronized successfully."
        })

    except Exception as exc:

        if connection is not None:
            connection.rollback()

        return jsonify({
            "status": "error",
            "message": str(exc)
        }), 500

    finally:
        if cursor is not None:
            cursor.close()

        if connection is not None and connection.is_connected():
            connection.close()


@app.route("/persons", methods=["GET"])
def get_persons():
    connection = None
    cursor = None

    try:
        connection = get_db_connection()

        cursor = connection.cursor(dictionary=True)

        cursor.execute("""
            SELECT
                p.PersonId,
                p.PersonName,
                p.Designation,
                p.ReferencePhotoPath,
                p.CreatedAt,
                COUNT(DISTINCT fm.PhotoId) AS MatchedPhotos
            FROM persons p
            LEFT JOIN facematches fm
                ON p.PersonId = fm.PersonId
            GROUP BY
                p.PersonId,
                p.PersonName,
                p.Designation,
                p.ReferencePhotoPath,
                p.CreatedAt
            ORDER BY p.PersonId DESC
        """)

        persons = cursor.fetchall()

        for person in persons:
            if person.get("CreatedAt"):
                person["CreatedAt"] = person["CreatedAt"].strftime(
                    "%Y-%m-%d %H:%M:%S"
                )

        return jsonify({
            "status": "success",
            "total_persons": len(persons),
            "persons": persons
        })

    except Exception as exc:
        return jsonify({
            "status": "error",
            "error": str(exc)
        }), 500

    finally:
        if cursor is not None:
            cursor.close()

        if connection is not None and connection.is_connected():
            connection.close()


if __name__ == "__main__":
    app.run(
        host="127.0.0.1",
        port=5005,
        debug=False
    )


