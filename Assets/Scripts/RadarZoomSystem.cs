using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem; 

public class RadarZoomSystem : MonoBehaviour
{
    public static RadarZoomSystem Instance;

    [System.Serializable]
    public struct ZoomLevel
    {
        public string name;
        public float rangeScale; // 1 = Normal, 2 = 2x Range (objects 1/2 size/speed)
        public float sweepSpeed; // Degrees per second
        public float blipLifetime; // How long blips last
    }

    [Header("Configuration")]
    public List<ZoomLevel> zoomLevels;
    public int currentLevelIndex = 0;

    public delegate void ZoomChangeAction(float oldScale, float newScale);
    public static event ZoomChangeAction OnZoomChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // THIS METHOD RUNS IN EDITOR WHEN YOU RESET THE COMPONENT
    private void Reset()
    {
        zoomLevels = new List<ZoomLevel>
        {
            new ZoomLevel { name = "Close (10km)", rangeScale = 1f, sweepSpeed = 180f, blipLifetime = 1.5f },
            new ZoomLevel { name = "Medium (30km)", rangeScale = 3f, sweepSpeed = 90f, blipLifetime = 3.0f },
            new ZoomLevel { name = "Long (60km)", rangeScale = 6f, sweepSpeed = 45f, blipLifetime = 6.0f }
        };
    }

    void Start()
    {
        // Safety: If list is still empty at runtime, fill it.
        if (zoomLevels == null || zoomLevels.Count == 0) Reset();

        // Safety: Prevent "divide by zero" bugs if rangeScale is 0
        for (int i = 0; i < zoomLevels.Count; i++)
        {
            var level = zoomLevels[i];
            if (level.rangeScale <= 0.1f) level.rangeScale = 1f; 
            zoomLevels[i] = level;
        }
        
        ApplyZoom(0);
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame) ApplyZoom(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) ApplyZoom(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) ApplyZoom(2);
    }

    public void ApplyZoom(int index)
    {
        if (index < 0 || index >= zoomLevels.Count) return;
        
        // Allow re-applying the same zoom on startup to force position updates
        float oldScale = (zoomLevels.Count > currentLevelIndex) ? zoomLevels[currentLevelIndex].rangeScale : 1f;
        
        currentLevelIndex = index;
        float newScale = zoomLevels[currentLevelIndex].rangeScale;

        if (OnZoomChanged != null) OnZoomChanged(oldScale, newScale);
        
        Debug.Log($"Zoom Set: {zoomLevels[index].name}");
    }

    public float GetSpeedMultiplier()
    {
        if (zoomLevels == null || zoomLevels.Count == 0) return 1f;
        return 1.0f / zoomLevels[currentLevelIndex].rangeScale;
    }

    public float GetCurrentSweepSpeed()
    {
        if (zoomLevels == null || zoomLevels.Count == 0) return 180f;
        return zoomLevels[currentLevelIndex].sweepSpeed;
    }

    public float GetCurrentBlipLifetime()
    {
        if (zoomLevels == null || zoomLevels.Count == 0) return 1.5f;
        return zoomLevels[currentLevelIndex].blipLifetime;
    }
}