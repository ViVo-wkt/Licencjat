using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Base Health")]
    public int maxHealth = 3;
    private int _currentHealth;

    [Header("Health UI (2D Sprites)")]
    public SpriteRenderer[] healthDiodes;
    public Sprite litDiodeSprite;
    public Sprite unlitDiodeSprite;

    [Header("Audio")]
    [Tooltip("Drag multiple damage sound variations here (e.g., WeHit, WeHit2).")]
    public AudioClip[] damageSounds;

    [Tooltip("Voiceline or alarm played when player shoots down a civilian.")]
    public AudioClip[] civilianPenaltySounds;

    [Header("Game Over Screen")]
    public GameObject gameOverPanel;
    public TMP_Text statsText; 

    [Header("Stats Tracking")]
    private int _enemiesDestroyed = 0;
    private float _startTime = 0f;
    private bool _isGameOver = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        _currentHealth = maxHealth;
        _startTime = Time.time;
        
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        
        UpdateHealthUI();
    }

    public void AddKill()
    {
        if (_isGameOver) return;
        _enemiesDestroyed++;
    }

    // Called when an enemy strikes the base
    public void TakeDamage()
    {
        if (_isGameOver) return;

        _currentHealth--;
        
        PlayRandomClip(damageSounds);
        UpdateHealthUI();

        if (_currentHealth <= 0)
        {
            TriggerGameOver();
        }
    }

    // Called when the player shoots down a civilian airliner
    public void PenalizeCivilianCasualty()
    {
        if (_isGameOver) return;

        _currentHealth--;

        PlayRandomClip(civilianPenaltySounds);
        UpdateHealthUI();

        if (_currentHealth <= 0)
        {
            TriggerGameOver();
        }
    }

    private void PlayRandomClip(AudioClip[] clips)
    {
        if (AudioManager.Instance != null && clips != null && clips.Length > 0)
        {
            int randomIndex = Random.Range(0, clips.Length);
            AudioClip selectedClip = clips[randomIndex];

            if (selectedClip != null)
            {
                AudioManager.Instance.PlayClickSound(selectedClip);
            }
        }
    }

    private void UpdateHealthUI()
    {
        if (healthDiodes == null) return;

        for (int i = 0; i < healthDiodes.Length; i++)
        {
            if (healthDiodes[i] != null)
            {
                healthDiodes[i].sprite = (i < _currentHealth) ? litDiodeSprite : unlitDiodeSprite;
            }
        }
    }

    private void TriggerGameOver()
    {
        _isGameOver = true;
        Time.timeScale = 0f; 

        // Play the defeat / casualty sound clip
        PlayRandomClip(civilianPenaltySounds);

        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (statsText != null)
        {
            float timeSurvived = Time.time - _startTime;
            int minutes = Mathf.FloorToInt(timeSurvived / 60F);
            int seconds = Mathf.FloorToInt(timeSurvived - minutes * 60);
            string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);

            statsText.text = $"TIME SURVIVED: {timeString}\n \nHOSTILES DESTROYED: {_enemiesDestroyed}";
        }
    }
}