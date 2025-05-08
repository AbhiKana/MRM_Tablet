using UnityEngine;

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
            SendDataToConfig();
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
            Configurator c = marbleUploader.GetComponent<Configurator>();

            if (c != null) 
            {
                string dataToSend = c.data;
                if (!string.IsNullOrEmpty(dataToSend))
                {
                    Debug.LogError("Data to send " + dataToSend);
                    tcpClientController.SendMessage(dataToSend);
                }
            }


            MarbleUploader m = marbleUploader.GetComponent<MarbleUploader>();
            if (m != null)
            {
                string dataToSend = m.data;
                if (!string.IsNullOrEmpty(dataToSend))
                {
                    Debug.LogError("Data to send "+dataToSend);
                    tcpClientController.SendMessage(dataToSend);
                }
            }
        }
    }

    public void SendTabID(int id)
    {
        string tabInfo = "tab_id:"+ id;

        if(tcpClientController == null)
            tcpClientController= FindObjectOfType<TCP_ClientController>();

        tcpClientController.SendMessage(tabInfo);
    }
}


