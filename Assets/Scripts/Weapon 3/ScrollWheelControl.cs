using UnityEngine;
using UnityEngine.InputSystem;

public class ScrollWheelControl : MonoBehaviour
{
    [Header("Input Settings")]
    public float dragSensitivity = 0.005f;
    public float scrollSensitivity = 0.05f;

    [Tooltip("If true, dragging the mouse Left/Right will rotate this wheel instead of Up/Down.")]
    public bool useHorizontalMouseDrag = false; // NEW TOGGLE

    [Header("State (-1 to 1)")]
    [Range(-1f, 1f)]
    public float currentValue = 0f;

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
            // NEW: Choose which mouse axis to read!
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

        // Apply
        if (valueChange != 0f)
        {
            currentValue += valueChange;
            currentValue = Mathf.Clamp(currentValue, -1f, 1f);

            // The 'override' limits we apply later in TargetingBracket might try to 
            // force this value, but we rotate visually based on the user's input
            UpdateVisualRotation();
        }
    }

    public void ForceValue(float val)
    {
        currentValue = Mathf.Clamp(val, -1f, 1f);
        UpdateVisualRotation();
    }

    private void UpdateVisualRotation()
    {
        transform.localRotation = Quaternion.Euler(rotationAxis * (currentValue * maxRotationAngle));
    }
}