using System.Net;
using UnityEngine.UI;
using UnityEngine;
using System;

public class ChatScreen : MonoBehaviourSingleton<ChatScreen>
{
    public GameObject[] players;
    public Text messages;
    public InputField inputMessage;

    protected override void Initialize()
    {
        inputMessage.onEndEdit.AddListener(OnEndEdit);

        this.gameObject.SetActive(false);

        NetworkManager.Instance.OnReceiveEvent += OnReceiveDataEvent;

    }

    private void Update()
    {
        if (!NetworkManager.Instance.isServer)
        {
            Vector3 move = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                move = new Vector3(0, 0, 5) * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                move = new Vector3(0, 0, -5) * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                move = new Vector3(-5, 0, 0) * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                move = new Vector3(5, 0, 0) * Time.deltaTime;
            }

            NetworkManager.Instance.SendToServer(new NetVector3(NetworkManager.Instance.players[NetworkManager.Instance.ownId - 1].GetComponent<Player>().pos, move));
            
        }
        
    }

    private void OnReceiveDataEvent(byte[] data, IPEndPoint ep)
    {
        if (NetworkManager.Instance.isServer)
        {
            NetworkManager.Instance.Broadcast(data);
        }

        switch ((MessageType)BitConverter.ToInt32(data, 0))
        {
            case MessageType.PlayerList:
                Debug.Log("Hola");
                break;
        }
    }

    private void OnEndEdit(string str)
    {
        //if (inputMessage.text != "")
        //{
        //    if (NetworkManager.Instance.isServer)
        //    {
        //        NetworkManager.Instance.Broadcast(new NetVector3(new Vector3(1,1,1)).Serialize());
        //        messages.text += inputMessage.text + System.Environment.NewLine;
        //    }
        //    else
        //    {
        //        NetworkManager.Instance.SendToServer(new NetVector3(new Vector3(1, 1, 1)));
        //    }

        //    inputMessage.ActivateInputField();
        //    inputMessage.Select();
        //    inputMessage.text = "";
        //}
    }
}
