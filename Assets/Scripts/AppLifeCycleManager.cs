using System.Collections;
using UnityEngine;

public class AppLifeCycleManager : MonoBehaviour
{
    TCP_ClientController tcpClient;
    ClientServerSelector clientServerSelector;
    bool Disconnected = false;
    bool isReconnecting = false;
    
    private void Start()
    {
        if (clientServerSelector == null)
            clientServerSelector = FindObjectOfType<ClientServerSelector>();
    }

    [ContextMenu("Disconnect")]
    public void OnAppKilledFromBackground()
    {
        if (isReconnecting) return;

        Disconnected = true;
        if (tcpClient == null)
            tcpClient = FindObjectOfType<TCP_ClientController>();

        if (tcpClient != null)
        {
            tcpClient.SendMessage("Pause");
            tcpClient.StopClient();
            Destroy(tcpClient.gameObject);
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) // App going to background
        {
            OnAppKilledFromBackground();
        }
        else
        {
            if (Disconnected && !isReconnecting)
            {
                isReconnecting = true;
                StartCoroutine(ReconnectAfterDelay(0.5f));
            }
        }
    }

    private IEnumerator ReconnectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (clientServerSelector == null)
            clientServerSelector = FindObjectOfType<ClientServerSelector>();

        clientServerSelector.GetSelectType();
        Disconnected = false;
        isReconnecting = false;
    }
}