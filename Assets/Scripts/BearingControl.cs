using UnityEngine;
using UnityEngine.InputSystem;

public class BearingControl : MonoBehaviour
{
    public enum ControlScheme { ScrollAndDrag, PointAndPull }

    [Header("Settings")]
    public bool isControllable = true;
    public ControlScheme inputType = ControlScheme.ScrollAndDrag;

    [Header("Scroll & Drag Settings")]
    public float scrollSpeed = 10f;
    public float dragSensitivity = 0.5f;

    [Header("Calibration")]
    public float knobAngleOffset = 0f;
    public float lineAngleOffset = 0f;

    [Header("Visual Connections")]
    public Transform radarIndicatorLine; 

    // --- NEW: Mechanical Limits ---
    [Header("Mechanical Limits")]
    [Tooltip("How fast the heavy dish physically turns (Degrees per second)")]
    public float maxTurnSpeed = 45f; 
    // ------------------------------

    [Header("Output")]
    public float currentBearing = 0f;
    private float _targetBearing = 0f; // Hidden target that the player actually controls

    private Collider2D _myCollider;
    private bool _isDragging = false;
    private Vector2 _lastMousePos;

    void Awake()
    {
        _myCollider = GetComponent<Collider2D>();
        if (radarIndicatorLine != null) radarIndicatorLine.gameObject.SetActive(false);

        // Sync the starting target so the beam doesn't wildly sweep the moment the game starts
        float relativeOffset = lineAngleOffset - knobAngleOffset;
        _targetBearing = transform.eulerAngles.z + relativeOffset;
        currentBearing = _targetBearing;
    }

    void Update()
    {
        if (!isControllable) return;
        if (Mouse.current == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        bool clickDown = Mouse.current.leftButton.wasPressedThisFrame;
        bool clickUp = Mouse.current.leftButton.wasReleasedThisFrame;

        bool isHovering = (_myCollider != null && _myCollider.OverlapPoint(mouseWorldPos));

        if (inputType == ControlScheme.ScrollAndDrag)
        {
            if (isHovering && clickDown)
            {
                _isDragging = true;
                _lastMousePos = mouseScreenPos;
            }
            if (clickUp) _isDragging = false;

            float deltaAngle = 0f;

            if (_isDragging)
            {
                Vector2 mouseDelta = mouseScreenPos - _lastMousePos;
                deltaAngle = -mouseDelta.x * dragSensitivity;
                _lastMousePos = mouseScreenPos;
            }
            else if (isHovering)
            {
                float scrollValue = Mouse.current.scroll.ReadValue().y;
                if (Mathf.Abs(scrollValue) > 0.01f)
                {
                    deltaAngle = -Mathf.Sign(scrollValue) * scrollSpeed;
                }
            }

            if (Mathf.Abs(deltaAngle) > 0.001f)
            {
                Vector3 currentEuler = transform.eulerAngles;
                // The Knob physically rotates instantly
                transform.rotation = Quaternion.Euler(currentEuler.x, currentEuler.y, currentEuler.z + deltaAngle);
                
                // But we only update our TARGET bearing, not the actual beam!
                UpdateTargetBearingFromKnob();
            }
        }
        else
        {
            if (clickDown && isHovering) _isDragging = true;
            if (clickUp) _isDragging = false;

            if (_isDragging)
            {
                Vector2 direction = mouseWorldPos - (Vector2)transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Knob rotates instantly
                float knobRotation = angle - 90f + knobAngleOffset;
                transform.rotation = Quaternion.Euler(0, 0, knobRotation);
                
                // Update Target Bearing
                _targetBearing = angle - 90f + lineAngleOffset;
            }
        }

        // --- THE MECHANICAL DELAY ---
        // Smoothly move the heavy machinery toward the target bearing the player requested
        currentBearing = Mathf.MoveTowardsAngle(currentBearing, _targetBearing, maxTurnSpeed * Time.deltaTime);

        // Apply the lagging visual rotation to the glowing line on the glass
        if (radarIndicatorLine != null)
        {
            radarIndicatorLine.rotation = Quaternion.Euler(0, 0, currentBearing);
        }
    }

    // Helper function to figure out where the beam SHOULD be based on the dial
    void UpdateTargetBearingFromKnob()
    {
        float relativeOffset = lineAngleOffset - knobAngleOffset;
        _targetBearing = transform.eulerAngles.z + relativeOffset;
    }
}