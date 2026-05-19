using System.Collections;
using UnityEngine;

public class BlinkingLight : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How fast the light blinks (in seconds)")]
    public float blinkRate = 0.5f;

    private Renderer _myRenderer;

    void Awake()
    {
        // This automatically grabs either a 2D SpriteRenderer or a 3D MeshRenderer!
        _myRenderer = GetComponent<Renderer>();
    }

    void OnEnable()
    {
        // The moment the BriefingManager turns this object ON, start blinking
        StartCoroutine(BlinkRoutine());
    }

    void OnDisable()
    {
        // Whenever the BriefingManager turns this object OFF, reset it to visible 
        // so it doesn't accidentally start 'invisible' the next time a message pops up!
        if (_myRenderer != null)
        {
            _myRenderer.enabled = true;
        }
    }

    IEnumerator BlinkRoutine()
    {
        // Loop infinitely while the object is active
        while (true)
        {
            // CRITICAL: We MUST use Realtime here, because Time.timeScale is currently 0!
            yield return new WaitForSecondsRealtime(blinkRate);

            if (_myRenderer != null)
            {
                _myRenderer.enabled = !_myRenderer.enabled; // Toggle it on/off
            }
        }
    }
}