using UnityEngine;

public class AppQuitController : MonoBehaviour
{
    private float backButtonLastPress;
    private const float DOUBLE_PRESS_DELAY = 1.5f;
    private bool backPressedOnce = false;

    // For detecting app termination
    private bool isQuitting = false;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        HandleBackButton();
    }

    void HandleBackButton()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!backPressedOnce)
            {
                backPressedOnce = true;
                backButtonLastPress = Time.time;
                ShowToast("Press back again to exit");
            }
            else if (Time.time - backButtonLastPress <= DOUBLE_PRESS_DELAY)
            {
                isQuitting = true;
                QuitApplication();
            }
            else
            {
                backPressedOnce = false;
            }
        }

        if (backPressedOnce && Time.time - backButtonLastPress > DOUBLE_PRESS_DELAY)
        {
            backPressedOnce = false;
        }
    }

    /*void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus && !isQuitting)
        {
            // App moved to background (could be followed by termination)
            SaveAppState();
        }
    }*/

    void OnApplicationQuit()
    {
        SaveAppState();
    }

    void SaveAppState()
    {
        // Implement your save logic here
        //TCP_ClientController tCP_ClientController = FindObjectOfType<TCP_ClientController>();
        //tCP_ClientController.StopClient();
    }

    void QuitApplication()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void ShowToast(string message)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass toastClass = new AndroidJavaClass("android.widget.Toast");
        
        activity.Call("runOnUiThread", new AndroidJavaRunnable(() => {
            toastClass.CallStatic<AndroidJavaObject>("makeText", 
                activity, 
                message, 
                toastClass.GetStatic<int>("LENGTH_SHORT")).Call("show");
        }));
#else
        Debug.Log("Toast: " + message);
#endif
    }
}