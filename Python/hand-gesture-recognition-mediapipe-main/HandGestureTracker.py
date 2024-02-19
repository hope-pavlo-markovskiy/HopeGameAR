import csv
import copy
import argparse
import itertools
from collections import Counter
from collections import deque

import cv2 as cv
import numpy as np

from model import KeyPointClassifier
from model import PointHistoryClassifier

from trackDataTags import Tag
from TCPSocket import trackDataSocket

from HandProcessor import handProcessor

class HandGestureTracker:
    def get_args(self):
        parser = argparse.ArgumentParser()

        parser.add_argument("--device", type=int, default=0)
        parser.add_argument("--width", help='cap width', type=int, default=1080)
        parser.add_argument("--height", help='cap height', type=int, default=920)
        # parser.add_argument("--width", help='cap width', type=int, default=960)
        # parser.add_argument("--height", help='cap height', type=int, default=540)

        args = parser.parse_args()

        return args


    def __init__(self):
        # Argument parsing #################################################################
        from collections import namedtuple

        self.frame = None
        self.handResults = namedtuple("NamedTuple",
                                      ['multi_hand_landmarks', 'multi_hand_world_landmarks', 'multi_handedness'])


        # Camera preparation ###############################################################
        # cap = cv.VideoCapture(cap_device, cv.CAP_DSHOW)
        # cap.set(cv.CAP_PROP_FRAME_WIDTH, cap_width)
        # cap.set(cv.CAP_PROP_FRAME_HEIGHT, cap_height)
        # cap.set(cv.CAP_PROP_FPS, 60)
        #cap = camera_feed.get_camera()




    def setImg(self, img):
        self.frame = img

    def start(self):
        args = self.get_args()

        cap_device = args.device
        cap_width = args.width
        cap_height = args.height



        keypoint_classifier = KeyPointClassifier()

        point_history_classifier = PointHistoryClassifier()

        # Read labels ###########################################################
        with open('model/keypoint_classifier/keypoint_classifier_label.csv',
                  encoding='utf-8-sig') as f:
            keypoint_classifier_labels = csv.reader(f)
            keypoint_classifier_labels = [
                row[0] for row in keypoint_classifier_labels
            ]
        with open(
                'model/point_history_classifier/point_history_classifier_label.csv',
                encoding='utf-8-sig') as f:
            point_history_classifier_labels = csv.reader(f)

        # Coordinate history #################################################################
        history_length = 16
        point_history = deque(maxlen=history_length)

        # Finger gesture history ################################################
        finger_gesture_history = deque(maxlen=history_length)

        #  ########################################################################
        while True:
            # Camera capture #####################################################
            # ret, image = cap.read()
            # if not ret:
            #     break
            #image = camera_feed.get_frame()

            if self.frame is None:
                continue

            # if self.handResults is None:
            #     continue

            debug_image = copy.deepcopy(self.frame)

            # Detection implementation #############################################################
            self.frame = cv.cvtColor(self.frame, cv.COLOR_BGR2RGB)

            self.frame.flags.writeable = False
            results = handProcessor.GetResult(self.frame)
            self.frame.flags.writeable = True

            #  ####################################################################
            trackData = []
            if results.multi_hand_landmarks is not None:
                for hand_landmarks, handedness in zip(results.multi_hand_landmarks,
                                                      results.multi_handedness):
                    # Landmark calculation
                    landmark_list = self.calc_landmark_list(debug_image, hand_landmarks)

                    # Conversion to relative coordinates / normalized coordinates
                    pre_processed_landmark_list = self.pre_process_landmark(
                        landmark_list)
                    pre_processed_point_history_list = self.pre_process_point_history(
                        debug_image, point_history)

                    # Hand sign classification
                    hand_sign_id = keypoint_classifier(pre_processed_landmark_list)
                    # if hand_sign_id == 2:  # Point gesture
                    if hand_sign_id == "Not applicable":  # Point gesture
                        point_history.append(landmark_list[8])
                    else:
                        point_history.append([0, 0])

                    # Finger gesture classification
                    finger_gesture_id = 0
                    point_history_len = len(pre_processed_point_history_list)
                    if point_history_len == (history_length * 2):
                        finger_gesture_id = point_history_classifier(
                            pre_processed_point_history_list)

                    # Calculates the gesture IDs in the latest detection
                    finger_gesture_history.append(finger_gesture_id)
                    most_common_fg_id = Counter(
                        finger_gesture_history).most_common()

                    # Save track data
                    handInfo = []
                    label = keypoint_classifier_labels[hand_sign_id]
                    handInfo.append((Tag.HAND_GESTURE.value, label))

                    fixedLm = []
                    for lm in landmark_list:
                        fixedLm.append([
                            lm[0],  # x
                            1080 - lm[1]])  # Flip y direction so it starts from bottom to top for unity
                    handInfo.append((Tag.HAND_LANDMARKS.value, fixedLm))

                    trackData.append(handInfo)
            else:
                point_history.append([0, 0])

            # Send Data
            trackDataSocket.SendStr(str(trackData))

            # cv.waitKey(1)
            # cv.imshow("2", debug_image)


        # cap.release()
        # cv.destroyAllWindows()


    def calc_bounding_rect(self, image, landmarks):
        image_width, image_height = image.shape[1], image.shape[0]

        landmark_array = np.empty((0, 2), int)

        for _, landmark in enumerate(landmarks.landmark):
            landmark_x = min(int(landmark.x * image_width), image_width - 1)
            landmark_y = min(int(landmark.y * image_height), image_height - 1)

            landmark_point = [np.array((landmark_x, landmark_y))]

            landmark_array = np.append(landmark_array, landmark_point, axis=0)

        x, y, w, h = cv.boundingRect(landmark_array)

        return [x, y, x + w, y + h]


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


    def pre_process_point_history(self, image, point_history):
        image_width, image_height = image.shape[1], image.shape[0]

        temp_point_history = copy.deepcopy(point_history)

        # Convert to relative coordinates
        base_x, base_y = 0, 0
        for index, point in enumerate(temp_point_history):
            if index == 0:
                base_x, base_y = point[0], point[1]

            temp_point_history[index][0] = (temp_point_history[index][0] -
                                            base_x) / image_width
            temp_point_history[index][1] = (temp_point_history[index][1] -
                                            base_y) / image_height

        # Convert to a one-dimensional list
        temp_point_history = list(
            itertools.chain.from_iterable(temp_point_history))

        return temp_point_history


handGestureTracker = HandGestureTracker()