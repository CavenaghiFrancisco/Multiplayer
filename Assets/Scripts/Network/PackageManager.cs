using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class PackageManager
{
    private static List<byte[]> lastStringMessagesReceived = new List<byte[]>();
    private static List<byte[]> lastDisconnectMessagesReceived = new List<byte[]>();
    private static List<byte[]> lastPlayerListMessagesReceived = new List<byte[]>();

    private static List<byte[]> lastStringMessagesSend = new List<byte[]>();
    private static List<byte[]> lastDisconnectMessagesSend = new List<byte[]>();
    private static List<byte[]> lastPlayerListMessagesSend = new List<byte[]>();

#if UNITY_SERVER
    public static Dictionary<int, Dictionary<MessageType, List<byte[]>>> lastMessagesReceivedPerClient = new Dictionary<int, Dictionary<MessageType, List<byte[]>>>();
    private static Dictionary<MessageType, List<byte[]>> lastMessagesSendServer = new Dictionary<MessageType, List<byte[]>>();
    public static Dictionary<int, Dictionary<TimerOut, byte[]>> messageTimersPerClient = new Dictionary<int, Dictionary<TimerOut, byte[]>>();
#endif

    private static Dictionary<TimerOut, byte[]> messageTimers = new Dictionary<TimerOut, byte[]>();

    const int timeOffset = 7;

#if UNITY_SERVER
    public static void ResendMessageServer(MessageType messageType, byte[] data)
    {
        if(lastMessagesSendServer.Count > 0 && lastMessagesSendServer[messageType].Count > 0)
        {
            byte[] newData = lastMessagesSendServer[messageType][lastMessagesSendServer.Count - 1];
            NetworkManager.Instance.Broadcast(newData, GetID(data));
        }
    }
#endif  

    public static void ResendMessage(MessageType messageType, byte[] data)
    {
        switch (messageType)
        {
            case MessageType.Console:
                if (lastStringMessagesSend.Count > 0)
                {
                    byte[] newData = lastStringMessagesSend[lastStringMessagesSend.Count - 1];
                    NetworkManager.Instance.SendToServer(newData);
                }
                break;
            case MessageType.Disconnect:
                if (lastDisconnectMessagesSend.Count > 0)
                {
                    byte[] newData = lastDisconnectMessagesSend[lastDisconnectMessagesSend.Count - 1];
                    NetworkManager.Instance.SendToServer(newData);
                }
                break;
            case MessageType.PlayerList:
                if (lastPlayerListMessagesSend.Count > 0)
                {
                    byte[] newData = lastPlayerListMessagesSend[lastPlayerListMessagesSend.Count - 1];
                    NetworkManager.Instance.SendToServer(newData);
                }
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

    public static int GetID(byte[] data)
    {
        int id = -20;
        try
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                stream.Position = 54;
                id = (int)formatter.Deserialize(stream);
            }
        }
        catch
        {
            id = BitConverter.ToInt32(data, 4);
        }
        return id;
    }

    public static bool CheckTail(byte[] data)
    {
        int dataLength = data.Length;
        try
        {
            using (MemoryStream stream = new MemoryStream(data, data.Length - 54, 54))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (dataLength - 54) == (int)formatter.Deserialize(stream) / 3;
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
                messageTimers.Add(new TimerOut((float)NetworkManager.Instance.latency / 1000.0f * timeOffset), data);
                UnityEngine.Debug.Log("Se agrego uno de consola");
                break;
            case MessageType.Disconnect:
                lastDisconnectMessagesReceived.Add(data);
                messageTimers.Add(new TimerOut((float)NetworkManager.Instance.latency / 1000.0f * timeOffset), data);
                break;
            case MessageType.PlayerList:
                lastPlayerListMessagesReceived.Add(data);
                messageTimers.Add(new TimerOut((float)NetworkManager.Instance.latency / 1000.0f * timeOffset), data);
                break;
            default:
                break;
        }
    }

#if UNITY_SERVER
    public static void ServerAddMessageReceived(byte[] data)
    {
        switch ((MessageType)CheckMessage(data))
        {
            case MessageType.Console:
            case MessageType.Disconnect:
            case MessageType.PlayerList:
                int id = GetID(data);
                if (!lastMessagesReceivedPerClient.ContainsKey(id))
                {
                    lastMessagesReceivedPerClient.Add(id, new Dictionary<MessageType, List<byte[]>>());
                    lastMessagesReceivedPerClient[id].Add(MessageType.Console, lastStringMessagesReceived);
                    lastMessagesReceivedPerClient[id].Add(MessageType.Disconnect, lastDisconnectMessagesReceived);
                    lastMessagesReceivedPerClient[id].Add(MessageType.PlayerList, lastPlayerListMessagesReceived);
                }
                lastMessagesReceivedPerClient[id][(MessageType)CheckMessage(data)].Add(data);

                if (!messageTimersPerClient.ContainsKey(id))
                    messageTimersPerClient.Add(id, new Dictionary<TimerOut, byte[]>());

                if (messageTimersPerClient.ContainsKey(id))
                    messageTimersPerClient[id].Add(new TimerOut((float)NetworkManager.Instance.clientsLatency[id] / 1000.0f * timeOffset), data);
                break;
            default:
                break;

        }
    }
#endif

    public static void AddMessageSend(byte[] data)
    {

        switch ((MessageType)CheckMessage(data))
        {
            case MessageType.Console:
                lastStringMessagesSend.Add(data);
                messageTimers.Add(new TimerOut((float)NetworkManager.Instance.latency / 1000.0f * timeOffset), data);
                break;
            case MessageType.Disconnect:
                lastDisconnectMessagesSend.Add(data);
                messageTimers.Add(new TimerOut((float)NetworkManager.Instance.latency / 1000.0f * timeOffset), data);
                break;
            case MessageType.PlayerList:
                lastPlayerListMessagesSend.Add(data);
                messageTimers.Add(new TimerOut((float)NetworkManager.Instance.latency / 1000.0f * timeOffset), data);
                break;
            default:
                break;
        }
    }

#if UNITY_SERVER
    public static void ServerAddMessageSend(byte[] data)
    {
        if(NetworkManager.Instance.clients.Count > 0)
        {
            int id = GetID(data);
            switch ((MessageType)CheckMessage(data))
            {
                case MessageType.Console:
                case MessageType.Disconnect:
                case MessageType.PlayerList:
                    if (!lastMessagesSendServer.ContainsKey((MessageType)CheckMessage(data)))
                    {
                        lastMessagesSendServer.Add((MessageType)CheckMessage(data), new List<byte[]>());
                    }
                    lastMessagesSendServer[(MessageType)CheckMessage(data)].Add(data);

                    if (!messageTimersPerClient.ContainsKey(id))
                        messageTimersPerClient.Add(id, new Dictionary<TimerOut, byte[]>());

                    if (messageTimersPerClient.ContainsKey(id))
                    {
                        if (id == -1)
                        {
                            messageTimersPerClient[id].Add(new TimerOut((float)NetworkManager.Instance.clientsLatency.Values.Max() / 1000.0f * timeOffset), data);
                        }
                        else
                        {
                            if (NetworkManager.Instance.clientsLatency.ContainsKey(id))
                                messageTimersPerClient[id].Add(new TimerOut((float)NetworkManager.Instance.clientsLatency[id] / 1000.0f * timeOffset), data);
                        }
                    }

                    break;
                default:
                    break;
            }
        }
    }
#endif

    public static void RemoveMessageReceived(byte[] data)
    {

        switch ((MessageType)CheckMessage(data))
        {
            case MessageType.Console:
                lastStringMessagesReceived.Remove(data);
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

#if UNITY_SERVER
    public static void ServerRemoveMessageReceived(byte[] data)
    {
        int id = GetID(data);
        switch ((MessageType)CheckMessage(data))
        {
            case MessageType.Console:
                lastMessagesReceivedPerClient[id][MessageType.Console].Remove(data);
                break;
            case MessageType.Disconnect:
                lastMessagesReceivedPerClient[id][MessageType.Disconnect].Remove(data);
                break;
            case MessageType.PlayerList:
                lastMessagesReceivedPerClient[id][MessageType.PlayerList].Remove(data);
                break;
            default:
                break;
        }
    }
#endif

    public static void RemoveMessageSend(byte[] data)
    {

        switch ((MessageType)CheckMessage(data))
        {
            case MessageType.Console:
                lastStringMessagesSend.Remove(data);
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

#if UNITY_SERVER
    public static void ServerRemoveMessageSend(byte[] data)
    {
        switch ((MessageType)CheckMessage(data))
        {
            case MessageType.Console:
                lastMessagesSendServer[MessageType.Console].Remove(data);
                break;
            case MessageType.Disconnect:
                lastMessagesSendServer[MessageType.Disconnect].Remove(data);
                break;
            case MessageType.PlayerList:
                lastMessagesSendServer[MessageType.PlayerList].Remove(data);
                break;
            default:
                break;
        }
    }
#endif

    public static void CheckTimers()
    {
        int i = 0;
        List<byte[]> datas = messageTimers.Values.ToList();
        foreach (TimerOut timer in messageTimers.Keys.ToList())
        {
            timer.UpdateTimer();
            if (timer.IsTimeOut())
            {
                try
                {
                    RemoveMessageReceived(datas[i]);
                }
                catch
                {
                    RemoveMessageSend(datas[i]);
                }
                messageTimers.Remove(timer);
            }
            i++;
        }
    }

#if UNITY_SERVER
    public static void ServerCheckTimers()
    {
        List<Dictionary<TimerOut, byte[]>> dictionaries = messageTimersPerClient.Values.ToList();
        foreach (Dictionary<TimerOut, byte[]> dictionary in dictionaries)
        {
            int i = 0;
            List<byte[]> datas = dictionary.Values.ToList();
            foreach (TimerOut timer in dictionary.Keys.ToList())
            {
                timer.UpdateTimer();
                if (timer.IsTimeOut())
                {
                    try
                    {
                        ServerRemoveMessageReceived(datas[i]);
                    }
                    catch
                    {
                        ServerRemoveMessageSend(datas[i]);
                    }
                    messageTimersPerClient.Remove(GetID(datas[i]));
                }
                i++;
            }
        }
    }
#endif
}
