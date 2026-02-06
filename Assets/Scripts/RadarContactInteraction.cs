using UnityEngine;
using UnityEngine.InputSystem;

public class RadarContactInteraction : MonoBehaviour
{
    private GameObject _trackedEnemy;
    private TargetSignature _enemyData;

    // Called by ActiveRadarSensor when spawning this marker
    public void Initialize(GameObject enemy)
    {
        _trackedEnemy = enemy;
        _enemyData = enemy.GetComponent<TargetSignature>();
    }

    void OnMouseDown()
    {
        // This relies on the object having a Collider2D (which your prefab likely has)
        if (_enemyData != null)
        {
            Debug.Log($"Selected Target: {_enemyData.codename}");
            

            RadarUIManager.Instance.ShowTargetInfo(_enemyData);
        }
    }
}