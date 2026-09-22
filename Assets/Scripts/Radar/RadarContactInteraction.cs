using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class RadarContactInteraction : MonoBehaviour
{
    [Header("References")]
    public RadarUIManager uiManager;
    public LayerMask contactLayer = ~0; 

    [Header("Selection Tolerance")]
    [Tooltip("Maximum distance on screen (world units) from the visible blip to register a click.")]
    public float clickSelectionRadius = 0.5f;

    void Awake()
    {
        if (GetComponent<TargetSignature>() != null || GetComponent<Collider2D>() != null)
        {
            Destroy(this);
            return;
        }
    }

    void Start()
    {
        if (uiManager == null)
        {
            uiManager = FindAnyObjectByType<RadarUIManager>(); 
        }
    }

    void Update()
    {
        // Block target selection while paused (e.g. Warbook mode or Game Over)
        if (Time.timeScale == 0f) return;

        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        // Prevent clicking through UI buttons
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return; 
        }

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 clickWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        
        TargetSignature validTarget = null;

        // --- METHOD 1: Direct Collider Raycast ---
        RaycastHit2D[] hits = Physics2D.RaycastAll(clickWorldPos, Vector2.zero, Mathf.Infinity, contactLayer);

        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                TargetSignature ts = hit.collider.GetComponent<TargetSignature>();
                if (ts == null) ts = hit.collider.GetComponentInParent<TargetSignature>();

                if (ts != null)
                {
                    validTarget = ts;
                    break; 
                }
            }
        }

        // --- METHOD 2: Check Visible Radar Blips ---
        // If the raycast missed the physical enemy body (because it flew ahead of its last blip ping),
        // find the visible blip closest to the click point!
        if (validTarget == null)
        {
            TargetSignature[] allTargets = FindObjectsByType<TargetSignature>(FindObjectsSortMode.None);
            float closestDist = clickSelectionRadius;

            foreach (var target in allTargets)
            {
                // Only allow selecting contacts that are currently rendered/visible on radar
                if (target != null && target.IsVisibleOnRadar())
                {
                    // Check distance to the visible blip's coordinates
                    float dist = Vector2.Distance(clickWorldPos, (Vector2)target.lastKnownPosition);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        validTarget = target;
                    }
                }
            }
        }

        // --- APPLY SELECTION ---
        if (validTarget != null)
        {
            Debug.Log($"<color=cyan>[Radar]</color> Selected track: {validTarget.gameObject.name}");
            
            if (uiManager != null)
            {
                uiManager.ShowTargetInfo(validTarget); 
            }
        }
    }
}