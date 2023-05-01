using System;
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
    
    public Dictionary<int,GameObject> players = new Dictionary<int, GameObject>();

    public int TimeOut = 30;

    public Action<byte[], IPEndPoint> OnReceiveEvent;
    public Action<string,int> OnReceiveConsoleMessage;
    public Action<float> OnPackageTimerUpdate;

    private UdpConnection connection;

    TimerOut serverTimer;

    private Dictionary<int, Client> clients = new Dictionary<int, Client>();
    private Dictionary<int, TimerOut> clientsTimer = new Dictionary<int, TimerOut>();
    private readonly Dictionary<IPEndPoint, int> ipToId = new Dictionary<IPEndPoint, int>();

    public Dictionary<Client, Dictionary<MessageType, int>> clientsMessages = new Dictionary<Client, Dictionary<MessageType, int>>();
    

    public int clientId = 0; // This id should be generated during first handshake
    public int ownId;

    public bool ownIdAssigned = false;

    float packageTimer = 0;

    private void Start()
    {
    #if UNITY_SERVER
        StartServer(10111);
    #endif
    }

    public void StartServer(int port)
    {
        isServer = true;
        this.port = port;
        connection = new UdpConnection(port, this);
    }

    public void StartClient(IPAddress ip, int port)
    {
        isServer = false;

        serverTimer = new TimerOut();

        this.port = port;
        this.ipAddress = ip;

        connection = new UdpConnection(ip, port, this);

        NetHandShake handShake = new NetHandShake(BitConverter.ToInt32(ip.GetAddressBytes(), 0), port);

        connection.Send(handShake.Serialize());
    }


    private void Update()
    {
        if (isServer)
        {
            if(connection != null)
            {
                Broadcast(new NetStayAlive().Serialize());
                foreach (KeyValuePair<int, TimerOut> kvp in clientsTimer)
                {
                    kvp.Value.UpdateTimer();
                    if (kvp.Value.IsTimeOut())
                    {
                        clients.Remove(kvp.Key);
                        players.Remove(kvp.Key);
                        clientsTimer.Remove(kvp.Key);
                        Debug.Log("Se cayo el jugador " + kvp.Key);
                        break;
                    }
                }
            }
            
        }
        else
        {
            if (ownIdAssigned)
            {
                packageTimer += Time.deltaTime;
                SendToServer(new NetStayAlive());
                serverTimer.UpdateTimer();
                if (serverTimer.IsTimeOut())
                {
                    Debug.Log("Se cayo el server");
                }
            }
        }



        if (connection != null)
            connection.FlushReceiveData();
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
            clientsTimer.Add(clientId, new TimerOut());
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
        }
        //}
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
        int messageType = PackageReceiver.CheckMessage(data);
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
                break;
            case MessageType.HandShake:
                AddClient(ip);
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
                }
                break;
            case MessageType.Disconnect:
                instance = BitConverter.ToInt32(data, 8);
                id = BitConverter.ToInt32(data, 4);
                if (ownId != BitConverter.ToInt32(data, 4))
                { 
                    if (!isServer)
                    {
                        Destroy(NetworkManager.Instance.players[id]);
                    }
                    clients.Remove(id);
                    players.Remove(id);
                    if(isServer)
                    {
                        clientsTimer.Remove(id);
                        Broadcast(new NetPlayersList(clients).Serialize());
                    }
                    else
                    {
                        SendToServer(new NetPlayersList(clients));
                    }
                }
                break;
            case MessageType.StayAlive:
                id = BitConverter.ToInt32(data, 4);

                
                if (isServer)
                {
                    clientsTimer[id].ResetTimer();
                    Broadcast(data);
                }
                else
                {
                    OnPackageTimerUpdate(packageTimer);
                    packageTimer = 0;
                    serverTimer.ResetTimer();
                    SendToServer(new NetStayAlive());
                }
                break;
            default:
                break;
        }

        if (OnReceiveEvent != null)
            OnReceiveEvent.Invoke(data, ip);
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
    }
}
