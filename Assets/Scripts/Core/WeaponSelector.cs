using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSelector : MonoBehaviour
{
    public enum WeaponType { SemiActive, Active, AutoTarget }

    [Header("Current State")]
    public WeaponType currentWeapon = WeaponType.SemiActive;

    [Header("Button Setup")]
    public SpriteRenderer[] buttonRenderers;
    public Collider2D[] buttonColliders;

    [Header("Visuals")]
    public Sprite unpressedSprite;
    public Sprite pressedSprite;

    [Header("Indicators")]
    public GameObject arhIndicator;
    [Tooltip("Drag the VISUAL graphic of the SARH beam here (not the sensor itself, just the sprite)")]
    public GameObject sarhIndicatorVisual; // NEW

    void Start() { UpdateVisuals(); }

    void Update()
    {
        if (Mouse.current == null) return;
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            for (int i = 0; i < buttonColliders.Length; i++)
            {
                if (buttonColliders[i] != null && buttonColliders[i].OverlapPoint(mousePos))
                {
                    SelectWeapon(i);
                    break;
                }
            }
        }
    }

    public void SelectWeapon(int index)
    {
        currentWeapon = (WeaponType)index;
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        for (int i = 0; i < buttonRenderers.Length; i++)
        {
            if (buttonRenderers[i] != null)
                buttonRenderers[i].sprite = (i == (int)currentWeapon) ? pressedSprite : unpressedSprite;
        }

        // Toggle both visuals correctly!
        if (arhIndicator != null) arhIndicator.SetActive(currentWeapon == WeaponType.Active);
        if (sarhIndicatorVisual != null) sarhIndicatorVisual.SetActive(currentWeapon == WeaponType.SemiActive);
    }
}