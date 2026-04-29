using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TimeController : MonoBehaviour
{
    [Header("Settings")]
    public float slowTimeScale = 0.3f; 
    public float maxFocus = 100f;
    public float drainRate = 30f; 
    public float rechargeRate = 15f; 

    [Header("UI Visuals")]
    public Slider focusBar;

    [Header("3D Tube Visuals")]
    [Tooltip("Drag your yellow cylinder model here!")]
    public Transform focusTubeFill; 
    public enum ShrinkAxis { X, Y, Z }
    public ShrinkAxis tubeShrinkAxis = ShrinkAxis.Y; 
    
    [Tooltip("Keep checked if the cylinder shrinks towards its middle.")]
    public bool hasCenterPivot = true; 
    [Tooltip("Check this if the tube drains from the wrong side!")]
    public bool invertDrainDirection = false;

    private float _currentFocus;
    private bool _isSlowingTime = false;

    private Vector3 _tubeStartScale;
    private Vector3 _tubeStartPos;

    void Start()
    {
        _currentFocus = maxFocus;
        if (focusBar != null) focusBar.maxValue = maxFocus;

        // Initialize the 3D Tube Starting State
        if (focusTubeFill != null)
        {
            _tubeStartScale = focusTubeFill.localScale;
            _tubeStartPos = focusTubeFill.localPosition;
        }
    }

    void Update()
    {
        HandleInput();
        UpdateFocus();
        UpdateVisuals();
    }

    void HandleInput()
    {
        // Check if a keyboard exists before trying to read it
        if (Keyboard.current == null) return;

        // Uses the Shift key (Left or Right Shift)
        if (Keyboard.current.shiftKey.wasPressedThisFrame && _currentFocus > 0)
        {
            StartSlowdown();
        }
        else if (Keyboard.current.shiftKey.wasReleasedThisFrame || (_isSlowingTime && _currentFocus <= 0))
        {
            StopSlowdown();
        }
    }

    void UpdateFocus()
    {
        if (_isSlowingTime)
        {
            // Drain focus (Remember to multiply by unscaledDeltaTime because Time.deltaTime is slowed!)
            _currentFocus -= drainRate * Time.unscaledDeltaTime;
            
            if (_currentFocus <= 0)
            {
                _currentFocus = 0;
                StopSlowdown();
            }
        }
        else if (_currentFocus < maxFocus)
        {
            // Recharge focus
            _currentFocus += rechargeRate * Time.unscaledDeltaTime;
            if (_currentFocus > maxFocus) _currentFocus = maxFocus;
        }
    }

    void StartSlowdown()
    {
        _isSlowingTime = true;
        Time.timeScale = slowTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; // Keeps physics smooth
    }

    void StopSlowdown()
    {
        _isSlowingTime = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    void UpdateVisuals()
    {
        // 1. Update the 2D UI Slider
        if (focusBar != null) focusBar.value = _currentFocus;

        // 2. Update the 3D Glass Tube
        if (focusTubeFill != null)
        {
            float fillRatio = _currentFocus / maxFocus; 

            // Scale the cylinder
            Vector3 newScale = _tubeStartScale;
            float shrinkAmount = 0f;

            if (tubeShrinkAxis == ShrinkAxis.X) 
            {
                newScale.x = _tubeStartScale.x * fillRatio;
                shrinkAmount = _tubeStartScale.x - newScale.x;
            }
            else if (tubeShrinkAxis == ShrinkAxis.Y) 
            {
                newScale.y = _tubeStartScale.y * fillRatio;
                shrinkAmount = _tubeStartScale.y - newScale.y;
            }
            else if (tubeShrinkAxis == ShrinkAxis.Z) 
            {
                newScale.z = _tubeStartScale.z * fillRatio;
                shrinkAmount = _tubeStartScale.z - newScale.z;
            }

            focusTubeFill.localScale = newScale;

            // Slide the base to keep it planted if it's a center-pivot model
            if (hasCenterPivot)
            {
                float directionMultiplier = invertDrainDirection ? 1f : -1f;
                Vector3 newPos = _tubeStartPos;

                if (tubeShrinkAxis == ShrinkAxis.X) newPos.x += (shrinkAmount / 2f) * directionMultiplier;
                else if (tubeShrinkAxis == ShrinkAxis.Y) newPos.y += (shrinkAmount / 2f) * directionMultiplier;
                else if (tubeShrinkAxis == ShrinkAxis.Z) newPos.z += (shrinkAmount / 2f) * directionMultiplier;
                
                focusTubeFill.localPosition = newPos;
            }
        }
    }
}