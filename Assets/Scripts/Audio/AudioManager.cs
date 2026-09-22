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
    public AudioSource uiSfxSource;

    [Header("Shared Audio Clips")]
    public AudioClip defaultClickSound;

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

    public void SetMasterVolume(float knobPercentage)
    {
        _currentKnobPercentage = knobPercentage;
        PlayerPrefs.SetFloat("AmbientVolume", knobPercentage);

        if (ambientHumSource != null) ambientHumSource.volume = knobPercentage * maxVolumeCap;
        if (launchSfxSource != null) launchSfxSource.volume = knobPercentage * maxVolumeCap;
        
        if (uiSfxSource != null) uiSfxSource.volume = knobPercentage * maxVolumeCap; 
    }

    public void PlayClickSound(AudioClip customClip = null)
    {
        if (uiSfxSource == null) return;

        AudioClip clipToPlay = customClip != null ? customClip : defaultClickSound;

        if (clipToPlay != null)
        {
            uiSfxSource.PlayOneShot(clipToPlay);
        }
    }

    void Start()
    {
        float initialVolume = PlayerPrefs.GetFloat("AmbientVolume", 0.5f);
        SetMasterVolume(initialVolume);
    }
}