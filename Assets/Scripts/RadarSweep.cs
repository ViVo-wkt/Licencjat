using UnityEngine;

public class RadarSweep : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Degrees per second. Negative is clockwise.")]
    public float rotationSpeed = -180f; 

    void Update()
    {
        float currentSpeed = rotationSpeed; // Default to local setting

        // OVERRIDE with Zoom System if it exists
        if (RadarZoomSystem.Instance != null)
        {
            // We flip the sign because the Zoom System returns positive numbers (e.g. 180),
            // but we want a Clockwise sweep (-180).
            currentSpeed = -RadarZoomSystem.Instance.GetCurrentSweepSpeed();
        }

        // Rotates the object around its Z axis
        transform.Rotate(0, 0, currentSpeed * Time.deltaTime);
    }
}