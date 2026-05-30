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
    public AudioClip[] damageSounds; // <-- CHANGED: Turned into an array!

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

    public void TakeDamage()
    {
        if (_isGameOver) return;

        _currentHealth--;
        
        // --- NEW: RANDOM AUDIO SELECTION ---
        if (AudioManager.Instance != null && damageSounds != null && damageSounds.Length > 0)
        {
            // Pick a completely random index from the array
            int randomIndex = Random.Range(0, damageSounds.Length);
            AudioClip selectedClip = damageSounds[randomIndex];

            if (selectedClip != null)
            {
                AudioManager.Instance.PlayClickSound(selectedClip);
            }
        }
        // ------------------------------------

        UpdateHealthUI();

        if (_currentHealth <= 0)
        {
            TriggerGameOver();
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

        if (gameOverPanel != null) gameOverPanel.SetActive(true);

        if (statsText != null)
        {
            float timeSurvived = Time.time - _startTime;
            int minutes = Mathf.FloorToInt(timeSurvived / 60F);
            int seconds = Mathf.FloorToInt(timeSurvived - minutes * 60);
            string timeString = string.Format("{0:00}:{1:00}", minutes, seconds);

            statsText.text = $"TIME SURVIVED: {timeString}\nHOSTILES DESTROYED: {_enemiesDestroyed}";
        }
    }

}