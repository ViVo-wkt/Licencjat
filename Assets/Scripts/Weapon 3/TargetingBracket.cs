using UnityEngine;
using System.Collections.Generic;

public class TargetingBracket : MonoBehaviour
{
    [Header("Controls")]
    public ScrollWheelControl xWheel;
    public ScrollWheelControl yWheel;

    [Header("Radar Settings")]
    public float radarRadius = 5.5f; // INCREASED DEFAULT FOR WIDER RANGE

    [Header("Links")]
    public WeaponSystem weaponSystem;
    public WeaponSelector weaponSelector;

    private SpriteRenderer _sprite;

    // NEW: Tracks targets we already shot at so we don't spam missiles!
    private HashSet<GameObject> _engagedTargets = new HashSet<GameObject>();

    void Start() { _sprite = GetComponent<SpriteRenderer>(); }

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

        if (weaponSelector != null && _sprite != null)
        {
            _sprite.enabled = ((int)weaponSelector.currentWeapon == 2);
        }
    }

    void ProcessTarget(Collider2D other)
    {
        if (weaponSelector == null || (int)weaponSelector.currentWeapon != 2) return;

        TargetSignature target = other.GetComponent<TargetSignature>();
        if (target == null) target = other.GetComponentInParent<TargetSignature>();

        if (target != null && weaponSystem != null)
        {
            // If we haven't engaged this specific target yet...
            if (!_engagedTargets.Contains(target.gameObject))
            {
                bool successfullyFired = weaponSystem.FireAutoMissile(target.gameObject);
                if (successfullyFired)
                {
                    _engagedTargets.Add(target.gameObject); // Mark as engaged!
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other) { ProcessTarget(other); }
    void OnTriggerStay2D(Collider2D other) { ProcessTarget(other); }

    // NEW: When the target leaves the bracket, forget about it!
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