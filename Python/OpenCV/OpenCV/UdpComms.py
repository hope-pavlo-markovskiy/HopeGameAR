import PIL.Image as Image
import cv2
import numpy as np
import socket
import errno
import sys


class UdpComms():
    def __init__(self, ip, sendPort, receivePort, enableReceiver = False, suppressWarnings = True):
        self.ip = ip
        self.sendPort = sendPort
        self.receivePort = receivePort
        self.enableReceiver = enableReceiver
        self.suppressWarnings = suppressWarnings # when true warnings are suppressed
        self.isDataReceived = False
        self.receivedData = None

        ## Innit Connection
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
        self.sock.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1) # allows the address/port to be reused immediately instead of it being stuck in the TIME_WAIT state waiting for late packets to arrive.
        self.sock.bind((ip, receivePort))
        self.sock.setblocking(0)

        # Create Receiving thread if required
        if enableReceiver:
            # Create receiving thread to prevent blocking
            import threading
            
            self.receiveThread = threading.Thread(
                target = self.ReadUdpThreadFunc,
                daemon = True)
            self.receiveThread.start()


    def CloseSocket(self):
        self.sock.close()

    def __del__(self):
        self.CloseSocket()


    def SendData(self, strToSend):
        self.sock.sendto(bytes(strToSend,'utf-8'), (self.ip, self.sendPort))


    def ReceiveDataAsStr(self):
        if not self.enableReceiver: # if RX is not enabled, raise error
            raise ValueError("Attempting to receive data without enabling this setting. Ensure this is enabled from the constructor")

        data = None
        try:
            data, _ = self.sock.recvfrom(1024)
            data = data.decode('utf-8')
        except WindowsError as e:
            if e.winerror == 10054: # An error occurs if you try to receive before connecting to other application
                if not self.suppressWarnings:
                    print("Are You connected to the other application? Connect to it!")
                else:
                    pass
            else:
                raise ValueError("Unexpected Error. Are you sure that the received data can be converted to a string")

        return data
    

    def ReceiveDataAsImg(self):
        if not self.enableReceiver: # if RX is not enabled, raise error
            raise ValueError("Attempting to receive data without enabling this setting. Ensure this is enabled from the constructor")

        ## Receive Cam Feed
        frameBytes = b''
        while True:
            try:
                chunk, _ = self.sock.recvfrom(5766655777)  # Use recvfrom() for UDP
                frameBytes += chunk
            except socket.error as e:
                err = e.args[0]
                # No chunk received.. break receive loop
                if err == errno.EAGAIN or err == errno.EWOULDBLOCK:
                    #print(err)
                    break
                # Fatal error
                else:
                    print(err)
                    sys.exit(1)

        camFrame = None
        if frameBytes != b'':
            npArr = np.frombuffer(frameBytes, np.uint8)
            camFrame = cv2.imdecode(npArr, cv2.IMREAD_COLOR)
        return camFrame


    def ReadUdpThreadFunc(self): # Should be called from thread
        self.isDataReceived = False # Initially nothing received
        while True:
            self.receivedData = self.ReceiveDataAsImg() # Blocks (in thread) until data is returned (OR MAYBE UNTIL SOME TIMEOUT AS WELL)
            self.isDataReceived = True # Data received is available


    def ReadReceivedData(self):
        data = None

        if self.isDataReceived: # if data has been received
            data = self.receivedData
            
            self.receivedData = None # Empty receive buffer
            self.isDataReceived = False

        return data