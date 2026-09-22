using UnityEngine;
using TMPro; 

public class RadarUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject targetInfoPanel; 

    [Header("Text Fields")]
    public TMP_Text distanceText;
    public TMP_Text speedText;
    public TMP_Text altitudeText;

    [Header("Formatting Settings")]
    public float distanceMultiplier = 10f; 
    public string distanceUnit = "km";
    public string speedUnit = "kn";
    public string altitudeUnit = "ft";

    private TargetSignature currentTarget;

    void Start()
    {
        if (targetInfoPanel != null)
        {
            targetInfoPanel.SetActive(false);
        }
    }

    void Update()
    {
        if (targetInfoPanel != null && targetInfoPanel.activeSelf)
        {
            if (currentTarget == null)
            {
                DeselectTarget();
            }
            else
            {
                UpdateDynamicData();
            }
        }
    }

    public void ShowTargetInfo(TargetSignature target)
    {
        if (target == null || targetInfoPanel == null) return;

        currentTarget = target;
        UpdateDynamicData();
        
        if (!targetInfoPanel.activeSelf) 
        {
            targetInfoPanel.SetActive(true);
        }
    }

    public void DeselectTarget()
    {
        currentTarget = null;
        if (targetInfoPanel != null)
        {
            targetInfoPanel.SetActive(false);
        }
    }

    private void UpdateDynamicData()
    {
        if (currentTarget == null) return;

        bool isLive = currentTarget.IsContinuouslyIlluminated();

        // 1. Distance Calculation
        if (distanceText != null)
        {
            // Use real-time position if illuminated; otherwise use the frozen position from the last sweep
            float rawDistance = isLive 
                ? currentTarget.transform.position.magnitude 
                : currentTarget.lastKnownPosition.magnitude;

            float currentZoomScale = 1f;
            if (RadarZoomSystem.Instance != null)
            {
                int idx = RadarZoomSystem.Instance.currentLevelIndex;
                if (idx >= 0 && idx < RadarZoomSystem.Instance.zoomLevels.Count)
                {
                    currentZoomScale = RadarZoomSystem.Instance.zoomLevels[idx].rangeScale;
                }
                if (currentZoomScale <= 0f) currentZoomScale = 1f; 
            }

            float normalizedDistance = rawDistance / currentZoomScale;
            float calculatedDistance = normalizedDistance * distanceMultiplier;

            distanceText.text = "DIST:\n" + calculatedDistance.ToString("F1") + " " + distanceUnit;
        }

        // 2. Speed Readout
        if (speedText != null)
        {
            int displaySpeed = isLive ? currentTarget.GetCurrentSpeed() : currentTarget.lastKnownSpeed;
            speedText.text = "SPD:\n" + displaySpeed + " " + speedUnit;
        }

        // 3. Altitude Readout
        if (altitudeText != null)
        {
            int displayAlt = isLive ? currentTarget.GetCurrentAltitude() : currentTarget.lastKnownAltitude;
            altitudeText.text = "ALT:\n" + displayAlt + " " + altitudeUnit;
        }
    }
}