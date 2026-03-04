using UnityEngine;

public class Rotator : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("How fast the object rotates in degrees per second. Use negative numbers to spin the other way.")]
    public float rotationSpeed = -200f; // Negative usually spins clockwise

    void Update()
    {
        // Rotates the object on the Z axis (perfect for 2D sprites)
        // Multiplying by Time.deltaTime ensures it spins smoothly regardless of framerate
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}