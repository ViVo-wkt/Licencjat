using UnityEngine;

public class RadarSweep : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Degrees per second. Negative is clockwise.")]
    public float rotationSpeed = -180f; 

    void Update()
    {
        // Rotates the object around its Z axis
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}