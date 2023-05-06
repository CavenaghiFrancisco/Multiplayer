using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetHandShake : IMessage<(long, int)>
{
    (long, int) data;


    public NetHandShake() { }

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
        outData.AddRange(BitConverter.GetBytes(outData.Count * 3));

        return outData.ToArray();
    }
}
