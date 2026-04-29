using UnityEngine;
using System.Collections.Generic;

public class TargetingBracket : MonoBehaviour
{
    [Header("Controls")]
    public ScrollWheelControl xWheel;
    public ScrollWheelControl yWheel;

    [Header("Radar Settings")]
    public float radarRadius = 5.5f;

    [Header("Links")]
    public WeaponSystem weaponSystem;
    public WeaponSelector weaponSelector;

    private SpriteRenderer _sprite;
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

            // --- THE ZOOM FIX ---
            // If our parent radar scales down, our local position needs to scale up 
            // to stay in the exact same physical spot on the glass screen!
            if (transform.parent != null)
            {
                intendedPos.x /= transform.parent.localScale.x;
                intendedPos.y /= transform.parent.localScale.y;
            }
            // --------------------

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