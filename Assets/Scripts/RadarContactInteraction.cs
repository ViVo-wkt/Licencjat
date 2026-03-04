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
            Debug.Log($"<color=orange>[RadarContactInteraction]</color> Self-destructing on {gameObject.name} because it has a Collider/TargetSignature.");
            Destroy(this);
            return;
        }
    }

    void Start()
    {
        if (uiManager == null)
        {
            uiManager = FindAnyObjectByType<RadarUIManager>();
            if (uiManager == null)
                Debug.LogError("<color=red>[RadarContactInteraction]</color> Could not find RadarUIManager in the scene!");
            else
                Debug.Log("<color=green>[RadarContactInteraction]</color> Successfully found and linked RadarUIManager.");
        }
    }

    void Update()
    {
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Debug.Log("<color=cyan>--- CLICK DETECTED ---</color>");

        // 1. Check if UI is blocking
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("<color=yellow>[Interaction]</color> Click ignored: The pointer is over a UI element (or invisible UI panel).");
            return;
        }

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        Debug.Log($"<color=yellow>[Interaction]</color> Raycasting at World Position: {worldPos}");

        // 2. Perform Raycast
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, Mathf.Infinity, contactLayer);

        if (hit.collider != null)
        {
            Debug.Log($"<color=green>[Interaction]</color> Raycast HIT object: <b>{hit.collider.gameObject.name}</b>");

            // 3. Look for Target Signature
            TargetSignature target = hit.collider.GetComponent<TargetSignature>();
            if (target == null) target = hit.collider.GetComponentInParent<TargetSignature>();

            if (target != null)
            {
                Debug.Log($"<color=green>[Interaction]</color> TargetSignature found on <b>{target.gameObject.name}</b>. Sending to UI Manager.");
                if (uiManager != null)
                {
                    uiManager.ShowTargetInfo(target);
                }
            }
            else
            {
                Debug.Log($"<color=orange>[Interaction]</color> Object <b>{hit.collider.gameObject.name}</b> does NOT have a TargetSignature script attached.");
            }
        }
        else
        {
            Debug.Log("<color=yellow>[Interaction]</color> Raycast hit NOTHING (clicked empty space). Deselecting target.");
            if (uiManager != null)
            {
                uiManager.DeselectTarget();
            }
        }
    }
}