using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class RadarContactInteraction : MonoBehaviour
{
    [Header("References")]
    public RadarUIManager uiManager;
    public LayerMask contactLayer = ~0; 

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
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        // Prevent clicking through UI buttons
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            // If the UI panel is too big, it will block your clicks and trigger this!
            return; 
        }

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        
        RaycastHit2D[] hits = Physics2D.RaycastAll(worldPos, Vector2.zero, Mathf.Infinity, contactLayer);

        TargetSignature validTarget = null;

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

        if (validTarget != null)
        {
            // THIS TELLS US IF THE CLICK WORKED
            Debug.Log($"<color=cyan>[Radar]</color> Switched lock to new target: {validTarget.gameObject.name}");
            
            if (uiManager != null)
            {
                uiManager.ShowTargetInfo(validTarget); 
            }
        }
    }
}