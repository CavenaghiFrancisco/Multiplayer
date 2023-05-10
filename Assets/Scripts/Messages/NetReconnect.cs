using System;
using System.Collections.Generic;

public class NetReconnect : IMessage<string>
{
    int id = NetworkManager.Instance.ownId;
    String data;


    public NetReconnect(String data)
    {
        this.data = data;
    }

    public String Deserialize(byte[] message)
    {
        String outData = "";

        for (int i = 12; i < message.Length; i += sizeof(char))
            outData += BitConverter.ToChar(message, i);

        return outData;
    }

    public MessageType GetMessageType()
    {
        return MessageType.Reconnect;
    }

    public byte[] Serialize()
    {
        List<byte> outData = new List<byte>();

        outData.AddRange(BitConverter.GetBytes((int)GetMessageType()));
        outData.AddRange(BitConverter.GetBytes(id));

        for (int i = 0; i < data.Length; i++)
        {
            outData.AddRange(BitConverter.GetBytes(data[i]));
        }

        outData.AddRange(BitConverter.GetBytes(outData.Count * 3));

        return outData.ToArray();
    }
}
