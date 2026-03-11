using UnityEngine;
using UnityEngine.InputSystem;

public class ScrollWheelControl : MonoBehaviour
{
    [Header("Input Sensitivity")]
    public float dragSensitivity = 0.005f;
    public float scrollSensitivity = 0.05f;

    [Header("State (-1 to 1)")]
    [Range(-1f, 1f)]
    public float currentValue = 0f;

    [Header("Visual Rotation")]
    [Tooltip("The axis to rotate visually. Usually Vector3.right (X), up (Y), or forward (Z)")]
    public Vector3 rotationAxis = Vector3.right;
    public float maxRotationAngle = 360f;

    private bool _isDragging = false;
    private Collider _myCollider;
    private Camera _mainCam;

    void Start()
    {
        _myCollider = GetComponent<Collider>(); // Requires a 3D collider!
        _mainCam = Camera.main;
    }

    void Update()
    {
        if (Mouse.current == null || _myCollider == null || _mainCam == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        bool isHovering = false;

        // 1. Raycast to check if mouse is over this 3D object
        Ray ray = _mainCam.ScreenPointToRay(mouseScreenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider == _myCollider) isHovering = true;
        }

        // 2. Drag Logic
        if (isHovering && Mouse.current.leftButton.wasPressedThisFrame) _isDragging = true;
        if (Mouse.current.leftButton.wasReleasedThisFrame) _isDragging = false;

        float valueChange = 0f;

        // 3. Process Input
        if (_isDragging)
        {
            // Dragging Up/Down
            valueChange = Mouse.current.delta.ReadValue().y * dragSensitivity;
        }
        else if (isHovering)
        {
            // Mouse Scroll Wheel
            float scrollValue = Mouse.current.scroll.ReadValue().y;
            if (Mathf.Abs(scrollValue) > 0.01f)
            {
                valueChange = Mathf.Sign(scrollValue) * scrollSensitivity;
            }
        }

        // 4. Apply Changes
        if (valueChange != 0f)
        {
            currentValue += valueChange;
            currentValue = Mathf.Clamp(currentValue, -1f, 1f);

            // Rotate the physical 3D model
            transform.localRotation = Quaternion.Euler(rotationAxis * (currentValue * maxRotationAngle));
        }
    }
}