using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AppLifeCycleManager : MonoBehaviour
{
    TCP_ClientController tcpClient;
    ClientServerSelector clientServerSelector;
    SocketConnectionChecker socketConnectionChecker;
    [SerializeField] UnityEvent OnAppKillFromBackground;
    bool Disconnected = false;
    bool isReconnecting = false;
    
    private void Start()
    {
        if (clientServerSelector == null)
            clientServerSelector = FindObjectOfType<ClientServerSelector>();

        socketConnectionChecker = FindObjectOfType<SocketConnectionChecker>();
    }

    [ContextMenu("Disconnect")]
    public void OnAppKilledFromBackground()
    {
        HandleConnection();
    }

    private void HandleConnection()
    {
        if (isReconnecting) return;

        Disconnected = true;
        if (tcpClient == null)
            tcpClient = FindObjectOfType<TCP_ClientController>();

        if (tcpClient != null)
        {
            tcpClient.SendMessage("Pause");
            OnAppKillFromBackground?.Invoke();
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
                StartCoroutine(ReconnectAfterDelay(0.3f));
            }
        }
    }

    private void OnApplicationQuit()
    {
        OnAppKilledFromBackground();
    }

    private IEnumerator ReconnectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (clientServerSelector == null)
            clientServerSelector = FindObjectOfType<ClientServerSelector>();

        clientServerSelector.GetSelectType();
        Disconnected = false;
        isReconnecting = false;
   
        yield return new WaitForSeconds(0.2f);

        socketConnectionChecker.CheckConnectionStatus();
    }

}