using System.Net;
using UnityEngine.UI;
using UnityEngine;
using System;

public class ChatScreen : MonoBehaviourSingleton<ChatScreen>
{

    public GameObject[] players;
    public GameObject chatPanel;
    public GameObject chargePanel;
    public Text messages;
    public Text pingText;
    public Text portText;
    public Text id;
    public Text clientsQuantity;
    public Text messagesQuantityText;
    public int messagesQuantity;
    public GameObject messagesGO;
    public InputField inputMessage;
    private bool isOn = false;
    private Animator anim;
    float timerLag = 0;

    protected override void Initialize()
    {
        anim = messagesGO.GetComponent<Animator>();
        NetworkManager.Instance.OnReceiveConsoleMessage += UpdateMessage;
        NetworkManager.Instance.OnPackageTimerUpdate += UpdatePing;
        NetworkManager.Instance.OnCharge += OnCharge;

        inputMessage.onEndEdit.AddListener(OnEndEdit);

        this.gameObject.SetActive(false);

    }

    private void OnApplicationQuit()
    {
        if(NetworkManager.Instance.ownIdAssigned)
            NetworkManager.Instance.SendToServer(new NetDisconnect("a"));
    }

    private void Update()
    {
        if (NetworkManager.Instance.ownIdAssigned)
        {
            timerLag += Time.deltaTime;

            if(timerLag > 1)
            {
                UpdatePing(timerLag * 20,true);
            }

            portText.text = "Port: " + NetworkManager.Instance.port;

            if (!NetworkManager.Instance.isServer)
            {
                clientsQuantity.text = "Hay " + NetworkManager.Instance.players.Count + " clientes conectados";
                id.text = "ID: " + NetworkManager.Instance.ownId;
            }


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
                        anim.Play("quiet");
                    }
                    if (!isOn)
                    {
                        messagesQuantity = 0;
                        messagesQuantityText.text = messagesQuantity + " new messages";
                        anim.Play("quiet");
                    }
                }

                Vector3 move = Vector3.zero;

                if (!isOn)
                {
                    if (!NetworkManager.Instance.isServer)
                    {
                        if (Input.GetKey(KeyCode.Q))
                        {
                            move = new Vector3(0, -5, 0) * Time.deltaTime;
                        }
                        if (Input.GetKey(KeyCode.E))
                        {
                            move = new Vector3(0, 5, 0) * Time.deltaTime;
                        }
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
                    NetworkManager.Instance.players[NetworkManager.Instance.ownId].GetComponent<Player>().transform.position += move;
                }
                NetworkManager.Instance.SendToServer(new NetVector3(NetworkManager.Instance.players[NetworkManager.Instance.ownId].GetComponent<Player>().transform.position));
            }
        }
       
    }

    private void UpdateMessage(string data, int id)
    {   
        Color color = NetworkManager.Instance.players[id].GetComponent<Player>().color;
        int r = (int)(color.r * 255f);
        int g = (int)(color.g * 255f);
        int b = (int)(color.b * 255f);
        int a = (int)(color.a * 255f);

        messages.text += "<color=" + string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a) + ">" +  data + System.Environment.NewLine + "</color>";
        if (!isOn)
        {
            messagesQuantity++;
            messagesQuantityText.text = messagesQuantity + " new messages!";
            anim.Play("noise");
        }
        
    }

    private void UpdatePing(double latency)
    {
        timerLag = 0;
        Color color = Color.white;
        if (latency <= 50)
        {
            color = Color.green;
        }
        else if (latency > 50 && latency <= 150 )
        {
            color = Color.yellow;
        }
        else if (latency > 150)
        {
            color = Color.red;
        }
        int r = (int)(color.r * 255f);
        int g = (int)(color.g * 255f);
        int b = (int)(color.b * 255f);
        int a = (int)(color.a * 255f);
        pingText.text = "<color=" + string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a) + ">PING: " + Mathf.Clamp((float)latency,0,999).ToString("0") + "ms </color>";
    }

    private void UpdatePing(float latency, bool TimeOut)
    {
        if (!TimeOut)
        {
            timerLag = 0;
        }
        Color color = Color.white;
        if (latency <= 50)
        {
            color = Color.green;
        }
        else if (latency > 50 && latency <= 150)
        {
            color = Color.yellow;
        }
        else if (latency > 150)
        {
            color = Color.red;
        }
        int r = (int)(color.r * 255f);
        int g = (int)(color.g * 255f);
        int b = (int)(color.b * 255f);
        int a = (int)(color.a * 255f);
        pingText.text = "<color=" + string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a) + ">PING: " + Mathf.Clamp((float)latency, 0, 999).ToString("0") + "ms </color>";
    }

    private void OnCharge()
    {
        chargePanel.SetActive(false);
    }

    private void OnEndEdit(string str)
    {
        Color color = NetworkManager.Instance.players[NetworkManager.Instance.ownId].GetComponent<Player>().color;
        int r = (int)(color.r * 255f);
        int g = (int)(color.g * 255f);
        int b = (int)(color.b * 255f);
        int a = (int)(color.a * 255f);

        if (inputMessage.text != "")
        {
            if (!NetworkManager.Instance.isServer)
            {
                NetworkManager.Instance.SendToServer(new NetString(inputMessage.text));
                messages.text += "<color=" + string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}", r, g, b, a) + ">" + "Yo: " + inputMessage.text + System.Environment.NewLine + "</color>";
            }

            inputMessage.ActivateInputField();
            inputMessage.Select();
            inputMessage.text = "";
        }
    }
}
