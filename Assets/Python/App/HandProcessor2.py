import mmap

import copy

import cv2 as cv

import mediapipe as mp
from trackDataTags import Tag

import time

file_path = 'hand_lm.bin'
file_size = 256564

with open(file_path, 'wb') as f:
    f.seek(file_size - 1)
    f.write(b'\0')

MAX_NUM_HANDS = 2

MIN_DETECTION_CONFIDENCE = 0.175
MIN_TRACKING_CONFIDENCE = 0.115
#MIN_DETECTION_CONFIDENCE = 0.7
#MIN_TRACKING_CONFIDENCE = 0.5


class HandProcessor():
    def __init__(self):
        mp_hands = mp.solutions.hands
        self.hands = mp_hands.Hands(
            static_image_mode=True,
            max_num_hands=MAX_NUM_HANDS,
            min_detection_confidence=MIN_DETECTION_CONFIDENCE,
            min_tracking_confidence=MIN_TRACKING_CONFIDENCE,
        )
        self.frame = None
        self.results = None
        self.allLm = []
        self.allHandedness = []
        self.active = False

    def setImg(self, img):
        self.frame = img

    def start(self):
        self.active = True

        with open(file_path, 'r+b') as f:
            mmapped_file = mmap.mmap(f.fileno(), 0)
            while self.active:
                if self.frame is None:
                    continue

                debug_image = copy.deepcopy(self.frame)

                self.frame = cv.cvtColor(self.frame, cv.COLOR_BGR2RGB)
                self.results = self.hands.process(self.frame)

                self.allLm = []
                self.allHandedness = []
                if self.results.multi_hand_landmarks is not None:
                    for hand_landmarks, handedness in zip(self.results.multi_hand_landmarks,
                                                          self.results.multi_handedness):
                        # Landmark calculation
                        landmark_list = self.calc_landmark_list(debug_image, hand_landmarks)

                        self.allLm.append(landmark_list)
                        self.allHandedness.append([handedness.classification[0].label[0:]])

                    #mapped_file.seek(0)
                    data_to_write = str((Tag.HAND_LM.value, self.allLm)).encode()
                    mmapped_file[:len(data_to_write)] = data_to_write

                time.sleep(0.001)

    def stop(self):
        self.active = False

    def calc_landmark_list(self, image, landmarks):
        image_width, image_height = image.shape[1], image.shape[0]

        landmark_point = []

        # Keypoint
        for _, landmark in enumerate(landmarks.landmark):
            landmark_x = min(int(landmark.x * image_width), image_width - 1)
            landmark_y = min(int(landmark.y * image_height), image_height - 1)
            landmark_z = landmark.z

            landmark_point.append([landmark_x, 1080 - landmark_y, landmark_z])

        return landmark_point

    def GetResult(self, img):
        return self.hands.process(img)


handProcessor = HandProcessor()