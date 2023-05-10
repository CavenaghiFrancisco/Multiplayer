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
    PlayerList = 2,
    Disconnect = 3,
    StayAlive = 4,
    Request = 5,
    Reconnect = 6
}

public interface IMessage<T>
{
    public MessageType GetMessageType();

    public byte[] Serialize();

    public T Deserialize(byte[] message);
}