using UnityEngine;

public class RadarKnob : MonoBehaviour
{
    [Header("Connections")]
    public Transform linkedBeam; // The Radar Beam object

    [Header("Settings")]
    public float rotationSpeed = 30f;
    
    // This function is called automatically by Unity when the mouse 
    // hovers over the CircleCollider2D attached to this object.
    void OnMouseOver()
    {
        // 1. Read Scroll Input
        float scroll = Input.mouseScrollDelta.y;

        // 2. If scrolling happened...
        if (Mathf.Abs(scroll) > 0.01f)
        {
            // Calculate rotation amount
            float rotationAmount = scroll * rotationSpeed * Time.deltaTime * 10f;

            // 3. Rotate the Knob (Visual feedback)
            transform.Rotate(0, 0, rotationAmount);

            // 4. Rotate the Linked Beam (Actual mechanic)
            if (linkedBeam != null)
            {
                // We match the beam's rotation to the knob's rotation
                // (Using localRotation implies the knob and beam start at the same angle '0')
                linkedBeam.rotation = transform.rotation;
            }
        }
    }
}