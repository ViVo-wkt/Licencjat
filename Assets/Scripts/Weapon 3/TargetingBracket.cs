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
        if (xWheel != null && yWheel != null)
        {
            // 1. Calculate intended position based on the wheels
            float xPos = xWheel.currentValue * radarRadius;
            float yPos = yWheel.currentValue * radarRadius;

            Vector2 intendedPos = new Vector2(xPos, yPos);

            // 2. TRUE CIRCULAR CLAMPING
            // If the bracket tries to leave the circle, we push it back to the edge.
            if (intendedPos.magnitude > radarRadius)
            {
                intendedPos = intendedPos.normalized * radarRadius;

                // IMPORTANT: We must tell the wheels to "roll back" to match the 
                // clamped position, otherwise they will keep spinning freely but 
                // the bracket will be stuck against the wall!
                xWheel.ForceValue(intendedPos.x / radarRadius);
                yWheel.ForceValue(intendedPos.y / radarRadius);
            }

            // 3. FIX Z-DEPTH
            // We force Z to -0.1 so it always draws on top of the radar screen
            transform.localPosition = new Vector3(intendedPos.x, intendedPos.y, -0.1f);
        }

        // Visuals
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
            weaponSystem.FireAutoMissile(target.gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other) { ProcessTarget(other); }
    void OnTriggerStay2D(Collider2D other) { ProcessTarget(other); }
}