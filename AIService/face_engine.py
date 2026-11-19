import os
import cv2
import numpy as np
from config import YUNET_MODEL, SFACE_MODEL

class FaceEngine:
    def __init__(self):
        if not os.path.isfile(YUNET_MODEL):
            raise RuntimeError("YuNet ONNX model missing: " + YUNET_MODEL)
        if not os.path.isfile(SFACE_MODEL):
            raise RuntimeError("SFace ONNX model missing: " + SFACE_MODEL)
        self.detector = cv2.FaceDetectorYN.create(YUNET_MODEL, "", (320, 320), 0.8, 0.3, 5000)
        self.recognizer = cv2.FaceRecognizerSF.create(SFACE_MODEL, "")

    def process_image(self, image_path):
        image = cv2.imread(image_path)
        if image is None:
            raise ValueError("Cannot read image: " + image_path)
        height, width = image.shape[:2]
        self.detector.setInputSize((width, height))
        _, faces = self.detector.detect(image)
        output = []
        if faces is None:
            return output
        for index, face in enumerate(faces):
            aligned = self.recognizer.alignCrop(image, face)
            feature = self.recognizer.feature(aligned).flatten().astype(np.float32)
            feature = feature / (np.linalg.norm(feature) + 1e-12)
            output.append({"face_index": index, "embedding": feature.tolist(),
                           "bbox": [float(v) for v in face[:4]]})
        return output

    @staticmethod
    def cosine_similarity(first, second):
        a = np.asarray(first, dtype=np.float32)
        b = np.asarray(second, dtype=np.float32)
        return float(np.dot(a, b) / ((np.linalg.norm(a) * np.linalg.norm(b)) + 1e-12))
