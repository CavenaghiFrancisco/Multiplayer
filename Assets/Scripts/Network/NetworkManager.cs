using System;
using System.Collections.Generic;
using System.IO;
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

    bool firstTime = true;

    public GameObject player;
    
    public List<GameObject> players;

    public int TimeOut = 30;

    public Action<byte[], IPEndPoint> OnReceiveEvent;

    private UdpConnection connection;

    private readonly Dictionary<int, Client> clients = new Dictionary<int, Client>();
    private readonly Dictionary<IPEndPoint, int> ipToId = new Dictionary<IPEndPoint, int>();

    public Dictionary<Client,Dictionary<MessageType,int>> clientsMessages = new Dictionary<Client,Dictionary<MessageType, int>>();

    public int clientId = 0; // This id should be generated during first handshake

    public int ownId;

    public bool ownIdAssigned;


    public void StartServer(int port)
    {
        isServer = true;
        this.port = port;
        connection = new UdpConnection(port, this);

        ownId = clientId;
    }

    public void StartClient(IPAddress ip, int port)
    {
        isServer = false;

        this.port = port;
        this.ipAddress = ip;

        connection = new UdpConnection(ip, port, this);

        NetHandShake handShake = new NetHandShake(BitConverter.ToInt32(ip.GetAddressBytes(), 0), port);

        connection.Send(handShake.Serialize());
    }





    private void AddClient(IPEndPoint ip)
    {
        //Descomentar cuando hagamos con diferentes ip
        //if (!ipToId.ContainsKey(ip)) 
        //{
        Debug.Log("Adding client: " + ip.Address);

        int id = clientId;
        ipToId[ip] = clientId;

        clients.Add(clientId, new Client(ip, id, Time.realtimeSinceStartup));

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

        switch ((MessageType)messageType)
        {
            case MessageType.Position:
                NetVector3 f = new NetVector3(Vector3.zero);
                if (!NetworkManager.Instance.isServer)
                    players[0].transform.position += f.Deserialize(data);
                else
                    players[1].transform.position += f.Deserialize(data);
                break;
            case MessageType.Console:
                NetString g = new NetString("");
                Debug.Log(g.Deserialize(data));
                break;
            case MessageType.HandShake:
                AddClient(ip);
                break;
            case MessageType.PlayerList:
                Debug.Log("Recibi la lista");
                if (!ownIdAssigned)
                {
                    ownIdAssigned = true;
                    ownId = new NetPlayersList().Deserialize(data).Count;
                }
                foreach(KeyValuePair<int, Client> kvp in new NetPlayersList().Deserialize(data))
                {
                    int count = 0;
                    foreach(GameObject player in players)
                    {
                        if(player.GetComponent<Player>().ID != kvp.Value.id)
                        { 
                            count++;
                        }
                    }
                    if(count == players.Count)
                    {
                        Player newPlayer = Instantiate(player).GetComponent<Player>();

                        newPlayer.ID = kvp.Value.id;

                        players.Add(newPlayer.gameObject);
                    }
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

    private void Update()
    {
        // Flush the data in main thread
        if (connection != null)
            connection.FlushReceiveData();        
    }
}
