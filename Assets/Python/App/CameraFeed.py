import cv2 as cv


class CameraFeed:
    def __init__(self, camera_index=0):
        # Video capture
        self.camera_index = camera_index
        self.capture = cv.VideoCapture(self.camera_index, cv.CAP_DSHOW)
        #self.capture.set(3, 1920)
        #self.capture.set(4, 1080)
        self.capture.set(cv.CAP_PROP_FRAME_WIDTH, 640)
        self.capture.set(cv.CAP_PROP_FRAME_HEIGHT, 480)
        self.capture.set(cv.CAP_PROP_FPS, 60)
        #
        # while True:
        #
        #     _, img = self.capture.read()
        #     cv.imshow("img", img)
        #
        #     cv.waitKey(1)

    # Get capture image by frame
    def get_frame(self):
        ret, frame = self.capture.read()
        return cv.flip(frame, 1)  # Mirror display
        # if ret:
        #     return frame
        # else:
        #     return None

    def release_camera(self):
        self.capture.release()

    # Get capture image from camera
    # def get_camera(self):
    #     return self.capture


camera_feed = CameraFeed()