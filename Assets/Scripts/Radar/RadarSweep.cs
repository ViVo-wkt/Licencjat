using UnityEngine;
using System.Collections.Generic;

public class RadarSweep : MonoBehaviour
{
    [Header("Settings")]
    public float rotationSpeed = -50f;
    public float detectionRadius = 150.0f; 
    public float sweepAngleWidth = 15f; 
    public bool flipDetectionDirection = false; 

    [Header("Visuals")]
    public Transform sweepVisual;

    private HashSet<TargetSignature> _targetsSeenThisSweep = new HashSet<TargetSignature>();
    private float _accumulatedAngle = 0f; 

    void Update()
    {
        float step = rotationSpeed * Time.deltaTime;
        transform.Rotate(0, 0, step);
        
        if (sweepVisual != null)
        {
            sweepVisual.rotation = transform.rotation;
        }

        _accumulatedAngle += Mathf.Abs(step);
        if (_accumulatedAngle >= 360f)
        {
            _accumulatedAngle -= 360f;
            _targetsSeenThisSweep.Clear();
        }

        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, detectionRadius);

        foreach (var hitCollider in hitColliders)
        {
            TargetSignature target = hitCollider.GetComponent<TargetSignature>();
            if (target == null) target = hitCollider.GetComponentInParent<TargetSignature>();

            if (target != null)
            {
                Vector2 dirToTarget = target.transform.position - transform.position;
                Vector2 sweepForward = flipDetectionDirection ? -transform.up : (Vector2)transform.up;
                
                float angleDifference = Vector2.Angle(sweepForward, dirToTarget);

                if (angleDifference < sweepAngleWidth / 2f)
                {
                    if (!_targetsSeenThisSweep.Contains(target))
                    {
                        // THE FIX: We ONLY tell the enemy to update its one, single blip.
                        // No new objects are spawned here!
                        target.PingLocation();
                        _targetsSeenThisSweep.Add(target);
                    }
                }
            }
        }
    }
}