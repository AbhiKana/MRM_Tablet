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
        }
    }

    private void OnDestroy()
    {
        TCP_ClientController.OnMessageReceived -= ProcessMessage;
    }
}
