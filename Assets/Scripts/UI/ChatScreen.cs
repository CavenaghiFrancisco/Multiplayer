﻿using System.Net;
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
            bool hasMove = false;
            if (Input.GetKey(KeyCode.W))
            {
                move = new Vector3(0, 0, 1) * Time.deltaTime;
                hasMove = true;
            }
            if (Input.GetKey(KeyCode.S))
            {
                move = new Vector3(0, 0, -1) * Time.deltaTime;
                hasMove = true;
            }
            if (Input.GetKey(KeyCode.A))
            {
                hasMove = true;
                move = new Vector3(-1, 0, 0) * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                hasMove = true;
                move = new Vector3(1, 0, 0) * Time.deltaTime;
            }

            if (hasMove)
            {
                NetworkManager.Instance.SendToServer(new NetVector3(move));
            }
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
