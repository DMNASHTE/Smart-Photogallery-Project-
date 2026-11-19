import os

BASE_DIR = os.path.dirname(os.path.abspath(__file__))
MODEL_DIR = os.path.join(BASE_DIR, "models")
YUNET_MODEL = os.path.join(MODEL_DIR, "face_detection_yunet_2023mar.onnx")
SFACE_MODEL = os.path.join(MODEL_DIR, "face_recognition_sface_2021dec.onnx")

MYSQL = {
    "host": "127.0.0.1",
    "port": 3306,
    "user": "gallery_user",
    "password": "CHANGE_ME",
    "database": "smart_photo_gallery"
}
MODEL_NAME = "OpenCV-YuNet-SFace"

