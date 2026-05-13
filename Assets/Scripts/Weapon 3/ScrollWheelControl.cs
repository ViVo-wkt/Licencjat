using UnityEngine;
using UnityEngine.InputSystem;

public class ScrollWheelControl : MonoBehaviour
{
    [Header("Input Settings")]
    public float dragSensitivity = 0.005f;
    public float scrollSensitivity = 0.05f;

    [Tooltip("If true, dragging the mouse Left/Right will rotate this wheel instead of Up/Down.")]
    public bool useHorizontalMouseDrag = false;

    // --- NEW: Mechanical Limits ---
    [Header("Mechanical Limits")]
    [Tooltip("How fast the targeting bracket catches up to the wheel (-1 to 1 scale)")]
    public float maxMoveSpeed = 1.5f; 

    [Header("State (-1 to 1)")]
    [Range(-1f, 1f)]
    public float currentValue = 0f;
    private float _targetValue = 0f;

    [Header("Visual Rotation")]
    public Vector3 rotationAxis = Vector3.right;
    public float maxRotationAngle = 360f;

    private bool _isDragging = false;
    private Collider _myCollider;
    private Camera _mainCam;

    void Start()
    {
        _myCollider = GetComponent<Collider>();
        _mainCam = Camera.main;
        
        // Sync starting position
        _targetValue = currentValue;
        UpdateVisualRotation();
    }

    void Update()
    {
        if (Mouse.current == null || _myCollider == null || _mainCam == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        bool isHovering = false;

        Ray ray = _mainCam.ScreenPointToRay(mouseScreenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider == _myCollider) isHovering = true;
        }

        if (isHovering && Mouse.current.leftButton.wasPressedThisFrame) _isDragging = true;
        if (Mouse.current.leftButton.wasReleasedThisFrame) _isDragging = false;

        float valueChange = 0f;

        if (_isDragging)
        {
            if (useHorizontalMouseDrag)
            {
                valueChange = Mouse.current.delta.ReadValue().x * dragSensitivity;
            }
            else
            {
                valueChange = Mouse.current.delta.ReadValue().y * dragSensitivity;
            }
        }
        else if (isHovering)
        {
            float scrollValue = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(scrollValue) > 0.01f)
            {
                valueChange = Mathf.Sign(scrollValue) * scrollSensitivity;
            }
        }

        if (valueChange != 0f)
        {
            // 1. Apply Input to the Target instantly
            _targetValue += valueChange;
            _targetValue = Mathf.Clamp(_targetValue, -1f, 1f);

            // 2. Instantly rotate the physical wheel so input feels highly responsive
            UpdateVisualRotation();
        }

        // --- THE MECHANICAL DELAY ---
        // Smoothly move the heavy targeting bracket towards the target
        currentValue = Mathf.MoveTowards(currentValue, _targetValue, maxMoveSpeed * Time.deltaTime);
    }

    public void ForceValue(float val)
    {
        // This is called by the TargetingBracket when it hits the edge of the radar.
        // We must update BOTH values instantly so the bracket doesn't "rubber-band" back!
        currentValue = Mathf.Clamp(val, -1f, 1f);
        _targetValue = currentValue; 
        UpdateVisualRotation();
    }

    private void UpdateVisualRotation()
    {
        // We use the TARGET value so the wheel spins instantly when the player scrolls
        transform.localRotation = Quaternion.Euler(rotationAxis * (_targetValue * maxRotationAngle));
    }
}