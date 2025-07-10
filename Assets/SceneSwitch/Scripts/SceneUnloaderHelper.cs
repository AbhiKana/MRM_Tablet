using Udar.SceneManager;
using UnityEngine;
using UnityEngine.UI;
public class SceneUnloaderHelper : MonoBehaviour
{
    Button closeButton;
    [SerializeField] SceneFieldRef sceneFieldRef;
    void Start()
    {
        closeButton = GetComponent<Button>();

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(() =>
            {
                ConnectTabWhileWaiting();
                SceneSwitchManager.instance.UnLoadScene(sceneFieldRef);
            });
        }
    }

    private void ConnectTabWhileWaiting()
    {
        var connectionStateManager = FindObjectOfType<ConnectionStateManager>();
        if (connectionStateManager != null)
        {
            if (connectionStateManager.CurrentState == ConnectionState.WaitingForConfigurator)
            {
                var socketChecker = FindObjectOfType<SocketConnectionChecker>();
                if (socketChecker != null)
                {
                    socketChecker.SendDataToConfig();
                }
            }
        }
    }
}
