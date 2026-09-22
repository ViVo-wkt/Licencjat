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
        
        if (buttonCollider == null) buttonCollider = GetComponent<Collider>();

        if (warningLight != null) warningLight.SetActive(false);
    }

    void Update()
    {
        if (Mouse.current == null || _mainCam == null || buttonCollider == null) return;

        if (_isArmed)
        {
            _timer -= Time.unscaledDeltaTime; 
            
            if (_timer <= 0f)
            {
                DisarmButton();
            }
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = _mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider == buttonCollider)
                {
                    if (!_isArmed)
                    {
                        ArmButton();
                    }
                    else
                    {
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
        Time.timeScale = 1f; 
        SceneManager.LoadScene(menuSceneName);
    }
}