using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TimeController : MonoBehaviour
{
    [Header("Settings")]
    public float slowTimeScale = 0.3f; // Game runs at 30% speed
    public float maxFocus = 100f;
    public float drainRate = 30f; // Loses 30 focus per second
    public float rechargeRate = 15f; // Gains 15 focus per second

    [Header("UI Reference")]
    public Slider focusBar; 
    public Image fillImage; 

    private float _currentFocus;
    // REMOVED: private bool _isSlowingTime; (This was causing the warning)

    void Start()
    {
        _currentFocus = maxFocus;
        if (focusBar != null)
        {
            focusBar.maxValue = maxFocus;
            focusBar.value = maxFocus;
        }
    }

    void Update()
    {
        // 1. Check Input (Left Shift)
        bool isShiftHeld = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;

        // 2. Logic: Should we slow down?
        if (isShiftHeld && _currentFocus > 0)
        {
            Time.timeScale = slowTimeScale;
            
            // Drain Stamina (using unscaledDeltaTime so the drain doesn't slow down with the game)
            _currentFocus -= drainRate * (1f / slowTimeScale) * Time.unscaledDeltaTime;
        }
        else
        {
            Time.timeScale = 1.0f;

            // Recharge Stamina
            if (_currentFocus < maxFocus)
            {
                _currentFocus += rechargeRate * Time.unscaledDeltaTime;
            }
        }

        // Clamp values
        _currentFocus = Mathf.Clamp(_currentFocus, 0, maxFocus);

        // 3. Update UI
        if (focusBar != null)
        {
            focusBar.value = _currentFocus;
        }

        // Failsafe: Ensure we return to normal speed if focus runs out while holding shift
        if (_currentFocus <= 0 && isShiftHeld)
        {
            Time.timeScale = 1.0f;
        }
    }
}