using UnityEngine;

public class ActiveRadarSensor : MonoBehaviour
{
    [Header("Settings")]
    public float detectionRadius = 150.0f; // Cranked up to catch off-screen enemies!
    public float coneAngle = 30f; 
    
    [Tooltip("Check this box if the beam locks onto targets behind it!")]
    public bool flipDetectionDirection = false; // NEW TOGGLE

    private GameObject _currentTarget;

    void Update()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);
        
        GameObject bestTarget = null;
        float bestDist = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            TargetSignature targetSig = hitCollider.GetComponent<TargetSignature>();
            if (targetSig == null) targetSig = hitCollider.GetComponentInParent<TargetSignature>();

            if (targetSig != null)
            {
                // --- THE FIX: Vector Math ---
                Vector2 dirToTarget = targetSig.transform.position - transform.position;
                Vector2 beamForward = flipDetectionDirection ? -transform.up : (Vector2)transform.up;

                float angleToTarget = Vector2.Angle(beamForward, dirToTarget);

                if (angleToTarget < coneAngle / 2f)
                {
                    float d = dirToTarget.sqrMagnitude;
                    if (d < bestDist)
                    {
                        bestDist = d;
                        bestTarget = targetSig.gameObject;
                    }

                    targetSig.RealTimeIllumination();
                }
            }
        }

        _currentTarget = bestTarget;
    }

    public GameObject GetCurrentTarget()
    {
        return _currentTarget;
    }

    public bool IsTracking(GameObject enemy)
    {
        if (enemy == null) return false;

        Vector2 dirToTarget = enemy.transform.position - transform.position;
        if (dirToTarget.magnitude > detectionRadius) return false;

        // Ensure the tracking check ALSO uses the flipped math!
        Vector2 beamForward = flipDetectionDirection ? -transform.up : (Vector2)transform.up;
        float angleToTarget = Vector2.Angle(beamForward, dirToTarget);
        
        return angleToTarget < coneAngle / 2f;
    }
}