using System.IO;
using System;
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

    starting starting;
    public string ipKey;
    public bool IsConnected = false;
    public bool IsReconnecting = false;

    //Drop ClientInputCanvas panel into ClientManagerwithInput Prefab
    //Change your Canvas UI according to your story board 
    //Reference the IP connection Gamobject into the Input Panel varliable in this script through Unity Editor 
    [SerializeField] GameObject InputPanel;

    public UnityEvent OnConnectToserver;
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
        TCP_ClientController.onServerDisconnect += ReconnectToServer;
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
            OnServerNotFound?.Invoke();
        }
    }
    private void GetIPFrom_InputField()
    {
        if (PlayerPrefs.HasKey(nameof(ipKey)))
        {
            ipKey = PlayerPrefs.GetString(nameof(ipKey));
            InputPanel.GetComponentInChildren<TMP_InputField>().text = ipKey;
            clientServerSelector.GetSelectType();
            //InitializeClient();
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
        TCP_ClientController.onServerDisconnect -= ReconnectToServer;
    }

    void ReconnectToServer()
    {
        InvokeRepeating("InitializeClient", 3f, 3f);
        IsConnected = false;
        IsReconnecting = true;

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
        TCP_ClientController.onConnect += ClientConnected;
    }

    [ContextMenu("Reconnect")]
    public void InitializeClient()
    {
        Debug.Log("Initialize");
        if (clientServerSelector.GetClientController() != null)
        {
            Debug.Log("Through streaming assets");
            clientServerSelector.GetClientController().GetComponent<TCP_ClientController>()._Initialze();
        }
    }
    public void ClientConnected()
    {
        IsConnected = true;
    }

    protected virtual void Update()
    {
        if (IsConnected)
        {
            //UI_Status(false);
            OnConnectToserver?.Invoke();
            IsConnected = false;
        
            if (IsReconnecting)
            {
                //UI_Status(true);
                Debug.LogError("Stop Invoke repeating");
                CancelInvoke("InitializeClient");
                IsReconnecting = false;
            }
        }
    }

    public void OnClientDisconnect()
    {
        //UI_Status(true, false);
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
