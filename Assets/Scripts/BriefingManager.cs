using UnityEngine;
using TMPro;

public class BriefingManager : MonoBehaviour
{
    [Header("UI Setup")]
    public GameObject briefingPanel; 
    public TMP_Text briefingTextDisplay;

    [Header("Indicators")]
    public GameObject messageDiodeLight;

    // --- NEW: THE CENTRAL POWER SWITCH ---
    [Header("Cockpit Controls")]
    [Tooltip("Drag the specific scripts (Knobs, Buttons, Wheels) you want disabled while reading!")]
    public MonoBehaviour[] cockpitControls;
    // -------------------------------------

    [Header("Mission Details")]
    [TextArea(5, 10)] 
    public string missionBriefing = "COMMANDER:\n\nMultiple unidentified contacts detected on radar. Intercept and destroy all hostile targets.\n\nGood luck.";

    private string _lastMessage = "";

    void Start()
    {
        ShowMessage(missionBriefing, true);
    }

    public void ShowMessage(string messageText, bool freezeTime = false)
    {
        _lastMessage = messageText;

        if (briefingPanel != null) briefingPanel.SetActive(true);
        if (briefingTextDisplay != null) briefingTextDisplay.text = messageText;
        if (messageDiodeLight != null) messageDiodeLight.SetActive(true);

        if (freezeTime) 
        {
            Time.timeScale = 0f;
            SetControlsActive(false); // Turn the cockpit OFF
        }
    }

    public void AcknowledgeAndStart()
    {
        if (briefingPanel != null) briefingPanel.SetActive(false);
        if (messageDiodeLight != null) messageDiodeLight.SetActive(false);

        Time.timeScale = 1f;
        SetControlsActive(true); // Turn the cockpit ON
    }

    public void ToggleMessage()
    {
        if (briefingPanel != null && briefingPanel.activeSelf)
        {
            AcknowledgeAndStart();
        }
        else if (!string.IsNullOrEmpty(_lastMessage))
        {
            ShowMessage(_lastMessage, false); 
        }
    }

    // --- NEW: Helper method to loop through the list ---
    private void SetControlsActive(bool isActive)
    {
        foreach (MonoBehaviour controlScript in cockpitControls)
        {
            if (controlScript != null)
            {
                controlScript.enabled = isActive;
            }
        }
    }
}