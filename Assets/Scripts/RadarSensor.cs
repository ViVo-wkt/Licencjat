using UnityEngine;

public class RadarSensor : MonoBehaviour
{
    [Header("Configuration")]
    public GameObject blipPrefab;      // The dot we spawn
    public Transform blipParent;       // Usually the Radar Scope object

    [Header("Filters")]
    public LayerMask targetLayer;      // What can we detect?

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. Check if the object we hit is in the "Target" layer
        // (1 << other.gameObject.layer) uses bitwise math to match the LayerMask
        if ((targetLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            RegisterHit(other.transform.position);
        }
    }

    void RegisterHit(Vector3 targetWorldPosition)
    {
        // 2. Spawn the blip at the enemy's EXACT position
        // We use Quaternion.identity because blips usually don't rotate
        Instantiate(blipPrefab, targetWorldPosition, Quaternion.identity, blipParent);
        
        // Debug log to confirm it works
        Debug.Log($"Contact detected at: {targetWorldPosition}");
    }
}