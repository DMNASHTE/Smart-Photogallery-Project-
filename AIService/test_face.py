
import cv2
from config import YUNET_MODEL, SFACE_MODEL

photos  = [
    r"D:\PhotoArchive\Photos\PhotoArchive\2025\CulturalEvent\20200119_063722.jpg",
    r"D:\PhotoArchive\Photos\PhotoArchive\2026\AnnualFunction\images\20221028_173953.jpg"
]


detector = cv2.FaceDetectorYN_create(YUNET_MODEL, "", (320, 320))
recognizer = cv2.FaceRecognizerSF_create(SFACE_MODEL, "")

for photo_path in photos:
    print("\nTesting:", photo_path)

    image = cv2.imread(photo_path)

    if image is None:
        print("ERROR: Photo not found or could not be opened.")
        continue

    height, width = image.shape[:2]
    detector.setInputSize((width, height))

    _, faces = detector.detect(image)

    if faces is None or len(faces) == 0:
        print("No faces detected.")
        continue

    print("Faces detected:", len(faces))

    for index, face in enumerate(faces):
        aligned_face = recognizer.alignCrop(image, face)
        feature = recognizer.feature(aligned_face)
        print("Face", index + 1, "embedding shape:", feature.shape)

print("\nFace detection test completed.")



