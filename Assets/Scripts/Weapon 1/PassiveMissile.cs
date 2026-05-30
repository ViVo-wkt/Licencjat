using UnityEngine;

public class PassiveMissile : MonoBehaviour 
{
    [Header("Flight Characteristics")]
    public float speed = 5f;
    public float turnSpeed = 200f;
    public float killDistance = 0.5f;

    [Header("Limitations")]
    public float maxRadarRange = 4.8f;
    public float maxFlightTime = 8.0f;

    [Header("Visuals")]
    public GameObject interceptionEffect;

    private GameObject _target;
    private ActiveRadarSensor _guidanceRadar;
    private bool _hasSignal = false;

    private bool _isFireAndForget = false;

    public void Launch(GameObject target, ActiveRadarSensor radar)
    {
        _target = target;
        _guidanceRadar = radar;
        _hasSignal = true;

        _isFireAndForget = (radar == null);

        Destroy(gameObject, maxFlightTime);
    }

    public void LoseLock()
    {
        _target = null;
        _guidanceRadar = null;
        _hasSignal = false;
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

        if (_target != null)
        {
            bool hasValidSignal = false;

            if (_isFireAndForget) hasValidSignal = true; 
            else if (_guidanceRadar != null && _guidanceRadar.IsTracking(_target)) hasValidSignal = true; 

            if (hasValidSignal)
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
                if (_hasSignal) { _hasSignal = false; }
            }
        }
    }

    void Detonate()
    {
        if (interceptionEffect != null)
        {
            Instantiate(interceptionEffect, transform.position, Quaternion.identity);
        }

        if (_target != null) 
        {
            // --- ADD THIS LINE HERE ---
            // Tally a kill in the GameManager before the enemy is obliterated!
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddKill();
            }
            // ---------------------------

            Destroy(_target);
        }
        
        Destroy(gameObject);
    }
}