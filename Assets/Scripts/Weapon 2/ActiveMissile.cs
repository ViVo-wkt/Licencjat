using UnityEngine;

public class ActiveHomingMissile : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 6f;
    public float turnSpeed = 250f;
    public float searchConeAngle = 45f; 
    public float killDistance = 0.5f;

    [Header("Limitations")]
    public float maxRadarRange = 4.8f;  
    public float maxFlightTime = 10.0f; 

    [Header("Visuals")]
    public GameObject interceptionEffect;

    private GameObject _target;
    private bool _hasTarget = false;

    public void Launch(Quaternion initialHeading)
    {
        transform.rotation = initialHeading;
        Destroy(gameObject, maxFlightTime);
    }

    void Update()
    {
        float zoomFactor = (RadarZoomSystem.Instance != null) ? RadarZoomSystem.Instance.GetSpeedMultiplier() : 1f;

        // Forward speed IS affected by visual map scale
        transform.Translate(Vector3.up * speed * zoomFactor * Time.deltaTime);

        if (transform.position.magnitude > maxRadarRange)
        {
            Destroy(gameObject);
            return;
        }

        if (!_hasTarget)
        {
            ScanForTargets();
        }
        else if (_target != null)
        {
            Vector2 direction = (Vector2)_target.transform.position - (Vector2)transform.position;
            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            
            // --- THE FIX ---
            // Turn speed is NEVER affected by map scale!
            transform.Rotate(0, 0, -rotateAmount * turnSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, _target.transform.position) < killDistance)
            {
                Detonate();
            }
        }
        else
        {
            _hasTarget = false;
        }
    }

    void ScanForTargets()
    {
        Collider2D[] potentialTargets = Physics2D.OverlapCircleAll(transform.position, 3.0f);

        float bestDist = Mathf.Infinity;
        GameObject potentialBest = null;

        foreach (var col in potentialTargets)
        {
            if (col.GetComponent<TargetSignature>() != null)
            {
                Vector2 dirToEnemy = col.transform.position - transform.position;
                float angle = Vector2.Angle(transform.up, dirToEnemy);

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
        }
    }

    void Detonate()
    {
        if (interceptionEffect != null)
        {
            Instantiate(interceptionEffect, transform.position, Quaternion.identity);
        }

        if (_target != null) Destroy(_target);
        Destroy(gameObject);
    }
}