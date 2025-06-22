import sys
import dlib
import math
from PIL import Image
import numpy as np

def distance(p1, p2):
    dx = p2.x - p1.x
    dy = p2.y - p1.y
    return math.sqrt(dx * dx + dy * dy)

def main(image_path, model_path):
    detector = dlib.get_frontal_face_detector()
    sp = dlib.shape_predictor(model_path)

    try:
        img = dlib.load_rgb_image(image_path)
    except Exception as e:
        print(f"[ERROR] {e}")
        return

    faces = detector(img)
    if not faces:
        print("No face detected")
        return

    shape = sp(img, faces[0])

    center = shape.part(30)
    left = shape.part(31)
    right = shape.part(34)
    radius = max(distance(center, left), distance(center, right)) * 1.2

    print(f"centerX={center.x}, centerY={center.y}, radius={radius}")

if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("Usage: python detect_nose.py [IMAGE_PATH] [shape_predictor_68_face_landmarks.dat]")
        sys.exit(1)

    image_path = sys.argv[1]
    model_path = sys.argv[2]
    main(image_path, model_path)
