using UnityEngine;

[System.Serializable]
public class MessageFormat
{
    public string MessageKey;
    public string MessageValue;
}

public class SocketConnectionChecker : MonoBehaviour
{
    TCP_ClientController tcpClientController;
    [SerializeField] ConnectViaInput connectViaInput;
    [SerializeField] ConnectionStateManager connectionStateManager;
    [SerializeField] UserData userData;

    [Header("Scriptable Objects")]
    [SerializeField] MessageSenderDetails user_id;
    [SerializeField] MessageSenderDetails tab_id;
    [SerializeField] MessageSenderDetails close_tab_id;

    private void Start()
    {
        userData = FindObjectOfType<UserData>();
    }
    public void CheckConnectionStatus()
    {
        if (connectViaInput.IsConnectedToServer)
        {
            if (connectionStateManager.CurrentState == ConnectionState.ConnectedToConfigurator)
                return;

            Debug.Log("Go to next page");
            Invoke(nameof(SendDataToConfig), 1f);
        }
    }

    public void SendDataToConfig()
    {
        if (userData != null)
        {
            if (!string.IsNullOrEmpty(userData.storeUserData.user_id))
            {
                SendTabID();
                SendUserData();
            }
        }
    }

    private void SendUserData()
    {
        if (userData.storeUserData.success)
        {
            if (tcpClientController == null)
                tcpClientController = FindObjectOfType<TCP_ClientController>();

            Debug.Log("Sending Info to Multitaction");
            MessageFormat messageFormat = new MessageFormat();
            messageFormat.MessageKey = user_id.nameVal;
            messageFormat.MessageValue = userData.storeUserData.user_id;
            string data = JsonUtility.ToJson(messageFormat);
            tcpClientController.SendMessage(data);
        }
    }

    public void SendTabID()
    {
        MessageFormat m = new MessageFormat();
        m.MessageKey = tab_id.nameVal;
        m.MessageValue = Tab_ID.tabId.ToString();

        string data = JsonUtility.ToJson(m);
        
        if(tcpClientController == null)
            tcpClientController= FindObjectOfType<TCP_ClientController>();

        Debug.Log("Tab ID to be sent");
        tcpClientController.SendMessage(data);
    }

    public void SendCloseTabID()
    {
        MessageFormat m = new MessageFormat();
        m.MessageKey =close_tab_id.name;
        m.MessageValue = Tab_ID.tabId.ToString();

        string data = JsonUtility.ToJson(m);

        if (tcpClientController == null)
            tcpClientController = FindObjectOfType<TCP_ClientController>();

        Debug.Log("Closing Tab ID to be sent: " + data);
        tcpClientController.SendMessage(data);
    }

    /*private void OnApplicationQuit()
    {
        SendCloseTabID();
    }*/
}


