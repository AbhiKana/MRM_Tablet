using UnityEngine;

public class AppLifeCycleManager : MonoBehaviour
{

    [SerializeField] bool Disconnected = false;
    TCP_ClientController tcpClient;
    ConnectViaInput connectViaInput;
    ClientServerSelector clientServerSelector;
    private void Start()
    {
        clientServerSelector= FindObjectOfType<ClientServerSelector>();
    }

    [ContextMenu("Disconnect")]
    public void OnAppKilledFromBackground()
    {
        Disconnected = true;
        if (tcpClient == null)
            tcpClient = FindObjectOfType<TCP_ClientController>();

        tcpClient.SendMessage("Pause");
        tcpClient.StopClient();
        
        if(clientServerSelector == null)
            clientServerSelector = FindObjectOfType<ClientServerSelector>();

        //clientServerSelector.DestroyClientObject();
        //
        Destroy(tcpClient.gameObject);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) // App going to background
        {
            OnAppKilledFromBackground();
        }
        else
        {
            if(Disconnected)
            {
                if (clientServerSelector == null)
                    clientServerSelector = FindObjectOfType<ClientServerSelector>();

                clientServerSelector.GetSelectType();
                Disconnected = false;
            }
        }
    }
}