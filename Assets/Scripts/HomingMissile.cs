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
        // 1. Move Forward
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // 2. Guidance Logic
        if (_target != null && _hasLock)
        {
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
            // --- NEW CODE: Self Destruct if target is lost ---
            // If we launched but the target is gone (destroyed by another missile), 
            // destroy this missile after a short delay so it doesn't fly forever.
            Destroy(gameObject, 0.5f); 
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