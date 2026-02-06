using UnityEngine;
using UnityEngine.InputSystem;

public class RadarKnob : MonoBehaviour
{
    [Header("Connections")]
    public Transform linkedBeam;

    [Header("Settings")]
    public float scrollSpeed = 10f;
    public float dragSensitivity = 0.5f; // Sensitivity for mouse movement

    private Collider2D _myCollider;
    private bool _isDragging = false;

    void Awake()
    {
        _myCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        
        bool isHovering = _myCollider.OverlapPoint(mouseWorldPos);
        bool isLeftClickDown = Mouse.current.leftButton.wasPressedThisFrame;
        bool isLeftClickHeld = Mouse.current.leftButton.isPressed;

        // 1. Handle Drag Start
        if (isHovering && isLeftClickDown)
        {
            _isDragging = true;
        }

        // 2. Handle Drag End
        if (!isLeftClickHeld)
        {
            _isDragging = false;
        }

        float rotationAmount = 0f;

        // 3. Priority: Dragging overrides Scrolling
        if (_isDragging)
        {
            // Get mouse movement X (Left/Right)
            float mouseDeltaX = Mouse.current.delta.x.ReadValue();
            
            // Invert direction if you want "pulling" feel vs "pushing" feel
            rotationAmount = -mouseDeltaX * dragSensitivity;
        }
        else if (isHovering)
        {
            // 4. Fallback: Scroll Wheel
            float scrollValue = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(scrollValue) > 0.01f)
            {
                float direction = Mathf.Sign(scrollValue);
                rotationAmount = direction * scrollSpeed * Time.deltaTime * 50f;
            }
        }

        // 5. Apply Rotation
        if (Mathf.Abs(rotationAmount) > 0.001f)
        {
            // Rotate Knob
            transform.Rotate(0, 0, rotationAmount);

            // Rotate Beam
            if (linkedBeam != null)
            {
                linkedBeam.rotation = transform.rotation;
            }
        }
    }
}