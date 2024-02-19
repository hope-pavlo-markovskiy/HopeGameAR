import cv2
import socket
import pickle
import struct
import threading

# Function to handle each client separately
def handle_client(client_socket, addr):
    print(f"Connection from {addr} established.")
    encode_param = [int(cv2.IMWRITE_JPEG_QUALITY), 90] ### ADJUST QUALITY

    while True:
        ret, frame = cap.read()
        result, frame = cv2.imencode('.jpg', frame, encode_param)
        data = pickle.dumps(frame, 0)
        size = len(data)
        size_data = struct.pack("L", size)

        try:
            client_socket.sendall(size_data + data)
        except socket.error as e:
            print(f"Error sending frame to {addr}: {e}")
            break

    client_socket.close()

# Start capturing video from the camera
cap = cv2.VideoCapture(0)
# Adjust resolution
# cap.set(cv2.CAP_PROP_FRAME_WIDTH, 640)  # Adjust width
# cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 480)  # Adjust height

# Create a socket object
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind(('localhost', 9999))
server_socket.listen(5)

while True:
    # Accept a client connection
    client_socket, addr = server_socket.accept()
    # Start a new thread to handle the client
    client_thread = threading.Thread(target=handle_client, args=(client_socket, addr))
    client_thread.start()
