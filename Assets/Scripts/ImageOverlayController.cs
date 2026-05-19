using UnityEngine;
using UnityEngine.InputSystem;

public class ImageOverlayController : MonoBehaviour
{
    public enum InteractionMode { HoldToView, ClickToToggle }

    [Header("Behavior Settings")]
    public InteractionMode mode = InteractionMode.HoldToView;

    [Header("References")]
    public GameObject overlayPanel;
    public Collider buttonCollider;

    private Camera _mainCam;
    private bool _isShowing = false;

    void Start()
    {
        _mainCam = Camera.main;
        if (buttonCollider == null) buttonCollider = GetComponent<Collider>();
        if (overlayPanel != null) overlayPanel.SetActive(false); 
    }

    void Update()
    {
        if (Mouse.current == null || Keyboard.current == null || _mainCam == null || buttonCollider == null || overlayPanel == null) return;

        // --- SAFETY GATEKEEPER ---
        // If the game is already paused by something ELSE (like the Briefing Screen), 
        // we completely ignore clicks so the player doesn't accidentally overlap screens!
        if (Time.timeScale == 0f && !_isShowing) return;

        bool clickDown = Mouse.current.leftButton.wasPressedThisFrame;
        bool clickUp = Mouse.current.leftButton.wasReleasedThisFrame;
        bool escPressed = Keyboard.current.escapeKey.wasPressedThisFrame;

        bool isHovering = false;
        
        // We only cast a ray to find the button if the player clicks AND the image isn't currently blocking the screen
        if (clickDown && !_isShowing) 
        {
            Ray ray = _mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider == buttonCollider) isHovering = true;
            }
        }

        // --- MODE 1: HOLD TO VIEW ---
        if (mode == InteractionMode.HoldToView)
        {
            if (isHovering && clickDown)
            {
                SetOverlay(true);
            }
            else if (clickUp && _isShowing) 
            {
                // If they let go of the mouse anywhere, hide it and unpause
                SetOverlay(false);
            }
        }
        
        // --- MODE 2: CLICK TO TOGGLE ---
        else if (mode == InteractionMode.ClickToToggle)
        {
            if (isHovering && clickDown && !_isShowing)
            {
                SetOverlay(true);
            }
            // If it's already showing, clicking ANYWHERE or pressing ESC will close it!
            else if ((clickDown || escPressed) && _isShowing)
            {
                SetOverlay(false);
            }
        }
    }

    private void SetOverlay(bool state)
    {
        _isShowing = state;
        overlayPanel.SetActive(state);
        
        // --- THE TIME CONTROLLER ---
        // If the state is true (showing), time is 0 (paused). Otherwise, time is 1 (normal speed).
        Time.timeScale = state ? 0f : 1f;
    }
}