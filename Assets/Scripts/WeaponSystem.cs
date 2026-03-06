using UnityEngine;
using TMPro; 

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

    [Header("Ammunition & Logistics")]
    public int sarhAmmo = 20;            // Starting SARH missiles
    public int arhAmmo = 10;             // Starting ARH missiles
    public TMP_Text sarhAmmoText;        // Drag UI Text for SARH ammo here
    public TMP_Text arhAmmoText;         // Drag UI Text for ARH ammo here

    [Header("ARH Settings")]
    public float arhCooldownTime = 5.0f; 
    public TMP_Text arhCooldownText;     
    private float _currentArhCooldown = 0f;

    private HomingMissile _activeSARHMissile;

    void Start()
    {
        // Force the UI to show the starting ammo right away
        UpdateAmmoUI();
    }

    void Update()
    {
        // ARH Cooldown Timer Logic
        if (_currentArhCooldown > 0)
        {
            _currentArhCooldown -= Time.deltaTime;
            
            if (arhCooldownText != null)
            {
                arhCooldownText.text = _currentArhCooldown > 0 ? _currentArhCooldown.ToString("F1") + "s" : "RDY";
            }
        }
        else if (arhCooldownText != null && arhCooldownText.text != "RDY")
        {
            arhCooldownText.text = "RDY";
        }
    }

    public void FireSequence()
    {
        if (selector.currentWeapon == WeaponSelector.WeaponType.SemiActive)
        {
            // --- MODE 1: SARH ---
            if (sarhAmmo > 0)
            {
                GameObject target = fireControlRadar.GetCurrentTarget();
                if (target != null)
                {
                    SpawnSARH(target);
                }
                else
                {
                    Debug.Log("No target locked! Cannot fire SARH.");
                }
            }
            else
            {
                Debug.Log("Out of SARH missiles!");
            }
        }
        else
        {
            // --- MODE 2: ARH ---
            if (arhAmmo > 0)
            {
                if (_currentArhCooldown <= 0f)
                {
                    SpawnARH();
                    _currentArhCooldown = arhCooldownTime; 
                }
                else
                {
                    Debug.Log("ARH is still reloading...");
                }
            }
            else
            {
                Debug.Log("Out of ARH missiles!");
            }
        }
    }

    void SpawnSARH(GameObject target)
    {
        // If we already have a missile flying, tell it to drop the lock!
        if (_activeSARHMissile != null)
        {
            _activeSARHMissile.LoseLock();
        }

        // Deduct ammo
        sarhAmmo--;
        UpdateAmmoUI();

        GameObject m = Instantiate(sarhMissilePrefab, launchPoint.position, Quaternion.identity);
        _activeSARHMissile = m.GetComponent<HomingMissile>();
        _activeSARHMissile.Launch(target, fireControlRadar);
    }

    void SpawnARH()
    {
        // Deduct ammo
        arhAmmo--;
        UpdateAmmoUI();

        float finalAngle = bearingComputer.currentBearing + firingAngleOffset;
        Quaternion launchRotation = Quaternion.Euler(0, 0, finalAngle);

        GameObject m = Instantiate(arhMissilePrefab, launchPoint.position, Quaternion.identity);
        m.GetComponent<ActiveHomingMissile>().Launch(launchRotation);
    }

    // A helper method to easily update both text elements at the same time
    void UpdateAmmoUI()
    {
        // You can format this string however you want your panel to look!
        if (sarhAmmoText != null) sarhAmmoText.text = sarhAmmo.ToString("D2"); 
        if (arhAmmoText != null) arhAmmoText.text = arhAmmo.ToString("D2"); 
    }
}