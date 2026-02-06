using UnityEngine;

public class EnemyNavigation : MonoBehaviour
{
    public enum MovementType { Linear, ZigZag }

    [Header("Identity")]
    public bool isHostile = true; // IF FALSE: Flies past the ship without hitting it

    [Header("Movement Characteristics")]
    public MovementType pattern = MovementType.Linear;
    public float speed = 0.5f;
    public float impactDistance = 0.2f;

    [Header("Zig-Zag Settings")]
    public float maneuverFrequency = 1.0f;
    public float maneuverMagnitude = 20.0f;

    private float _spawnTime;
    private Vector3 _fixedFlybyDirection; // Used only for Neutrals

    void Start()
    {
        _spawnTime = Time.time;

        if (!isHostile)
        {
            // Calculate a "Fly-by" trajectory
            // 1. Find the point exactly opposite to where we spawned
            Vector3 oppositeSide = -transform.position;
            
            // 2. Add a large random offset so we don't fly through the center
            // (Random point inside a circle of radius 3)
            Vector3 randomOffset = (Vector3)Random.insideUnitCircle * 3.5f;
            
            // 3. Set our permanent heading
            _fixedFlybyDirection = (oppositeSide + randomOffset - transform.position).normalized;
        }
    }

    void Update()
    {
        Vector3 baseDirection;

        // 1. Determine Base Direction
        if (isHostile)
        {
            // HOSTILE: Constantly adjust to hit the center (Homing)
            baseDirection = (Vector3.zero - transform.position).normalized;
        }
        else
        {
            // NEUTRAL: Keep flying the same way (Fly-by)
            baseDirection = _fixedFlybyDirection;

            // Cleanup: If a neutral flies too far away (off screen), delete it
            if (transform.position.magnitude > 7.0f) 
            {
                Destroy(gameObject);
                return;
            }
        }

        // 2. Apply Maneuvers (ZigZag)
        Vector3 finalDirection = baseDirection;

        if (pattern == MovementType.ZigZag)
        {
            float angleOffset = Mathf.Sin((Time.time - _spawnTime) * maneuverFrequency) * maneuverMagnitude;
            finalDirection = Quaternion.Euler(0, 0, angleOffset) * baseDirection;
        }

        // 3. Move
        transform.position += finalDirection * speed * Time.deltaTime;

        // 4. Rotate sprite
        float zAngle = Mathf.Atan2(finalDirection.y, finalDirection.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, zAngle);

        // 5. Check Impact (Only if Hostile)
        if (isHostile && Vector3.Distance(transform.position, Vector3.zero) < impactDistance)
        {
            Impact();
        }
    }

    void Impact()
    {
        Debug.Log($"<color=red><b>IMPACT!</b> {name} hit the base!</color>");
        Destroy(gameObject);
    }
}