import cv2
from cvzone.HandTrackingModule import HandDetector

import socket
import errno
import sys

import time
import numpy as np


## Hand Detector Vars
MAX_HANDS = 4
DETECTION_CON = 0.8 # Accuracy (0-1)

## Communication Vars
IP = "127.0.0.1" # Local
PORT = 8000 # MAKE SURE THE PORT ISN'T BEING USED BY ANOTHER PROCESS!


## Init Hand Detector
handDetector = HandDetector(
    maxHands = MAX_HANDS,
    detectionCon = DETECTION_CON)

## Init Comms
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect((IP, PORT))
sock.setblocking(0)


## Loop Process
while True:
    ## Receive Cam Feed
    frameBytes = b''
    while True:
        try:
            chunk = sock.recv(4024)
            frameBytes += chunk
        except socket.error as e:
            err = e.args[0]
            # No chunk received.. break receive loop
            if err == errno.EAGAIN or err == errno.EWOULDBLOCK:
                break
            # Fatal error
            else:
                sys.exit(1)
     # No data received.. next frame
    if frameBytes == b'':
        continue
    

    # Decode image
    npArr = np.frombuffer(frameBytes, np.uint8)
    camFrame = cv2.imdecode(npArr, cv2.IMREAD_COLOR)
    camFrameHeight, camFrameWidth, camFrameChannels = camFrame.shape
    

    ## Track hand 
    hands, camFrame = handDetector.findHands(camFrame) # Get result from image (detected hands and new image)
    handsData = [] # New list per frame
    if hands:
        for hand in hands:
            landmarkList = hand['lmList'] # Get list of 21 landmarks from dictionary

            # Remove square brackets per lm by extending to the same element per hand in list
            data = []
            for lm in landmarkList:
                data.extend([
                    lm[0], # x
                    720 - lm[1], # Flip y direction so it starts from bottom to top for unity
                    lm[2]]) # z
            handsData.append(data)

        ## Send Hand Landmarks
        sock.sendall((str(handsData)).encode())

    #cv2.imshow("Frame", camFrame) # Dispay image of title "Frame" inside the window
    cv2.waitKey(1) # Wait 1ms delay