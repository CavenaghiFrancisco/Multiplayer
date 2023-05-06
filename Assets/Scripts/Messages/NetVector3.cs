    using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetVector3 : IMessage<UnityEngine.Vector3>
{
    int id = NetworkManager.Instance.ownId;
    Vector3 data;
    static int instance = 1;

    public NetVector3(Vector3 position)
    {
        data = position;
    }

    public Vector3 Deserialize(byte[] message)
    {
        Vector3 outData;

        outData.x = BitConverter.ToSingle(message, 12);
        outData.y = BitConverter.ToSingle(message, 16);
        outData.z = BitConverter.ToSingle(message, 20);

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
        outData.AddRange(BitConverter.GetBytes(data.x));
        outData.AddRange(BitConverter.GetBytes(data.y));
        outData.AddRange(BitConverter.GetBytes(data.z));
        outData.AddRange(BitConverter.GetBytes(outData.Count * 3));

        return outData.ToArray();
    }
}
