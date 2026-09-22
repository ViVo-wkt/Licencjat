using UnityEngine;

public class TargetSignature : MonoBehaviour
{
    [Header("Flight Data")]
    public int speed = 400; 
    public int altitude = 15000; 

    [Header("Fluctuation Settings")]
    public float speedFluctuation = 12f;
    public float altitudeFluctuation = 250f;

    [Header("Optimization")]
    public float maxDistanceBeforeDespawn = 150f;

    [Header("Radar Visuals")]
    public GameObject blipPrefab; 
    public GameObject activeLockIndicatorPrefab;

    private GameObject _myBlip;
    private GameObject _myLockIndicator; 
    
    private float _blipMemoryTime = 8f; 
    private float _blipTimer = 0f;
    private float _lockTimer = 0f; 
    private float _noiseSeed;

    // Track whether target is under active continuous beam
    private bool _isContinuouslyIlluminated = false;

    // Cached telemetry from the latest sweep ping
    [HideInInspector] public Vector3 lastKnownPosition;
    [HideInInspector] public int lastKnownSpeed;
    [HideInInspector] public int lastKnownAltitude;

    void Awake()
    {
        _noiseSeed = Random.Range(0f, 1000f);
        lastKnownPosition = transform.position;
        lastKnownSpeed = speed;
        lastKnownAltitude = altitude;
    }

    void Start()
    {
        if (blipPrefab != null)
        {
            _myBlip = Instantiate(blipPrefab, transform.position, Quaternion.identity);
            
            Vector3 fixedPos = _myBlip.transform.position;
            fixedPos.z = -0.1f;
            _myBlip.transform.position = fixedPos;
            
            _myBlip.SetActive(false); 

            if (activeLockIndicatorPrefab != null)
            {
                _myLockIndicator = Instantiate(activeLockIndicatorPrefab, _myBlip.transform);
                _myLockIndicator.transform.localPosition = new Vector3(0, 0, -0.05f); 
                _myLockIndicator.SetActive(false);
            }
        }
    }

    void Update()
    {
        if (_myBlip != null && _myBlip.activeSelf)
        {
            _blipTimer -= Time.deltaTime;
            if (_blipTimer <= 0)
            {
                _myBlip.SetActive(false); 
            }
        }

        if (_myLockIndicator != null && _myLockIndicator.activeSelf)
        {
            _lockTimer -= Time.deltaTime;
            if (_lockTimer <= 0)
            {
                _myLockIndicator.SetActive(false);
                _isContinuouslyIlluminated = false; // Illumination expired
            }
        }

        if (transform.position.magnitude > maxDistanceBeforeDespawn)
        {
            Destroy(gameObject); 
        }
    }

    public bool IsVisibleOnRadar()
    {
        return _myBlip != null && _myBlip.activeSelf;
    }

    public bool IsContinuouslyIlluminated()
    {
        return _isContinuouslyIlluminated;
    }

    public int GetCurrentSpeed()
    {
        float wave = Mathf.Sin((Time.time * 0.8f) + _noiseSeed) * speedFluctuation;
        return Mathf.RoundToInt(speed + wave);
    }

    public int GetCurrentAltitude()
    {
        float wave = Mathf.Cos((Time.time * 0.4f) + _noiseSeed) * altitudeFluctuation;
        return Mathf.RoundToInt(altitude + wave);
    }

    // Called periodically by the rotating sweep line
    public void PingLocation()
    {
        if (_myBlip != null)
        {
            lastKnownPosition = transform.position;
            lastKnownSpeed = GetCurrentSpeed();
            lastKnownAltitude = GetCurrentAltitude();

            Vector3 snapPos = lastKnownPosition;
            snapPos.z = -0.1f; 
            _myBlip.transform.position = snapPos;

            _myBlip.SetActive(true);
            _blipTimer = _blipMemoryTime;
        }
    }

    // Called continuously by the steerable radar beam
    public void RealTimeIllumination()
    {
        if (_myBlip != null)
        {
            lastKnownPosition = transform.position;
            lastKnownSpeed = GetCurrentSpeed();
            lastKnownAltitude = GetCurrentAltitude();
            
            Vector3 realPos = transform.position;
            realPos.z = -0.1f;
            _myBlip.transform.position = realPos;

            _myBlip.SetActive(true);
            _blipTimer = 0.5f; 

            if (_myLockIndicator != null)
            {
                _myLockIndicator.SetActive(true);
                _lockTimer = 0.2f; 
                _isContinuouslyIlluminated = true;
            }
        }
    }

    void OnDestroy()
    {
        if (_myBlip != null)
        {
            Destroy(_myBlip);
        }
    }
}