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
    public GameObject autoMissilePrefab; // NEW: The 3rd missile
    public Transform launchPoint;
    public float firingAngleOffset = 180f;

    [Header("Ammunition & Logistics")]
    public int sarhAmmo = 20;
    public int arhAmmo = 10;
    public int autoAmmo = 15;            // NEW: Auto missile ammo
    public TMP_Text sarhAmmoText;
    public TMP_Text arhAmmoText;
    public TMP_Text autoAmmoText;        // NEW: Auto ammo UI text

    [Header("ARH Settings")]
    public float arhCooldownTime = 5.0f;
    public TMP_Text arhCooldownText;
    private float _currentArhCooldown = 0f;

    [Header("Auto Missile Settings")]
    public float autoCooldownTime = 1.0f; // Time between automatic launches
    private float _currentAutoCooldown = 0f;

    private HomingMissile _activeSARHMissile;

    void Start()
    {
        UpdateAmmoUI();
    }

    void Update()
    {
        // ARH Cooldown
        if (_currentArhCooldown > 0)
        {
            _currentArhCooldown -= Time.deltaTime;
            if (arhCooldownText != null)
                arhCooldownText.text = _currentArhCooldown > 0 ? _currentArhCooldown.ToString("F1") + "s" : "RDY";
        }
        else if (arhCooldownText != null && arhCooldownText.text != "RDY")
            arhCooldownText.text = "RDY";

        // Auto Missile Cooldown
        if (_currentAutoCooldown > 0)
        {
            _currentAutoCooldown -= Time.deltaTime;
        }
    }

    // Manual Fire (triggered by Launch Button)
    public void FireSequence()
    {
        if ((int)selector.currentWeapon == 0)
        {
            if (sarhAmmo > 0)
            {
                GameObject target = fireControlRadar.GetCurrentTarget();
                if (target != null) SpawnSARH(target);
            }
        }
        else if ((int)selector.currentWeapon == 1)
        {
            if (arhAmmo > 0 && _currentArhCooldown <= 0f)
            {
                SpawnARH();
                _currentArhCooldown = arhCooldownTime;
            }
        }
        else
        {
            Debug.Log("Auto Mode Selected: Missiles fire automatically via the Targeting Bracket.");
        }
    }

    // NEW: Automatic Fire (Triggered by the Bracket)
    public void FireAutoMissile(GameObject target)
    {
        // Check ammo and ensure cooldown is finished
        if (autoAmmo > 0 && _currentAutoCooldown <= 0f)
        {
            autoAmmo--;
            UpdateAmmoUI();
            _currentAutoCooldown = autoCooldownTime;

            // Spawn and launch
            GameObject m = Instantiate(autoMissilePrefab, launchPoint.position, Quaternion.identity);
            m.GetComponent<HomingMissile>().Launch(target, fireControlRadar);
        }
    }

    void SpawnSARH(GameObject target)
    {
        if (_activeSARHMissile != null) _activeSARHMissile.LoseLock();
        sarhAmmo--;
        UpdateAmmoUI();

        GameObject m = Instantiate(sarhMissilePrefab, launchPoint.position, Quaternion.identity);
        _activeSARHMissile = m.GetComponent<HomingMissile>();
        _activeSARHMissile.Launch(target, fireControlRadar);
    }

    void SpawnARH()
    {
        arhAmmo--;
        UpdateAmmoUI();

        float finalAngle = bearingComputer.currentBearing + firingAngleOffset;
        Quaternion launchRotation = Quaternion.Euler(0, 0, finalAngle);

        GameObject m = Instantiate(arhMissilePrefab, launchPoint.position, Quaternion.identity);
        m.GetComponent<ActiveHomingMissile>().Launch(launchRotation);
    }

    void UpdateAmmoUI()
    {
        if (sarhAmmoText != null) sarhAmmoText.text = "SARH: " + sarhAmmo.ToString("D2");
        if (arhAmmoText != null) arhAmmoText.text = "ARH: " + arhAmmo.ToString("D2");
        if (autoAmmoText != null) autoAmmoText.text = "AUTO: " + autoAmmo.ToString("D2");
    }
}