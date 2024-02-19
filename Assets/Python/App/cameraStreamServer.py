import cv2
import socket
import struct

# Create a TCP socket
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
host_ip = "127.0.0.1"  # Replace with your server's IP address
port = 9999
socket_address = (host_ip, port)

# Bind the socket to the address
server_socket.bind(socket_address)

# Listen for incoming connections
server_socket.listen(5)

print(f"Listening on {host_ip}:{port}")

# Accept a connection from a client
client_socket, addr = server_socket.accept()
print(f"Connection from {addr}")

# Open the camera
cap = cv2.VideoCapture(0)
cap.set(3, 1280)
cap.set(4, 720)

while True:
    # Read a frame from the camera
    ret, frame = cap.read()

    # Compress the frame to JPEG format
    _, buffer = cv2.imencode(".jpg", frame)

    # Get the size of the compressed frame
    size = len(buffer)

    # Pack the frame size and send it in big-endian format (fix issue of incorrect byte order (little-endian instead of big-endian)
    client_socket.sendall(struct.pack("!L", size))

    # Send the compressed frame data
    client_socket.sendall(buffer.tobytes())

# Release resources
cap.release()
server_socket.close()