using UnityEngine;
using System.Collections.Generic;

public class TargetingBracket : MonoBehaviour
{
    [Header("Controls")]
    public ScrollWheelControl xWheel;
    public ScrollWheelControl yWheel;

    [Header("Radar Settings")]
    public float radarRadius = 5.5f;

    [Header("Visuals")]
    [ColorUsage(true, true)]
    public Color glowColor = new Color(0f, 1f, 0f, 1f);

    [Header("Links")]
    public WeaponSystem weaponSystem;
    public WeaponSelector weaponSelector;

    // CHANGED: Using base Renderer class so it supports BOTH 2D Sprites and 3D Meshes!
    private Renderer _myRenderer;
    private HashSet<GameObject> _engagedTargets = new HashSet<GameObject>();

    void Awake()
    {
        _myRenderer = GetComponent<Renderer>();
        
        // Only apply the color tint if it is still a 2D Sprite!
        if (_myRenderer is SpriteRenderer sr) sr.color = glowColor;
    }

    void OnValidate()
    {
        if (_myRenderer == null) _myRenderer = GetComponent<Renderer>();
        if (_myRenderer is SpriteRenderer sr) sr.color = glowColor;
    }

    void OnEnable()
    {
        RadarZoomSystem.OnZoomChanged += HandleZoomChange;
    }

    void OnDisable()
    {
        RadarZoomSystem.OnZoomChanged -= HandleZoomChange;
    }

    void HandleZoomChange(float oldScale, float newScale)
    {
        float ratio = oldScale / newScale;
        transform.localScale = transform.localScale * ratio;
    }

    void Update()
    {
        if (xWheel != null && yWheel != null)
        {
            float xPos = xWheel.currentValue * radarRadius;
            float yPos = yWheel.currentValue * radarRadius;

            Vector2 intendedPos = new Vector2(xPos, yPos);

            if (intendedPos.magnitude > radarRadius)
            {
                intendedPos = intendedPos.normalized * radarRadius;
                xWheel.ForceValue(intendedPos.x / radarRadius);
                yWheel.ForceValue(intendedPos.y / radarRadius);
            }

            transform.localPosition = new Vector3(intendedPos.x, intendedPos.y, -0.1f);
        }

        if (weaponSelector != null && _myRenderer != null)
        {
            // CHANGED: Safely compares the exact state instead of using a hardcoded '2'
            _myRenderer.enabled = (weaponSelector.currentWeapon == WeaponSelector.WeaponType.AutoTarget); 
        }
    }

    void ProcessTarget(Collider2D other)
    {
        // Safely checks enum here as well!
        if (weaponSelector == null || weaponSelector.currentWeapon != WeaponSelector.WeaponType.AutoTarget) return;

        TargetSignature target = other.GetComponent<TargetSignature>();
        if (target == null) target = other.GetComponentInParent<TargetSignature>();

        if (target != null && weaponSystem != null)
        {
            if (!_engagedTargets.Contains(target.gameObject))
            {
                bool successfullyFired = weaponSystem.FireAutoMissile(target.gameObject);
                if (successfullyFired)
                {
                    _engagedTargets.Add(target.gameObject); 
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) { ProcessTarget(other); }
    void OnTriggerStay2D(Collider2D other)  { ProcessTarget(other); }

    void OnTriggerExit2D(Collider2D other)
    {
        TargetSignature target = other.GetComponent<TargetSignature>();
        if (target == null) target = other.GetComponentInParent<TargetSignature>();

        if (target != null)
        {
            _engagedTargets.Remove(target.gameObject);
        }
    }
}