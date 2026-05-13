using UnityEngine;
using TMPro;

public class BriefingManager : MonoBehaviour
{
    [Header("UI Setup")]
    public GameObject briefingPanel; 
    public TMP_Text briefingTextDisplay;

    [Header("Indicators")]
    public GameObject messageDiodeLight;

    [Header("Mission Details")]
    [TextArea(5, 10)] 
    public string missionBriefing = "COMMANDER:\n\nMultiple unidentified contacts detected on radar. Intercept and destroy all hostile targets.\n\nGood luck.";

    // --- NEW: Memory Cache ---
    private string _lastMessage = "";

    void Start()
    {
        // When the scene starts, show the initial briefing and freeze time
        ShowMessage(missionBriefing, true);
    }

    public void ShowMessage(string messageText, bool freezeTime = false)
    {
        // Save the text so the player can recall it later!
        _lastMessage = messageText;

        if (briefingPanel != null) briefingPanel.SetActive(true);
        if (briefingTextDisplay != null) briefingTextDisplay.text = messageText;
        if (messageDiodeLight != null) messageDiodeLight.SetActive(true);

        if (freezeTime) Time.timeScale = 0f;
    }

    public void AcknowledgeAndStart()
    {
        if (briefingPanel != null) briefingPanel.SetActive(false);
        if (messageDiodeLight != null) messageDiodeLight.SetActive(false);

        Time.timeScale = 1f;
    }

    // --- NEW: Toggle Logic ---
    public void ToggleMessage()
    {
        // If the panel is currently open, close it!
        if (briefingPanel != null && briefingPanel.activeSelf)
        {
            AcknowledgeAndStart();
        }
        // If the panel is closed (and we actually have a message), open it!
        else if (!string.IsNullOrEmpty(_lastMessage))
        {
            // We pass 'false' for freezeTime so recalling a message mid-flight 
            // doesn't pause the chaos!
            ShowMessage(_lastMessage, false); 
        }
    }
}