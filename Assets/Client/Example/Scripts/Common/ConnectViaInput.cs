using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ConnectViaInput : MonoBehaviour
{
    [Tooltip("Select start as a Server or Client")]
    [Header("Start as")]

    public ClientServerSelector clientServerSelector;
    
    private bool isConnectedToServer;
    public bool IsConnectedToServer
    {
        get { return isConnectedToServer; }
        set
        {
            isConnectedToServer = value;
            Debug.Log("Connection status changed: " + value);
        }
    }

    //Drop ClientInputCanvas panel into ClientManagerwithInput Prefab
    //Change your Canvas UI according to your story board 
    //Reference the IP connection Gamobject into the Input Panel varliable in this script through Unity Editor 
    [SerializeField] GameObject InputPanel;

    public GameObject panel => InputPanel;

    starting starting;
    public string ipKey;
    public bool IsConnected = false;
    public bool IsReconnecting = false;
    
    public UnityEvent OnConnectToserver;
    public UnityEvent OnServerDisconnect;
    public UnityEvent OnServerNotFound;

    public void Awake()
    {
        if (InputPanel != null)
        {
            InputPanel.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                Debug.Log("Auto Click");
                SetIP(InputPanel.GetComponentInChildren<TMP_InputField>());
                clientServerSelector.GetSelectType();
            });
        }
    }

    private void Start()
    {
        ConnectToServer();
        TCP_ClientController.OnMessageReceived += ReconnectToServer;
    }
    public void SetIP(TMP_InputField inputText)
    {
        if (string.IsNullOrEmpty(inputText.text.Trim()) || string.IsNullOrWhiteSpace(inputText.text.Trim()) || !Regex.IsMatch(inputText.text, @"([0-9]{1,3})[.]([0-9]{1,3})[.]([0-9]{1,3})[.]([0-9]{1,3})"))
        {
            inputText.text = "Enter Correct IP";
            return;
        }
        
        if (!IsConnected)
        {
            ipKey = inputText.text;
            PlayerPrefs.SetString(nameof(ipKey), ipKey);
            Debug.Log("Server not found");
            IsConnectedToServer = false;
            OnServerNotFound?.Invoke();
        }
    }
    /*public void SetIP(TMP_InputField inputText)
    {
        if (string.IsNullOrEmpty(inputText.text.Trim()) ||
           !Regex.IsMatch(inputText.text, @"^([0-9]{1,3}\.){3}[0-9]{1,3}$"))
        {
            inputText.text = "Enter Correct IP";
            return;
        }

        ipKey = inputText.text;
        PlayerPrefs.SetString(nameof(ipKey), ipKey);

        // Disable button during connection attempt
        var connectButton = InputPanel.GetComponentInChildren<Button>();
        connectButton.interactable = false;
        connectButton.GetComponentInChildren<TMP_Text>().text = "Connecting...";

        // Start connection with 5 second timeout
        var tcpController = clientServerSelector.GetClientController().GetComponent<TCP_ClientController>();
        tcpController.ConnectWithTimeout(ipKey, 8052, 5f, (success) =>
        {
            UnityMainThreadDispatcher.Enqueue(() =>
            {
                connectButton.interactable = true;
                connectButton.GetComponentInChildren<TMP_Text>().text = "Connect";

                if (success)
                {
                    IsConnectedToServer = true;
                    OnConnectToserver?.Invoke();
                }
                else
                {
                    IsConnectedToServer = false;
                    OnServerNotFound?.Invoke();
                }
            });
        });
    }*/
    private void GetIPFrom_InputField()
    {
        if (PlayerPrefs.HasKey(nameof(ipKey)))
        {
            ipKey = PlayerPrefs.GetString(nameof(ipKey));
            InputPanel.GetComponentInChildren<TMP_InputField>().text = ipKey;
            clientServerSelector.GetSelectType();
        }
        else
        {
            InputPanel.SetActive(true);
            InputPanel.GetComponentInChildren<TMP_InputField>().text = string.Empty;
        }
    }

    private void UI_Status(bool value1)
    {
        if (InputPanel != null)
            InputPanel.SetActive(value1);
    }

    private void OnDestroy()
    {
        TCP_ClientController.OnMessageReceived -= ReconnectToServer;
    }

    void ReconnectToServer(string msg)
    {
        if (msg.Contains("Server Disconnected"))
        {
            OnServerDisconnect?.Invoke();
            IsConnected = false;
            IsConnectedToServer = false;
            IsReconnecting = true;
            Debug.Log("Please, Reconnect to server");
            //InvokeRepeating(nameof(InitializeClient), 3f, 3f);
        }
    }
    private void ConnectToServer()
    {
        starting = (starting)(int)clientServerSelector.selectedType;
        if (starting == starting.Client)
        {
            if (InputPanel != null)
            {
                GetIPFrom_InputField();
            }
        }
        TCP_ClientController.OnConnect += ClientConnected;
    }

    [ContextMenu("Reconnect")]
    public void InitializeClient()
    {
        if (clientServerSelector.GetClientController() != null)
        {
            clientServerSelector.GetClientController().GetComponent<TCP_ClientController>()._Initialze();
        }
    }

    public void ClientConnected()
    {
        UpdateConnectionStatus();
        OnConnectToserver?.Invoke();
    }

    private void UpdateConnectionStatus()
    {
        IsConnected = true;
        if (IsReconnecting)
        {
            CancelInvoke(nameof(InitializeClient));
            IsReconnecting = false;
        }
    }

    /*protected virtual void Update()
    {
        if (IsConnected)
        {
            OnConnectToserver?.Invoke();
            IsConnected = false;
        
            if (IsReconnecting)
            {
                Debug.LogError("Stop Invoke repeating");
                CancelInvoke("InitializeClient");
                IsReconnecting = false;
            }
        }
    }*/

    public void EnableInputField(bool val)
    {
        InputPanel.SetActive(val);
    }

    public void QuitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
