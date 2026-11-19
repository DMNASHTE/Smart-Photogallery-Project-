from flask import Flask, request, jsonify
from face_engine import FaceEngine
import os

app = Flask(__name__)
_engine = None

def get_engine():
    global _engine
    if _engine is None:
        _engine = FaceEngine()
    return _engine

@app.route("/health", methods=["GET"])
def health():
    return jsonify({"status": "ok", "service": "SmartPhotoGallery AI",
                    "models_loaded": _engine is not None})

@app.route("/analyze", methods=["POST"])
def analyze():
    body = request.get_json(silent=True) or {}
    path = body.get("image_path", "")
    if not path or not os.path.isfile(path):
        return jsonify({"error": "Provide an existing local image_path"}), 400
    try:
        faces = get_engine().process_image(path)
        return jsonify({"model": "OpenCV-YuNet-SFace", "face_count": len(faces), "faces": faces})
    except Exception as exc:
        return jsonify({"error": str(exc)}), 500

@app.route("/compare", methods=["POST"])
def compare():
    body = request.get_json(silent=True) or {}
    try:
        score = get_engine().cosine_similarity(body["embedding1"], body["embedding2"])
        return jsonify({"similarity": score})
    except Exception as exc:
        return jsonify({"error": str(exc)}), 400

if __name__ == "__main__":
    app.run(host="127.0.0.1", port=5005, debug=False)
