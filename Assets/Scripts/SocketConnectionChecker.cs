using System.Globalization;
using UnityEngine;

[System.Serializable]
public class MessageFormat
{
    public string MessageKey;
    public string MessageValue;
}

public class SocketConnectionChecker : MonoBehaviour
{
    [SerializeField] GameObject marbleUploader;
    TCP_ClientController tcpClientController;

    [SerializeField] ConnectViaInput connectViaInput;
    public void CheckConnectionStatus()
    {
        if (connectViaInput.IsConnectedToServer)
        {
            Debug.Log("Go to next page");
            Invoke(nameof(SendDataToConfig), 0.5f);
        }
        else
        {
            Debug.Log("Connect to server first");
            //connectViaInput.EnableInputField();
        }
    }

    public void SendDataToConfig()
    {
        if(tcpClientController == null)
            tcpClientController = FindObjectOfType<TCP_ClientController>();

        if (marbleUploader != null)
        {

            Debug.Log("Check data is filled or empty");

            Configurator c = marbleUploader.GetComponent<Configurator>();

            if (c != null) 
            {
                Debug.Log("Config is not null");
                string dataToSend = c.data;
                if (!string.IsNullOrEmpty(dataToSend))
                {
                    Debug.LogError("Data to send " + dataToSend);
                    tcpClientController.SendMessage(dataToSend);
                }
                else
                {
                    Debug.Log("Configurator uploader is empty");
                    SendUserData();
                }
            }


            MarbleUploader m = marbleUploader.GetComponent<MarbleUploader>();
            if (m != null)
            {
                Debug.Log("Marble uploader is not null");
                string dataToSend = m.data;
                if (!string.IsNullOrEmpty(dataToSend))
                {
                    Debug.LogError("Data to send "+dataToSend);
                    tcpClientController.SendMessage(dataToSend);
                }
                else
                {
                    Debug.Log("Message uploader is empty");
                    SendUserData();
                }
            }
        }
    }

    private void SendUserData()
    {
        UserData userData = FindObjectOfType<UserData>();
        if (userData.storeUserData.success)
        {
            MessageFormat messageFormat = new MessageFormat();
            messageFormat.MessageKey = "user_id";
            messageFormat.MessageValue = userData.storeUserData.user_id;
            string data = JsonUtility.ToJson(messageFormat);
            tcpClientController.SendMessage(data);
        }
    }

    public void SendTabID(int id)
    {
        MessageFormat m = new MessageFormat();
        m.MessageKey = "tab_id";
        m.MessageValue = id.ToString();
        

        string data = JsonUtility.ToJson(m);
        //string tabInfo = "tab_id:"+ id;

        if(tcpClientController == null)
            tcpClientController= FindObjectOfType<TCP_ClientController>();

        tcpClientController.SendMessage(data);
    }
}


