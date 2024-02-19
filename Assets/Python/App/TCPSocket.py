import socket
import struct
import threading
import time

# Communication Vars
IP = "127.0.0.1"  # Local
CAM_FEED_PORT, TRACK_DATA_PORT, REQUEST_PORT = 8000, 8001, 8002


# Class for establishing a TCP socket connection
class TCPSocket:
    def __init__(self, IP, PORT):
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.sock.connect((IP, PORT))

    def Send(self, data):
        time.sleep(0.01)
        self.sock.sendall(data)  # Send the compressed frame data


# Class to send image buffer over TCP
class CamFeedSender(TCPSocket):
    def SendImgBuffer(self, buffer):
        # Pack the frame size and send it in big-endian format
        self.Send(struct.pack("!L", len(buffer)))
        self.Send(buffer.tobytes())  # Send the compressed frame data


# Class to send string data over TCP
class TrackDataSender(TCPSocket):
    def SendStr(self, str):
        self.Send(str.encode())


camFeedSender = CamFeedSender(IP, CAM_FEED_PORT)
trackDataSender = TrackDataSender(IP, TRACK_DATA_PORT)
