import cv2
from cvzone.HandTrackingModule import HandDetector

import socket
import errno
import sys
from threading import Timer

import time
import numpy as np
import math


## Hand Detector Vars
MAX_HANDS = 45
DETECTION_CON = 0.8 # Accuracy (0-1)

## Hand Gesture Classification Vars
CROP_OFFSET = 20
WHITE_IMG_SIZE = 300
SAVE_WHITE_IMG_TIME = 0.5
FOLDER = "Data/Open"


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


# Matrix of ones
whiteImg = None

def SaveWhiteImg():
    cv2.imwrite(f'{FOLDER}/WhiteImg_{time.time()}.jpg', whiteImg)
    
    Timer(SAVE_WHITE_IMG_TIME, SaveWhiteImg).start()
    
Timer(SAVE_WHITE_IMG_TIME, SaveWhiteImg).start()


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
    

    ## Track hand 
    hands, camFrame = handDetector.findHands(camFrame) # Get result from image (detected hands and new image)
    handsData = [] # New list per frame
    if hands:
        hand = hands[0] # first hand
        handX, handY, handWidth, handHeight = hand['bbox'] # Get bounding box from dictionary
        
        # Crop frame around hand bounding box
        cropImg = camFrame[handY - CROP_OFFSET : handY + handHeight + CROP_OFFSET, # Down, Up
                               handX - CROP_OFFSET : handX + handWidth + CROP_OFFSET] # Left, Right   
        
        # Reset whiteImg
        whiteImg = np.ones(
            (WHITE_IMG_SIZE, WHITE_IMG_SIZE, 3), # Coloured square
            np.uint8) * 255 # Values in matrix range from 0 to 255, Start with

        ## Get Target Img
        # Resize
        if handHeight / handWidth > 1: # aspect ratio is > 1 - height is greater than width
            # Use ceiling so it always rounds up (e.g. even 3.2 would be 4)
            resizedW = math.ceil(
                (WHITE_IMG_SIZE / handHeight)
                * handWidth)
            
            imgResize = cv2.resize(cropImg, (resizedW, WHITE_IMG_SIZE))
            halfGapW = math.ceil((WHITE_IMG_SIZE - resizedW) / 2)

            # Overlay handCrop on to of handWhite
            whiteImg[:, # Height
                     halfGapW : resizedW + halfGapW] = imgResize # Width
            
        else: # width is greater than height
             # Use ceiling so it always rounds up (e.g. even 3.2 would be 4)
            resizedY = math.ceil(
                (WHITE_IMG_SIZE / handWidth)
                * handHeight)
            
            imgResize = cv2.resize(cropImg, (WHITE_IMG_SIZE, resizedY))
            halfGapH = math.ceil((WHITE_IMG_SIZE - resizedY) / 2)

            # Overlay handCrop on to of handWhite
            whiteImg[halfGapH : resizedY + halfGapH, # Height
                     :] = imgResize # Width

        cv2.imshow("Hand Crop", cropImg)
        cv2.imshow("Hand Img White", whiteImg)
        

    cv2.imshow("Frame", camFrame) # Dispay image of title "Frame" inside the window
    
    cv2.waitKey(1)