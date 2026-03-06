using UnityEngine;
using TMPro; // Required for UI Text

public class WeaponSystem : MonoBehaviour
{
    [Header("Hardware")]
    public WeaponSelector selector; 
    public ActiveRadarSensor fireControlRadar; 
    public BearingControl bearingComputer; 

    [Header("Armory")]
    public GameObject sarhMissilePrefab;
    public GameObject arhMissilePrefab; 
    public Transform launchPoint;
    public float firingAngleOffset = 180f; 

    [Header("ARH Settings")]
    public float arhCooldownTime = 5.0f; // Seconds between ARH launches
    public TMP_Text arhCooldownText;     // Drag your UI Text here
    private float _currentArhCooldown = 0f;

    // We store a reference to the single active SARH missile
    private HomingMissile _activeSARHMissile;

    void Update()
    {
        // ARH Cooldown Timer Logic
        if (_currentArhCooldown > 0)
        {
            _currentArhCooldown -= Time.deltaTime;
            
            // Update UI Text while cooling down
            if (arhCooldownText != null)
            {
                arhCooldownText.text = _currentArhCooldown > 0 ? _currentArhCooldown.ToString("F1") + "s" : "RDY";
            }
        }
        else if (arhCooldownText != null && arhCooldownText.text != "RDY")
        {
            // Safety catch to ensure it says RDY when at 0
            arhCooldownText.text = "RDY";
        }
    }

    public void FireSequence()
    {
        if (selector.currentWeapon == WeaponSelector.WeaponType.SemiActive)
        {
            // --- MODE 1: SARH ---
            GameObject target = fireControlRadar.GetCurrentTarget();
            if (target != null)
            {
                SpawnSARH(target);
            }
        }
        else
        {
            // --- MODE 2: ARH ---
            // Only fire if the cooldown has finished!
            if (_currentArhCooldown <= 0f)
            {
                SpawnARH();
                _currentArhCooldown = arhCooldownTime; // Start the timer!
            }
            else
            {
                Debug.Log("ARH is still reloading...");
            }
        }
    }

    void SpawnSARH(GameObject target)
    {
        // 1. If we already have a missile flying, tell it to drop the lock!
        if (_activeSARHMissile != null)
        {
            _activeSARHMissile.LoseLock();
        }

        // 2. Spawn the new missile and save it as the active one
        GameObject m = Instantiate(sarhMissilePrefab, launchPoint.position, Quaternion.identity);
        _activeSARHMissile = m.GetComponent<HomingMissile>();
        _activeSARHMissile.Launch(target, fireControlRadar);
    }

    void SpawnARH()
    {
        float finalAngle = bearingComputer.currentBearing + firingAngleOffset;
        Quaternion launchRotation = Quaternion.Euler(0, 0, finalAngle);

        GameObject m = Instantiate(arhMissilePrefab, launchPoint.position, Quaternion.identity);
        m.GetComponent<ActiveHomingMissile>().Launch(launchRotation);
    }
}