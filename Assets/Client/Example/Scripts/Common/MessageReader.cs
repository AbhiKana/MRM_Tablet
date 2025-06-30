using UnityEngine;

public class MessageReader : MonoBehaviour
{
    private Configurator configurator;
    public MessageFormat revmsg;
    private void Start()
    {
        configurator = FindObjectOfType<Configurator>();
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
                        Debug.Log("Please proceed to the configration: " + revmsg.MessageValue);
                    }
                    break;

                case "disconnect_tab_id":
                    if (revmsg.MessageValue == Tab_ID.GetID().ToString())
                    {
                        Debug.Log("Tab disconnected");
                        configurator.ResetConfig();
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
