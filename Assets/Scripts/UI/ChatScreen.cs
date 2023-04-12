using System.Net;
using UnityEngine.UI;
using UnityEngine;
using System;

public class ChatScreen : MonoBehaviourSingleton<ChatScreen>
{

    public GameObject[] players;
    public GameObject chatPanel;
    public Text messages;
    public Text messagesQuantityText;
    public int messagesQuantity;
    public GameObject messagesGO;
    public InputField inputMessage;
    private bool isOn = false;

    protected override void Initialize()
    {
        NetworkManager.Instance.OnReceiveConsoleMessage += UpdateMessage;

        inputMessage.onEndEdit.AddListener(OnEndEdit);

        this.gameObject.SetActive(false);

        NetworkManager.Instance.OnReceiveEvent += OnReceiveDataEvent;

    }

    private void Update()
    {
        if (!NetworkManager.Instance.isServer)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                messages.gameObject.SetActive(!isOn);
                inputMessage.gameObject.SetActive(!isOn);
                chatPanel.SetActive(!isOn);
                messagesGO.SetActive(isOn);
                isOn = !isOn;
                if (isOn)
                {
                    messagesQuantity = 0;
                }
                if (!isOn)
                {
                    messagesQuantity = 0;
                    messagesQuantityText.text = messagesQuantity + " new messages";
                }
            }
            
            Vector3 move = Vector3.zero;

            if (!isOn)
            {
                if (!NetworkManager.Instance.isServer)
                {

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



                }
                NetworkManager.Instance.players[NetworkManager.Instance.ownId - 1].GetComponent<Player>().transform.position += move;

                NetworkManager.Instance.SendToServer(new NetVector3(NetworkManager.Instance.players[NetworkManager.Instance.ownId - 1].GetComponent<Player>().transform.position));
            }
            else
            {

            }
            
       
        }
        
        
    }

    private void OnReceiveDataEvent(byte[] data, IPEndPoint ep)
    {
        if (NetworkManager.Instance.isServer)
        {
            NetworkManager.Instance.Broadcast(data);
        }
    }

    private void UpdateMessage(string data)
    {
        messages.text += "Otro: " + data + System.Environment.NewLine;
        if (!isOn)
        {
            messagesQuantity++;
            messagesQuantityText.text = messagesQuantity + " new messages!";
        }
        
    }

    private void OnEndEdit(string str)
    {
        if (inputMessage.text != "")
        {
            if (!NetworkManager.Instance.isServer)
            {
                NetworkManager.Instance.SendToServer(new NetString(inputMessage.text));
                messages.text += "Yo: " + inputMessage.text + System.Environment.NewLine;
            }

            inputMessage.ActivateInputField();
            inputMessage.Select();
            inputMessage.text = "";
        }
    }
}
