using UnityEngine;
using UnityEngine.InputSystem;

public class RadarContactInteraction : MonoBehaviour
{
    private TargetSignature _enemyData;
    private Collider2D _myCollider;

    void Awake()
    {
        _myCollider = GetComponent<Collider2D>();
    }

    // Called by ActiveRadarSensor when spawning this marker
    public void Initialize(GameObject enemy)
    {
        if (enemy != null)
        {
            _enemyData = enemy.GetComponent<TargetSignature>();
        }
    }

    void Update()
    {
        // 1. Safety Checks
        if (Mouse.current == null || _myCollider == null) return;

        // 2. Detect Left Click
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // 3. Check if mouse is over THIS object
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            
            if (_myCollider.OverlapPoint(mousePos))
            {
                SelectTarget();
            }
        }
    }

    void SelectTarget()
    {
        if (_enemyData != null)
        {
            Debug.Log($"Selected Target: {_enemyData.codename}");
            
            if (RadarUIManager.Instance != null)
            {
                RadarUIManager.Instance.ShowTargetInfo(_enemyData);
            }
            else
            {
                Debug.LogWarning("RadarUIManager Instance not found in scene!");
            }
        }
    }
}