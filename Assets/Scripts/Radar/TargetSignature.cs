using UnityEngine;

public class TargetSignature : MonoBehaviour
{
    [Header("Flight Data")]
    public string codename = "BOGEY";
    public string classification = "UNKNOWN";
    public int speed = 400; 
    public int altitude = 15000; 

    [Header("Optimization")]
    public float maxDistanceBeforeDespawn = 150f;

    [Header("Radar Visuals")]
    public GameObject blipPrefab; 
    
    [Tooltip("The yellow ring that appears when locked by the SARH beam")]
    public GameObject activeLockIndicatorPrefab; // NEW SLOT!

    private GameObject _myBlip;
    private GameObject _myLockIndicator; // Tracks the yellow ring
    private float _blipMemoryTime = 6f; 
    private float _blipTimer = 0f;
    private float _lockTimer = 0f; // Tracks how long the beam stays on them

    [HideInInspector] public Vector3 lastKnownPosition;

    void Start()
    {
        if (blipPrefab != null)
        {
            _myBlip = Instantiate(blipPrefab, transform.position, Quaternion.identity);
            
            Vector3 fixedPos = _myBlip.transform.position;
            fixedPos.z = -0.1f;
            _myBlip.transform.position = fixedPos;
            
            _myBlip.SetActive(false); 

            // Initialize the yellow lock indicator as a child of the blip!
            if (activeLockIndicatorPrefab != null)
            {
                _myLockIndicator = Instantiate(activeLockIndicatorPrefab, _myBlip.transform);
                _myLockIndicator.transform.localPosition = new Vector3(0, 0, -0.05f); // Slightly above the blip
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

        // Handle the yellow lock ring disappearing
        if (_myLockIndicator != null && _myLockIndicator.activeSelf)
        {
            _lockTimer -= Time.deltaTime;
            if (_lockTimer <= 0)
            {
                _myLockIndicator.SetActive(false);
            }
        }

        if (transform.position.magnitude > maxDistanceBeforeDespawn)
        {
            Destroy(gameObject); 
        }
    }

    public void PingLocation()
    {
        if (_myBlip != null)
        {
            lastKnownPosition = transform.position;

            Vector3 snapPos = lastKnownPosition;
            snapPos.z = -0.1f; 
            _myBlip.transform.position = snapPos;

            _myBlip.SetActive(true);
            _blipTimer = _blipMemoryTime;
        }
    }

    public void RealTimeIllumination()
    {
        if (_myBlip != null)
        {
            lastKnownPosition = transform.position;
            
            Vector3 realPos = transform.position;
            realPos.z = -0.1f;
            _myBlip.transform.position = realPos;

            _myBlip.SetActive(true);
            _blipTimer = 0.5f; 

            // Turn on the yellow ring!
            if (_myLockIndicator != null)
            {
                _myLockIndicator.SetActive(true);
                _lockTimer = 0.2f; // Instantly turns off if the beam leaves
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