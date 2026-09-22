using UnityEngine;
using TMPro; 

public class RadarUIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject targetInfoPanel; 

    [Header("Text Fields")]
    public TMP_Text trackNameText;
    public TMP_Text distanceText;
    public TMP_Text speedText;
    public TMP_Text altitudeText;

    [Header("Selection Reticle (2D Circle Object)")]
    [Tooltip("Drag the 2D selection circle/bracket object that sits over the selected blip.")]
    public GameObject selectedTrackIndicator;

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

        if (selectedTrackIndicator != null)
        {
            selectedTrackIndicator.SetActive(false);
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
                UpdateSelectionIndicatorPosition();
            }
        }
    }

    public void ShowTargetInfo(TargetSignature target)
    {
        if (target == null || targetInfoPanel == null) return;

        currentTarget = target;

        if (trackNameText != null)
        {
            trackNameText.text = currentTarget.trackDesignation;
        }

        UpdateDynamicData();
        UpdateSelectionIndicatorPosition();
        
        if (!targetInfoPanel.activeSelf) 
        {
            targetInfoPanel.SetActive(true);
        }

        if (selectedTrackIndicator != null)
        {
            selectedTrackIndicator.SetActive(true);
        }
    }

    public void DeselectTarget()
    {
        currentTarget = null;

        if (targetInfoPanel != null)
        {
            targetInfoPanel.SetActive(false);
        }

        if (selectedTrackIndicator != null)
        {
            selectedTrackIndicator.SetActive(false);
        }
    }

    public void HideForWarbook()
    {
        if (targetInfoPanel != null) targetInfoPanel.SetActive(false);
        if (selectedTrackIndicator != null) selectedTrackIndicator.SetActive(false);
    }

    public void RestoreFromWarbook()
    {
        if (currentTarget != null)
        {
            if (targetInfoPanel != null) targetInfoPanel.SetActive(true);
            if (selectedTrackIndicator != null) selectedTrackIndicator.SetActive(true);
            UpdateDynamicData();
            UpdateSelectionIndicatorPosition();
        }
    }
    private void UpdateSelectionIndicatorPosition()
    {
        if (selectedTrackIndicator == null || currentTarget == null) return;

        bool isLive = currentTarget.IsContinuouslyIlluminated();

        Vector3 targetPos = isLive ? currentTarget.transform.position : currentTarget.lastKnownPosition;
        targetPos.z = -0.15f;

        selectedTrackIndicator.transform.position = targetPos;
    }

    private void UpdateDynamicData()
    {
        if (currentTarget == null) return;

        bool isLive = currentTarget.IsContinuouslyIlluminated();

        if (distanceText != null)
        {
            float visualDistance = isLive 
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

            float trueWorldDistance = visualDistance * currentZoomScale;
            float calculatedDistance = trueWorldDistance * distanceMultiplier;

            distanceText.text = "DIST:\n" + calculatedDistance.ToString("F1") + " " + distanceUnit;
        }

        if (speedText != null)
        {
            int displaySpeed = isLive ? currentTarget.GetCurrentSpeed() : currentTarget.lastKnownSpeed;
            speedText.text = "SPD:\n" + displaySpeed + " " + speedUnit;
        }

        if (altitudeText != null)
        {
            int displayAlt = isLive ? currentTarget.GetCurrentAltitude() : currentTarget.lastKnownAltitude;
            altitudeText.text = "ALT:\n" + displayAlt + " " + altitudeUnit;
        }
    }
}