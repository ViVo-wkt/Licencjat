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
        if (Time.timeScale == 0f) return;

        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return; 
        }

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 clickWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        
        TargetSignature validTarget = null;

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

        if (validTarget == null)
        {
            TargetSignature[] allTargets = FindObjectsByType<TargetSignature>(FindObjectsSortMode.None);
            float closestDist = clickSelectionRadius;

            foreach (var target in allTargets)
            {
                if (target != null && target.IsVisibleOnRadar())
                {
                    float dist = Vector2.Distance(clickWorldPos, (Vector2)target.lastKnownPosition);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        validTarget = target;
                    }
                }
            }
        }

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