using UnityEngine;

public class TargetingBracket : MonoBehaviour
{
    [Header("Controls")]
    public ScrollWheelControl xWheel;
    public ScrollWheelControl yWheel;

    [Header("Radar Settings")]
    [Tooltip("How far the bracket can move from the center (Matches radar radius)")]
    public float radarRadius = 5.0f;

    [Header("Links")]
    public WeaponSystem weaponSystem;
    public WeaponSelector weaponSelector;

    private SpriteRenderer _sprite;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 1. Move the bracket based on 3D wheel values
        if (xWheel != null && yWheel != null)
        {
            float xPos = xWheel.currentValue * radarRadius;
            float yPos = yWheel.currentValue * radarRadius;

            // Clamp so it stays inside the circular radar bounds
            Vector2 newPos = new Vector2(xPos, yPos);
            if (newPos.magnitude > radarRadius)
            {
                newPos = newPos.normalized * radarRadius;
            }

            transform.localPosition = newPos;
        }

        // 2. Visuals: Only show the bracket if Auto Mode (index 2) is selected
        if (weaponSelector != null && _sprite != null)
        {
            _sprite.enabled = ((int)weaponSelector.currentWeapon == 2);
        }
    }

    // Handles both entering and staying. This way, if you select Auto Mode 
    // while a target is ALREADY inside the bracket, it still fires!
    void ProcessTarget(Collider2D other)
    {
        if (weaponSelector == null || (int)weaponSelector.currentWeapon != 2) return;

        TargetSignature target = other.GetComponent<TargetSignature>();
        if (target == null) target = other.GetComponentInParent<TargetSignature>();

        if (target != null && weaponSystem != null)
        {
            weaponSystem.FireAutoMissile(target.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other) { ProcessTarget(other); }
    void OnTriggerStay2D(Collider2D other) { ProcessTarget(other); }
}