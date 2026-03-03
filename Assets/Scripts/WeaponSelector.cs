using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    public enum WeaponType { SemiActive, FireAndForget }
    public WeaponType currentWeapon = WeaponType.SemiActive;

    [Header("SARH Configuration")]
    public GameObject sarhBeamObject; // The Active_Beam GameObject
    public SpriteRenderer sarhRenderer; // Button visual
    public Sprite sarhUnpressed;
    public Sprite sarhPressed;
    public RadarKnob sarhKnobScript; // DRAG "Radar_Knob" HERE

    [Header("ARH Configuration")]
    public GameObject arhlineObject; 
    public SpriteRenderer arhRenderer; // Button visual
    public Sprite arhUnpressed;
    public Sprite arhPressed; 
    public BearingControl bearingKnobScript; // DRAG "Knob_Bearing" HERE

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
        // 1. Get Components on the Beam
        SpriteRenderer beamSprite = null;
        ActiveRadarSensor sensor = null;

        if (sarhBeamObject != null)
        {
            // Ensure object is ACTIVE so scripts run
            sarhBeamObject.SetActive(true); 
            
            beamSprite = sarhBeamObject.GetComponent<SpriteRenderer>();
            sensor = sarhBeamObject.GetComponent<ActiveRadarSensor>();
        }

        // 2. Apply Logic
        if (currentWeapon == WeaponType.SemiActive)
        {
            // --- SARH MODE ---
            if (sarhRenderer) sarhRenderer.sprite = sarhPressed;
            if (arhRenderer) arhRenderer.sprite = arhUnpressed;

            // SARH: Visible, Controllable, Markers ON
            if (beamSprite) beamSprite.enabled = true;
            if (sarhKnobScript) sarhKnobScript.isControllable = true;
            if (sensor) sensor.SetGhostMode(false);

            // ARH: Hidden, Locked
            if (arhlineObject) arhlineObject.SetActive(false);
            if (bearingKnobScript) bearingKnobScript.isControllable = false;
        }
        else
        {
            // --- ARH MODE ---
            if (sarhRenderer) sarhRenderer.sprite = sarhUnpressed;
            if (arhRenderer) arhRenderer.sprite = arhPressed;

            // SARH: Invisible (Ghost), Locked, Markers OFF
            if (beamSprite) beamSprite.enabled = false;
            if (sarhKnobScript) sarhKnobScript.isControllable = false; // LOCK IT
            if (sensor) sensor.SetGhostMode(true); // GHOST MODE

            // ARH: Visible, Controllable
            if (arhlineObject) arhlineObject.SetActive(true);
            if (bearingKnobScript) bearingKnobScript.isControllable = true;
        }
    }
}