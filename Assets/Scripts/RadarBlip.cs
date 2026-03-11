using UnityEngine;

public class RadarBlip : MonoBehaviour
{
    [Header("Phosphor Decay")]
    public float decayTime = 2.0f; // Time in seconds to vanish

    private SpriteRenderer _renderer;
    private float _timer;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {

        Vector3 fixedPos = transform.position;
        fixedPos.z = -0.1f;
        transform.position = fixedPos;
        // Default to the inspector value
        float life = decayTime;

        // OVERRIDE with Zoom System
        // (Long range = Blips stay on screen longer)
        if (RadarZoomSystem.Instance != null)
        {
            life = RadarZoomSystem.Instance.GetCurrentBlipLifetime();
        }

        // Update internal timer and reference
        decayTime = life;
        _timer = life;
    }

    void Update()
    {
        // 1. Count down
        _timer -= Time.deltaTime;

        // 2. Calculate transparency (1.0 is visible, 0.0 is invisible)
        float alpha = Mathf.Clamp01(_timer / decayTime);

        // 3. Apply the fading color
        if (_renderer != null)
        {
            Color newColor = _renderer.color;
            newColor.a = alpha;
            _renderer.color = newColor;
        }

        // 4. Cleanup to keep the game running smooth
        if (_timer <= 0)
        {
            Destroy(gameObject);
        }
    }
}