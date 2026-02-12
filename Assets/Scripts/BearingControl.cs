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

    [Header("Output")]
    public float currentBearing = 0f;

    private Collider2D _myCollider;
    private bool _isDragging = false;
    private Vector2 _lastMousePos;

    void Awake()
    {
        _myCollider = GetComponent<Collider2D>();
        if (radarIndicatorLine != null) radarIndicatorLine.gameObject.SetActive(false);
    }

    void Update()
    {
        if (!isControllable) return;
        if (Mouse.current == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        bool clickDown = Mouse.current.leftButton.wasPressedThisFrame;
        bool clickUp = Mouse.current.leftButton.wasReleasedThisFrame;
        bool isHovering = _myCollider.OverlapPoint(mouseWorldPos);

        //  SCHEME A: Scroll & Drag 
        if (inputType == ControlScheme.ScrollAndDrag)
        {
            if (isHovering && clickDown)
            {
                _isDragging = true;
                _lastMousePos = mouseScreenPos;
            }
            if (clickUp) _isDragging = false;

            float rotationAmount = 0f;

            // 1. Dragging
            if (_isDragging)
            {
                float deltaX = mouseScreenPos.x - _lastMousePos.x;
                rotationAmount = -deltaX * dragSensitivity;
                _lastMousePos = mouseScreenPos;
            }
            // 2. Scrolling
            else if (isHovering)
            {
                float scrollValue = Mouse.current.scroll.ReadValue().y;
                if (Mathf.Abs(scrollValue) > 0.01f)
                {
                    float direction = Mathf.Sign(scrollValue);
                    rotationAmount = direction * scrollSpeed * Time.deltaTime * 50f;
                }
            }

            // Apply Rotation
            if (Mathf.Abs(rotationAmount) > 0.001f)
            {
                // Rotate Knob
                transform.Rotate(0, 0, rotationAmount);
                
                // Sync Line & Update Bearing
                UpdateLineAndBearingFromKnob();
            }
        }
        
        // SCHEME B: Point & Pull 
        else
        {
            if (clickDown && isHovering) _isDragging = true;
            if (clickUp) _isDragging = false;

            if (_isDragging)
            {
                Vector2 direction = mouseWorldPos - (Vector2)transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                // Knob Rotation
                float knobRotation = angle - 90f + knobAngleOffset;
                transform.rotation = Quaternion.Euler(0, 0, knobRotation);
                
                // Line Rotation & Bearing
                if (radarIndicatorLine != null)
                {
                    float lineRotation = angle - 90f + lineAngleOffset;
                    radarIndicatorLine.rotation = Quaternion.Euler(0, 0, lineRotation);
                    currentBearing = lineRotation;
                }
            }
        }
    }

    // Helper function to ensure consistent math
    void UpdateLineAndBearingFromKnob()
    {
        if (radarIndicatorLine != null)
        {
            float relativeOffset = lineAngleOffset - knobAngleOffset;
            
            float finalLineAngle = transform.eulerAngles.z + relativeOffset;
            
            radarIndicatorLine.rotation = Quaternion.Euler(0, 0, finalLineAngle);
            
            currentBearing = finalLineAngle;
        }
        else
        {
            currentBearing = transform.eulerAngles.z;
        }
    }
}