using UnityEngine;
using UnityEngine.InputSystem;

public class VolumeKnob : MonoBehaviour
{
    [Header("Settings")]
    public float dragSensitivity = 0.5f;
    public float minAngle = 140f;   // Visual angle when at 0%
    public float maxAngle = -140f;  // Visual angle when at 100%
    
    private Collider2D _col2D;
    private Camera _cam;
    private bool _isDragging = false;
    private Vector2 _lastMousePos;

    void Awake()
    {
        _col2D = GetComponent<Collider2D>();
        _cam = Camera.main;
        UpdateRotation();
    }

    void Update()
    {
        if (Mouse.current == null || _cam == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        bool clickDown = Mouse.current.leftButton.wasPressedThisFrame;
        bool clickUp = Mouse.current.leftButton.wasReleasedThisFrame;
        
        bool isHovering = _col2D != null && _col2D.OverlapPoint(_cam.ScreenToWorldPoint(mouseScreenPos));

        if (isHovering && clickDown)
        {
            _isDragging = true;
            _lastMousePos = mouseScreenPos;
        }
        
        if (clickUp) _isDragging = false;

        if (_isDragging)
        {
            // Calculate mouse movement
            float deltaX = mouseScreenPos.x - _lastMousePos.x;
            _lastMousePos = mouseScreenPos;

            // Change actual game volume (Clamp keeps it between 0.0 and 1.0)
            AudioListener.volume = Mathf.Clamp01(AudioListener.volume + (deltaX * dragSensitivity * 0.01f));
            UpdateRotation();
        }
    }

    void UpdateRotation()
    {
        // Visually rotate the knob to match the new volume percentage
        float targetAngle = Mathf.Lerp(minAngle, maxAngle, AudioListener.volume);
        transform.localRotation = Quaternion.Euler(0, 0, targetAngle); 
    }
}