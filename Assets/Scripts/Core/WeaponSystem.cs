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
    public TMP_Text sarhStatusText;

    [Header("Ammo Resupply (1 per 60s after first shot, max 99)")]
    public float resupplyInterval = 60.0f;
    public int maxAmmoCap = 99;
    public float flashDuration = 0.5f;

    private bool _sarhStartedResupply = false;
    private bool _arhStartedResupply = false;
    private bool _autoStartedResupply = false;

    private float _sarhResupplyTimer = 0f;
    private float _arhResupplyTimer = 0f;
    private float _autoResupplyTimer = 0f;

    private float _sarhFlashTimer = 0f;
    private float _arhFlashTimer = 0f;
    private float _autoFlashTimer = 0f;

    [Header("ARH Settings")]
    public float arhCooldownTime = 5.0f;
    public TMP_Text arhCooldownText;
    private float _currentArhCooldown = 0f;

    [Header("Auto Missile Settings")]
    public float autoCooldownTime = 4.0f;
    public TMP_Text autoCooldownText;
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
        // 1. ARH Cooldown
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

        // 2. AUTO Cooldown
        if (_currentAutoCooldown > 0)
        {
            _currentAutoCooldown -= Time.deltaTime;
            if (autoCooldownText != null)
                autoCooldownText.text = _currentAutoCooldown > 0 ? "Reloading: " + _currentAutoCooldown.ToString("F1") + "s" : "Ready to fire";
        }
        else if (autoCooldownText != null && autoCooldownText.text != "Ready to fire")
        {
            autoCooldownText.text = "Ready to fire";
        }

        // 3. Flash Timers
        if (_sarhFlashTimer > 0) _sarhFlashTimer -= Time.deltaTime;
        if (_arhFlashTimer > 0) _arhFlashTimer -= Time.deltaTime;
        if (_autoFlashTimer > 0) _autoFlashTimer -= Time.deltaTime;

        // 4. Resupply Tick
        UpdateResupply();

        // 5. Screen & UI Updates
        UpdateAmmoUI();
        UpdateDataScreens();
        UpdateSarhStatusUI();
    }

    void UpdateResupply()
    {
        if (_sarhStartedResupply && sarhAmmo < maxAmmoCap)
        {
            _sarhResupplyTimer += Time.deltaTime;
            if (_sarhResupplyTimer >= resupplyInterval)
            {
                sarhAmmo = Mathf.Min(sarhAmmo + 1, maxAmmoCap);
                _sarhResupplyTimer = 0f;
                _sarhFlashTimer = flashDuration;
            }
        }

        if (_arhStartedResupply && arhAmmo < maxAmmoCap)
        {
            _arhResupplyTimer += Time.deltaTime;
            if (_arhResupplyTimer >= resupplyInterval)
            {
                arhAmmo = Mathf.Min(arhAmmo + 1, maxAmmoCap);
                _arhResupplyTimer = 0f;
                _arhFlashTimer = flashDuration;
            }
        }

        if (_autoStartedResupply && autoAmmo < maxAmmoCap)
        {
            _autoResupplyTimer += Time.deltaTime;
            if (_autoResupplyTimer >= resupplyInterval)
            {
                autoAmmo = Mathf.Min(autoAmmo + 1, maxAmmoCap);
                _autoResupplyTimer = 0f;
                _autoFlashTimer = flashDuration;
            }
        }
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
            _autoStartedResupply = true;
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
        _sarhStartedResupply = true;

        GameObject m = Instantiate(sarhMissilePrefab, launchPoint.position, Quaternion.identity);
        _activeSARHMissile = m.GetComponent<PassiveMissile>();
        _activeSARHMissile.Launch(target, fireControlRadar);
    }

    void SpawnARH()
    {
        arhAmmo--;
        _arhStartedResupply = true;

        float finalAngle = bearingComputer.currentBearing + firingAngleOffset;
        Quaternion launchRotation = Quaternion.Euler(0, 0, finalAngle);

        GameObject m = Instantiate(arhMissilePrefab, launchPoint.position, Quaternion.identity);
        m.GetComponent<ActiveHomingMissile>().Launch(launchRotation);
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

    void UpdateAmmoUI()
    {
        if (sarhAmmoText != null)
        {
            string colorCode = GetAmmoColor(sarhAmmo, _sarhFlashTimer > 0, false);
            sarhAmmoText.text = $"<color={colorCode}>Interceptors: {sarhAmmo:D2}</color>";
        }

        if (arhAmmoText != null)
        {
            string colorCode = GetAmmoColor(arhAmmo, _arhFlashTimer > 0, _currentArhCooldown > 0);
            arhAmmoText.text = $"<color={colorCode}>Interceptors: {arhAmmo:D2}</color>";
        }

        if (autoAmmoText != null)
        {
            string colorCode = GetAmmoColor(autoAmmo, _autoFlashTimer > 0, _currentAutoCooldown > 0);
            autoAmmoText.text = $"<color={colorCode}>Interceptors: {autoAmmo:D2}</color>";
        }
    }

    string GetAmmoColor(int ammoCount, bool isFlashing, bool isReloading)
    {
        if (isFlashing) return "#00FF00";      
        if (ammoCount <= 0) return "#FF0000";   
        if (isReloading) return "#888888";      
        return "#FFFFFF";                       
    }

    void UpdateSarhStatusUI()
    {
        if (sarhStatusText == null) return;

        if (_activeSARHMissile == null)
        {
            sarhStatusText.text = "<color=#AAAAAA>WAITING</color>";
        }
        else if (_activeSARHMissile.IsTrackingTarget())
        {
            sarhStatusText.text = "<color=#00FF00>TRACKING</color>";
        }
        else
        {
            sarhStatusText.text = "<color=#FF0000>LOST TRACK</color>";
        }
    }

    public void PlayLaunchSound(WeaponSelector.WeaponType weaponType)
    {
        if (AudioManager.Instance == null || AudioManager.Instance.launchSfxSource == null) return;

        if (Time.time - _lastLaunchTime >= launchSoundCooldown)
        {
            AudioClip clipToPlay = null;
            if (weaponType == WeaponSelector.WeaponType.SemiActive) clipToPlay = sarhLaunchClip;
            else if (weaponType == WeaponSelector.WeaponType.Active) clipToPlay = arhLaunchClip;
            else if (weaponType == WeaponSelector.WeaponType.AutoTarget) clipToPlay = autoLaunchClip;

            if (clipToPlay != null)
            {
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