using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSelector : MonoBehaviour
{
    public enum WeaponType
    {
        SemiActive, // 0 (SARH)
        Active,     // 1 (ARH)
        AutoTarget  // 2 (AUTO)
    }

    [Header("Current State")]
    public WeaponType currentWeapon = WeaponType.SemiActive;

    [Header("Button Setup")]
    [Tooltip("Element 0 = SARH, Element 1 = ARH, Element 2 = AUTO")]
    public SpriteRenderer[] buttonRenderers;
    public Collider2D[] buttonColliders;

    [Header("Visuals")]
    public Sprite unpressedSprite;
    public Sprite pressedSprite;

    [Header("Indicators")]
    [Tooltip("Drag your ARH Indicator (the cone/line) here!")]
    public GameObject arhIndicator;

    void Start()
    {
        UpdateVisuals();
    }

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
        // 1. Swap button graphics
        for (int i = 0; i < buttonRenderers.Length; i++)
        {
            if (buttonRenderers[i] != null)
            {
                buttonRenderers[i].sprite = (i == (int)currentWeapon) ? pressedSprite : unpressedSprite;
            }
        }

        // 2. Toggle the ARH visual indicator!
        if (arhIndicator != null)
        {
            arhIndicator.SetActive(currentWeapon == WeaponType.Active);
        }
    }
}