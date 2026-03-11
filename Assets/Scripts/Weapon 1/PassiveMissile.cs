using UnityEngine;

public class PassiveMissile : MonoBehaviour // Updated to match the new file name
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

    // NEW: Tracks if this is an AUTO missile that shouldn't rely on the SARH beam
    private bool _isFireAndForget = false;

    public void Launch(GameObject target, ActiveRadarSensor radar)
    {
        _target = target;
        _guidanceRadar = radar;
        _hasSignal = true;

        // NEW: If the bracket passes a 'null' radar, we automatically become Fire-and-Forget!
        _isFireAndForget = (radar == null);

        Destroy(gameObject, maxFlightTime);
    }

    public void LoseLock()
    {
        // Wipes the target data so the missile flies dumb
        _target = null;
        _guidanceRadar = null;
        _hasSignal = false;
    }

    void Update()
    {
        // --- ZOOM LOGIC START ---
        float zoomFactor = (RadarZoomSystem.Instance != null) ? RadarZoomSystem.Instance.GetSpeedMultiplier() : 1f;
        // --- ZOOM LOGIC END ---

        // 1. Move Forward (Scaled by Zoom)
        transform.Translate(Vector3.up * speed * zoomFactor * Time.deltaTime);

        // 2. RANGE CHECK
        if (transform.position.magnitude > maxRadarRange)
        {
            Destroy(gameObject);
            return;
        }

        // 3. Guidance Logic
        if (_target != null)
        {
            // Check if we have a valid signal either from the radar OR by being fire-and-forget
            bool hasValidSignal = false;

            if (_isFireAndForget)
            {
                hasValidSignal = true; // AUTO missiles ALWAYS have a signal!
            }
            else if (_guidanceRadar != null && _guidanceRadar.IsTracking(_target))
            {
                hasValidSignal = true; // Normal SARH checks the main radar beam
            }

            if (hasValidSignal)
            {
                // SIGNAL GOOD
                Vector2 direction = (Vector2)_target.transform.position - (Vector2)transform.position;
                float rotateAmount = Vector3.Cross(direction, transform.up).z;

                // Turn Speed is ALSO scaled by zoomFactor to maintain correct turn radius
                transform.Rotate(0, 0, -rotateAmount * turnSpeed * zoomFactor * Time.deltaTime);

                if (Vector2.Distance(transform.position, _target.transform.position) < killDistance)
                {
                    Detonate();
                }
            }
            else
            {
                // SIGNAL LOST
                if (_hasSignal) { _hasSignal = false; }
            }
        }
        else
        {
            // Target destroyed, fly straight
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