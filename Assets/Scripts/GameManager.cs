using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] Button QuitButton;
    private void Start()
    {
        QuitButton.onClick.AddListener(QuitApp);
    }

    public void QuitApp()
    {
        Application.Quit();
    }
    private void OnApplicationPause(bool pause)
    {
        Debug.Log("App Paused");
        if (pause)
        {
            //TCP_ClientController.StopClient();
        }
    }

    private void OnApplicationQuit()
    {
        //StopClient();
    }
}
