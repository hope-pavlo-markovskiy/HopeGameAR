using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.IO;

public class CamFeedReceiver : TcpReceiver
{
    protected override int PORT => 8000;

    public event Action<byte[]> OnReceived;

    public static CamFeedReceiver Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    protected override void ReadData()
    {
        // First receive image size then data
        int frameSize = (int)ReadUInt32BigEndian(binaryReader);
        byte[] frameBytes = binaryReader.ReadBytes(frameSize);

        // Call on main thread
        UnityMainThreadDispatcher.Instance().Enqueue(
            () => OnReceived?.Invoke(frameBytes)); // CamDisplay.cs is listening for frames to display


        uint ReadUInt32BigEndian(BinaryReader reader)
        {
            byte[] bytes = reader.ReadBytes(4);
            Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }
    }
}