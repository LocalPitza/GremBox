using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class ResolutionManager : MonoBehaviour
{
    [Header("References")]
    public Button fullscreenButton;
    public Image fullscreenIndicator;
    public Sprite fullscreenIcon;
    public Sprite windowedIcon;

    [Header("Settings")]
    public Vector2 referenceResolution = new Vector2(1920, 1080);
    public bool startInFullscreen = false;

    private CanvasScaler canvasScaler;
    private bool isFullscreen;
    private Vector2 storedWindowedSize;

    void Start()
    {
        canvasScaler = GetComponent<CanvasScaler>();
        storedWindowedSize = new Vector2(Screen.width, Screen.height);
        
        // Initialize fullscreen state
        isFullscreen = startInFullscreen;
        UpdateFullscreen();
        UpdateUI();
        
        fullscreenButton.onClick.AddListener(ToggleFullscreen);
    }

    void Update()
    {
        // Handle browser window resize
        if (!isFullscreen && (Screen.width != storedWindowedSize.x || Screen.height != storedWindowedSize.y))
        {
            storedWindowedSize = new Vector2(Screen.width, Screen.height);
            UpdateCanvasScaling();
        }
    }

    void ToggleFullscreen()
    {
        isFullscreen = !isFullscreen;
        UpdateFullscreen();
        UpdateUI();
    }

    void UpdateFullscreen()
    {
        if (isFullscreen)
        {
            // Store current window size before going fullscreen
            storedWindowedSize = new Vector2(Screen.width, Screen.height);
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
            Screen.SetResolution(
                (int)storedWindowedSize.x,
                (int)storedWindowedSize.y,
                false
            );
        }
        
        // Force canvas update
        UpdateCanvasScaling();
    }

    void UpdateCanvasScaling()
    {
        // Dynamic scaling based on current resolution
        float screenRatio = (float)Screen.width / Screen.height;
        float referenceRatio = referenceResolution.x / referenceResolution.y;
        
        if (screenRatio >= referenceRatio)
        {
            // Wider screen - scale based on height
            canvasScaler.matchWidthOrHeight = 1;
        }
        else
        {
            // Taller screen - scale based on width
            canvasScaler.matchWidthOrHeight = 0;
        }
        
        // Force UI rebuild
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform as RectTransform);
    }

    void UpdateUI()
    {
        fullscreenIndicator.sprite = isFullscreen ? windowedIcon : fullscreenIcon;
        fullscreenIndicator.SetNativeSize();
    }

    // For WebGL initialization
    public void ForceRefresh()
    {
        UpdateCanvasScaling();
        UpdateUI();
    }
}
