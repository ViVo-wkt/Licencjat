using UnityEngine;
using System.Collections.Generic;

public class RadarZoomSystem : MonoBehaviour
{
    public static RadarZoomSystem Instance;

    [System.Serializable]
    public struct ZoomLevel
    {
        public string name;
        public float rangeScale; 
        public float sweepSpeed; 
        public float blipLifetime; 
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
        ApplyZoom(0);
    }

    public void ApplyZoom(int index)
    {
        if (zoomLevels == null || index < 0 || index >= zoomLevels.Count) return;
        
        float oldScale = (zoomLevels.Count > currentLevelIndex) ? zoomLevels[currentLevelIndex].rangeScale : 1f;
        currentLevelIndex = index;
        float newScale = zoomLevels[currentLevelIndex].rangeScale;

        // The '?' cleanly checks if anyone is listening before invoking the event
        OnZoomChanged?.Invoke(oldScale, newScale); 
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