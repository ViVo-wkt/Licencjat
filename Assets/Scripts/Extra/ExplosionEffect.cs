using UnityEngine;

public class ExplosionEffect : MonoBehaviour
{
    [Header("Settings")]
    public float lifetime = 0.5f;
    public float expandSpeed = 5f;

    void Start()
    {
        // Auto-cleanup
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        transform.localScale += Vector3.one * expandSpeed * Time.deltaTime;
    }
}