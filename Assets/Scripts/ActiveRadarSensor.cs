using UnityEngine;
using System.Collections.Generic;

public class ActiveRadarSensor : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject trackMarkerPrefab;
    public LayerMask targetLayer;        // "RadarTarget"

    // Returns the first valid target we are currently locking
    // Returns the target closest to the center of the beam

    public GameObject GetCurrentTarget()
    {
        GameObject bestTarget = null;
        float minAngle = Mathf.Infinity;

        foreach (var enemy in _activeLocks.Keys)
        {
            if (enemy == null) continue;

            Vector2 directionToEnemy = enemy.transform.position - transform.position;

            float angle = Vector2.Angle(-transform.up, directionToEnemy);

            if (angle < minAngle)
            {
                minAngle = angle;
                bestTarget = enemy;
            }
        }

        return bestTarget;
    }

    // Stores <Enemy, Marker> pairs
    private Dictionary<GameObject, GameObject> _activeLocks = new Dictionary<GameObject, GameObject>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. New target entered the beam
        if (IsTarget(other.gameObject))
        {
            AddLock(other.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 2. Target left the beam
        if (_activeLocks.ContainsKey(other.gameObject))
        {
            RemoveLock(other.gameObject);
        }
    }

    private void Update()
    {
        // 3. Update marker positions to follow moving enemies
        // Separate list to track destroyed enemies to avoid errors
        List<GameObject> enemiesLost = new List<GameObject>();

        foreach (var pair in _activeLocks)
        {
            GameObject enemy = pair.Key;
            GameObject marker = pair.Value;

            if (enemy != null && marker != null)
            {
                marker.transform.position = enemy.transform.position;
            }
            else
            {
                enemiesLost.Add(enemy);
            }
        }

        // Cleanup destroyed enemies
        foreach (var enemy in enemiesLost)
        {
            RemoveLock(enemy);
        }
    }

    void AddLock(GameObject enemy)
    {
        if (!_activeLocks.ContainsKey(enemy))
        {
            GameObject newMarker = Instantiate(trackMarkerPrefab, enemy.transform.position, Quaternion.identity);
            
            // Initialize the interaction script on the marker
            var interaction = newMarker.GetComponent<RadarContactInteraction>();
            if (interaction != null)
            {
                interaction.Initialize(enemy);
            }

            _activeLocks.Add(enemy, newMarker);
        }
    }

    void RemoveLock(GameObject enemy)
    {
        if (_activeLocks.ContainsKey(enemy))
        {
            if (_activeLocks[enemy] != null)
            {
                Destroy(_activeLocks[enemy]);
            }
            _activeLocks.Remove(enemy);
        }
    }

    bool IsTarget(GameObject obj)
    {
        return (targetLayer.value & (1 << obj.layer)) > 0;
    }

    // NEW: Allow missiles to check if a specific enemy is still locked
    public bool IsTracking(GameObject enemy)
    {
        if (enemy == null) return false;
        return _activeLocks.ContainsKey(enemy);
    }
}