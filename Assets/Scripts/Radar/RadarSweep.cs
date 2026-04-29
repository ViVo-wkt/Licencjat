using UnityEngine;
using System.Collections.Generic;

public class RadarSweep : MonoBehaviour
{
    [Header("Settings")]
    public float rotationSpeed = -50f;
    
    [Tooltip("CRITICAL: Set this to a huge number (e.g. 150) so the sweep circle reaches all the way to the edge of the map, regardless of zoom level!")]
    public float detectionRadius = 150.0f; 
    public float sweepAngleWidth = 15f; 
    
    public bool flipDetectionDirection = false; 

    [Header("Visuals")]
    public Transform sweepVisual;

    private HashSet<TargetSignature> _targetsSeenThisSweep = new HashSet<TargetSignature>();
    
    // NEW: A much safer way to track when to clear the radar memory!
    private float _accumulatedAngle = 0f; 

    void Update()
    {
        // 1. Rotate the sweep
        float step = rotationSpeed * Time.deltaTime;
        transform.Rotate(0, 0, step);
        
        if (sweepVisual != null)
        {
            sweepVisual.rotation = transform.rotation;
        }

        // 2. THE FIX: Safely clear the memory every full 360-degree rotation
        _accumulatedAngle += Mathf.Abs(step);
        if (_accumulatedAngle >= 360f)
        {
            _accumulatedAngle -= 360f;
            _targetsSeenThisSweep.Clear();
        }

        // 3. Find enemies
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
                        target.PingLocation();
                        _targetsSeenThisSweep.Add(target);
                    }
                }
            }
        }
    }
}