using UnityEngine;
using TMPro;

public class BriefingManager : MonoBehaviour
{
    [Header("UI Setup")]
    [Tooltip("The main UI Panel containing the text and button")]
    public GameObject briefingPanel; 
    [Tooltip("The TextMeshPro element that displays the message")]
    public TMP_Text briefingTextDisplay;

    [Header("Mission Details")]
    [TextArea(5, 10)] // This makes the text box bigger in the Inspector!
    public string missionBriefing = "COMMANDER:\n\nMultiple unidentified contacts detected on radar. Intercept and destroy all hostile targets.\n\nGood luck.";

    void Start()
    {
        // The moment the scene loads, show the panel and freeze time!
        if (briefingPanel != null)
        {
            briefingPanel.SetActive(true);
            
            if (briefingTextDisplay != null)
            {
                briefingTextDisplay.text = missionBriefing;
            }

            // Freeze the game engine
            Time.timeScale = 0f;
        }
    }

    public void AcknowledgeAndStart()
    {
        // Hide the panel
        if (briefingPanel != null)
        {
            briefingPanel.SetActive(false);
        }

        // Unfreeze the game!
        Time.timeScale = 1f;
    }
}