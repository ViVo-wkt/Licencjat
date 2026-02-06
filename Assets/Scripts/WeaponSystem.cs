using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSystem : MonoBehaviour
{
    [Header("Hardware")]
    public WeaponSelector selector; // Drag your UI object here
    public ActiveRadarSensor fireControlRadar; // Existing SARH Radar
    public BearingControl bearingComputer; // Your NEW Compass Knob

    [Header("Armory")]
    public GameObject sarhMissilePrefab;
    public GameObject arhMissilePrefab; // New Fire & Forget prefab
    public Transform launchPoint;

    // Call this from Spacebar or Fire Button
    public void FireSequence()
    {
        if (selector.currentWeapon == WeaponSelector.WeaponType.SemiActive)
        {
            // --- MODE 1: SARH (Old Logic) ---
            GameObject target = fireControlRadar.GetCurrentTarget();
            if (target != null)
            {
                SpawnSARH(target);
            }
        }
        else
        {
            // --- MODE 2: ARH (New Logic) ---
            // No lock needed! Just fire at the compass bearing.
            SpawnARH();
        }
    }

    void SpawnSARH(GameObject target)
    {
        GameObject m = Instantiate(sarhMissilePrefab, launchPoint.position, Quaternion.identity);
        m.GetComponent<HomingMissile>().Launch(target, fireControlRadar);
    }

    void SpawnARH()
    {
        // Calculate launch rotation based on the knob's bearing
        Quaternion launchRotation = Quaternion.Euler(0, 0, bearingComputer.currentBearing);
        
        GameObject m = Instantiate(arhMissilePrefab, launchPoint.position, Quaternion.identity);
        m.GetComponent<ActiveHomingMissile>().Launch(launchRotation);
    }
}