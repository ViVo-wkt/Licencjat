using UnityEngine;

public class EnemyNavigation : MonoBehaviour
{
    public enum MovementType { Linear, ZigZag }

    [Header("Identity")]
    public bool isHostile = true;

    [Header("Movement Characteristics")]
    public MovementType pattern = MovementType.Linear;
    public float speed = 0.5f;
    public float impactDistance = 0.2f;

    [Header("Zig-Zag Settings")]
    public float maneuverFrequency = 1.0f;
    public float maneuverMagnitude = 20.0f;

    [Header("Stats")]
    public float altitude = 30000f;

    private float _spawnTime;
    private Vector3 _fixedFlybyDirection;

    void Start()
    {
        _spawnTime = Time.time;

        if (!isHostile)
        {
            Vector3 oppositeSide = -transform.position;
            
            Vector3 randomOffset = (Vector3)Random.insideUnitCircle * 3.5f;
            
            _fixedFlybyDirection = (oppositeSide + randomOffset - transform.position).normalized;
        }
    }

    void Update()
    {
        float zoomFactor = (RadarZoomSystem.Instance != null) ? RadarZoomSystem.Instance.GetSpeedMultiplier() : 1f;
        float currentSpeed = speed * zoomFactor;

        Vector3 baseDirection;

        if (isHostile)
        {
            baseDirection = (Vector3.zero - transform.position).normalized;
        }
        else
        {
            baseDirection = _fixedFlybyDirection;

            if (transform.position.magnitude > 15.0f) 
            {
                Destroy(gameObject);
                return;
            }
        }

        Vector3 finalDirection = baseDirection;

        if (pattern == MovementType.ZigZag)
        {
            float angleOffset = Mathf.Sin((Time.time - _spawnTime) * maneuverFrequency) * maneuverMagnitude;
            finalDirection = Quaternion.Euler(0, 0, angleOffset) * baseDirection;
        }

        transform.position += finalDirection * currentSpeed * Time.deltaTime;

        float zAngle = Mathf.Atan2(finalDirection.y, finalDirection.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, zAngle);

        if (isHostile && Vector3.Distance(transform.position, Vector3.zero) < impactDistance)
        {
            Impact();
        }
    }

    void Impact()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.TakeDamage();
        }
        
        Debug.Log($"<color=red><b>IMPACT!</b> {name} hit the base!</color>");
        
        if (BaseAlarm.Instance != null)
        {
            BaseAlarm.Instance.TriggerAlarm();
        }
        
        Destroy(gameObject);
    }
}