using UnityEngine;
using UnityEngine.UI;

public class WeaponSelector : MonoBehaviour
{
    public enum WeaponType { SemiActive, FireAndForget }
    
    [Header("State")]
    public WeaponType currentWeapon = WeaponType.SemiActive;

    [Header("Button References")]
    // Assign the GameObjects that hold your SpriteRenderers or UI Buttons
    public GameObject sarhButtonObj; 
    public GameObject arhButtonObj;

    [Header("Visuals")]
    public Sprite sarhSelected;
    public Sprite sarhUnselected;
    public Sprite arhSelected;
    public Sprite arhUnselected;

    private SpriteRenderer _sarhRenderer;
    private SpriteRenderer _arhRenderer;

    void Start()
    {
        // Get renderers
        _sarhRenderer = sarhButtonObj.GetComponent<SpriteRenderer>();
        _arhRenderer = arhButtonObj.GetComponent<SpriteRenderer>();
        
        // Initialize state
        UpdateVisuals();
    }

    // Call this when clicking the Semi-Active Button
    public void SelectSemiActive()
    {
        currentWeapon = WeaponType.SemiActive;
        UpdateVisuals();
    }

    // Call this when clicking the Active Button
    public void SelectActive()
    {
        currentWeapon = WeaponType.FireAndForget;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (currentWeapon == WeaponType.SemiActive)
        {
            _sarhRenderer.sprite = sarhSelected;
            _arhRenderer.sprite = arhUnselected;
        }
        else
        {
            _sarhRenderer.sprite = sarhUnselected;
            _arhRenderer.sprite = arhSelected;
        }
    }
}