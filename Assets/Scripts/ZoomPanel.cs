using UnityEngine;
using UnityEngine.InputSystem;

public class ZoomPanel : MonoBehaviour
{
    public enum ZoomLevel { Near, Medium, Far }

    [Header("Current State")]
    public ZoomLevel currentZoom = ZoomLevel.Near;

    [Header("Connections")]
    public RadarZoomSystem radarZoomSystem; 

    [Header("3D Hardware")]
    [Tooltip("The physical 3D colliders of your 3 buttons here.")]
    public Collider[] buttonColliders3D; 
    
    [Tooltip("The Mesh Renderers of your 3 buttons here.")]
    public Renderer[] buttonRenderers; 

    [Header("Materials")]
    [Tooltip("The glowing material for the selected state.")]
    public Material activeMaterial;
    
    [Tooltip("The dark material for the unselected state.")]
    public Material inactiveMaterial;

    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
        UpdateVisuals();
    }

    void Update()
    {
        // --- TIME GATEKEEPER ---
        if (Time.timeScale == 0f) return;

        if (Mouse.current == null || _cam == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                // Check 3D Colliders
                if (buttonColliders3D != null)
                {
                    for (int i = 0; i < buttonColliders3D.Length; i++)
                    {
                        if (buttonColliders3D[i] != null && hitInfo.collider == buttonColliders3D[i])
                        {
                            SetZoomLevel(i);
                            return;
                        }
                    }
                }
            }
        }
    }

    public void SetZoomLevel(int index)
    {
        currentZoom = (ZoomLevel)index;
        
        // Push the command to the actual radar using your exact method name!
        if (radarZoomSystem != null)
        {
            radarZoomSystem.ApplyZoom(index); 
        }
        
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (buttonRenderers != null)
        {
            for (int i = 0; i < buttonRenderers.Length; i++)
            {
                if (buttonRenderers[i] != null)
                {
                    // Swaps out the light element without touching the button's physical frame
                    buttonRenderers[i].material = (i == (int)currentZoom) ? activeMaterial : inactiveMaterial;
                }
            }
        }
    }
}