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
    [SerializeField] UserData userData;

    private void Start()
    {
        userData = FindObjectOfType<UserData>();
    }
    public void CheckConnectionStatus()
    {
        if (connectViaInput.IsConnectedToServer)
        {
            Debug.Log("Go to next page");
            Invoke(nameof(SendDataToConfig), 1f);
        }
    }

    public void SendDataToConfig()
    {
        if(tcpClientController == null)
            tcpClientController = FindObjectOfType<TCP_ClientController>();

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
            Debug.Log("Sending Info to Multitaction");
            MessageFormat messageFormat = new MessageFormat();
            messageFormat.MessageKey = "user_id";
            messageFormat.MessageValue = userData.storeUserData.user_id;
            string data = JsonUtility.ToJson(messageFormat);
            tcpClientController.SendMessage(data);
        }
    }

    public void SendTabID()
    {
        MessageFormat m = new MessageFormat();
        m.MessageKey = "tab_id";
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
        m.MessageKey = "close_tab_id";
        m.MessageValue = Tab_ID.tabId.ToString();

        string data = JsonUtility.ToJson(m);

        if (tcpClientController == null)
            tcpClientController = FindObjectOfType<TCP_ClientController>();

        Debug.Log("Closing Tab ID to be sent");
        tcpClientController.SendMessage(data);
    }
}


