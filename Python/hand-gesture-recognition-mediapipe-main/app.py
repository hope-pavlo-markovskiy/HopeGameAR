import threading

from CameraFeed import camera_feed

from humanSegmentation import humanSegmentation
from HandProcessor import handProcessor
from HandGestureTracker import handGestureTracker


#########################################
from trackDataTags import Tag

#data sock.recv()
#tags = (Tag)data  <- int of TrackDataReader.Tags
############################################


def human_segmentation():
    humanSegmentation.start(camera_feed.get_frame())


def hand_lm():
    handProcessor.start(camera_feed.get_frame())


def hand_gesture():
    handGestureTracker.start()


thread = threading.Thread(target=human_segmentation)
thread.start()
# thread1 = threading.Thread(target=hand_lm)
# thread1.start()
thread2 = threading.Thread(target=hand_gesture)
thread2.start()


while True:
    image = camera_feed.get_frame()
    humanSegmentation.setImg(image)
    #handProcessor.setImg(image)
    handGestureTracker.setImg(image)

    # if humanSegmentation.buffer is not None:
    #     camFeedSocket.SendImgBuffer(humanSegmentation.buffer)

    #trackDataSocket.SendStr(str(handProcessor.trackData))
    #trackDataSocket.SendStr(str(handGestureTracker.trackData))