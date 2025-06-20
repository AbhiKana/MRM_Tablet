using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.IO;
using System;
using System.Collections;

public enum starting { Server, Client }

public class StartAs : MonoBehaviour
{
    [Tooltip("Select start as a Server or Client")]
    [Header("Start as")]

    public ClientServerSelector clientServerSelector;
    [SerializeField] private string StramingAssetFileName = "baseurl.txt";
    
    starting starting;
    public string ipKey;
    public bool IsConnected = false;
    public bool IsReconnecting = false;

    public void Start()
    {
        ConnectToServer();
        TCP_ClientController.OnServerDisconnected += ReconnectToServer;
    }

    private void OnDestroy()
    {
        TCP_ClientController.OnServerDisconnected -= ReconnectToServer;
    }
    void ReconnectToServer()
    {
        InvokeRepeating(nameof(InitializeClient), 3f, 3f);
        IsConnected = false;
        IsReconnecting = true;
    }
    private void ConnectToServer()
    {
        starting = (starting)(int)clientServerSelector.selectedType;
        if (starting == starting.Client)
        {
                ipKey = GetBaseURL();
                if (!string.IsNullOrEmpty(ipKey))
                {
                    Debug.Log("IP from base URL");
                    PlayerPrefs.SetString(nameof(ipKey), ipKey);
                }
                else
                {
                    Debug.LogError("Failed to load IP from file. Please check the file or provide a valid IP.");
                }
        }

        TCP_ClientController.OnConnect += ClientConnected;
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

        if (IsReconnecting)
        {
            CancelInvoke("InitializeClient");
            IsReconnecting = false;
        }
    }

    /*protected virtual void Update()
    {
        if (IsConnected)
        {
            if (IsReconnecting)
            {
                Debug.LogError("Stop Invoke repeating");
                CancelInvoke("InitializeClient");
                IsReconnecting = false;
            }
        }
    }*/

    public void QuitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
                 Application.Quit();
#endif
    }

    public string GetBaseURL()
    {
        string fullpath;
        fullpath = Path.Combine(Application.streamingAssetsPath, StramingAssetFileName);
        string dataToLoad = "";
        if (File.Exists(fullpath))
        {
            try
            {
                using FileStream stream = new FileStream(fullpath, FileMode.Open);
                using StreamReader reader = new StreamReader(stream);
                dataToLoad = reader.ReadToEnd().Trim();
            }
            catch (Exception e)
            {
                Debug.LogError("Error Occured. Path not Exist " + fullpath + "\n" + e);
            }
        }
        Debug.Log("Base URL: " + dataToLoad);
        return dataToLoad;
    }
}