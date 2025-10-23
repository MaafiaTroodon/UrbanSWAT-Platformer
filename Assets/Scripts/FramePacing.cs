using UnityEngine;
public class FramePacing : MonoBehaviour
{
    void Awake()
    {
        QualitySettings.vSyncCount = 1; // respect monitor vsync
        Application.targetFrameRate = Screen.currentResolution.refreshRate; // ~165
    }
}
