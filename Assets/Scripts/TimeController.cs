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
    
    // --- THE FIX ---
    [Tooltip("If the tube shrinks from both sides, tweak this number! (Standard Unity cylinders are 2)")]
    public float modelLengthMultiplier = 2f; 
    // ---------------

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

        if (focusTubeFill != null)
        {
            _tubeStartScale = focusTubeFill.localScale;
            _tubeStartPos = focusTubeFill.localPosition;
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;

        HandleInput();
        UpdateFocus();
        UpdateVisuals();
    }

    void HandleInput()
    {
        if (Keyboard.current == null) return;

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
            _currentFocus -= drainRate * Time.unscaledDeltaTime;
            
            if (_currentFocus <= 0)
            {
                _currentFocus = 0;
                StopSlowdown();
            }
        }
        else if (_currentFocus < maxFocus)
        {
            _currentFocus += rechargeRate * Time.unscaledDeltaTime;
            if (_currentFocus > maxFocus) _currentFocus = maxFocus;
        }
    }

    void StartSlowdown()
    {
        _isSlowingTime = true;
        Time.timeScale = slowTimeScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale; 
    }

    void StopSlowdown()
    {
        _isSlowingTime = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
    }

    void UpdateVisuals()
    {
        if (focusBar != null) focusBar.value = _currentFocus;

        if (focusTubeFill != null)
        {
            float fillRatio = _currentFocus / maxFocus; 

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

            if (hasCenterPivot)
            {
                float directionMultiplier = invertDrainDirection ? 1f : -1f;
                Vector3 newPos = _tubeStartPos;

                // We multiply the math by the length of your specific 3D model!
                if (tubeShrinkAxis == ShrinkAxis.X) newPos.x += (shrinkAmount / 2f) * modelLengthMultiplier * directionMultiplier;
                else if (tubeShrinkAxis == ShrinkAxis.Y) newPos.y += (shrinkAmount / 2f) * modelLengthMultiplier * directionMultiplier;
                else if (tubeShrinkAxis == ShrinkAxis.Z) newPos.z += (shrinkAmount / 2f) * modelLengthMultiplier * directionMultiplier;
                
                focusTubeFill.localPosition = newPos;
            }
        }
    }
}