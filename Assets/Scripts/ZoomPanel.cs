using UnityEngine;
using UnityEngine.InputSystem;

public class ZoomPanel : MonoBehaviour
{
    [Header("Visuals")]
    [Tooltip("The SpriteRenderer of this panel object")]
    public SpriteRenderer panelRenderer;

    [Tooltip("The 3 sprites representing the panel states.\nElement 0 = Close Range Active\nElement 1 = Medium Range Active\nElement 2 = Long Range Active")]
    public Sprite[] stateSprites;

    [Header("Triggers")]
    [Tooltip("The 3 colliders for the buttons.\nElement 0 = Click zone for Close\nElement 1 = Click zone for Medium\nElement 2 = Click zone for Long")]
    public Collider2D[] triggerZones;

    void Start()
    {
        // Initial setup
        UpdateVisuals(0, 0);
    }

    void OnEnable()
    {
        RadarZoomSystem.OnZoomChanged += UpdateVisuals;
    }

    void OnDisable()
    {
        RadarZoomSystem.OnZoomChanged -= UpdateVisuals;
    }

    // Parameters (a, b) are required by the event signature but we ignore them
    // and read the true index directly from the system for accuracy.
    void UpdateVisuals(float oldScale, float newScale)
    {
        if (RadarZoomSystem.Instance == null || panelRenderer == null) return;
        if (stateSprites == null || stateSprites.Length == 0) return;

        int index = RadarZoomSystem.Instance.currentLevelIndex;

        // Safety check to prevent crash if index is out of bounds
        if (index >= 0 && index < stateSprites.Length)
        {
            panelRenderer.sprite = stateSprites[index];
        }
    }

    void Update()
    {
        // 1. Check for Mouse Input
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        // 2. Raycast to find what was clicked
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // 3. Check each trigger zone
        for (int i = 0; i < triggerZones.Length; i++)
        {
            if (triggerZones[i] != null && triggerZones[i].OverlapPoint(worldPos))
            {
                // Click detected! Switch zoom.
                RadarZoomSystem.Instance.ApplyZoom(i);
                return; // Stop checking
            }
        }
    }
}