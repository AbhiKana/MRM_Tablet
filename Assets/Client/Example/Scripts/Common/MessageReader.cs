using UnityEngine;

public class MessageReader : MonoBehaviour
{
    private ConnectionStateManager connectionStateManager;
    private Configurator configurator;
    public MessageFormat revmsg;
    
    private void Start()
    {
        configurator = FindObjectOfType<Configurator>();
        connectionStateManager = FindObjectOfType<ConnectionStateManager>();
        TCP_ClientController.OnMessageReceived += ProcessMessage;
    }

    public void ProcessMessage(string recvMsg)
    {
        if (recvMsg.StartsWith("{\"MessageKey\""))
        {
            revmsg = JsonUtility.FromJson<MessageFormat>(recvMsg);

            Debug.Log(recvMsg);
            switch (revmsg.MessageKey)
            {
                case "accepted_tab_id":
                    if (revmsg.MessageValue == Tab_ID.GetID().ToString())
                    {
                        configurator.OnTabAccepted();
                        connectionStateManager.ConfiguratorAccepted();
                        Debug.Log("Please proceed to the configration: " + revmsg.MessageValue);
                    }
                    break;
                case "disconnect_tab_id":
                    if (revmsg.MessageValue == Tab_ID.GetID().ToString())
                    {
                        Debug.Log("Tab disconnected");
                        connectionStateManager.DisconnectFromConfigurator();
                        configurator.TabDisconnectThroughConfig();
                    }
                    break;
            }
        }
    }

    private void OnDestroy()
    {
        TCP_ClientController.OnMessageReceived -= ProcessMessage;
    }
}
