using UnityEngine;

public class ActiveHomingMissile : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 6f;
    public float turnSpeed = 250f;
    public float searchConeAngle = 45f; // How wide it sees
    public float killDistance = 0.5f;

    private GameObject _target;
    private bool _hasTarget = false;

    // Called by WeaponSystem upon launch
    public void Launch(Quaternion initialHeading)
    {
        // Start facing the bearing direction
        transform.rotation = initialHeading;
        Destroy(gameObject, 15f); // Fuel limit
    }

    void Update()
    {
        // 1. Move Forward
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // 2. Search Logic (If no target)
        if (!_hasTarget)
        {
            ScanForTargets();
        }
        // 3. Kill Logic (If target found)
        else if (_target != null)
        {
            // Guide towards target
            Vector2 direction = (Vector2)_target.transform.position - (Vector2)transform.position;
            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            transform.Rotate(0, 0, -rotateAmount * turnSpeed * Time.deltaTime);

            // Hit check
            if (Vector2.Distance(transform.position, _target.transform.position) < killDistance)
            {
                Detonate();
            }
        }
        else
        {
            _hasTarget = false; // Target lost/destroyed, go back to searching
        }
    }

    void ScanForTargets()
    {
        // Find all colliders in a radius
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, 3.0f);

        float bestDist = Mathf.Infinity;
        GameObject potentialBest = null;

        foreach (var col in potentialTargets)
        {
            // Check if it's an enemy
            if (col.GetComponent<TargetSignature>() != null)
            {
                Vector2 dirToEnemy = col.transform.position - transform.position;
                float angle = Vector2.Angle(transform.up, dirToEnemy);

                // Is it in my cone?
                if (angle < searchConeAngle / 2f)
                {
                    float d = dirToEnemy.sqrMagnitude;
                    if (d < bestDist)
                    {
                        bestDist = d;
                        potentialBest = col.gameObject;
                    }
                }
            }
        }

        if (potentialBest != null)
        {
            _target = potentialBest;
            _hasTarget = true;
            Debug.Log("PITBULL! Missile is autonomous.");
        }
    }

    void Detonate()
    {
        if (_target != null) Destroy(_target);
        Destroy(gameObject);
    }
}