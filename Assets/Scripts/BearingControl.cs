using UnityEngine;
using UnityEngine.InputSystem;

public class BearingControl : MonoBehaviour
{
    [Header("Visual Connections")]
    public Transform radarIndicatorLine; 

    [Header("Calibration")]
    public float knobAngleOffset = 0f; // Tweak this if knob points wrong way
    public float lineAngleOffset = 0f; // Tweak this if line doesn't match knob

    [Header("Output")]
    public float currentBearing = 0f;

    private Collider2D _myCollider;
    private bool _isDragging = false;

    void Awake()
    {
        _myCollider = GetComponent<Collider2D>();
        
        // Hide the line by default (handled by WeaponSelector later)
        if (radarIndicatorLine != null) 
            radarIndicatorLine.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        
        bool clickDown = Mouse.current.leftButton.wasPressedThisFrame;
        bool clickUp = Mouse.current.leftButton.wasReleasedThisFrame;

        // 1. Start Dragging
        if (clickDown && _myCollider.OverlapPoint(mouseWorldPos))
        {
            _isDragging = true;
        }

        // 2. Stop Dragging
        if (clickUp)
        {
            _isDragging = false;
        }

        // 3. Calculate Angle
        if (_isDragging)
        {
            Vector2 direction = mouseWorldPos - (Vector2)transform.position;
            
            // Basic angle (0 is Right, 90 is Up)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // Apply rotation to Knob (with offset)
            // -90 is a standard correction because Unity sprites usually point Up, but Atan2 assumes Right.
            float knobRotation = angle - 90f + knobAngleOffset;
            transform.rotation = Quaternion.Euler(0, 0, knobRotation);
            
            // Apply rotation to Radar Line (with its own offset)
            if (radarIndicatorLine != null)
            {
                float lineRotation = angle - 90f + lineAngleOffset;
                radarIndicatorLine.rotation = Quaternion.Euler(0, 0, lineRotation);
                
                // Keep the internal bearing matching the LINE, which is what the missile uses
                currentBearing = lineRotation;
            }
        }
    }
}