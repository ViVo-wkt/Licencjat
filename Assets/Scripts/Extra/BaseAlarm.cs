using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BaseAlarm : MonoBehaviour
{
    public static BaseAlarm Instance;

    [Header("Lighting")]
    public Light2D globalLight;
    public Color normalColor = Color.white;
    public Color alarmColor = Color.red;
    
    [Header("Timing")]
    public float alarmDuration = 0.5f;
    
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
            // --- FIXED: Uses unscaledDeltaTime to ignore Time.timeScale = 0f ---
            _alarmTimer -= Time.unscaledDeltaTime;
            
            float t = 1 - (_alarmTimer / alarmDuration);
            globalLight.color = Color.Lerp(alarmColor, normalColor, t);
        }
    }

    public void TriggerAlarm()
    {
        _alarmTimer = alarmDuration;
        if (globalLight != null) globalLight.color = alarmColor;
    }
}