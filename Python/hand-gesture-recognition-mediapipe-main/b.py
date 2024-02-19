import cv2
import socket
import pickle
import struct
import numpy as np

# Create a socket object
client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
client_socket.connect(('localhost', 9999))

data = b""
payload_size = struct.calcsize("L")

while True:
    while len(data) < payload_size:
        packet = client_socket.recv(4 * 1024)
        if not packet:
            break
        data += packet

    packed_msg_size = data[:payload_size]
    data = data[payload_size:]
    msg_size = struct.unpack("L", packed_msg_size)[0]

    while len(data) < msg_size:
        data += client_socket.recv(4 * 1024)

    frame_data = data[:msg_size]
    data = data[msg_size:]

    # Deserialize the frame
    frame = pickle.loads(frame_data)

    # Convert the frame to a numpy array
    nparr = np.frombuffer(frame, np.uint8)
    # Decode the frame
    img_decode = cv2.imdecode(nparr, cv2.IMREAD_COLOR)

    # Display the received frame
    cv2.imshow('Client 1', img_decode)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cv2.destroyAllWindows()
