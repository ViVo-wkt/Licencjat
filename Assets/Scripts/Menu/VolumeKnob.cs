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

    void Awake()
    {
        _myCollider2D = GetComponent<Collider2D>();
        _myCollider3D = GetComponent<Collider>();
        _cam = Camera.main;
    }

    void Start()
    {
        // Save exactly where the tube starts so we can calculate the drain accurately
        if (volumeTubeFill != null)
        {
            _tubeStartScale = volumeTubeFill.localScale;
            _tubeStartPos = volumeTubeFill.localPosition;
        }

        UpdateVisuals();
    }

    void Update()
    {
        if (Mouse.current == null || _cam == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        bool clickDown = Mouse.current.leftButton.wasPressedThisFrame;
        bool clickUp = Mouse.current.leftButton.wasReleasedThisFrame;

        bool isHovering = false;
        if (_myCollider2D != null) 
        {
            isHovering = _myCollider2D.OverlapPoint(_cam.ScreenToWorldPoint(mouseScreenPos));
        }
        else if (_myCollider3D != null) 
        {
            Ray ray = _cam.ScreenPointToRay(mouseScreenPos);
            if (Physics.Raycast(ray, out RaycastHit hitInfo) && hitInfo.collider == _myCollider3D) isHovering = true;
        }

        if (isHovering && clickDown)
        {
            _isDragging = true;
            _lastMousePos = mouseScreenPos;
        }
        
        if (clickUp) _isDragging = false;

        float volumeChange = 0f;

        if (_isDragging)
        {
            float deltaX = mouseScreenPos.x - _lastMousePos.x;
            volumeChange = deltaX * dragSensitivity * 0.005f; 
            _lastMousePos = mouseScreenPos;
        }
        else if (isHovering)
        {
            float scrollValue = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(scrollValue) > 0.01f)
            {
                volumeChange = Mathf.Sign(scrollValue) * scrollSensitivity * 0.01f;
            }
        }

        if (Mathf.Abs(volumeChange) > 0.0001f)
        {
            currentVolume = Mathf.Clamp01(currentVolume + volumeChange);
            UpdateVisuals();
        }
    }

    void UpdateVisuals()
    {
        // 1. Rotate the 3D Knob Model
        float targetAngle = Mathf.Lerp(minAngle, maxAngle, currentVolume);
        Vector3 currentEuler = transform.localEulerAngles;

        if (knobAxis == RotationAxis.X) 
            transform.localRotation = Quaternion.Euler(targetAngle, currentEuler.y, currentEuler.z);
        else if (knobAxis == RotationAxis.Y) 
            transform.localRotation = Quaternion.Euler(currentEuler.x, targetAngle, currentEuler.z);
        else 
            transform.localRotation = Quaternion.Euler(currentEuler.x, currentEuler.y, targetAngle);

        // 2. Scale the 3D Glass Tube
        if (volumeTubeFill != null)
        {
            // currentVolume is exactly what we need (a 0.0 to 1.0 ratio)
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