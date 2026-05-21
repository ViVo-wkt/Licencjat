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
    public AudioSource uiSfxSource; // <-- NEW: Dedicated channel for buttons

    [Header("Shared Audio Clips")]
    public AudioClip defaultClickSound; // <-- NEW: Store your click sound here!

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
        
        // --- NEW: Syncs the UI clicks to the knob and the 50% cap! ---
        if (uiSfxSource != null) uiSfxSource.volume = knobPercentage * maxVolumeCap; 
    }

    // --- NEW: Any script in the game can trigger this single method ---
    public void PlayClickSound(AudioClip customClip = null)
    {
        if (uiSfxSource == null) return;

        // Decide which clip to use
        AudioClip clipToPlay = customClip != null ? customClip : defaultClickSound;

        if (clipToPlay != null)
        {
            uiSfxSource.PlayOneShot(clipToPlay);
        }
    }

    void Start()
    {
        if (PlayerPrefs.HasKey("AmbientVolume"))
            SetMasterVolume(PlayerPrefs.GetFloat("AmbientVolume"));
    }
}