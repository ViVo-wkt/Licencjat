using UnityEngine;
using UnityEngine.InputSystem;

public class RadarKnob : MonoBehaviour
{
    public enum RotationAxis { X, Y, Z }

    [Header("Settings")]
    public bool isControllable = true; 
    public float rotationSpeed = 10f;
    public float dragSensitivity = 0.5f;
    
    [Tooltip("Which axis should the knob rotate around?")]
    public RotationAxis knobAxis = RotationAxis.Z;

    [Header("Calibration")]
    public float rotationOffset = 90f; // Tweak this (0, 90, 180, -90) to align them!

    [Header("References")]
    public Transform radarBeam;

    private Collider2D _myCollider;
    private bool _isDragging = false;
    private Vector2 _lastMousePos;

    void Awake()
    {
        _myCollider = GetComponent<Collider2D>();
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

        if (Mathf.Abs(rotationAmount) > 0.001f)
        {
            // 1. Rotate the Knob itself on the chosen axis
            if (knobAxis == RotationAxis.X) transform.Rotate(rotationAmount, 0, 0);
            else if (knobAxis == RotationAxis.Y) transform.Rotate(0, rotationAmount, 0);
            else transform.Rotate(0, 0, rotationAmount);

            // 2. Rotate the Beam with OFFSET
            if (radarBeam != null)
            {
                // Get the knob's current angle from the chosen axis
                float knobAngle = 0f;
                if (knobAxis == RotationAxis.X) knobAngle = transform.eulerAngles.x;
                else if (knobAxis == RotationAxis.Y) knobAngle = transform.eulerAngles.y;
                else knobAngle = transform.eulerAngles.z;
                
                // Apply offset to match the visual sprites
                radarBeam.rotation = Quaternion.Euler(0, 0, knobAngle + rotationOffset);
            }
        }
    }
}