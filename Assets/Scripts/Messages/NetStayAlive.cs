using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class NetStayAlive : IMessage<(DateTime, double)>
{
    int id = NetworkManager.Instance.ownId;
    (DateTime, double) data;

    public NetStayAlive() { }

    public NetStayAlive(DateTime dateData, double latencyData)
    {
        this.data.Item1 = dateData;
        this.data.Item2 = latencyData;
    }

    public (DateTime, double) Deserialize(byte[] message)
    {
        using (MemoryStream stream = new MemoryStream(message, 108, message.Length - 162))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            data = ((DateTime, double))formatter.Deserialize(stream);
        }

        return data;
    }

    public MessageType GetMessageType()
    {
        return MessageType.StayAlive;
    }

    public byte[] Serialize()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        MemoryStream stream = new MemoryStream();
        formatter.Serialize(stream, (int)GetMessageType());
        formatter.Serialize(stream, id);
        formatter.Serialize(stream, data);
        formatter.Serialize(stream, (int)stream.Length * 3);
        stream.Close();

        return stream.ToArray();
    }
}