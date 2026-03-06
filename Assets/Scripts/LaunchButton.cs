using UnityEngine;
using UnityEngine.InputSystem;

public class LaunchButton : MonoBehaviour
{
    [Header("Visuals")]
    public Sprite unpressedSprite;
    public Sprite pressedSprite;

    [Header("Connections")]
    public WeaponSystem weaponSystem;

    private SpriteRenderer _renderer;
    private Collider2D _myCollider;
    
    // We track these separately so holding the mouse and space at the same time doesn't glitch the visuals
    private bool _isMousePressed;
    private bool _isSpacePressed;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _myCollider = GetComponent<Collider2D>();
        
        // Start with default look
        if (unpressedSprite != null) _renderer.sprite = unpressedSprite;
    }

    void Update()
    {
        // 1. MOUSE INPUT
        if (Mouse.current != null)
        {
            bool clickDown = Mouse.current.leftButton.wasPressedThisFrame;
            bool clickUp = Mouse.current.leftButton.wasReleasedThisFrame;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            // Press
            if (clickDown && _myCollider.OverlapPoint(mousePos))
            {
                _isMousePressed = true;
                UpdateVisuals(true);
            }

            // Release
            if (clickUp && _isMousePressed)
            {
                _isMousePressed = false;
                UpdateVisuals(_isSpacePressed); // Only pop up if space isn't also being held

                // Fire only if mouse was released while hovering over the button
                if (_myCollider.OverlapPoint(mousePos))
                {
                    Fire();
                }
            }
        }

        // 2. KEYBOARD INPUT (Spacebar)
        if (Keyboard.current != null)
        {
            // Press Space
            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                _isSpacePressed = true;
                UpdateVisuals(true);
                Fire(); // Fire immediately on space down
            }

            // Release Space
            if (Keyboard.current.spaceKey.wasReleasedThisFrame)
            {
                _isSpacePressed = false;
                UpdateVisuals(_isMousePressed); // Only pop up if mouse isn't also being held
            }
        }
    }

    void UpdateVisuals(bool isDown)
    {
        if (isDown && pressedSprite != null)
        {
            _renderer.sprite = pressedSprite;
        }
        else if (!isDown && unpressedSprite != null)
        {
            _renderer.sprite = unpressedSprite;
        }
    }

    void Fire()
    {
        Debug.Log("Button Fire!");
        if (weaponSystem != null) weaponSystem.FireSequence();
    }
}