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

    void Start()
    {
        // Force the visuals to match the starting weapon immediately
        UpdateVisuals();
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // Detect a mouse click
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            // Check if the click landed on any of our 3 buttons
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
        // Loop through all buttons and set the correct sprite
        for (int i = 0; i < buttonRenderers.Length; i++)
        {
            if (buttonRenderers[i] != null)
            {
                if (i == (int)currentWeapon)
                {
                    buttonRenderers[i].sprite = pressedSprite; // Pushed down
                }
                else
                {
                    buttonRenderers[i].sprite = unpressedSprite; // Popped up
                }
            }
        }
    }
}