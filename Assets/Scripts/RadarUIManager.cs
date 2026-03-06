using UnityEngine;
using TMPro; 

public class RadarUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject targetInfoPanel; 

    [Header("Text Fields")]
    public TMP_Text typeText;
    public TMP_Text speedText;
    public TMP_Text altitudeText;
    public TMP_Text distanceText;

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

        // UPGRADE: Now it will say "TYPE: UNKNOWN Hostile" or "TYPE: BOGEY Drone"
        if (typeText != null) typeText.text = "TYPE:\n" + target.codename + " " + target.classification;
        if (speedText != null) speedText.text = "SPD:\n" + target.speed + " " + speedUnit;
        if (altitudeText != null) altitudeText.text = "ALT:\n" + target.altitude + " " + altitudeUnit;

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
        if (currentTarget == null || distanceText == null) return;

        float rawDistance = currentTarget.transform.position.magnitude;

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
}