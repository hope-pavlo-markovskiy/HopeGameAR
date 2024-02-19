using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public abstract class TcpSocket : MonoBehaviour
{
    const string IP = "127.0.0.1"; // Local host
    protected abstract int PORT { get; }

    Thread streamThread;

    TcpListener listener;
    protected TcpClient client;
    protected NetworkStream stream;

    void Start()
    {
        // Create receiving thread to prevent blocking
        StartThread(ref streamThread, new(StreamThreadStart));


        void StartThread(ref Thread t, ThreadStart ts)
        {
            t = new Thread(ts);
            t.IsBackground = true;
            t.Start();
        }
    }

    protected virtual void StreamThreadStart()
    {
        ListenForConnection();
    }

    protected virtual void ListenForConnection()
    {
        // Listen for connection
        listener = new TcpListener(IPAddress.Parse(IP), PORT);
        listener.Start();

        // Connect to Python program
        client = listener.AcceptTcpClient();
        stream = client.GetStream();
    }

    // Properly end communication
    protected virtual void EndComms()
    {
        streamThread?.Abort();
        listener?.Stop();
        client?.Close();
        stream?.Close();
    }

    private void OnDestroy() => EndComms();
    private void OnApplicationQuit() => EndComms();
}