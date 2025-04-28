using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundRunner : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float backgroundFPS = 5f;
    [SerializeField] private bool pauseAudioWhenBackgrounded = true;

    private int normalFPS;
    private bool isApplicationFocused = true;

    void Start()
    {
        normalFPS = Application.targetFrameRate;
        Application.focusChanged += OnFocusChanged;
        
        #if UNITY_WEBGL
        // WebGL-specific initialization
        Application.runInBackground = true;
        #endif
    }

    void OnFocusChanged(bool hasFocus)
    {
        isApplicationFocused = hasFocus;
        UpdateGameState();
    }

    void OnApplicationFocus(bool hasFocus)
    {
        isApplicationFocused = hasFocus;
        UpdateGameState();
    }

    void UpdateGameState()
    {
        if (isApplicationFocused)
        {
            // Normal operation
            Time.timeScale = 1f;
            Application.targetFrameRate = normalFPS;
            if (pauseAudioWhenBackgrounded) AudioListener.pause = false;
        }
        else
        {
            // Background operation
            Time.timeScale = 1f; // Keep simulation running
            Application.targetFrameRate = (int)backgroundFPS;
            if (pauseAudioWhenBackgrounded) AudioListener.pause = true;
        }
    }

    void OnDestroy()
    {
        Application.focusChanged -= OnFocusChanged;
    }
}
