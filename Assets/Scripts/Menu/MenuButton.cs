using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections;

public class MenuButton : MonoBehaviour
{
    [Header("Visuals")]
    public Sprite unpressedSprite;
    public Sprite pressedSprite;
    private SpriteRenderer _spriteRenderer;

    [Header("Settings")]
    public float actionDelay = 0.2f;
    
    [Header("Action")]
    public UnityEvent onClickAction;

    private Collider2D _col2D;
    private Collider _col3D; // Future-proofed in case you change to 3D buttons
    private Camera _cam;
    private bool _isPressed = false;

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _col2D = GetComponent<Collider2D>();
        _col3D = GetComponent<Collider>();
        _cam = Camera.main;
        
        if (_spriteRenderer != null && unpressedSprite != null)
            _spriteRenderer.sprite = unpressedSprite;
    }

    void Update()
    {
        if (_isPressed || Mouse.current == null || _cam == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            bool hit = false;

            // Support both 2D and 3D colliders
            if (_col2D != null) 
            {
                if (_col2D.OverlapPoint(_cam.ScreenToWorldPoint(mouseScreenPos))) hit = true;
            }
            else if (_col3D != null) 
            {
                Ray ray = _cam.ScreenPointToRay(mouseScreenPos);
                if (Physics.Raycast(ray, out RaycastHit hitInfo) && hitInfo.collider == _col3D) hit = true;
            }

            if (hit) StartCoroutine(PressRoutine());
        }
    }

    IEnumerator PressRoutine()
    {
        _isPressed = true;
        
        // Swap to pressed visual
        if (_spriteRenderer != null && pressedSprite != null)
            _spriteRenderer.sprite = pressedSprite;

        // Wait for the delay
        yield return new WaitForSeconds(actionDelay);

        // Execute the assigned action
        onClickAction.Invoke();

        // Swap back to normal visual (in case this is the Options/Back button)
        if (_spriteRenderer != null && unpressedSprite != null)
            _spriteRenderer.sprite = unpressedSprite;
            
        _isPressed = false;
    }
}