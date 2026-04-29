using UnityEngine;

public class Rotator : MonoBehaviour
{
    public enum RotationAxis { X, Y, Z }

    [Header("Rotation Settings")]
    [Tooltip("Which axis should the fan rotate around?")]
    public RotationAxis axis = RotationAxis.X; // Defaults to X for your new 3D model!

    [Tooltip("How fast the object rotates in degrees per second. Use negative numbers to spin the other way.")]
    public float rotationSpeed = -200f;

    void Update()
    {
        // Calculate the rotation step for this frame
        float rotationStep = rotationSpeed * Time.deltaTime;

        // Apply the rotation to the selected axis
        if (axis == RotationAxis.X)
        {
            transform.Rotate(rotationStep, 0, 0);
        }
        else if (axis == RotationAxis.Y)
        {
            transform.Rotate(0, rotationStep, 0);
        }
        else if (axis == RotationAxis.Z)
        {
            transform.Rotate(0, 0, rotationStep);
        }
    }
}