using System;
using System.Collections.Generic;

public class NetDisconnect : IMessage<string>
{
    int id = NetworkManager.Instance.ownId;
    String data;
    static int instance = 1;


    public NetDisconnect(String data)
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
        return MessageType.Disconnect;
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

        outData.AddRange(BitConverter.GetBytes(outData.Count * 3));

        return outData.ToArray();
    }
}
