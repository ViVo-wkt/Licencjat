using UnityEngine;
using UnityEngine.InputSystem;

public class RadarKnob : MonoBehaviour
{
    [Header("Connections")]
    public Transform linkedBeam; 

    [Header("Settings")]
    public float scrollSpeed = 10f;
    public float dragSensitivity = 0.5f; 
    public float angleOffset = 0f; // NEW: Adjust this to sync visual orientation

    private Collider2D _myCollider;
    private bool _isDragging = false;
    private Vector2 _lastMousePos;

    void Awake()
    {
        _myCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        if (Mouse.current == null) return;

        // --- Input Logic (Same as before) ---
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        bool isLeftClickDown = Mouse.current.leftButton.wasPressedThisFrame;
        bool isLeftClickReleased = Mouse.current.leftButton.wasReleasedThisFrame;
        bool isHovering = _myCollider.OverlapPoint(mouseWorldPos);

        if (isHovering && isLeftClickDown)
        {
            _isDragging = true;
            _lastMousePos = mouseScreenPos;
        }
        if (isLeftClickReleased) _isDragging = false;

        float rotationAmount = 0f;

        if (_isDragging)
        {
            float deltaX = mouseScreenPos.x - _lastMousePos.x;
            rotationAmount = -deltaX * dragSensitivity;
            _lastMousePos = mouseScreenPos;
        }
        else if (isHovering)
        {
            float scrollValue = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(scrollValue) > 0.01f)
            {
                float direction = Mathf.Sign(scrollValue);
                rotationAmount = direction * scrollSpeed * Time.deltaTime * 50f;
            }
        }

        // --- Rotation Application (Updated) ---
        if (Mathf.Abs(rotationAmount) > 0.001f || linkedBeam != null)
        {
            // Rotate the Knob itself
            transform.Rotate(0, 0, rotationAmount);

            // Rotate the Beam to match Knob + Offset
            if (linkedBeam != null)
            {
                // We set the beam's Z rotation to equal Knob's Z rotation + Offset
                float currentKnobAngle = transform.eulerAngles.z;
                linkedBeam.rotation = Quaternion.Euler(0, 0, currentKnobAngle + angleOffset);
            }
        }
    }
}