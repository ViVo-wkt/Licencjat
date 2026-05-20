using UnityEngine;

public class AmbientHumManager : MonoBehaviour
{
    public static AmbientHumManager Instance;
    private AudioSource _myAudio;

    [Header("Settings")]
    [Tooltip("The absolute maximum volume the hum will reach when the visual knob is at 100%.")]
    [Range(0f, 1f)]
    public float maxVolumeCap = 0.5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _myAudio = GetComponent<AudioSource>();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SetHumVolume(float knobPercentage)
    {
        if (_myAudio != null)
        {
            // --- THE FIX ---
            // If the knob is at 1.0 (100%), it multiplies by 0.5 to equal 0.5 volume.
            // If the knob is at 0.5 (50%), it multiplies by 0.5 to equal 0.25 volume.
            _myAudio.volume = knobPercentage * maxVolumeCap;
            
            // We save the raw KNOB percentage, not the limited volume
            PlayerPrefs.SetFloat("AmbientVolume", knobPercentage);
        }
    }

    void Start()
    {
        if (_myAudio != null && PlayerPrefs.HasKey("AmbientVolume"))
        {
            // Load the saved knob position and apply the cap to it
            float savedKnobValue = PlayerPrefs.GetFloat("AmbientVolume");
            _myAudio.volume = savedKnobValue * maxVolumeCap;
        }
    }
}