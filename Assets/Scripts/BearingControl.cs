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

    [Header("Mechanical Limits")]
    [Tooltip("How fast the heavy dish physically turns (Degrees per second)")]
    public float maxTurnSpeed = 45f; 

    // --- NEW: AUDIO SECTION ---
    [Header("Audio: General")]
    [Tooltip("Drag a satisfying clack or heavy switch sound here.")]
    public AudioClip turnSound;

    [Header("Audio: Scroll & Drag Mode")]
    [Tooltip("How many degrees the knob must turn to play a tick sound.")]
    public float degreesPerTick = 4f; 
    private float _accumulatedRotation = 0f;

    [Header("Audio: Point & Pull Mode")]
    [Tooltip("How long the knob must sit still (in seconds) before the final 'clack' plays.")]
    public float settleTime = 0.15f;
    private bool _hasPlayedFlickSound = true;
    private float _lastMoveTime = 0f;
    // --------------------------

    [Header("Output")]
    public float currentBearing = 0f;
    private float _targetBearing = 0f; 

    private Collider2D _myCollider;
    private bool _isDragging = false;
    private Vector2 _lastMousePos;

    void Awake()
    {
        _myCollider = GetComponent<Collider2D>();
        if (radarIndicatorLine != null) radarIndicatorLine.gameObject.SetActive(false);

        float relativeOffset = lineAngleOffset - knobAngleOffset;
        _targetBearing = transform.eulerAngles.z + relativeOffset;
        currentBearing = _targetBearing;
    }

    void Update()
    {
        // --- TIME GATEKEEPER ---
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

        // Keep track of where we were before this frame calculated any movement
        float previousTarget = _targetBearing;

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
                transform.rotation = Quaternion.Euler(currentEuler.x, currentEuler.y, currentEuler.z + deltaAngle);
                UpdateTargetBearingFromKnob();

                // AUDIO: RATCHET LOGIC (For Scroll & Drag)
                _accumulatedRotation += Mathf.Abs(deltaAngle);
                if (_accumulatedRotation >= degreesPerTick)
                {
                    if (AudioManager.Instance != null) 
                        AudioManager.Instance.PlayClickSound(turnSound);
                    _accumulatedRotation %= degreesPerTick;
                }
            }
        }
        else // PointAndPull Mode
        {
            if (clickDown && isHovering) _isDragging = true;
            if (clickUp) _isDragging = false;

            if (_isDragging)
            {
                Vector2 direction = mouseWorldPos - (Vector2)transform.position;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                float knobRotation = angle - 90f + knobAngleOffset;
                transform.rotation = Quaternion.Euler(0, 0, knobRotation);
                
                _targetBearing = angle - 90f + lineAngleOffset;
            }

            // AUDIO: FLICK & SETTLE LOGIC (For Point & Pull)
            // 1. If the target bearing changed by even a fraction of a degree, the player is actively moving it
            if (Mathf.Abs(Mathf.DeltaAngle(previousTarget, _targetBearing)) > 0.05f)
            {
                _lastMoveTime = Time.time;
                _hasPlayedFlickSound = false; // Arm the sound!
            }

            // 2. If the sound is armed, AND the player either let go OR held perfectly still for the Settle Time...
            if (!_hasPlayedFlickSound && (!_isDragging || Time.time - _lastMoveTime >= settleTime))
            {
                if (AudioManager.Instance != null)
                    AudioManager.Instance.PlayClickSound(turnSound);
                
                _hasPlayedFlickSound = true; // Lock the sound so it doesn't double-play
            }
        }

        // Smoothly move the heavy machinery toward the target bearing
        currentBearing = Mathf.MoveTowardsAngle(currentBearing, _targetBearing, maxTurnSpeed * Time.deltaTime);

        if (radarIndicatorLine != null)
        {
            radarIndicatorLine.rotation = Quaternion.Euler(0, 0, currentBearing);
        }
    }

    void UpdateTargetBearingFromKnob()
    {
        float relativeOffset = lineAngleOffset - knobAngleOffset;
        _targetBearing = transform.eulerAngles.z + relativeOffset;
    }
}