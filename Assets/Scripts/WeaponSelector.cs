using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    public enum WeaponType { SemiActive, FireAndForget }
    public WeaponType currentWeapon = WeaponType.SemiActive;

    [Header("SARH Configuration")]
    public SpriteRenderer sarhRenderer; // The Sprite Renderer on the SARH button object
    public Sprite sarhUnpressed;
    public Sprite sarhPressed; // "Lit up" or pushed in

    [Header("ARH Configuration")]
    public SpriteRenderer arhRenderer; // The Sprite Renderer on the ARH button object
    public Sprite arhUnpressed;
    public Sprite arhPressed; // "Lit up" or pushed in

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
            // SARH is ON (Pressed), ARH is OFF (Unpressed)
            sarhRenderer.sprite = sarhPressed;
            arhRenderer.sprite = arhUnpressed;
        }
        else
        {
            // SARH is OFF (Unpressed), ARH is ON (Pressed)
            sarhRenderer.sprite = sarhUnpressed;
            arhRenderer.sprite = arhPressed;
        }
    }
}