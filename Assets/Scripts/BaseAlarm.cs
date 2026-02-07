using UnityEngine;
using UnityEngine.Rendering.Universal; // Required for Light2D

public class BaseAlarm : MonoBehaviour
{
    public static BaseAlarm Instance;

    [Header("Lighting")]
    public Light2D globalLight;
    public Color normalColor = Color.white;
    public Color alarmColor = Color.red;
    
    [Header("Timing")]
    public float alarmDuration = 0.5f; // How long it stays red
    
    private float _alarmTimer = 0f;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (globalLight == null) return;

        if (_alarmTimer > 0)
        {
            // Flash is active
            _alarmTimer -= Time.deltaTime;
            
            // Lerp back to normal as timer runs out
            float t = 1 - (_alarmTimer / alarmDuration); // 0 to 1
            globalLight.color = Color.Lerp(alarmColor, normalColor, t);
        }
    }

    // Call this when something bad happens
    public void TriggerAlarm()
    {
        _alarmTimer = alarmDuration;
        if (globalLight != null) globalLight.color = alarmColor;
    }
}