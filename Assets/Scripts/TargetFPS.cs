using UnityEngine;

public class TargetFPS : MonoBehaviour
{
    void Start()
    {
        // Disable VSync
        QualitySettings.vSyncCount = 0;

        // Optionally set a target frame rate
        Application.targetFrameRate = 60; // or whatever you prefer
    }
}
