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
        if (currentTarget != null && targetInfoPanel.activeSelf)
        {
            UpdateDynamicData();
        }
    }

    public void ShowTargetInfo(TargetSignature target)
    {
        if (target == null || targetInfoPanel == null) return;

        currentTarget = target;

        // FIXED: Using 'classification' instead of 'targetType'
        if (typeText != null) typeText.text = "TYPE:\n" + target.classification;

        // FIXED: Using the new speed and altitude fields
        if (speedText != null) speedText.text = "SPD:\n" + target.speed + " " + speedUnit;
        if (altitudeText != null) altitudeText.text = "ALT:\n" + target.altitude + " " + altitudeUnit;

        UpdateDynamicData();
        targetInfoPanel.SetActive(true);
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
            // FIXED: Digging into your ZoomLevel list to get the actual rangeScale
            int idx = RadarZoomSystem.Instance.currentLevelIndex;

            if (idx >= 0 && idx < RadarZoomSystem.Instance.zoomLevels.Count)
            {
                currentZoomScale = RadarZoomSystem.Instance.zoomLevels[idx].rangeScale;
            }

            if (currentZoomScale <= 0f) currentZoomScale = 1f;
        }

        // Calculate True Distance
        float normalizedDistance = rawDistance / currentZoomScale;
        float calculatedDistance = normalizedDistance * distanceMultiplier;

        distanceText.text = "DIST:\n" + calculatedDistance.ToString("F1") + " " + distanceUnit;
    }
}