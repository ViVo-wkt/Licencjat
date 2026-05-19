using UnityEngine;
using UnityEngine.InputSystem;

public class SliderSwitch : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag the child object that physically slides left and right into here.")]
    public Transform slidingPart;

    [Header("Rail Limits (Local Z Coordinate)")]
    [Tooltip("The Z position when the switch is fully to the Left.")]
    public float leftLocalZ = -0.5f;
    [Tooltip("The Z position when the switch is fully to the Right.")]
    public float rightLocalZ = 0.5f;

    [Header("Interaction Settings")]
    public float dragSensitivity = 0.005f;
    [Tooltip("How fast it snaps into place after letting go of the mouse.")]
    public float snapSpeed = 5f;

    [Header("Output State")]
    [Tooltip("Is the switch currently resting on the Right side?")]
    public bool isOnRightSide = false;

    private Collider _myCollider;
    private Camera _mainCam;
    private bool _isDragging = false;
    
    private float _currentLocalZ;
    private float _targetLocalZ;

    void Start()
    {
        // Automatically find the 3D collider (Make sure the collider covers the whole rail!)
        _myCollider = GetComponent<Collider>();
        if (_myCollider == null) _myCollider = GetComponentInChildren<Collider>();
        
        _mainCam = Camera.main;

        // Initialize the switch to its starting position
        _currentLocalZ = isOnRightSide ? rightLocalZ : leftLocalZ;
        _targetLocalZ = _currentLocalZ;
        ApplyPosition(_currentLocalZ);
    }

    void Update()
    {
        // --- TIME GATEKEEPER ---
        if (Time.timeScale == 0f)
        {
            _isDragging = false;
            return;
        }

        if (Mouse.current == null || _myCollider == null || _mainCam == null || slidingPart == null) return;

        bool isHovering = false;
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        // 3D Raycast to see if the mouse is over the switch
        Ray ray = _mainCam.ScreenPointToRay(mouseScreenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider == _myCollider) isHovering = true;
        }

        // Detect clicks
        if (isHovering && Mouse.current.leftButton.wasPressedThisFrame)
        {
            _isDragging = true;
        }

        // Detect releases
        if (Mouse.current.leftButton.wasReleasedThisFrame && _isDragging)
        {
            _isDragging = false;

            // --- THE 50% SNAP CALCULATION ---
            float midPoint = (leftLocalZ + rightLocalZ) / 2f;
            
            // Check which side of the midpoint we landed on
            if (leftLocalZ < rightLocalZ) 
                isOnRightSide = (_currentLocalZ > midPoint);
            else 
                isOnRightSide = (_currentLocalZ < midPoint); // In case right is negative!

            // Set the target to the furthest extreme of that side
            _targetLocalZ = isOnRightSide ? rightLocalZ : leftLocalZ;
        }

        // Apply movement logic
        if (_isDragging)
        {
            // We STILL read the mouse X (screen left/right) 
            // but we apply it to the 3D Z axis!
            float mouseDeltaX = Mouse.current.delta.ReadValue().x;
            _currentLocalZ += mouseDeltaX * dragSensitivity;

            // Hard clamp so the player can't pull the switch off the rail
            float minZ = Mathf.Min(leftLocalZ, rightLocalZ);
            float maxZ = Mathf.Max(leftLocalZ, rightLocalZ);
            _currentLocalZ = Mathf.Clamp(_currentLocalZ, minZ, maxZ);

            _targetLocalZ = _currentLocalZ; // Target matches current while dragging
        }
        else
        {
            // Smoothly move towards the final snapped target
            _currentLocalZ = Mathf.MoveTowards(_currentLocalZ, _targetLocalZ, snapSpeed * Time.deltaTime);
        }

        ApplyPosition(_currentLocalZ);
    }

    // Helper method to move ONLY the Z axis while keeping X and Y exactly the same
    void ApplyPosition(float zPos)
    {
        Vector3 pos = slidingPart.localPosition;
        pos.z = zPos;
        slidingPart.localPosition = pos;
    }
}