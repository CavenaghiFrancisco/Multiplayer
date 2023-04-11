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

public class NetVector3 : IMessage<(UnityEngine.Vector3, UnityEngine.Vector3)>
{
    int id = NetworkManager.Instance.ownId;
    (Vector3,Vector3) data;
    static int instance = 1;

    public NetVector3(Vector3 position,Vector3 movement)
    {
        data.Item1 = position;
        data.Item2 = movement;
    }

    public (Vector3, Vector3) Deserialize(byte[] message)
    {
        (Vector3,Vector3) outData;

        outData.Item1.x = BitConverter.ToSingle(message, 12);
        outData.Item1.y = BitConverter.ToSingle(message, 16);
        outData.Item1.z = BitConverter.ToSingle(message, 20);
        outData.Item2.x = BitConverter.ToSingle(message, 24);
        outData.Item2.y = BitConverter.ToSingle(message, 28);
        outData.Item2.z = BitConverter.ToSingle(message, 32);

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
        outData.AddRange(BitConverter.GetBytes(id));
        outData.AddRange(BitConverter.GetBytes(instance++));
        outData.AddRange(BitConverter.GetBytes(data.Item1.x));
        outData.AddRange(BitConverter.GetBytes(data.Item1.y));
        outData.AddRange(BitConverter.GetBytes(data.Item1.z));
        outData.AddRange(BitConverter.GetBytes(data.Item2.x));
        outData.AddRange(BitConverter.GetBytes(data.Item2.y));
        outData.AddRange(BitConverter.GetBytes(data.Item2.z));

        return outData.ToArray();
    }
}


public class NetString : IMessage<String>
{
    int id = NetworkManager.Instance.ownId;
    String data;
    static int instance = 1;


    public NetString(String data)
    {
        this.data = data;
    }

    public String Deserialize(byte[] message)
    {
        String outData = "";

        for(int i = 12; i < message.Length; i += sizeof(char))
            outData += BitConverter.ToChar(message,i);

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
        outData.AddRange(BitConverter.GetBytes(id));
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


    public NetHandShake(){}

    public NetHandShake(long ip, int port)
    {
        data.Item1 = ip;
        data.Item2 = port;
    }

    public (long, int) Deserialize(byte[] message)
    {
        data.Item1 = BitConverter.ToInt64(message, 4);
        data.Item2 = BitConverter.ToInt32(message, 12);

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
        outData.AddRange(BitConverter.GetBytes(data.Item1));
        outData.AddRange(BitConverter.GetBytes(data.Item2));

        Debug.Log(outData.ToArray());

        return outData.ToArray();
    }
}

public class NetPlayersList : IMessage<Dictionary<int,Client>>
{
    Dictionary<int, Client> data;

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
        formatter.Serialize(stream, data);
        stream.Close();

        return stream.ToArray();
    }

    public System.Collections.Generic.Dictionary<int, Client> Deserialize(byte[] message)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream(message);
        stream.Position = 54;
        data = (Dictionary<int, Client>)formatter.Deserialize(stream);

        return data;
    }

    
}

