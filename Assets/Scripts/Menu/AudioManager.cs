using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Volume Settings")]
    [Range(0f, 1f)]
    public float maxVolumeCap = 0.5f;

    [Header("Audio Sources")]
    public AudioSource ambientHumSource;
    public AudioSource launchSfxSource;

    private float _currentKnobPercentage = 0.5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // This is called by your VolumeKnob script
    public void SetMasterVolume(float knobPercentage)
    {
        _currentKnobPercentage = knobPercentage;
        PlayerPrefs.SetFloat("AmbientVolume", knobPercentage);

        // Apply volume to Ambient Hum
        if (ambientHumSource != null)
            ambientHumSource.volume = knobPercentage * maxVolumeCap;

        // Apply volume to Launch SFX
        if (launchSfxSource != null)
            launchSfxSource.volume = knobPercentage * maxVolumeCap;
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("AmbientVolume"))
            SetMasterVolume(PlayerPrefs.GetFloat("AmbientVolume"));
    }
}