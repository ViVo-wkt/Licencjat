using UnityEngine;
using UnityEngine.InputSystem;

public class ZoomPanel : MonoBehaviour
{
    public enum ZoomLevel { Near, Medium, Far }

    [Header("Current State")]
    public ZoomLevel currentZoom = ZoomLevel.Near;

    [Header("Connections")]
    public RadarZoomSystem radarZoomSystem; 

    [Header("Button Setup")]
    [Tooltip("The invisible colliders that detect mouse clicks.")]
    public Collider2D[] buttonColliders; 
    public Collider[] buttonColliders3D; 

    [Header("3D Visual Objects")]
    [Tooltip("Drag your 3 state models here in order (Near, Medium, Far).")]
    public GameObject[] activeStateObjects; 

    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
        UpdateVisuals();
    }

    void Update()
    {
        if (Mouse.current == null || _cam == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector2 mouseWorldPos2D = _cam.ScreenToWorldPoint(mouseScreenPos);
            
            Ray ray = _cam.ScreenPointToRay(mouseScreenPos);
            bool hit3D = Physics.Raycast(ray, out RaycastHit hitInfo);

            // Check 2D Colliders
            for (int i = 0; i < buttonColliders.Length; i++)
            {
                if (buttonColliders[i] != null && buttonColliders[i].OverlapPoint(mouseWorldPos2D))
                {
                    SetZoomLevel(i);
                    return; 
                }
            }

            // Check 3D Colliders (if applicable)
            if (buttonColliders3D != null)
            {
                for (int i = 0; i < buttonColliders3D.Length; i++)
                {
                    if (hit3D && buttonColliders3D[i] != null && hitInfo.collider == buttonColliders3D[i])
                    {
                        SetZoomLevel(i);
                        return;
                    }
                }
            }
        }
    }

    public void SetZoomLevel(int index)
    {
        currentZoom = (ZoomLevel)index;
        
        // --- THE FIX IS HERE ---
        if (radarZoomSystem != null)
        {
            // We use your exact method name: ApplyZoom!
            radarZoomSystem.ApplyZoom(index); 
        }
        
        UpdateVisuals();
    }

    void UpdateVisuals()
    {
        if (activeStateObjects != null)
        {
            for (int i = 0; i < activeStateObjects.Length; i++)
            {
                if (activeStateObjects[i] != null)
                {
                    activeStateObjects[i].SetActive(i == (int)currentZoom);
                }
            }
        }
    }
}