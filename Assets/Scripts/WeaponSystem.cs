using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSystem : MonoBehaviour
{
    [Header("Armory")]
    public GameObject missilePrefab;
    public Transform launchPoint; // Where missiles come from (center of screen or a silo)

    [Header("Sensors")]
    public ActiveRadarSensor fireControlRadar; // Reference to the Active Beam script

    void Update()
    {
        // Check for Spacebar input (New Input System or Old)
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            FireSequence();
        }
    }

    void FireSequence()
    {
        // 1. Get the current locked target from the Active Radar
        GameObject target = fireControlRadar.GetCurrentTarget();

        if (target != null)
        {
            Debug.Log("Fox One! Missile away!");
            SpawnMissile(target);
        }
        else
        {
            Debug.Log("Negative. No valid firing solution.");
        }
    }

    void SpawnMissile(GameObject target)
    {
        // Spawn at launch point, or center of screen if null
        Vector3 spawnPos = launchPoint != null ? launchPoint.position : Vector3.zero;
        
        GameObject missileObj = Instantiate(missilePrefab, spawnPos, Quaternion.identity);
        HomingMissile missileScript = missileObj.GetComponent<HomingMissile>();
        
        if (missileScript != null)
        {
            missileScript.Launch(target);
        }
    }
}