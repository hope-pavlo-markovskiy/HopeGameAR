using System;
using System.IO;
using System.Text;
using UnityEngine;

public abstract class TcpSender : TcpSocket
{
    protected BinaryWriter binaryWriter;

    public bool receiving = true;

    protected override void ListenForConnection()
    {
        base.ListenForConnection();

        binaryWriter = new(stream);
    }

    protected void SendData(byte[] bytes)
    {
        try
        {
            stream.Write(bytes, 0, bytes.Length);
        }
        catch (Exception err)
        {
            Debug.LogWarning(err.ToString());
        }
    }

    protected void SendData(string str) => SendData(Encoding.ASCII.GetBytes(str));

    protected override void EndComms()
    {
        base.EndComms();

        binaryWriter?.Close();
    }
}
