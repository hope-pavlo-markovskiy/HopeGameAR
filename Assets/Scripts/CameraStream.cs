using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using PimDeWitte.UnityMainThreadDispatcher;
using System;

public class CameraStream : MonoBehaviour
{
    private TcpClient client;
    private BinaryReader reader;
    private Texture2D receivedTexture;
    private RawImage rawImg;

    // Unity Start function
    void Start()
    {
        receivedTexture = new Texture2D(1280, 720);
        rawImg = GetComponent<RawImage>();

        // Replace with your server's IP address and port
        string serverIP = "127.0.0.1";
        int port = 9999;

        // Connect to the server
        ConnectToServer(serverIP, port);

        // Start a thread to receive frames from the server
        Thread receiveThread = new Thread(new ThreadStart(ReceiveFrames));
        receiveThread.Start();
    }

    // Unity Update function
    void Update()
    {
        // Display the received texture on the RawImage component
        rawImg.texture = receivedTexture;
    }

    // Connect to the server
    private void ConnectToServer(string serverIP, int port)
    {
        client = new TcpClient(serverIP, port);
        reader = new BinaryReader(client.GetStream());
    }

    // Receive frames from the server
    private void ReceiveFrames()
    {
        while (true)
        {
            // Read the frame size
            int dataSize = (int)ReadUInt32BigEndian(reader);

            // Read the frame data
            byte[] data = reader.ReadBytes(dataSize);

            // Deserialize the frame
            UnityMainThreadDispatcher.Instance().Enqueue(() => DeserializeTexture(data));
        }
    }

    // Deserialize the byte array to a Texture2D
    void DeserializeTexture(byte[] data)
    {
        receivedTexture.LoadImage(data);
    }

    // Read a 32-bit unsigned integer in big-endian format
    private uint ReadUInt32BigEndian(BinaryReader reader)
    {
        byte[] bytes = reader.ReadBytes(4);
        Array.Reverse(bytes);
        return BitConverter.ToUInt32(bytes, 0);
    }
}
