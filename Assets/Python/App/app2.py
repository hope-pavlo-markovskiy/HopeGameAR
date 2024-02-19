#from TCPSocket import requestReceiver

#from trackDataTags import Tag

import threading

from CameraFeed import camera_feed

from humanSegmentation2 import humanSegmentation
from HandProcessor2 import handProcessor
#from HandGestureTracker import handGestureTracker

from enum import Enum


class Request(Enum):
    HUMAN_SEGMENTATION = 0
    HAND_HANDEDNESS = 1
    HAND_GESTURE = 2



import time
import cv2 as cv

threading.Thread(target=humanSegmentation.start).start()
threading.Thread(target=handProcessor.start).start()
#threading.Thread(target=handGestureTracker.start).start()


while True:
    # Read requests
    # request = requestReceiver.ReadReceivedData()
    # print(request)

    # Pass cam feed to trackers
    image = camera_feed.get_frame()

    humanSegmentation.setImg(image)
    handProcessor.setImg(image)
    #handGestureTracker.setImg(image, handProcessor.results)


    #time.sleep(0.001)
