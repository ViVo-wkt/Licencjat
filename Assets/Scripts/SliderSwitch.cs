using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class SliderSwitch : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag the child object that physically slides left and right into here.")]
    public Transform slidingPart;
    public Collider handleCollider; 

    [Header("Rail Limits (Local Z Coordinate)")]
    public float leftLocalZ = -0.5f;
    public float rightLocalZ = 0.5f;

    [Header("Interaction Settings")]
    public float dragSensitivity = 0.005f;
    public float snapSpeed = 5f;

    [Header("Output State")]
    public bool isOnRightSide = false;

    // Fired whenever the state toggles
    public event Action<bool> OnSwitchToggled;

    private Camera _mainCam;
    private bool _isDragging = false;
    private float _currentLocalZ;
    private float _targetLocalZ;
    private bool _previousState;

    void Start()
    {
        _mainCam = Camera.main;

        if (handleCollider == null && slidingPart != null)
        {
            handleCollider = slidingPart.GetComponent<Collider>();
        }

        _currentLocalZ = isOnRightSide ? rightLocalZ : leftLocalZ;
        _targetLocalZ = _currentLocalZ;
        _previousState = isOnRightSide;
        ApplyPosition(_currentLocalZ);
    }

    void Update()
    {
        // Notice: Time.timeScale check REMOVED so you can drag it out of the paused Warbook!
        if (Mouse.current == null || handleCollider == null || _mainCam == null || slidingPart == null) return;

        bool isHovering = false;
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        Ray ray = _mainCam.ScreenPointToRay(mouseScreenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider == handleCollider) isHovering = true;
        }

        if (isHovering && Mouse.current.leftButton.wasPressedThisFrame)
        {
            _isDragging = true;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && _isDragging)
        {
            _isDragging = false;

            float midPoint = (leftLocalZ + rightLocalZ) / 2f;
            if (leftLocalZ < rightLocalZ) 
                isOnRightSide = (_currentLocalZ > midPoint);
            else 
                isOnRightSide = (_currentLocalZ < midPoint);

            _targetLocalZ = isOnRightSide ? rightLocalZ : leftLocalZ;

            if (isOnRightSide != _previousState)
            {
                _previousState = isOnRightSide;
                OnSwitchToggled?.Invoke(isOnRightSide);
            }
        }

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
            // Uses unscaledDeltaTime to animate smoothly during pause
            _currentLocalZ = Mathf.MoveTowards(_currentLocalZ, _targetLocalZ, snapSpeed * Time.unscaledDeltaTime);
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