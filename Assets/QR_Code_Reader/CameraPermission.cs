using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Android;

public class CameraPermission : MonoBehaviour
{
    public static CameraPermission Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    /*    void Start()
        {

    #if UNITY_ANDROID
            if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                AskCameraPermission();
                return;
            }
    #endif
        }*/

    private void Start()
    {
        GetCameraAccess();
    }

    public void RequestCameraAccess(Action onGranted, Action onDenied, Action onDeniedPermanently = null)
    {
        if (Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Debug.Log($"Permission already granted.");
            onGranted?.Invoke();
            return;
        }

        var callbacks = new PermissionCallbacks();
        callbacks.PermissionGranted += permissionName =>
        {
            Debug.Log($"{permissionName} granted.");
            onGranted?.Invoke();
        };
        callbacks.PermissionDenied += permissionName =>
        {
            Debug.Log($"{permissionName} denied.");
            onDenied?.Invoke();
            // User checked "Don't ask again" or it's the first request
        };
        callbacks.PermissionDeniedAndDontAskAgain += permissionName =>
        {
            Debug.Log($"{permissionName} denied and won't be asked again.");
            onDenied?.Invoke();
        };
        Permission.RequestUserPermission(Permission.Camera, callbacks);
    }

    public void GetCameraAccess()
    {
        RequestCameraAccess(
            onGranted: () =>
            {
                Debug.Log("Camera permission granted");
                // Proceed with camera functionality
            },
            onDenied: () =>
            {
                Debug.Log("Camera permission denied - can ask again");
                // Show explanation why you need the permission and ask again
                //ShowPermissionRationale();
                //Permission.RequestUserPermission(Permission.Camera);
                ShowPermissionSettingsRedirect();
            },
            onDeniedPermanently: () =>
            {
                Debug.Log("Camera permission denied permanently");
                // Direct user to app settings to enable permission manually
                ShowPermissionSettingsRedirect();
            }
        );
    }

    private void ShowPermissionSettingsRedirect()
    {
        // Show UI explaining that permission is required
        // and provide a button to open app settings
        // When clicked:
        OpenAppSettings();
    }

    private void OpenAppSettings()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
    try
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
        using (AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent"))
        {
            string packageName = currentActivity.Call<string>("getPackageName");
            
            // Correct way to create the URI
            using (AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri"))
            {
                AndroidJavaObject uri = uriClass.CallStatic<AndroidJavaObject>("fromParts", "package", packageName, null);
                
                intent.Call<AndroidJavaObject>("setAction", "android.settings.APPLICATION_DETAILS_SETTINGS");
                intent.Call<AndroidJavaObject>("setData", uri);
                intent.Call<AndroidJavaObject>("addCategory", "android.intent.category.DEFAULT");
                intent.Call<AndroidJavaObject>("setFlags", 0x10000000); // FLAG_ACTIVITY_NEW_TASK
                
                currentActivity.Call("startActivity", intent);
            }
        }
    }
    catch (System.Exception e)
    {
        Debug.LogError("Exception opening app settings: " + e.Message);
    }
#endif
    }

    private bool ShouldShowRequestPermissionRationale(string permissionName)
    {
        // This method checks if we should show rationale for the permission
        // On Android, returns false if "Don't ask again" was checked
        if (Application.platform == RuntimePlatform.Android)
        {
            using (var activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"))
            using (var runtimePermission = new AndroidJavaClass("androidx.core.content.PermissionChecker"))
            {
                int result = runtimePermission.CallStatic<int>("checkSelfPermission", activity, permissionName);
                bool shouldShow = activity.Call<bool>("shouldShowRequestPermissionRationale", permissionName);
                return result != 0 && shouldShow;
            }
        }
        return false;
    }


















#if UNITY_ANDROID              
    private void PermissionCallbacksPermissionGranted(string permissionName)
    {
        StartCoroutine(DelayedCameraInitialization());
    }

    private IEnumerator DelayedCameraInitialization()
    {
        yield return null;
        //InitializeCamera();
    }

    private void PermissionCallbacksPermissionDenied(string permissionName)
    {
        Debug.LogWarning($"Permission {permissionName} Denied");
        AskCameraPermission();
    }

    private void AskCameraPermission()
    {
        var callbacks = new PermissionCallbacks();
        callbacks.PermissionDenied += PermissionCallbacksPermissionDenied;
        callbacks.PermissionGranted += PermissionCallbacksPermissionGranted;
        Permission.RequestUserPermission(Permission.Camera, callbacks);
    }
#endif
    /*private void Start()
    {
        GetCameraAccess();
    }

    public void GetCameraAccess()
    {
        if (Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Debug.Log("Already has camera permission");
            // The user authorized use of the microphone.
        }
        else
        {
            Debug.Log("Ask for camera permission");
            bool useCallbacks = false;
            if (!useCallbacks)
            {
                // We do not have permission to use the microphone.
                // Ask for permission or proceed without the functionality enabled.
                Debug.Log("Request for camera permission");
                Permission.RequestUserPermission(Permission.Camera);
            }
            else
            {
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
                callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
                callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
                Permission.RequestUserPermission(Permission.Camera, callbacks);
            }
        }
    }


    internal void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
    {
        GetCameraAccess();
        Debug.Log($"{permissionName} PermissionDeniedAndDontAskAgain");
    }

    internal void PermissionCallbacks_PermissionGranted(string permissionName)
    {
        Debug.Log($"{permissionName} PermissionCallbacks_PermissionGranted");
    }

    internal void PermissionCallbacks_PermissionDenied(string permissionName)
    {
        GetCameraAccess();
        Debug.Log($"{permissionName} PermissionCallbacks_PermissionDenied");
    }*/
}
