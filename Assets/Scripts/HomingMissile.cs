using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    [Header("Flight Characteristics")]
    public float speed = 5f;
    public float turnSpeed = 200f;
    public float killDistance = 0.5f;
    
    [Header("Safety")]
    public float maxRadarRange = 4.5f; // Radius of your radar screen

    private GameObject _target;
    private ActiveRadarSensor _guidanceRadar;
    private bool _hasSignal = false;

    public void Launch(GameObject target, ActiveRadarSensor radar)
    {
        _target = target;
        _guidanceRadar = radar;
        _hasSignal = true;
        
        // Safety timer in case it circles forever inside
        Destroy(gameObject, 15f); 
    }

    void Update()
    {
        // 1. Move Forward
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // 2. RANGE CHECK (The Fix)
        // If we fly further than the radar rim, destroy immediately.
        // We assume the radar center is at (0,0,0). If not, use Vector3.Distance(transform.position, _guidanceRadar.transform.position)
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
                // SIGNAL GOOD: Rotate towards target
                Vector2 direction = (Vector2)_target.transform.position - (Vector2)transform.position;
                float rotateAmount = Vector3.Cross(direction, transform.up).z;
                transform.Rotate(0, 0, -rotateAmount * turnSpeed * Time.deltaTime);

                // Hit Detection
                if (Vector2.Distance(transform.position, _target.transform.position) < killDistance)
                {
                    Detonate();
                }
            }
            else
            {
                // SIGNAL LOST: Fly straight (simulated by doing nothing here)
                if (_hasSignal) 
                {
                    _hasSignal = false; 
                    // Optional: Change sprite color to gray to show it's "dead"?
                }
            }
        }
        else
        {
            // Target dead, keep flying straight until Range Check kills us
        }
    }

    void Detonate()
    {
        if (_target != null) Destroy(_target);
        Destroy(gameObject);
    }
}