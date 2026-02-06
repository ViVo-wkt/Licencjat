using UnityEngine;
using UnityEngine.InputSystem;

public class BearingControl : MonoBehaviour
{
    [Header("Visual Connections")]
    public Transform radarIndicatorLine; // The simple line on the radar screen

    [Header("Output")]
    public float currentBearing = 0f; // WeaponSystem reads this

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
            
            // Calculate angle in degrees (0 is Up)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            
            // Apply rotation to Knob
            transform.rotation = Quaternion.Euler(0, 0, angle);
            
            // Apply rotation to Radar Line
            if (radarIndicatorLine != null)
            {
                radarIndicatorLine.rotation = Quaternion.Euler(0, 0, angle);
            }

            currentBearing = angle;
        }
    }
}