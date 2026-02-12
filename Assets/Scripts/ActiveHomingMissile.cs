using UnityEngine;

public class ActiveHomingMissile : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 6f;
    public float turnSpeed = 250f;
    public float searchConeAngle = 45f; // Seeker with in degrees
    public float killDistance = 0.5f;

    [Header("Limitations")]
    public float maxRadarRange = 4.8f;  // Screen radius (matches SARH setting)
    public float maxFlightTime = 10.0f; // Seconds before fuel runs out

    [Header("Visuals")]
    public GameObject interceptionEffect; // Drag FX_Interception here

    private GameObject _target;
    private bool _hasTarget = false;

    // Called by WeaponSystem upon launch
    public void Launch(Quaternion initialHeading)
    {
        // Start facing the bearing direction determined by the knob
        transform.rotation = initialHeading;

        // Fuel limit: Destroy self after X seconds automatically
        Destroy(gameObject, maxFlightTime);
    }

    void Update()
    {
        // 1. Move Forward constantly
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // 2. Range Safety Check (If it flies off screen)
        if (transform.position.magnitude > maxRadarRange)
        {
            Destroy(gameObject);
            return;
        }

        // 3. Guidance & Search Logic
        if (!_hasTarget)
        {
            // Phase 1: Search (Pitbull Mode)
            // The missile looks for targets on its own
            ScanForTargets();
        }
        else if (_target != null)
        {
            // Phase 2: Intercept (Guidance)
            // Steer towards the locked target
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
            // Target was locked but is now destroyed/null
            // Go back to search mode in case there is another target nearby
            _hasTarget = false;
        }
    }

    void ScanForTargets()
    {
        // Find all colliders in a small radius around the missile
        // 3.0f is the missile's own onboard seeker range
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, 3.0f);

        float bestDist = Mathf.Infinity;
        GameObject potentialBest = null;

        foreach (var col in potentialTargets)
        {
            // Check if it's an enemy (must have TargetSignature script)
            if (col.GetComponent<TargetSignature>() != null)
            {
                Vector2 dirToEnemy = col.transform.position - transform.position;
                float angle = Vector2.Angle(transform.up, dirToEnemy);

                // Is it within the seeker cone?
                if (angle < searchConeAngle / 2f)
                {
                    // Pick the closest one
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
        }
    }

   void Detonate()
    {
        // 1. Spawn Effect
        if (interceptionEffect != null)
        {
            Instantiate(interceptionEffect, transform.position, Quaternion.identity);
        }

        // 2. Destroy Target & Self
        if (_target != null) Destroy(_target);
        Destroy(gameObject);
    }
}