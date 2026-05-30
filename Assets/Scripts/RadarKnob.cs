using UnityEngine;
using UnityEngine.InputSystem;

public class RadarKnob : MonoBehaviour
{
    public enum RotationAxis { X, Y, Z }

    [Header("Settings")]
    public bool isControllable = true; 
    public float scrollSensitivity = 10f;
    public float dragSensitivity = 0.5f;
    public RotationAxis knobAxis = RotationAxis.Z;

    [Header("Calibration")]
    public float rotationOffset = 90f; 

    [Header("References")]
    public Transform radarBeam;

    [Header("Mechanical Speed Limits")]
    public bool limitKnobSpeed = true;
    public float maxKnobSpeed = 270f; 
    public bool limitBeamSpeed = false;
    public float maxBeamSpeed = 45f; 

    [Header("Audio")]
    public AudioClip turnSound;
    public float degreesPerTick = 4f; 
    private float _accumulatedRotation = 0f;

    private Collider2D _myCollider;
    private bool _isDragging = false;
    private Vector2 _lastMousePos;

    private float _currentBeamAngle = 0f;
    private float _targetBeamAngle = 0f;
    private float _currentKnobAngle = 0f;

    void Awake()
    {
        _myCollider = GetComponent<Collider2D>();
    }

    void Start()
    {
        _currentKnobAngle = GetKnobAngle();
        if (_currentKnobAngle > 180f) _currentKnobAngle -= 360f;

        float startAngle = _currentKnobAngle + rotationOffset;
        _targetBeamAngle = startAngle;
        _currentBeamAngle = startAngle;

        if (radarBeam != null)
        {
            radarBeam.rotation = Quaternion.Euler(0, 0, _currentBeamAngle);
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f)
        {
            _isDragging = false;
            return;
        }

        if (!isControllable || Mouse.current == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        
        bool clickDown = Mouse.current.leftButton.wasPressedThisFrame;
        bool clickUp = Mouse.current.leftButton.wasReleasedThisFrame;
        bool isHovering = (_myCollider != null && _myCollider.OverlapPoint(mouseWorldPos));

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

            // --- CURSOR WARPING (Active during drag only) ---
            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
            bool didWarp = false;

            if (mouseScreenPos.x >= screenWidth - 1)
            {
                mouseScreenPos.x = 1;
                didWarp = true;
            }
            else if (mouseScreenPos.x <= 0)
            {
                mouseScreenPos.x = screenWidth - 2;
                didWarp = true;
            }

            if (mouseScreenPos.y >= screenHeight - 1)
            {
                mouseScreenPos.y = 1;
                didWarp = true;
            }
            else if (mouseScreenPos.y <= 0)
            {
                mouseScreenPos.y = screenHeight - 2;
                didWarp = true;
            }

            if (didWarp)
            {
                Mouse.current.WarpCursorPosition(mouseScreenPos);
                _lastMousePos = mouseScreenPos; 
            }
        }
        else if (isHovering)
        {
            float scrollValue = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(scrollValue) > 0.01f)
            {
                rotationAmount = Mathf.Sign(scrollValue) * scrollSensitivity * Time.deltaTime * 50f;
            }
        }

        if (Mathf.Abs(rotationAmount) > 0.001f)
        {
            if (limitKnobSpeed)
            {
                float maxStepThisFrame = maxKnobSpeed * Time.deltaTime;
                rotationAmount = Mathf.Clamp(rotationAmount, -maxStepThisFrame, maxStepThisFrame);
            }

            _currentKnobAngle += rotationAmount;

            // --- AUDIO RATCHET ---
            _accumulatedRotation += Mathf.Abs(rotationAmount);
            if (_accumulatedRotation >= degreesPerTick)
            {
                if (AudioManager.Instance != null) 
                    AudioManager.Instance.PlayClickSound(turnSound);
                
                _accumulatedRotation %= degreesPerTick; 
            }

            if (knobAxis == RotationAxis.X) transform.localRotation = Quaternion.Euler(_currentKnobAngle, 0, 0);
            else if (knobAxis == RotationAxis.Y) transform.localRotation = Quaternion.Euler(0, _currentKnobAngle, 0);
            else transform.localRotation = Quaternion.Euler(0, 0, _currentKnobAngle);

            _targetBeamAngle = _currentKnobAngle + rotationOffset;
        }

        if (radarBeam != null)
        {
            if (limitBeamSpeed)
            {
                _currentBeamAngle = Mathf.MoveTowardsAngle(_currentBeamAngle, _targetBeamAngle, maxBeamSpeed * Time.deltaTime);
            }
            else
            {
                _currentBeamAngle = _targetBeamAngle;
            }
            
            radarBeam.rotation = Quaternion.Euler(0, 0, _currentBeamAngle);
        }
    }

    private float GetKnobAngle()
    {
        if (knobAxis == RotationAxis.X) return transform.localEulerAngles.x;
        if (knobAxis == RotationAxis.Y) return transform.localEulerAngles.y;
        return transform.localEulerAngles.z;
    }
}