import cv2 as cv
import mediapipe as mp
import numpy as np

from cvzone.SelfiSegmentationModule import SelfiSegmentation
from TCPSocket import camFeedSocket

class HumanSegmentation:
    def __init__(self, model=1):
        self.frame = None
        self.buffer = None

        self.model = model
        self.mpDraw = mp.solutions.drawing_utils
        self.mpSelfieSegmentation = mp.solutions.selfie_segmentation
        self.selfieSegmentation = self.mpSelfieSegmentation.SelfieSegmentation(model_selection=self.model)

    def setImg(self, img):
        self.frame = img

    def start(self, img):
        self.setImg(img)

        # Segmentation function called
        segmentor = SelfiSegmentation()

        # Initial Background colour for alpha processing
        img_bg = (255, 255, 255)

        # Threshold for cut-off in segmentation mask
        cut_threshold = 0.5

        blur_radius = 15

        while True:
            # Get the current frame
            img = self.frame

            # Convert the current frame to an RGB colour format
            img_rgb = cv.cvtColor(img, cv.COLOR_BGR2RGB)

            # Process the image using MediaPipe learning model
            result = segmentor.selfieSegmentation.process(img_rgb)

            # Create a binary condition for segmentation mask based on the threshold
            condition = np.stack(
                (result.segmentation_mask,) * 4, axis=-1) > cut_threshold

            # Create a background image with the specified color
            _imgBg = np.full_like(img, img_bg, dtype=np.uint8)

            # Convert _imgBg so that it has an alpha channel
            _imgBg = cv.cvtColor(_imgBg, cv.COLOR_BGR2BGRA)

            # Set the background image alpha channel to zero (fully transparent)
            _imgBg[..., 3] = 0

            # Convert original image to have an alpha channel
            img = cv.cvtColor(img, cv.COLOR_BGR2BGRA)

            # Create the output image by blending the base input
            # image and background based on binary condition
            img_out = np.where(condition, img, _imgBg)

            # Resize image to 480p to reduce delay
            img_result = cv.resize(img_out, (640, 480))

            # seg_blur = cv.GaussianBlur(img, (blur_radius, blur_radius), 0)
            #
            # glow_effect = cv.addWeighted(img_result, 1, seg_blur, 0.7, 0)

            # Encode the output image to PNG format and store it in the buffer
            _, buffer = cv.imencode(".png", img_result)
            camFeedSocket.SendImgBuffer(buffer)

            cv.waitKey(1)


humanSegmentation = HumanSegmentation()
