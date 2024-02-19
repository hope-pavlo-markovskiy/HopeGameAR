import mmap

import time

import cv2 as cv
import mediapipe as mp
import numpy as np

from cvzone.SelfiSegmentationModule import SelfiSegmentation


file_path = 'shared_memory.bin'
file_size = 921600

with open(file_path, 'wb') as f:
    f.seek(file_size - 1)
    f.write(b'\0')

class HumanSegmentation:
    def __init__(self, model=1):
        self.model = model
        self.mpDraw = mp.solutions.drawing_utils
        self.mpSelfieSegmentation = mp.solutions.selfie_segmentation
        self.selfieSegmentation = self.mpSelfieSegmentation.SelfieSegmentation(model_selection=self.model)

    def setImg(self, img):
        self.frame = img

    def start(self):
        # Segmentation function called
        segmentor = SelfiSegmentation()

        # Initial Background colour for alpha processing
        img_bg = (255, 255, 255)

        # Threshold for cut-off in segmentation mask
        cut_threshold = 0.5

        cap = cv.VideoCapture(0, cv.CAP_DSHOW)
        cap.set(3, 1920)  # Adjust resolution
        cap.set(4, 1080)

        with open(file_path, 'r+b') as f:
            mmapped_file = mmap.mmap(f.fileno(), 0)

            while True:
                # Get the current frame
                ret, img = cap.read()
                img = cv.flip(img, 1)  # Mirror display
                #img = cv.resize(self.frame, (640, 480))

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
                # img_result = cv.resize(img_out, (640, 480))

                # Encode the output image to PNG format and store it in the buffer
                # _, buffer = cv.imencode(".png", img_out)
                # camFeedSender.SendImgBuffer(buffer)

                result = cv.resize(img_out, (640, 360))  # Downscale the frame
                _, buffer = cv.imencode(".png", result)  # Use JPEG format
                mmapped_file.seek(0)
                mmapped_file.write(buffer.tobytes())

                time.sleep(0.001)


humanSegmentation = HumanSegmentation()

humanSegmentation.start()