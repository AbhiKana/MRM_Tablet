using System.IO;
using System;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectViaInput : MonoBehaviour
{
    [Tooltip("Select start as a Server or Client")]
    [Header("Start as")]

    public ClientServerSelector clientServerSelector;

    starting starting;
    public string ipKey;
    public bool IsConnected = false;
    public bool IsReconnecting = false;
    [SerializeField] GameObject FirstPage, SecondPage;

    public void Awake()
    {
        if (FirstPage != null)
        {
            FirstPage.GetComponentInChildren<Button>().onClick.AddListener(() =>
            {
                Debug.Log("Auto Click");
                clientServerSelector.GetSelectType();
                SetIP(FirstPage.GetComponentInChildren<TMP_InputField>());
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
        }
    }
    private void GetIPFrom_InputField()
    {
        if (PlayerPrefs.HasKey(nameof(ipKey)))
        {
            ipKey = PlayerPrefs.GetString(nameof(ipKey));
            FirstPage.GetComponentInChildren<TMP_InputField>().text = ipKey;
            clientServerSelector.GetSelectType();
            //InitializeClient();
        }
        else
        {
            FirstPage.SetActive(true);
            FirstPage.GetComponentInChildren<TMP_InputField>().text = string.Empty;
        }
    }

    private void UI_Status(bool value1, bool value2)
    {
        if (FirstPage != null)
            FirstPage.SetActive(value1);

        if (SecondPage != null)
            SecondPage.SetActive(value2);

        IsConnected = false;
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
            if (FirstPage != null)
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
            UI_Status(false, true);
        }
        if (IsConnected)
        {
            //Debug.Log("Got connected");
            if (IsReconnecting)
            {
                Debug.LogError("Stop Invoke repeating");
                CancelInvoke("InitializeClient");
                IsReconnecting = false;
            }
            UI_Status(false, true);
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
