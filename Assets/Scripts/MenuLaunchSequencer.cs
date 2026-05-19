using UnityEngine;

public class MenuLaunchSequencer : MonoBehaviour
{
    [Header("Hardware")]
    public SliderSwitch modeSwitch;
    
    // CHANGED: We now ask for the MenuButton script directly instead of a Collider!
    [Tooltip("Drag your final Play button here!")]
    public MenuButton finalPlayButton; 
    
    [Header("Visual Feedback (Optional)")]
    [Tooltip("What to show when unlocked (e.g., glowing text or active light)")]
    public GameObject armedVisuals;
    
    [Tooltip("What to show when locked (e.g., dimmed text or red light)")]
    public GameObject lockedVisuals;

    void Update()
    {
        if (modeSwitch == null || finalPlayButton == null) return;

        // Check if the player has pulled the switch to the right
        bool isArmed = modeSwitch.isOnRightSide;

        // Turn the button's ability to be clicked ON or OFF
        // (Disabling the script stops it from reading the mouse entirely!)
        finalPlayButton.enabled = isArmed;

        // Swap the visuals so the player knows it's unlocked
        if (armedVisuals != null) armedVisuals.SetActive(isArmed);
        if (lockedVisuals != null) lockedVisuals.SetActive(!isArmed);
    }
}