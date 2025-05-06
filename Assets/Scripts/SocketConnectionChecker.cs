using UnityEngine;

public class SocketConnectionChecker : MonoBehaviour
{
    MarbleUploader marbleUploader;
    TCP_ClientController tcpClientController;

    [SerializeField] ConnectViaInput connectViaInput;

    private void Start()
    {
        marbleUploader = GetComponent<MarbleUploader>();
        
    }
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
            string dataToSend = marbleUploader.data;
            if (!string.IsNullOrEmpty(dataToSend))
            {
                tcpClientController.SendMessage(dataToSend);
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


