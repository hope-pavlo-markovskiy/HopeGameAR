import csv
import copy
import itertools

from model import KeyPointClassifier

from trackDataTags import Tag
from TCPSocket import trackDataSender

import time


class HandGestureTracker:
    def __init__(self):
        self.frame = None
        self.handResults = None

    def setImg(self, img, handResults):
        self.frame = img
        self.handResults = handResults

    def start(self):
        keypoint_classifier = KeyPointClassifier()

        #  ########################################################################
        while True:
            if self.frame is None or self.handResults is None:
                continue

            debug_image = self.frame

            #debug_image = copy.deepcopy(self.frame)

            # Detection implementation #############################################################
            #self.frame = cv.cvtColor(self.frame, cv.COLOR_BGR2RGB)

            # self.frame.flags.writeable = False
            # results = handProcessor.GetResult(self.frame)
            # self.frame.flags.writeable = True

            #  ####################################################################
            trackData = []
            if self.handResults.multi_hand_landmarks is not None:
                for hand_landmarks, handedness in zip(self.handResults.multi_hand_landmarks,
                                                      self.handResults.multi_handedness):
                    # Landmark calculation
                    landmark_list = self.calc_landmark_list(debug_image, hand_landmarks)

                    # Conversion to relative coordinates / normalized coordinates
                    pre_processed_landmark_list = self.pre_process_landmark(
                        landmark_list)

                    # Hand sign classification
                    hand_sign_id = keypoint_classifier(pre_processed_landmark_list)

                    # Save track data
                    handInfo = []
                    handInfo.append(hand_sign_id)

                    trackData.append(handInfo)

                # Send Data
                trackDataSender.SendStr(str((Tag.HAND_GESTURE.value, trackData)))


            time.sleep(0.001)

    def calc_landmark_list(self, image, landmarks):
        image_width, image_height = image.shape[1], image.shape[0]

        landmark_point = []

        # Keypoint
        for _, landmark in enumerate(landmarks.landmark):
            landmark_x = min(int(landmark.x * image_width), image_width - 1)
            landmark_y = min(int(landmark.y * image_height), image_height - 1)
            # landmark_z = landmark.z

            landmark_point.append([landmark_x, landmark_y])

        return landmark_point

    def pre_process_landmark(self, landmark_list):
        temp_landmark_list = copy.deepcopy(landmark_list)

        # Convert to relative coordinates
        base_x, base_y = 0, 0
        for index, landmark_point in enumerate(temp_landmark_list):
            if index == 0:
                base_x, base_y = landmark_point[0], landmark_point[1]

            temp_landmark_list[index][0] = temp_landmark_list[index][0] - base_x
            temp_landmark_list[index][1] = temp_landmark_list[index][1] - base_y

        # Convert to a one-dimensional list
        temp_landmark_list = list(
            itertools.chain.from_iterable(temp_landmark_list))

        # Normalization
        max_value = max(list(map(abs, temp_landmark_list)))

        def normalize_(n):
            return n / max_value

        temp_landmark_list = list(map(normalize_, temp_landmark_list))

        return temp_landmark_list


handGestureTracker = HandGestureTracker()