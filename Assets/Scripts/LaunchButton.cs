using UnityEngine;
using UnityEngine.InputSystem;

public class LaunchButton : MonoBehaviour
{
    [Header("2D Visuals")]
    public Sprite unpressedSprite;
    public Sprite pressedSprite;

    // --- NEW SECTION ---
    [Header("3D Visuals (Optional)")]
    [Tooltip("Drag your 3D button model here")]
    public Renderer buttonMeshRenderer; 
    
    [Tooltip("Drag your raw .jpg / .png textures here")]
    public Texture2D unpressedTexture;
    public Texture2D pressedTexture;
    // -------------------

    [Header("Connections")]
    public WeaponSystem weaponSystem;

    private SpriteRenderer _renderer;
    private Collider2D _myCollider;
    
    // We track these separately so holding the mouse and spacebar at the same time doesn't double-fire
    private bool _isPressed = false;
    private float _resetTimer = 0f;
    private float _pressedDuration = 0.2f;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _myCollider = GetComponent<Collider2D>();
        
        // Ensure we start in the unpressed visual state
        ResetVisuals();
    }

    void Update()
    {
        // 1. Handle visual resetting after the button is pressed
        if (_isPressed)
        {
            _resetTimer -= Time.deltaTime;
            if (_resetTimer <= 0)
            {
                _isPressed = false;
                ResetVisuals();
            }
        }

        // 2. Handle Inputs
        if (Mouse.current == null || Keyboard.current == null) return;

        bool spacebarPressed = Keyboard.current.spaceKey.wasPressedThisFrame;
        bool clickPressed = false;

        // Check if the physical mouse clicked exactly on the button's 2D collider
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            if (_myCollider != null && _myCollider.OverlapPoint(mousePos))
            {
                clickPressed = true;
            }
        }

        // 3. Trigger the Launch!
        if ((spacebarPressed || clickPressed) && !_isPressed)
        {
            TriggerLaunch();
        }
    }

    private void TriggerLaunch()
    {
        _isPressed = true;
        _resetTimer = _pressedDuration;

        // Swap 2D Sprite
        if (_renderer != null) _renderer.sprite = pressedSprite;

        // Swap 3D Material Texture
        if (buttonMeshRenderer != null && pressedTexture != null)
        {
            buttonMeshRenderer.material.mainTexture = pressedTexture;
        }

        // Fire the weapon! 
        if (weaponSystem != null)
        {
            // IMPORTANT: If your original script used a slightly different method name 
            // (like LaunchMissile() or FireSelectedWeapon()), just update this single line to match!
            weaponSystem.FireSelectedWeapon();
        }
    }

    private void ResetVisuals()
    {
        // Reset 2D Sprite
        if (_renderer != null) _renderer.sprite = unpressedSprite;

        // Reset 3D Material Texture
        if (buttonMeshRenderer != null && unpressedTexture != null)
        {
            buttonMeshRenderer.material.mainTexture = unpressedTexture;
        }
    }
}