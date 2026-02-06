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
    private bool _isPressed;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _myCollider = GetComponent<Collider2D>();
        
        // Start with default look
        if (unpressedSprite != null) _renderer.sprite = unpressedSprite;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        bool clickDown = Mouse.current.leftButton.wasPressedThisFrame;
        bool clickUp = Mouse.current.leftButton.wasReleasedThisFrame;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        // Check if we clicked ON the button
        if (clickDown && _myCollider.OverlapPoint(mousePos))
        {
            Press();
        }

        // Release anywhere (standard button behavior)
        if (clickUp && _isPressed)
        {
            Release(mousePos);
        }
    }

    void Press()
    {
        _isPressed = true;
        if (pressedSprite != null) _renderer.sprite = pressedSprite;
    }

    void Release(Vector2 mousePos)
    {
        _isPressed = false;
        if (unpressedSprite != null) _renderer.sprite = unpressedSprite;

        // Only fire if we release the mouse WHILE still hovering over the button
        if (_myCollider.OverlapPoint(mousePos))
        {
            Debug.Log("Button Fire!");
            if (weaponSystem != null) weaponSystem.FireSequence();
        }
    }
}