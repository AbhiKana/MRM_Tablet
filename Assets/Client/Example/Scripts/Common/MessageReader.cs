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
        //revmsg = JsonUtility.FromJson<MessageFormat>(recvMsg);
        //Debug.Log(JsonUtility.ToJson(recvMsg));
        switch (recvMsg)
        {
            case "accepted_tab_id":
                configurator.OnTabAccepted();
                Debug.Log("Please proceed to the configration");
                break;
        }
    }

    private void OnDestroy()
    {
        TCP_ClientController.OnMessageReceived -= ProcessMessage;
    }
}
