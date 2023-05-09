using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class NetPlayersList : IMessage<Dictionary<int, Client>>
{
    int id = NetworkManager.Instance.ownId;
    Dictionary<int, Client> data;

    public NetPlayersList() { }

    public NetPlayersList(Dictionary<int, Client> playerList)
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
        formatter.Serialize(stream, id);
        formatter.Serialize(stream, data);
        formatter.Serialize(stream, (int)stream.Length * 3);
        stream.Close();

        return stream.ToArray();
    }

    public System.Collections.Generic.Dictionary<int, Client> Deserialize(byte[] message)
    {
        using (MemoryStream stream = new MemoryStream(message, 108, message.Length - 162))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            data = (Dictionary<int,Client>)formatter.Deserialize(stream);
        }

        return data;
    }


}

