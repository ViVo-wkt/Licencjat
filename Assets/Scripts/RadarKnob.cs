using UnityEngine;
using UnityEngine.InputSystem;

public class RadarKnob : MonoBehaviour
{
    // NEW: Creates a dropdown menu in the Inspector
    public enum RotationAxis { X, Y, Z }

    [Header("Settings")]
    public bool isControllable = true; 
    public float rotationSpeed = 10f;
    public float dragSensitivity = 0.5f;

    [Tooltip("Which axis should the physical 3D knob rotate around?")]
    public RotationAxis knobAxis = RotationAxis.Z; // Change this directly in Unity!

    [Header("Calibration")]
    public float rotationOffset = 90f; 

    [Header("References")]
    public Transform radarBeam;

    // Upgraded to support both 2D and 3D colliders!
    private Collider2D _myCollider2D;
    private Collider _myCollider3D;
    private Camera _mainCam;
    
    private bool _isDragging = false;
    private Vector2 _lastMousePos;

    void Awake()
    {
        // The script checks for whichever collider you have attached
        _myCollider2D = GetComponent<Collider2D>();
        _myCollider3D = GetComponent<Collider>();
        _mainCam = Camera.main;
    }

    void Update()
    {
        if (!isControllable) return;
        if (Mouse.current == null || _mainCam == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        
        // --- 1. HOVER DETECTION (2D & 3D SUPPORT) ---
        bool isHovering = false;
        
        if (_myCollider2D != null)
        {
            Vector2 mouseWorldPos = _mainCam.ScreenToWorldPoint(mouseScreenPos);
            isHovering = _myCollider2D.OverlapPoint(mouseWorldPos);
        }
        else if (_myCollider3D != null)
        {
            Ray ray = _mainCam.ScreenPointToRay(mouseScreenPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider == _myCollider3D) isHovering = true;
            }
        }
        
        // --- 2. INPUT DETECTION ---
        bool clickDown = Mouse.current.leftButton.wasPressedThisFrame;
        bool clickUp = Mouse.current.leftButton.wasReleasedThisFrame;

        if (isHovering && clickDown)
        {
            _isDragging = true;
            _lastMousePos = mouseScreenPos;
        }
        if (clickUp) _isDragging = false;

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
                rotationAmount = direction * rotationSpeed * Time.deltaTime * 50f;
            }
        }

        // --- 3. APPLY ROTATION ---
        if (Mathf.Abs(rotationAmount) > 0.001f)
        {
            // Apply rotation to the axis you selected in the Inspector
            Vector3 rotationVector = Vector3.zero;
            if (knobAxis == RotationAxis.X) rotationVector.x = rotationAmount;
            else if (knobAxis == RotationAxis.Y) rotationVector.y = rotationAmount;
            else if (knobAxis == RotationAxis.Z) rotationVector.z = rotationAmount;
            
            transform.Rotate(rotationVector);

            // Sync the 2D Radar Beam to match the 3D Knob
            if (radarBeam != null)
            {
                // Read the angle from the correct axis
                float knobAngle = 0f;
                if (knobAxis == RotationAxis.X) knobAngle = transform.eulerAngles.x;
                else if (knobAxis == RotationAxis.Y) knobAngle = transform.eulerAngles.y;
                else if (knobAxis == RotationAxis.Z) knobAngle = transform.eulerAngles.z;
                
                // The 2D radar beam itself is always rotated on the Z axis
                radarBeam.rotation = Quaternion.Euler(0, 0, knobAngle + rotationOffset);
            }
        }
    }
}