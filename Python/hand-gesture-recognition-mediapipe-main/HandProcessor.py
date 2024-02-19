import mediapipe as mp
from trackDataTags import Tag

from TCPSocket import trackDataSocket


NUM_PLAYERS = 5
MIN_DETECTION_CONFIDENCE = 0.7
MIN_TRACKING_CONFIDENCE = 0.5


class HandProcessor:
    def __init__(self):
        from collections import namedtuple

        mp_hands = mp.solutions.hands
        self.hands = mp_hands.Hands(
            static_image_mode=True,
            max_num_hands=5 * 2,
            min_detection_confidence=MIN_DETECTION_CONFIDENCE,
            min_tracking_confidence=MIN_TRACKING_CONFIDENCE,
        )
        self.frame = None
        self.trackData = []
        self.results = None

    def setImg(self, img):
        self.frame = img

    def start(self, img):
        self.setImg(img)

        while True:
            self.results = self.hands.process(self.frame)

            # trackDataTemp = []
            # if self.results.multi_hand_landmarks is not None:
            #     for hand_landmarks, handedness in zip(self.results.multi_hand_landmarks,
            #                                           self.results.multi_handedness):
            #         # Landmark calculation
            #         landmark_list = self.calc_landmark_list(self.frame, hand_landmarks)
            #
            #         handInfo = []
            #
            #         fixedLm = []
            #         for lm in landmark_list:
            #             fixedLm.append([
            #                 lm[0],  # x
            #                 1080 - lm[1]])  # Flip y direction so it starts from bottom to top for unity
            #         handInfo.append((Tag.HAND_LANDMARKS.value, fixedLm))
            #
            #         trackDataTemp.append(handInfo)
            #
            # self.trackData = trackDataTemp

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


    def GetResult(self, img):
        return self.hands.process(img)


handProcessor = HandProcessor()