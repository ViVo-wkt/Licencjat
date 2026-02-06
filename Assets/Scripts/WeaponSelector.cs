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
        if (currentWeapon == WeaponType.SemiActive)
        {
            // 1. Swap Buttons
            sarhRenderer.sprite = sarhPressed;
            arhRenderer.sprite = arhUnpressed;

            // 2. Swap Radar Tools
            if (sarhBeamObject != null) sarhBeamObject.SetActive(true);
            if (arhlineObject != null) arhlineObject.SetActive(false);

            if (bearingKnobScript != null) bearingKnobScript.isControllable = false; // LOCK KNOB
        }
        else
        {
            // 1. Swap Buttons
            sarhRenderer.sprite = sarhUnpressed;
            arhRenderer.sprite = arhPressed;

            // 2. Swap Radar Tools
            if (sarhBeamObject != null) sarhBeamObject.SetActive(false);
            if (arhlineObject != null) arhlineObject.SetActive(true);

            if (bearingKnobScript != null) bearingKnobScript.isControllable = true; // UNLOCK KNOB
        }
    }
}