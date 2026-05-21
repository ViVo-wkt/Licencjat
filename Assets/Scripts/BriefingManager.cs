using UnityEngine;
using TMPro;

public class BriefingManager : MonoBehaviour
{
    [Header("UI Setup")]
    public GameObject briefingPanel; 
    public TMP_Text briefingTextDisplay;

    [Header("Indicators")]
    public GameObject messageDiodeLight;

    [Header("Audio")]
    public AudioSource notificationSound;
    private bool _hasPlayedIntroSound = false;

    [Header("Cockpit Controls")]
    public MonoBehaviour[] cockpitControls;

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

        // One-time sound latch
        if (notificationSound != null && !_hasPlayedIntroSound)
        {
            notificationSound.Play();       
            _hasPlayedIntroSound = true;    
        }

        if (freezeTime) 
        {
            Time.timeScale = 0f;
            SetControlsActive(false); 
        }
    }

    public void AcknowledgeAndStart()
    {
        if (briefingPanel != null) briefingPanel.SetActive(false);
        if (messageDiodeLight != null) messageDiodeLight.SetActive(false);

        Time.timeScale = 1f;
        SetControlsActive(true); 
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

    private void SetControlsActive(bool isActive)
    {
        if (cockpitControls == null) return;
        
        foreach (var control in cockpitControls)
        {
            if (control != null) control.enabled = isActive;
        }
    }
}