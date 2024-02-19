import socket
import struct

# Communication Vars
IP = "127.0.0.1"  # Local
# MAKE SURE THE PORTS AREN'T BEING USED BY OTHER PROCESSES!
CAM_FEED_PORT, TRACK_DATA_PORT = 8000, 8001


class TCPSocket:
    def __init__(self, IP, PORT):
        self.sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.sock.connect((IP, PORT))

    def Send(self, data):
        self.sock.sendall(data)  # Send the compressed frame data


class CamFeedSocket(TCPSocket):
    def SendImgBuffer(self, buffer):
        # Pack the frame size and send it in big-endian format (fix issue of incorrect byte order (little-endian instead of big-endian)
        self.Send(struct.pack("!L", len(buffer)))
        self.Send(buffer.tobytes())  # Send the compressed frame data


class TrackDataSocket(TCPSocket):
    def SendStr(self, str):
        self.Send(str.encode())


camFeedSocket = CamFeedSocket(IP, CAM_FEED_PORT)
trackDataSocket = TrackDataSocket(IP, TRACK_DATA_PORT)
