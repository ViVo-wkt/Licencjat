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
    public Collider[] buttonColliders3D; 
    public Renderer[] buttonRenderers; 

    [Header("Materials")]
    public Material activeMaterial;
    public Material inactiveMaterial;

    [Header("Audio")]
    public AudioClip customButtonSound;

    private Camera _cam;

    void Start()
    {
        _cam = Camera.main;
        UpdateVisuals();
    }

    void Update()
    {
        if (Time.timeScale == 0f || Mouse.current == null || _cam == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = _cam.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hitInfo))
            {
                if (buttonColliders3D == null) return;

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

    public void SetZoomLevel(int index)
    {
        currentZoom = (ZoomLevel)index;
        
        if (radarZoomSystem != null) radarZoomSystem.ApplyZoom(index); 
        
        UpdateVisuals();

        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound(customButtonSound);
    }

    private void UpdateVisuals()
    {
        if (buttonRenderers == null) return;

        for (int i = 0; i < buttonRenderers.Length; i++)
        {
            if (buttonRenderers[i] != null)
            {
                buttonRenderers[i].material = (i == (int)currentZoom) ? activeMaterial : inactiveMaterial;
            }
        }
    }
}