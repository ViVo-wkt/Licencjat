using UnityEngine;
using UnityEngine.InputSystem; // Required for the New Input System

public class RadarKnob : MonoBehaviour
{
    [Header("Connections")]
    public Transform linkedBeam; // The Radar Beam object

    [Header("Settings")]
    public float rotationSpeed = 10f; // Adjusted for New Input sensitivity

    private Collider2D _myCollider;

    void Awake()
    {
        _myCollider = GetComponent<Collider2D>();
    }

    void Update()
    {
        // 1. Safety Check: Do we have a mouse?
        if (Mouse.current == null) return;

        // 2. Get Mouse Position in the World
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        // 3. Manual "OnMouseOver" Check
        // We ask the collider: "Is this point inside you?"
        if (_myCollider.OverlapPoint(mouseWorldPos))
        {
            // 4. Read Scroll Input
            // (The new system often returns larger values like 120, so we normalize it)
            float scrollValue = Mouse.current.scroll.ReadValue().y;

            if (Mathf.Abs(scrollValue) > 0.01f)
            {
                // Normalize to 1 or -1 to keep speed consistent
                float direction = Mathf.Sign(scrollValue);

                // Calculate rotation
                float rotationAmount = direction * rotationSpeed * Time.deltaTime * 50f;

                // Rotate Knob
                transform.Rotate(0, 0, rotationAmount);

                // Rotate Beam
                if (linkedBeam != null)
                {
                    linkedBeam.rotation = transform.rotation;
                }
            }
        }
    }
}