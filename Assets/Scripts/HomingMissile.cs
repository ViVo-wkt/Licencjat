using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    [Header("Flight Characteristics")]
    public float speed = 5f;
    public float turnSpeed = 200f;
    public float killDistance = 0.5f;

    private GameObject _target;
    private bool _hasLock = false;

    // We call this when spawning the missile
    public void Launch(GameObject target)
    {
        _target = target;
        _hasLock = true;
        
        // Optional: Destroy after 10 seconds if it misses everything
        Destroy(gameObject, 10f); 
    }

    void Update()
    {
        // 1. Move Forward constanty
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // 2. Guidance Logic
        if (_target != null)
        {
            // CHECK: Is the target still locked? (Does it have the Red Marker?)
            // We assume if the marker exists as a child or is tracked by the sensor, it's locked.
            // For this prototype, we will trust the launch command, but you can add a check here 
            // to simulate "Lost Signal" if the player moves the beam.

            Vector2 direction = (Vector2)_target.transform.position - (Vector2)transform.position;
            
            // Rotate towards target
            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            transform.Rotate(0, 0, -rotateAmount * turnSpeed * Time.deltaTime);

            // 3. Proximity Fuze (Hit detection)
            if (Vector2.Distance(transform.position, _target.transform.position) < killDistance)
            {
                Detonate();
            }
        }
    }

    void Detonate()
    {
        // Boom
        Debug.Log("SPLASH ONE! Target Destroyed.");
        
        // Destroy Enemy
        if (_target != null) Destroy(_target);
        
        // Destroy Missile
        Destroy(gameObject);
        
        // TODO: Add explosion particle effect here later
    }
}