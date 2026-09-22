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
        _myRenderer = GetComponent<Renderer>();
    }

    void OnEnable()
    {
        StartCoroutine(BlinkRoutine());
    }

    void OnDisable()
    { 
        if (_myRenderer != null)
        {
            _myRenderer.enabled = true;
        }
    }

    IEnumerator BlinkRoutine()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(blinkRate);

            if (_myRenderer != null)
            {
                _myRenderer.enabled = !_myRenderer.enabled;
            }
        }
    }
}