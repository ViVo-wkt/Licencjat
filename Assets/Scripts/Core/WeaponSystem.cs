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

    [Header("Screen UI (TextMeshPro)")]
    public TMP_Text screenTitleText;
    public TMP_Text screenDetailsText;
    public string[] weaponTitles = new string[3] { "SARH - AIM-7", "ARH - AIM-120", "AUTO - AIM-9" };
    [TextArea(3, 6)]
    public string[] weaponDetails = new string[3] { 
        "GUIDANCE: SEMI-ACTIVE\nRANGE: 40 NMI\nREQUIRES CONTINUOUS LOCK", 
        "GUIDANCE: ACTIVE\nRANGE: 60 NMI\nFIRE AND FORGET CAPABLE", 
        "GUIDANCE: INFRARED\nRANGE: 10 NMI\nSHORT RANGE DOGFIGHTING" 
    };

    [Header("Launch Audio")]
    public AudioSource launchAudioSource;
    public AudioClip sarhLaunchClip;
    public AudioClip arhLaunchClip;
    public AudioClip autoLaunchClip;
    public float launchSoundCooldown = 2.0f;
    private float _lastLaunchTime = -999f;

    private PassiveMissile _activeSARHMissile;

    void Start() 
    { 
        UpdateAmmoUI(); 
    }

    void Update()
    {
        if (_currentArhCooldown > 0)
        {
            _currentArhCooldown -= Time.deltaTime;
            if (arhCooldownText != null)
                arhCooldownText.text = _currentArhCooldown > 0 ? "Reloading: " + _currentArhCooldown.ToString("F1") + "s" : "Ready to fire";
        }
        else if (arhCooldownText != null && arhCooldownText.text != "Ready to fire")
        {
            arhCooldownText.text = "Ready to fire";
        }

        if (_currentAutoCooldown > 0) _currentAutoCooldown -= Time.deltaTime;

        UpdateDataScreens();
    }

    public void FireSequence()
    {
        if ((int)selector.currentWeapon == 0)
        {
            if (sarhAmmo > 0)
            {
                GameObject target = fireControlRadar.GetCurrentTarget();
                if (target != null)
                {
                    SpawnSARH(target);
                    PlayLaunchSound(WeaponSelector.WeaponType.SemiActive);
                }
            }
        }
        else if ((int)selector.currentWeapon == 1)
        {
            if (arhAmmo > 0 && _currentArhCooldown <= 0f)
            {
                SpawnARH();
                _currentArhCooldown = arhCooldownTime;
                PlayLaunchSound(WeaponSelector.WeaponType.Active);
            }
        }
    }

    public bool FireAutoMissile(GameObject target)
    {
        if (autoAmmo > 0 && _currentAutoCooldown <= 0f)
        {
            autoAmmo--;
            UpdateAmmoUI();
            _currentAutoCooldown = autoCooldownTime;

            GameObject m = Instantiate(autoMissilePrefab, launchPoint.position, Quaternion.identity);
            m.GetComponent<PassiveMissile>().Launch(target, null);
            
            PlayLaunchSound(WeaponSelector.WeaponType.AutoTarget);
            return true;
        }
        return false;
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
        if (sarhAmmoText != null) sarhAmmoText.text = "Interceptors: " + sarhAmmo.ToString("D2");
        if (arhAmmoText != null) arhAmmoText.text = "Interceptors: " + arhAmmo.ToString("D2");
        if (autoAmmoText != null) autoAmmoText.text = "Interceptors: " + autoAmmo.ToString("D2");
    }

    public void FireSelectedWeapon()
    {
        if (selector == null) return;

        if (selector.currentWeapon == WeaponSelector.WeaponType.Active)
        {
            if (_currentArhCooldown <= 0 && arhAmmo > 0)
            {
                SpawnARH();
                _currentArhCooldown = arhCooldownTime;
                PlayLaunchSound(WeaponSelector.WeaponType.Active);
            }
        }
        else if (selector.currentWeapon == WeaponSelector.WeaponType.SemiActive)
        {
            if (sarhAmmo > 0 && fireControlRadar != null)
            {
                GameObject target = fireControlRadar.GetCurrentTarget();
                if (target != null)
                {
                    SpawnSARH(target);
                    PlayLaunchSound(WeaponSelector.WeaponType.SemiActive);
                }
            }
        }
    }

    public void PlayLaunchSound(WeaponSelector.WeaponType weaponType)
    {
    // Point this to the AudioManager's source instead of a local variable
    if (AudioManager.Instance == null || AudioManager.Instance.launchSfxSource == null) return;

    if (Time.time - _lastLaunchTime >= launchSoundCooldown)
    {
        AudioClip clipToPlay = null;
        if (weaponType == WeaponSelector.WeaponType.SemiActive) clipToPlay = sarhLaunchClip;
        else if (weaponType == WeaponSelector.WeaponType.Active) clipToPlay = arhLaunchClip;
        else if (weaponType == WeaponSelector.WeaponType.AutoTarget) clipToPlay = autoLaunchClip;

        if (clipToPlay != null)
        {
            // Play through the AudioManager's source so it's already capped and scaled!
            AudioManager.Instance.launchSfxSource.PlayOneShot(clipToPlay);
            _lastLaunchTime = Time.time; 
        }
    }
    }   

    void UpdateDataScreens()
    {
        if (selector == null) return;
        int index = (int)selector.currentWeapon;

        if (screenTitleText != null) screenTitleText.text = weaponTitles[index];

        if (screenDetailsText != null)
        {
            string baseText = weaponDetails[index];
            string liveData = "\n\n";

            if (selector.currentWeapon == WeaponSelector.WeaponType.SemiActive)
            {
                liveData += "AMMO: " + sarhAmmo + "\nSTATUS: <color=#00FF00>READY</color>";
            }
            else if (selector.currentWeapon == WeaponSelector.WeaponType.Active)
            {
                liveData += "AMMO: " + arhAmmo + "\nSTATUS: ";
                liveData += (_currentArhCooldown > 0) ? "<color=#FF0000>RELOADING</color>" : "<color=#00FF00>READY</color>";
            }
            else if (selector.currentWeapon == WeaponSelector.WeaponType.AutoTarget)
            {
                liveData += "AMMO: " + autoAmmo + "\nSTATUS: ";
                liveData += (_currentAutoCooldown > 0) ? "<color=#FF0000>RELOADING</color>" : "<color=#00FF00>READY</color>";
            }

            screenDetailsText.text = baseText + liveData;
        }
    }
}