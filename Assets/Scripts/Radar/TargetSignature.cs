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
    public GameObject activeLockIndicatorPrefab;

    private GameObject _myBlip;
    private GameObject _myLockIndicator; 
    
    // INCREASED: If the radar takes ~7.2 seconds to do a 360-degree rotation, 
    // the blip needs to survive for 8 seconds so it doesn't vanish before the refresh!
    private float _blipMemoryTime = 8f; 
    private float _blipTimer = 0f;
    private float _lockTimer = 0f; 

    [HideInInspector] public Vector3 lastKnownPosition;

    void Start()
    {
        if (blipPrefab != null)
        {
            // We spawn exactly ONE blip for this enemy!
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
            // We simply MOVE our one existing blip to the newest location
            lastKnownPosition = transform.position;

            Vector3 snapPos = lastKnownPosition;
            snapPos.z = -0.1f; 
            _myBlip.transform.position = snapPos;

            _myBlip.SetActive(true);
            _blipTimer = _blipMemoryTime; // Reset the fade timer
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

            if (_myLockIndicator != null)
            {
                _myLockIndicator.SetActive(true);
                _lockTimer = 0.2f; 
            }
        }
    }

    void OnDestroy()
    {
        // If the enemy is destroyed (or despawns), the one blip is destroyed with it
        if (_myBlip != null)
        {
            Destroy(_myBlip);
        }
    }
}