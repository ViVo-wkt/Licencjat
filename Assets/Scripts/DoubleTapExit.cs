using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class DoubleTapExit : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How much time the player has to click a second time (in seconds).")]
    public float doubleClickWindow = 1.5f;
    
    [Tooltip("The exact name of your main menu scene.")]
    public string menuSceneName = "Menu";

    [Header("References")]
    [Tooltip("The visual light/object that turns on after the first click.")]
    public GameObject warningLight;
    
    [Tooltip("The physical 3D collider of your button (Auto-fills if left blank).")]
    public Collider buttonCollider;

    private Camera _mainCam;
    private bool _isArmed = false;
    private float _timer = 0f;

    void Start()
    {
        _mainCam = Camera.main;
        
        // Auto-grab the collider if it's on the same object
        if (buttonCollider == null) buttonCollider = GetComponent<Collider>();

        // Ensure the warning light starts OFF
        if (warningLight != null) warningLight.SetActive(false);
    }

    void Update()
    {
        if (Mouse.current == null || _mainCam == null || buttonCollider == null) return;

        // --- TIMER LOGIC ---
        if (_isArmed)
        {
            // We specifically use unscaledDeltaTime here! 
            // This ensures the timer ticks down even if the game is currently paused.
            _timer -= Time.unscaledDeltaTime; 
            
            if (_timer <= 0f)
            {
                DisarmButton(); // Time ran out, reset the button
            }
        }

        // --- CLICK DETECTION ---
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = _mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider == buttonCollider)
                {
                    if (!_isArmed)
                    {
                        // First Click: Arm the button and turn on the light
                        ArmButton();
                    }
                    else
                    {
                        // Second Click: Execute the exit!
                        ExecuteExit();
                    }
                }
            }
        }
    }

    private void ArmButton()
    {
        _isArmed = true;
        _timer = doubleClickWindow;
        if (warningLight != null) warningLight.SetActive(true);
    }

    private void DisarmButton()
    {
        _isArmed = false;
        _timer = 0f;
        if (warningLight != null) warningLight.SetActive(false);
    }

    private void ExecuteExit()
    {
        // CRITICAL FIX: Reset time scale to normal before leaving the scene!
        // If we don't do this, the Main Menu will load entirely frozen in time.
        Time.timeScale = 1f; 
        SceneManager.LoadScene(menuSceneName);
    }
}