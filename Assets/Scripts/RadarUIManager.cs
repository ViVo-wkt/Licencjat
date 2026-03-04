using UnityEngine;
using TMPro; // Standard Unity UI Text

public class RadarUIManager : MonoBehaviour
{
    public static RadarUIManager Instance;

    [Header("UI References")]
    public GameObject infoPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    
    [Header("Live Telemetry")]
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI altitudeText;
    public TextMeshProUGUI distanceText;

    private TargetSignature _selectedTarget;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Hide panel at start
        if (infoPanel != null) infoPanel.SetActive(false);
    }

    public void ShowTargetInfo(TargetSignature target)
    {
        _selectedTarget = target;
        
        if (infoPanel != null) infoPanel.SetActive(true);

        // Set Static Info (Name/Desc)
        if (nameText != null) nameText.text = target.codename;
        if (descriptionText != null) descriptionText.text = target.description;
    }

    public void Deselect()
    {
        _selectedTarget = null;
        if (infoPanel != null) infoPanel.SetActive(false);
    }

    void Update()
    {
        // If we have a target, update the numbers live
        if (_selectedTarget != null)
        {
            // 1. Calculate Distance (from center 0,0)
            // We multiply by 10 to simulate kilometers/miles instead of Unity Units
            float dist = Vector3.Distance(Vector3.zero, _selectedTarget.transform.position) * 10f;
            
            // 2. Get Speed & Altitude
            float speed = 0f;
            float alt = 0f;

            // Try to find the navigation script on the enemy
            var nav = _selectedTarget.GetComponent<EnemyNavigation>();
            if (nav != null)
            {
                // Multiply speed for display flair (e.g. Mach 1.5 logic)
                speed = nav.speed * 1000f; 
                alt = nav.altitude;
            }

            // 3. Update Text Mesh Pro fields
            if (distanceText != null) distanceText.text = $"RNG: {dist:F1} km";
            if (speedText != null)    speedText.text    = $"SPD: {speed:F0} kts";
            if (altitudeText != null) altitudeText.text = $"ALT: {alt:F0} ft";
        }
        else
        {
            // If target was destroyed while selected, close panel
            if (infoPanel != null && infoPanel.activeSelf) 
            {
                Deselect();
            }
        }
    }
    // Add this method to allow closing the panel
    public void DeselectTarget()
    {
        // IMPORTANT: Replace 'targetInfoPanel' with the actual name of your UI panel variable
        // It might be called 'infoPanel', 'panelObject', etc. check your variables at the top.
        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }
}