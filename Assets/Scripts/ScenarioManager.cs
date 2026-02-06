using UnityEngine;

public class ScenarioManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemyPrefab;
    public float spawnRadius = 5.0f; // Edge of the radar
    public float spawnInterval = 5.0f; // Seconds between enemies

    private float _timer;

    void Update()
    {
        _timer -= Time.deltaTime;

        if (_timer <= 0)
        {
            SpawnEnemy();
            _timer = spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        // 1. Pick a random angle
        float angle = Random.Range(0f, 360f);
        
        // 2. Convert angle to a position on the circle edge
        // Math: x = cos(angle) * radius, y = sin(angle) * radius
        Vector2 spawnPos = new Vector2(
            Mathf.Cos(angle * Mathf.Deg2Rad), 
            Mathf.Sin(angle * Mathf.Deg2Rad)
        ) * spawnRadius;

        // 3. Spawn
        Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
    }
}