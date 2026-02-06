using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    [Header("Flight Characteristics")]
    public float speed = 5f;
    public float turnSpeed = 200f;
    public float killDistance = 0.5f;
    
    [Header("Limitations")]
    public float maxRadarRange = 4.8f;
    public float maxFlightTime = 8.0f; // NEW: Fuel limit in seconds

    private GameObject _target;
    private ActiveRadarSensor _guidanceRadar;
    private bool _hasSignal = false;

    public void Launch(GameObject target, ActiveRadarSensor radar)
    {
        _target = target;
        _guidanceRadar = radar;
        _hasSignal = true;
        
        // NEW: Self-destruct after fuel runs out
        Destroy(gameObject, maxFlightTime); 
    }

    void Update()
    {
        // 1. Move Forward
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // 2. RANGE CHECK (Backup safety)
        if (transform.position.magnitude > maxRadarRange)
        {
            Destroy(gameObject);
            return;
        }

        // 3. Guidance Logic
        if (_target != null && _guidanceRadar != null)
        {
            if (_guidanceRadar.IsTracking(_target))
            {
                // SIGNAL GOOD
                Vector2 direction = (Vector2)_target.transform.position - (Vector2)transform.position;
                float rotateAmount = Vector3.Cross(direction, transform.up).z;
                transform.Rotate(0, 0, -rotateAmount * turnSpeed * Time.deltaTime);

                if (Vector2.Distance(transform.position, _target.transform.position) < killDistance)
                {
                    Detonate();
                }
            }
            else
            {
                // SIGNAL LOST
                if (_hasSignal) { _hasSignal = false; }
            }
        }
        else
        {
            // Target destroyed, fly straight until fuel runs out
        }
    }

    void Detonate()
    {
        if (_target != null) Destroy(_target);
        Destroy(gameObject);
    }
}