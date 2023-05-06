using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class PackageManager
{
    private static List<byte[]> lastStringMessagesReceived = new List<byte[]>();
    private static List<byte[]> lastHandShakeMessagesReceived = new List<byte[]>();
    private static List<byte[]> lastDisconnectMessagesReceived = new List<byte[]>();
    private static List<byte[]> lastPlayerListMessagesReceived = new List<byte[]>();

    private static List<byte[]> lastStringMessagesSend = new List<byte[]>();
    private static List<byte[]> lastHandShakeMessagesSend = new List<byte[]>();
    private static List<byte[]> lastDisconnectMessagesSend = new List<byte[]>();
    private static List<byte[]> lastPlayerListMessagesSend = new List<byte[]>();

    private static Dictionary<TimerOut, byte[]> messageTimers = new Dictionary<TimerOut, byte[]>();

    private static float latency;


    public static float Latency
    {
        get { return latency; }
        set { latency = value; }
    }


    public static void RequestMessage(MessageType messageType, byte[] data)
    {
        switch (messageType)
        {
            case MessageType.Console:
                if(lastStringMessagesReceived.Count > 0 && data == lastStringMessagesReceived[lastStringMessagesReceived .Count- 1])
                {
                    
                }
                break;
            case MessageType.HandShake:
                break;
            case MessageType.Disconnect:
                break;
            case MessageType.PlayerList:
                break;
            default:
                break;
        }

    }

    public static int CheckMessage(byte[] data)
    {
        int messageType = -20;
        try
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                messageType = (int)formatter.Deserialize(stream);
            }
        }
        catch
        {
            messageType = BitConverter.ToInt32(data, 0);
        }
        return messageType;
    }

    public static bool CheckTail(byte[] data)
    {
        int dataLength = data.Length;
        try
        {
            using (MemoryStream stream = new MemoryStream(data, data.Length - 54, 54))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (dataLength - 54) == (int)formatter.Deserialize(stream)/3;
            }
        }
        catch
        {
            return (dataLength) - 4 == (BitConverter.ToInt32(data, data.Length - 4) / 3);
        }
    }

    public static void AddMessageReceived(byte[] data)
    {

        switch ((MessageType)CheckMessage(data))
        {
            case MessageType.Console:
                lastStringMessagesReceived.Add(data);
                break;
            case MessageType.HandShake:
                lastHandShakeMessagesReceived.Add(data);
                break;
            case MessageType.Disconnect:
                lastDisconnectMessagesReceived.Add(data);
                break;
            case MessageType.PlayerList:
                lastPlayerListMessagesReceived.Add(data);
                break;
            default:
                break;
        }

        messageTimers.Add(new TimerOut(latency), data);

    }

    public static void AddMessageSend(byte[] data)
    {

        switch ((MessageType)CheckMessage(data))
        {
            case MessageType.Console:
                lastStringMessagesSend.Add(data);
                break;
            case MessageType.HandShake:
                lastHandShakeMessagesSend.Add(data);
                break;
            case MessageType.Disconnect:
                lastDisconnectMessagesSend.Add(data);
                break;
            case MessageType.PlayerList:
                lastPlayerListMessagesSend.Add(data);
                break;
            default:
                break;
        }

        messageTimers.Add(new TimerOut(latency), data);

    }

    public static void RemoveMessageReceived(byte[] data)
    {

        switch ((MessageType)CheckMessage(data))
        {
            case MessageType.Console:
                lastStringMessagesReceived.Remove(data);
                break;
            case MessageType.HandShake:
                lastHandShakeMessagesReceived.Remove(data);
                break;
            case MessageType.Disconnect:
                lastDisconnectMessagesReceived.Remove(data);
                break;
            case MessageType.PlayerList:
                lastPlayerListMessagesReceived.Remove(data);
                break;
            default:
                break;
        }
    }

    public static void RemoveMessageSend(byte[] data)
    {

        switch ((MessageType)CheckMessage(data))
        {
            case MessageType.Console:
                lastStringMessagesSend.Remove(data);
                break;
            case MessageType.HandShake:
                lastHandShakeMessagesSend.Remove(data);
                break;
            case MessageType.Disconnect:
                lastDisconnectMessagesSend.Remove(data);
                break;
            case MessageType.PlayerList:
                lastPlayerListMessagesSend.Remove(data);
                break;
            default:
                break;
        }
    }

    public static void CheckTimers()
    {
        foreach (KeyValuePair < TimerOut,byte[]> keyValuePair in messageTimers)
        {
            keyValuePair.Key.UpdateTimer();
            if (keyValuePair.Key.IsTimeOut())
            {
                try
                {
                    RemoveMessageReceived(keyValuePair.Value);
                }
                catch
                {
                    RemoveMessageSend(keyValuePair.Value);
                }
                messageTimers.Remove(keyValuePair.Key);
            }
        }
    }
}
