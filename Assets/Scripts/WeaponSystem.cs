using UnityEngine;

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

    // Note: Update() with the Spacebar input was removed from here

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
        float finalAngle = bearingComputer.currentBearing + firingAngleOffset;
        Quaternion launchRotation = Quaternion.Euler(0, 0, finalAngle);

        GameObject m = Instantiate(arhMissilePrefab, launchPoint.position, Quaternion.identity);
        m.GetComponent<ActiveHomingMissile>().Launch(launchRotation);
    }
}