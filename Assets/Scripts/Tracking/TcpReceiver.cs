using System;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;

public abstract class TcpReceiver : TcpSocket
{
    protected BinaryReader binaryReader;

    public bool active = true;

    protected abstract void ReadData();

    protected override void ListenForConnection()
    {
        base.ListenForConnection();

        binaryReader = new(stream);
    }

    protected override void StreamThreadStart()
    {
        base.StreamThreadStart();

        // Keep receiving data until functionality to stop
        while (active)
        {
            // Handle exception errors
            try
            {
                ReadData();
            }
            catch (Exception err)
            {
                Debug.LogWarning(err.ToString());
            }
        }
    }

    // Properly end communication
    protected override void EndComms()
    {
        base.EndComms();

        binaryReader?.Close();
    }
}