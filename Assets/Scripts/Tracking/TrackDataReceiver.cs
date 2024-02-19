using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Text;

public class TrackDataReceiver : TcpReceiver
{
    protected override int PORT => 8001;

    public event Action<string> OnReceived;

    public static TrackDataReceiver Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    protected override void ReadData()
    {
        byte[] bytes = new byte[client.ReceiveBufferSize];
        int bytesRead = stream.Read(bytes, 0, bytes.Length); // Get bytes length to read whole data
        string data = Encoding.UTF8.GetString(bytes, 0, bytesRead); // Convert bytes to string

        // Call on main thread
        UnityMainThreadDispatcher.Instance().Enqueue(
            () => OnReceived?.Invoke(data)); // TrackDataReader.cs is listening for track data to deserialse
    }
}