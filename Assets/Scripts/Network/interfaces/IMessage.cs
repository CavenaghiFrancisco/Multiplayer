using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public enum MessageType
{
    Console = 0,
    Position = 1
}

public interface IMessage<T>
{
    public MessageType GetMessageType();

    public byte[] Serialize();

    public T Deserialize(byte[] message);
}

public class NetVector3 : IMessage<UnityEngine.Vector3>
{
    Vector3 data; 

    public NetVector3(Vector3 data)
    {
        this.data = data;
    }

    public Vector3 Deserialize(byte[] message)
    {
        Vector3 outData;

        outData.x = BitConverter.ToSingle(message, 4);
        outData.y = BitConverter.ToSingle(message, 8);
        outData.z = BitConverter.ToSingle(message, 12);

        return outData;
    }

    public MessageType GetMessageType()
    {
        return MessageType.Position;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));

        outData.AddRange(BitConverter.GetBytes(data.x));
        outData.AddRange(BitConverter.GetBytes(data.y));
        outData.AddRange(BitConverter.GetBytes(data.z));

        return outData.ToArray();
    }
}


public class NetString : IMessage<String>
{
    String data;

    public NetString(String data)
    {
        this.data = data;
    }

    public String Deserialize(byte[] message)
    {
        String outData;

        outData = BitConverter.ToString(message,4);

        return outData;
    }

    public MessageType GetMessageType()
    {
        return MessageType.Console;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));

        for(int i = 0; i < data.Length; i++)
        {
            outData.AddRange(BitConverter.GetBytes(data[i]));
        }

        return outData.ToArray();
    }
}
