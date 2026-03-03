using UnityEngine;
using System.Collections.Generic;

public class ActiveRadarSensor : MonoBehaviour
{
    [Header("Sensor Settings")]
    public Transform radarCenter;
    public float scanRange = 4.0f;
    public float beamWidth = 25f;
    public LayerMask targetLayer;
    public GameObject lockMarkerPrefab;

    // Internal State
    private Dictionary<GameObject, GameObject> _activeLocks = new Dictionary<GameObject, GameObject>();
    private bool _isGhostMode = false; // If true, markers are hidden but tracking continues

    void Start()
    {
        if (radarCenter == null) radarCenter = transform;
    }

    // --- NEW: Toggle Visibility without breaking lock ---
    public void SetGhostMode(bool enabled)
    {
        _isGhostMode = enabled;
        
        // Update all existing markers immediately
        foreach (var marker in _activeLocks.Values)
        {
            if (marker != null) marker.SetActive(!_isGhostMode);
        }
    }

    public GameObject GetCurrentTarget()
    {
        GameObject bestTarget = null;
        float minAngle = Mathf.Infinity;
        Vector3 forwardDir = -radarCenter.up;

        foreach (var enemy in _activeLocks.Keys)
        {
            if (enemy == null) continue;

            Vector2 directionToEnemy = enemy.transform.position - radarCenter.position;
            float angle = Vector2.Angle(forwardDir, directionToEnemy);

            if (angle < minAngle)
            {
                minAngle = angle;
                bestTarget = enemy;
            }
        }
        return bestTarget;
    }

    public bool IsTracking(GameObject target)
    {
        return target != null && _activeLocks.ContainsKey(target);
    }

    void Update()
    {
        if (radarCenter == null) return;

        Vector3 forwardDir = -radarCenter.up; // Assuming beam sprite points Down

        // 1. SCAN
        Collider2D[] hits = Physics2D.OverlapCircleAll(radarCenter.position, scanRange, targetLayer);
        foreach (var hit in hits)
        {
            if (hit == null) continue;

            Vector2 dirToTarget = hit.transform.position - radarCenter.position;
            float angle = Vector2.Angle(forwardDir, dirToTarget);

            if (angle < beamWidth / 2f)
            {
                if (!_activeLocks.ContainsKey(hit.gameObject))
                {
                    // Create Marker
                    GameObject marker = null;
                    if (lockMarkerPrefab != null)
                    {
                        marker = Instantiate(lockMarkerPrefab, hit.transform.position, Quaternion.identity);
                        
                        // IMPORTANT: Respect Ghost Mode on spawn
                        marker.SetActive(!_isGhostMode);
                    }
                    _activeLocks.Add(hit.gameObject, marker);
                }
            }
        }

        // 2. MAINTAIN
        List<GameObject> toRemove = new List<GameObject>();

        foreach (var kvp in _activeLocks)
        {
            GameObject enemy = kvp.Key;
            GameObject marker = kvp.Value;

            // Check if valid
            bool isLost = (enemy == null);
            if (!isLost)
            {
                Vector2 dirToEnemy = enemy.transform.position - radarCenter.position;
                float angle = Vector2.Angle(forwardDir, dirToEnemy);
                
                // Lost check: Angle or Range
                if (angle > beamWidth / 2f || dirToEnemy.magnitude > scanRange)
                {
                    isLost = true;
                }
            }

            if (isLost)
            {
                toRemove.Add(enemy);
                if (marker != null) Destroy(marker);
            }
            else if (marker != null)
            {
                // Update Position
                Vector3 newPos = enemy.transform.position;
                newPos.z = -1f; // Force on top of background
                marker.transform.position = newPos;
                
                // Enforce Visibility (in case it was changed)
                if (marker.activeSelf == _isGhostMode)
                {
                    marker.SetActive(!_isGhostMode);
                }
            }
        }

        // 3. CLEANUP
        foreach (var dead in toRemove)
        {
            _activeLocks.Remove(dead);
        }
    }
}