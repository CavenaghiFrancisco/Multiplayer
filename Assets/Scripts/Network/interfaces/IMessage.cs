using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

public enum MessageType
{
    HandShake = -1,
    Console = 0,
    Position = 1,
    PlayerList = 2
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
    static int instance = 0;

    public NetVector3(Vector3 data)
    {
        this.data = data;
    }

    public Vector3 Deserialize(byte[] message)
    {
        Vector3 outData;

        outData.x = BitConverter.ToSingle(message, 8);
        outData.y = BitConverter.ToSingle(message, 12);
        outData.z = BitConverter.ToSingle(message, 16);

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
        outData.AddRange(BitConverter.GetBytes(instance++));
        outData.AddRange(BitConverter.GetBytes(data.x));
        outData.AddRange(BitConverter.GetBytes(data.y));
        outData.AddRange(BitConverter.GetBytes(data.z));

        return outData.ToArray();
    }
}


public class NetString : IMessage<String>
{
    String data;
    static int instance = 0;


    public NetString(String data)
    {
        this.data = data;
    }

    public String Deserialize(byte[] message)
    {
        String outData;

        outData = BitConverter.ToString(message,8);

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
        outData.AddRange(BitConverter.GetBytes(instance++));

        for (int i = 0; i < data.Length; i++)
        {
            outData.AddRange(BitConverter.GetBytes(data[i]));
        }

        return outData.ToArray();
    }
}

public class NetHandShake : IMessage<(long, int)>
{
    (long, int) data;
    static int instance = 0;


    public NetHandShake(){}

    public NetHandShake(long ip, int port)
    {
        data.Item1 = ip;
        data.Item2 = port;
    }

    public (long, int) Deserialize(byte[] message)
    {
        data.Item1 = BitConverter.ToInt64(message, 8);
        data.Item2 = BitConverter.ToInt32(message, 16);

        return data;
    }

    public MessageType GetMessageType()
    {
        return MessageType.HandShake;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(BitConverter.GetBytes(instance++));
        outData.AddRange(BitConverter.GetBytes(data.Item1));
        outData.AddRange(BitConverter.GetBytes(data.Item2));

        Debug.Log(outData.ToArray());

        return outData.ToArray();
    }
}

public class NetPlayersList : IMessage<Dictionary<int,Client>>
{
    Dictionary<int, Client> data;
    static int instance = 0;

    public NetPlayersList() { }

    public NetPlayersList(Dictionary<int,Client> playerList)
    {
        data = playerList;
    }

    public MessageType GetMessageType()
    {
        return MessageType.PlayerList;
    }

    public byte[] Serialize()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();
        formatter.Serialize(stream, (int)GetMessageType());
        formatter.Serialize(stream, instance++); 
        formatter.Serialize(stream, data);
        stream.Close();

        return stream.ToArray();
    }

    public System.Collections.Generic.Dictionary<int, Client> Deserialize(byte[] message)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream(message);
        stream.Position = 108;
        data = (Dictionary<int, Client>)formatter.Deserialize(stream);

        return data;
    }

    
}

