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
    public GameObject autoMissilePrefab;
    public Transform launchPoint;
    public float firingAngleOffset = 180f;

    [Header("Ammunition & Logistics")]
    public int sarhAmmo = 20;
    public int arhAmmo = 10;
    public int autoAmmo = 15;
    public TMP_Text sarhAmmoText;
    public TMP_Text arhAmmoText;
    public TMP_Text autoAmmoText;

    [Header("ARH Settings")]
    public float arhCooldownTime = 5.0f;
    public TMP_Text arhCooldownText;
    private float _currentArhCooldown = 0f;

    [Header("Auto Missile Settings")]
    public float autoCooldownTime = 1.0f;
    private float _currentAutoCooldown = 0f;

    private PassiveMissile _activeSARHMissile;

    void Start() { UpdateAmmoUI(); }

    void Update()
    {
        if (_currentArhCooldown > 0)
        {
            _currentArhCooldown -= Time.deltaTime;
            if (arhCooldownText != null)
                arhCooldownText.text = _currentArhCooldown > 0 ? _currentArhCooldown.ToString("F1") + "s" : "RDY";
        }
        else if (arhCooldownText != null && arhCooldownText.text != "RDY")
            arhCooldownText.text = "RDY";

        if (_currentAutoCooldown > 0) _currentAutoCooldown -= Time.deltaTime;
    }

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
    }

    // CHANGED: Now returns a bool, and passes 'null' to radar!
    public bool FireAutoMissile(GameObject target)
    {
        if (autoAmmo > 0 && _currentAutoCooldown <= 0f)
        {
            autoAmmo--;
            UpdateAmmoUI();
            _currentAutoCooldown = autoCooldownTime;

            GameObject m = Instantiate(autoMissilePrefab, launchPoint.position, Quaternion.identity);

            // PASSING NULL disconnects it from the SARH dependency!
            m.GetComponent<PassiveMissile>().Launch(target, null);
            return true;
        }
        return false; // Failed to fire (cooldown or no ammo)
    }

    void SpawnSARH(GameObject target)
    {
        if (_activeSARHMissile != null) _activeSARHMissile.LoseLock();
        sarhAmmo--;
        UpdateAmmoUI();

        GameObject m = Instantiate(sarhMissilePrefab, launchPoint.position, Quaternion.identity);
        _activeSARHMissile = m.GetComponent<PassiveMissile>();
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
        if (sarhAmmoText != null) sarhAmmoText.text = "SARH Interceptors: " + sarhAmmo.ToString("D2");
        if (arhAmmoText != null) arhAmmoText.text = "ARH Interceptors: " + arhAmmo.ToString("D2");
        if (autoAmmoText != null) autoAmmoText.text = "AUTO Interceptors: " + autoAmmo.ToString("D2");
    }

    public void FireSelectedWeapon()
    {
        if (selector == null) return;

        // Fire ARH
        if (selector.currentWeapon == WeaponSelector.WeaponType.Active)
        {
            if (_currentArhCooldown <= 0 && arhAmmo > 0)
            {
                SpawnARH();
                _currentArhCooldown = arhCooldownTime;
            }
        }
        // Fire SARH
        else if (selector.currentWeapon == WeaponSelector.WeaponType.SemiActive)
        {
            if (sarhAmmo > 0 && fireControlRadar != null)
            {
                GameObject target = fireControlRadar.GetCurrentTarget();
                if (target != null)
                {
                    SpawnSARH(target);
                }
            }
        }
        // Fire AUTO
        else if (selector.currentWeapon == WeaponSelector.WeaponType.AutoTarget)
        {
             // For AUTO, the TargetingBracket handles the actual firing, 
             // but if you want the button to trigger a general sweep or just click, 
             // you might need to link the TargetBracket here. 
             // For now, we will leave this blank because your Targeting Bracket handles AUTO firing on its own!
             Debug.Log("Auto Missiles are fired automatically via the Targeting Bracket!");
        }
    }
}