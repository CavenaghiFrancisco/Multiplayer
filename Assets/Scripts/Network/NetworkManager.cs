﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[Serializable]
public struct Client
{
    public float timeStamp;
    public int id;
    public IPEndPoint ipEndPoint;
    public int lastConsoleMessage;
    public int lastPositionMessage;

    public Client(IPEndPoint ipEndPoint, int id, float timeStamp)
    {
        this.timeStamp = timeStamp;
        this.id = id;
        this.ipEndPoint = ipEndPoint;
        this.lastConsoleMessage = 0;
        this.lastPositionMessage = 0;
    }
}

public class NetworkManager : MonoBehaviourSingleton<NetworkManager>, IReceiveData
{

    public IPAddress ipAddress
    {
        get; private set;
    }

    public int port
    {
        get; private set;
    }

    public bool isServer
    {
        get; private set;
    }

    public GameObject player;

    public Dictionary<int, GameObject> players = new Dictionary<int, GameObject>();

    public int TimeOut = 30;

    public Action<byte[], IPEndPoint> OnReceiveEvent;
    public Action<string, int> OnReceiveConsoleMessage;
    public Action<double> OnPackageTimerUpdate;

    private UdpConnection connection;

    TimerOut serverTimer;

    const int limitPlayers = 2;

    private Dictionary<int, Client> clients = new Dictionary<int, Client>();
    private Dictionary<int, TimerOut> clientsTimer = new Dictionary<int, TimerOut>();
    public Dictionary<int, double> clientsLatency = new Dictionary<int, double>();
    private readonly Dictionary<IPEndPoint, int> ipToId = new Dictionary<IPEndPoint, int>();

    public Dictionary<Client, Dictionary<MessageType, int>> clientsMessages = new Dictionary<Client, Dictionary<MessageType, int>>();

    public double latency = 0;

    DateTime lastTime = default;

    public int clientId = 0; // This id should be generated during first handshake
    public int ownId;

    public bool ownIdAssigned = false;

    float packageTimer = 0;

    public void Start()
    {
#if UNITY_SERVER
        Debug.Log("Hola");
        StartServer(10100);
#endif
    }

    public void StartServer(int port)
    {
        ownId = -1;
        isServer = true;
        this.port = port;
        connection = new UdpConnection(port, this);
    }

    public void StartClient(IPAddress ip, int port)
    {
        isServer = false;

        serverTimer = new TimerOut(4);

        this.port = port;
        this.ipAddress = ip;

        connection = new UdpConnection(ip, port, this);

        NetHandShake handShake = new NetHandShake(BitConverter.ToInt32(ip.GetAddressBytes(), 0), port);

        connection.Send(handShake.Serialize());
    }


    private void Update()
    {
        if (connection != null)
            connection.FlushReceiveData();



        if (isServer)
        {
            if (connection != null)
            {
                foreach (KeyValuePair<int, TimerOut> kvp in clientsTimer)
                {
                    kvp.Value.UpdateTimer();
                    if (kvp.Value.IsTimeOut())
                    {
                        clients.Remove(kvp.Key);
                        players.Remove(kvp.Key);
                        clientsTimer.Remove(kvp.Key);
                        clientsLatency.Remove(kvp.Key);
                        break;
                    }
                }
#if UNITY_SERVER
                PackageManager.ServerCheckTimers();
#endif
            }

        }
        else
        {
            if (ownIdAssigned)
            {
                packageTimer += Time.deltaTime;
                serverTimer.UpdateTimer();
                if (serverTimer.IsTimeOut())
                {
                    Debug.Log("Se cayo el server");
                }
                PackageManager.CheckTimers();
            }
        }


    }


    private void AddClient(IPEndPoint ip)
    {
        //Descomentar cuando hagamos con diferentes ip
        //if (!ipToId.ContainsKey(ip)) 
        //{
        Debug.Log("Adding client: " + ip.Address);

        int id = clientId;
        ipToId[ip] = clientId;

        Client client = new Client(ip, id, Time.realtimeSinceStartup);

        if (!clients.ContainsKey(clientId))
        {
            clients.Add(clientId, client);
            clientsTimer.Add(clientId, new TimerOut(4));
            clientsLatency.Add(clientId, 0);

        }
        clientsMessages.Add(client, new Dictionary<MessageType, int>());
        clientsMessages[client].Add(MessageType.Position, 0);
        clientsMessages[client].Add(MessageType.Console, 0);
        clientsMessages[client].Add(MessageType.Disconnect, 0);



        clientId++;

        if (isServer)
        {
            Debug.Log("Enviando lista");
            Broadcast(new NetPlayersList(clients).Serialize());
            Broadcast(new NetStayAlive(DateTime.Now, 0).Serialize());
            Broadcast(new NetPlayersList(clients).Serialize());
        }

    }

    private void RemoveClient(IPEndPoint ip)
    {
        if (ipToId.ContainsKey(ip))
        {
            Debug.Log("Removing client: " + ip.Address);
            clients.Remove(ipToId[ip]);
        }
    }

    public void OnReceiveData(byte[] data, IPEndPoint ip)
    {
        int messageType = PackageManager.CheckMessage(data);
        if (!PackageManager.CheckTail(data))
        {
            Debug.Log("Se rompio");
            return;
        }
        int instance;
        int id;
        switch ((MessageType)messageType)
        {
            case MessageType.Position:
                NetVector3 f = new NetVector3(Vector3.zero);
                id = BitConverter.ToInt32(data, 4);
                if (!isServer && ownId != BitConverter.ToInt32(data, 4))
                {
                    instance = BitConverter.ToInt32(data, 8);
                    id = BitConverter.ToInt32(data, 4);
                    if (instance > clientsMessages[clients[id]][MessageType.Position])
                    {
                        clientsMessages[clients[id]][MessageType.Position] = instance;
                        NetworkManager.Instance.players[id].GetComponent<Player>().Move(f.Deserialize(data));
                    }
                    else
                    {
                        Debug.Log("Se perdio el mensaje" + instance);
                    }
                }
                if (isServer)
                {
                    Broadcast(data);
                }
                break;
            case MessageType.Console:
                instance = BitConverter.ToInt32(data, 8);
                id = BitConverter.ToInt32(data, 4);
                if (!isServer && ownId != BitConverter.ToInt32(data, 4))
                {
                    if (instance > clientsMessages[clients[id]][MessageType.Console])
                    {
                        NetString g = new NetString("");
                        clientsMessages[clients[id]][MessageType.Console] = instance;
                        OnReceiveConsoleMessage("Cliente-ID-" + (id) + ":" + g.Deserialize(data), id);
                    }
                    else
                    {
                        Debug.Log("Ser perdio el mensaje de consola" + instance);
                    }
                }
                if (isServer)
                {
                    Broadcast(data);
                }
                break;
            case MessageType.HandShake:
                if (isServer)
                {
                    if (clients.Count < limitPlayers)
                    {
                        Debug.Log("Recibi el handshake");
                        AddClient(ip);
                        Broadcast(data);
                        Broadcast(new NetPlayersList(clients).Serialize());
                    }
                    else
                    {
                        Debug.Log("Se lleno el server");
                        //Mandar a otro server
                    }
                }
                else
                {
                    AddClient(ip);
                }
                break;
            case MessageType.PlayerList:
                Debug.Log("Recibi la lista");
                if (!ownIdAssigned)
                {
                    ownIdAssigned = true;
                    ownId = new NetPlayersList().Deserialize(data)[new NetPlayersList().Deserialize(data).Keys.Last()].id;
                }
                clients = new NetPlayersList().Deserialize(data);
                if (!isServer)
                {
                    //if (PackageManager.GetID(data) != ownId)
                    //{
                        Debug.Log(PackageManager.GetID(data) + "Mando la playlist");
                        foreach (KeyValuePair<int, Client> kvp in new NetPlayersList().Deserialize(data))
                        {
                            int count = 0;
                            foreach (KeyValuePair<int, GameObject> player in players)
                            {
                                if (player.Value.GetComponent<Player>().ID != kvp.Value.id)
                                {
                                    count++;
                                }
                            }
                            if (count == players.Count)
                            {
                                if (!clients.ContainsKey(kvp.Value.id))
                                {
                                    clients.Add(kvp.Value.id, kvp.Value);
                                }
                                Player newPlayer = Instantiate(player, transform.position, Quaternion.identity).GetComponent<Player>();

                                newPlayer.ID = kvp.Value.id;

                                players.Add(newPlayer.ID, newPlayer.gameObject);

                                clientsMessages.Add(kvp.Value, new Dictionary<MessageType, int>());
                                clientsMessages[kvp.Value].Add(MessageType.Position, 0);
                                clientsMessages[kvp.Value].Add(MessageType.Console, 0);
                                clientsMessages[kvp.Value].Add(MessageType.Disconnect, 0);
                            }
                        }
                    //}
                }
                else
                {
                    Broadcast(new NetPlayersList(clients).Serialize());
                }
                break;
            case MessageType.Disconnect:
                instance = BitConverter.ToInt32(data, 8);
                id = BitConverter.ToInt32(data, 4);
                Debug.Log("Se desconecto el jugador " + id);
                if (!isServer)
                {
                    Destroy(NetworkManager.Instance.players[id]);
                }
                else
                {
                    //Broadcast(data);
                }
                clientsMessages.Remove(clients[id]);
                clients.Remove(id);
                players.Remove(id);
                clientsTimer.Remove(id);
                clientsLatency.Remove(id);
                if (isServer)
                {
#if UNITY_SERVER
                        PackageManager.lastMessagesReceivedPerClient.Remove(id);
                        PackageManager.messageTimersPerClient.Remove(id);
#endif
                    Broadcast(data);
                    Broadcast(new NetPlayersList(clients).Serialize());
                }
                else
                {
                    SendToServer(new NetPlayersList(clients));
                }
                break;
            case MessageType.StayAlive:
                using (MemoryStream stream = new MemoryStream(data))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    stream.Position = 54;
                    id = (int)formatter.Deserialize(stream);
                }

                (DateTime, double) dataStayAlive = new NetStayAlive().Deserialize(data);

                latency = 0;

                if (lastTime == default)
                {
                    lastTime = dataStayAlive.Item1;
                }
                else
                {
                    if (lastTime < dataStayAlive.Item1)
                    {
                        latency = (dataStayAlive.Item1 - lastTime).TotalMilliseconds;
                        if (!isServer)
                            OnPackageTimerUpdate(latency);
                        lastTime = dataStayAlive.Item1;
                    }
                }
                if (isServer)
                {
                    clientsTimer[id].ResetTimer();
                    clientsLatency[id] = latency;
                    Broadcast(new NetStayAlive(DateTime.Now, latency).Serialize(), id);
                }
                else
                {
                    serverTimer.ResetTimer();
                    SendToServer(new NetStayAlive(DateTime.Now, latency));
                }
                break;
            case MessageType.Request:
                if (isServer)
                {
                    byte[] aux = new NetRequest().Deserialize(data);
                    int msgType = PackageManager.CheckMessage(aux);
                    PackageManager.RequestMessage((MessageType)msgType, aux);
                }
                break;
            default:
                break;
        }
    }

    public void SendToServer<T>(IMessage<T> data)
    {
        connection.Send(data.Serialize());
    }

    public void Broadcast(byte[] data)
    {
        using (var iterator = clients.GetEnumerator())
        {
            while (iterator.MoveNext())
            {
                connection.Send(data, iterator.Current.Value.ipEndPoint);
            }
        }
#if UNITY_SERVER
        PackageManager.ServerAddMessageSend(data);
#endif
    }

    public void Broadcast(byte[] data, int id)
    {
        foreach (KeyValuePair<int, Client> client in clients)
        {
            if (client.Key == id)
            {
                connection.Send(data, client.Value.ipEndPoint);
            }
        }
    }
}
