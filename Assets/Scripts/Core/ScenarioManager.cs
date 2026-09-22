using UnityEngine;
using System.Collections.Generic;

public class ScenarioManager : MonoBehaviour
{
    [System.Serializable]
    public struct EnemyThreat
    {
        public string name;
        public GameObject prefab;
        [Range(1, 100)] public int spawnWeight;
    }

    [Header("Threat Definition")]
    public EnemyThreat[] possibleThreats;

    [Header("Spawn Settings")]
    public float spawnRadius = 5.0f;
    public float spawnInterval = 5.0f;

    private float _timer;

    void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            SpawnRandomThreat();
            _timer = spawnInterval;
        }
    }

    void SpawnRandomThreat()
    {
        if (possibleThreats.Length == 0) return;

        int totalWeight = 0;
        foreach (var threat in possibleThreats) totalWeight += threat.spawnWeight;

        int randomValue = Random.Range(0, totalWeight);

        GameObject selectedPrefab = null;
        int currentWeightSum = 0;

        foreach (var threat in possibleThreats)
        {
            currentWeightSum += threat.spawnWeight;
            if (randomValue < currentWeightSum)
            {
                selectedPrefab = threat.prefab;
                break;
            }
        }

        if (selectedPrefab != null)
        {
            float angle = Random.Range(0f, 360f);
            Vector2 spawnPos = new Vector2(
                Mathf.Cos(angle * Mathf.Deg2Rad), 
                Mathf.Sin(angle * Mathf.Deg2Rad)
            ) * spawnRadius;

            Instantiate(selectedPrefab, spawnPos, Quaternion.identity);
            
            if (RadarWarningRing.Instance != null)
            {
                RadarWarningRing.Instance.NotifyThreatSpawn(spawnPos);
            }
        }
    }
}