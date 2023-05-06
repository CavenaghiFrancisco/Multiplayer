using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetRequest
{
    byte[] data;

    public NetRequest() { }

    public NetRequest(byte[] data)
    {
        this.data = data;
    }

    public byte[] Deserialize(byte[] message)
    {
        byte[] newData = new byte[message.Length - 4];

        Array.Copy(message, 4, data, 0, newData.Length);

        return newData;
    }

    public MessageType GetMessageType()
    {
        return MessageType.Request;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(data);

        return outData.ToArray();
    }
}
