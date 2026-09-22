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

        if (Time.timeScale == 0f && !_isShowing) return;

        bool clickDown = Mouse.current.leftButton.wasPressedThisFrame;
        bool clickUp = Mouse.current.leftButton.wasReleasedThisFrame;
        bool escPressed = Keyboard.current.escapeKey.wasPressedThisFrame;

        bool isHovering = false;
        
        if (clickDown && !_isShowing) 
        {
            Ray ray = _mainCam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider == buttonCollider) isHovering = true;
            }
        }

        if (mode == InteractionMode.HoldToView)
        {
            if (isHovering && clickDown)
            {
                SetOverlay(true);
            }
            else if (clickUp && _isShowing) 
            {
                SetOverlay(false);
            }
        }
        
        else if (mode == InteractionMode.ClickToToggle)
        {
            if (isHovering && clickDown && !_isShowing)
            {
                SetOverlay(true);
            }
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
        
        Time.timeScale = state ? 0f : 1f;
    }
}