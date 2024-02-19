import copy

import cv2 as cv

import mediapipe as mp
from trackDataTags import Tag

from TCPSocket import trackDataSender

import time

MAX_NUM_HANDS = 2

#MIN_DETECTION_CONFIDENCE = 0.175
#MIN_TRACKING_CONFIDENCE = 0.115
MIN_DETECTION_CONFIDENCE = 0.5
MIN_TRACKING_CONFIDENCE = 0.2


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
                    handedness_id = 0
                    if handedness.classification[0].label[0:] == 'Left':
                        handedness_id = 0
                    else:
                        handedness_id = 1
                    self.allHandedness.append([handedness_id])

                trackDataSender.SendStr(str((Tag.HAND_LM.value, self.allLm)))
                trackDataSender.SendStr(str((Tag.HAND_HANDEDNESS.value, self.allHandedness)))


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

            landmark_point.append([landmark_x, landmark_y, landmark_z])

        return landmark_point

    def GetResult(self, img):
        return self.hands.process(img)


handProcessor = HandProcessor()