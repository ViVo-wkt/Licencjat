using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    public enum WeaponType { SemiActive, FireAndForget }
    public WeaponType currentWeapon = WeaponType.SemiActive;

    [Header("SARH Configuration")]
    public GameObject sarhBeamObject; // The Active Radar Beam (Orange)
    public SpriteRenderer sarhRenderer; 
    public Sprite sarhUnpressed;
    public Sprite sarhPressed; 

    [Header("ARH Configuration")]
    public GameObject arhlineObject; // The new Compass Line
    public SpriteRenderer arhRenderer; 
    public Sprite arhUnpressed;
    public Sprite arhPressed; 

    [Header("Controls")]
    public BearingControl bearingKnobScript;

    void Start()
    {
        UpdateVisuals();
    }

    public void SelectSemiActive()
    {
        currentWeapon = WeaponType.SemiActive;
        UpdateVisuals();
    }

    public void SelectActive()
    {
        currentWeapon = WeaponType.FireAndForget;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        // 1. Get the SpriteRenderer from the beam object
        // (Make sure your Active_Beam object has a SpriteRenderer on it!)
        SpriteRenderer beamSprite = null;
        if (sarhBeamObject != null) beamSprite = sarhBeamObject.GetComponent<SpriteRenderer>();

        if (currentWeapon == WeaponType.SemiActive)
        {
            // --- SARH MODE ---
            sarhRenderer.sprite = sarhPressed;
            arhRenderer.sprite = arhUnpressed;

            // Enable Beam VISUALS
            if (beamSprite != null) beamSprite.enabled = true; 
            
            // Hide ARH Line
            if (arhlineObject != null) arhlineObject.SetActive(false);

            // Unlock Knob
            if (bearingKnobScript != null) bearingKnobScript.isControllable = false;
        }
        else
        {
            // --- ARH MODE ---
            sarhRenderer.sprite = sarhUnpressed;
            arhRenderer.sprite = arhPressed;

            // Disable Beam VISUALS only (Logic stays active!)
            if (beamSprite != null) beamSprite.enabled = false;

            // Show ARH Line
            if (arhlineObject != null) arhlineObject.SetActive(true);

            // Lock Knob
            if (bearingKnobScript != null) bearingKnobScript.isControllable = true;
        }
    }
}