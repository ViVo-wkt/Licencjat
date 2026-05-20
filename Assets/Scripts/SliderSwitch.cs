using UnityEngine;
using UnityEngine.InputSystem;

public class SliderSwitch : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag the child object that physically slides left and right into here.")]
    public Transform slidingPart;

    // --- NEW: WE NOW ONLY LOOK FOR THE HANDLE'S COLLIDER ---
    [Tooltip("The collider specifically on the sliding handle. Auto-fills if left blank.")]
    public Collider handleCollider; 

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

    private Camera _mainCam;
    private bool _isDragging = false;
    
    private float _currentLocalZ;
    private float _targetLocalZ;

    void Start()
    {
        _mainCam = Camera.main;

        // Auto-find the collider on the sliding handle if you didn't drag it in manually
        if (handleCollider == null && slidingPart != null)
        {
            handleCollider = slidingPart.GetComponent<Collider>();
        }

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

        if (Mouse.current == null || handleCollider == null || _mainCam == null || slidingPart == null) return;

        bool isHovering = false;
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        // 3D Raycast - Now ONLY checks if the mouse is touching the physical Handle!
        Ray ray = _mainCam.ScreenPointToRay(mouseScreenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider == handleCollider) isHovering = true;
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
            
            if (leftLocalZ < rightLocalZ) 
                isOnRightSide = (_currentLocalZ > midPoint);
            else 
                isOnRightSide = (_currentLocalZ < midPoint);

            _targetLocalZ = isOnRightSide ? rightLocalZ : leftLocalZ;
        }

        // Apply movement logic
        if (_isDragging)
        {
            float mouseDeltaX = Mouse.current.delta.ReadValue().x;
            _currentLocalZ += mouseDeltaX * dragSensitivity;

            float minZ = Mathf.Min(leftLocalZ, rightLocalZ);
            float maxZ = Mathf.Max(leftLocalZ, rightLocalZ);
            _currentLocalZ = Mathf.Clamp(_currentLocalZ, minZ, maxZ);

            _targetLocalZ = _currentLocalZ; 
        }
        else
        {
            _currentLocalZ = Mathf.MoveTowards(_currentLocalZ, _targetLocalZ, snapSpeed * Time.deltaTime);
        }

        ApplyPosition(_currentLocalZ);
    }

    void ApplyPosition(float zPos)
    {
        Vector3 pos = slidingPart.localPosition;
        pos.z = zPos;
        slidingPart.localPosition = pos;
    }
}