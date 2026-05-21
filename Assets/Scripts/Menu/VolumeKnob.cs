using UnityEngine;
using UnityEngine.InputSystem;

public class GameVolumeKnob : MonoBehaviour
{
    public enum RotationAxis { X, Y, Z }

    [Header("Knob Settings")]
    [Tooltip("Which axis should the knob rotate around?")]
    public RotationAxis knobAxis = RotationAxis.X; 
    public float dragSensitivity = 0.5f;
    public float scrollSensitivity = 5f;

    [Header("Volume Limits")]
    [Range(0f, 1f)]
    public float currentVolume = 0.5f; 
    
    public float minAngle = 140f;   
    public float maxAngle = -140f;  

    [Header("3D Tube Visuals")]
    [Tooltip("Drag your copied yellow cylinder model here!")]
    public Transform volumeTubeFill; 
    public enum ShrinkAxis { X, Y, Z }
    public ShrinkAxis tubeShrinkAxis = ShrinkAxis.Y; 
    
    [Tooltip("Keep checked if the cylinder shrinks towards its middle.")]
    public bool hasCenterPivot = true; 
    public float modelLengthMultiplier = 2f; 
    [Tooltip("Check this if the tube drains from the wrong side!")]
    public bool invertDrainDirection = false;

    private Collider2D _myCollider2D;
    private Collider _myCollider3D;
    private Camera _cam;
    private bool _isDragging = false;
    private Vector2 _lastMousePos;
    
    private Vector3 _tubeStartScale;
    private Vector3 _tubeStartPos;

    void Start()
    {
        _cam = Camera.main;
        _myCollider2D = GetComponent<Collider2D>();
        _myCollider3D = GetComponent<Collider>();

        if (volumeTubeFill != null)
        {
            _tubeStartScale = volumeTubeFill.localScale;
            _tubeStartPos = volumeTubeFill.localPosition;
        }

        // Initialize physical knob position and yellow tube level on startup
        ApplyRotation();
        ApplyTubeVisuals();
        
        // --- AUDIO HOOK: INITIAL STARTUP ---
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(currentVolume);
        }
    }

    void Update()
    {
        // Cancel interaction if time is frozen (e.g. Briefing Screen)
        if (Time.timeScale == 0f) 
        {
            _isDragging = false;
            return;
        }

        if (Mouse.current == null || _cam == null) return;

        bool isHovering = false;
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();

        // 3D & 2D Raycasting to detect hover
        if (_myCollider3D != null)
        {
            Ray ray = _cam.ScreenPointToRay(mouseScreenPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider == _myCollider3D) isHovering = true;
            }
        }
        else if (_myCollider2D != null)
        {
            Vector2 mouseWorldPos = _cam.ScreenToWorldPoint(mouseScreenPos);
            if (_myCollider2D.OverlapPoint(mouseWorldPos)) isHovering = true;
        }

        // Input Detection
        if (isHovering && Mouse.current.leftButton.wasPressedThisFrame)
        {
            _isDragging = true;
            _lastMousePos = mouseScreenPos;
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            _isDragging = false;
        }

        // Volume Change Logic
        float volumeDelta = 0f;

        // Scroll Wheel Interaction
        if (isHovering)
        {
            float scrollValue = Mouse.current.scroll.ReadValue().y;
            if (scrollValue != 0f)
            {
                volumeDelta = (scrollValue > 0 ? 1f : -1f) * scrollSensitivity * Time.deltaTime;
            }
        }

        // Mouse Drag Interaction
        if (_isDragging)
        {
            float deltaX = mouseScreenPos.x - _lastMousePos.x;
            volumeDelta = deltaX * dragSensitivity * Time.deltaTime;
            _lastMousePos = mouseScreenPos;
        }

        // Apply the changes if the user moved the mouse/scrollwheel
        if (volumeDelta != 0f)
        {
            currentVolume += volumeDelta;
            currentVolume = Mathf.Clamp01(currentVolume); // Locks it exactly between 0.0 and 1.0

            ApplyRotation();
            ApplyTubeVisuals();

            // --- AUDIO HOOK: LIVE UPDATE ---
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetMasterVolume(currentVolume);
            }
        }
    }

    // Rotates the physical 3D knob base
    void ApplyRotation()
    {
        float targetAngle = Mathf.Lerp(minAngle, maxAngle, currentVolume);
        Vector3 rot = transform.localEulerAngles;
        
        if (knobAxis == RotationAxis.X) rot.x = targetAngle;
        else if (knobAxis == RotationAxis.Y) rot.y = targetAngle;
        else if (knobAxis == RotationAxis.Z) rot.z = targetAngle;
        
        transform.localEulerAngles = rot;
    }

    // Shrinks the yellow inner cylinder and moves it to simulate "draining"
    void ApplyTubeVisuals()
    {
        if (volumeTubeFill != null)
        {
            Vector3 newScale = _tubeStartScale;
            float shrinkAmount = 0f;

            if (tubeShrinkAxis == ShrinkAxis.X) 
            {
                newScale.x = _tubeStartScale.x * currentVolume;
                shrinkAmount = _tubeStartScale.x - newScale.x;
            }
            else if (tubeShrinkAxis == ShrinkAxis.Y) 
            {
                newScale.y = _tubeStartScale.y * currentVolume;
                shrinkAmount = _tubeStartScale.y - newScale.y;
            }
            else if (tubeShrinkAxis == ShrinkAxis.Z) 
            {
                newScale.z = _tubeStartScale.z * currentVolume;
                shrinkAmount = _tubeStartScale.z - newScale.z;
            }

            volumeTubeFill.localScale = newScale;

            // Slide the base to keep it planted
            if (hasCenterPivot)
            {
                float directionMultiplier = invertDrainDirection ? 1f : -1f;
                Vector3 newPos = _tubeStartPos;

                if (tubeShrinkAxis == ShrinkAxis.X) newPos.x += (shrinkAmount / 2f) * modelLengthMultiplier * directionMultiplier;
                else if (tubeShrinkAxis == ShrinkAxis.Y) newPos.y += (shrinkAmount / 2f) * modelLengthMultiplier * directionMultiplier;
                else if (tubeShrinkAxis == ShrinkAxis.Z) newPos.z += (shrinkAmount / 2f) * modelLengthMultiplier * directionMultiplier;
                
                volumeTubeFill.localPosition = newPos;
            }
        }
    }
}