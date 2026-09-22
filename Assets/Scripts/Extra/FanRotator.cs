using UnityEngine;

public class Rotator : MonoBehaviour
{
    public enum RotationAxis { X, Y, Z }

    [Header("Rotation Settings")]
    [Tooltip("Which axis should the fan rotate around?")]
    public RotationAxis axis = RotationAxis.X;

    [Tooltip("How fast the object rotates in degrees per second. Use negative numbers to spin the other way.")]
    public float rotationSpeed = -200f;

    void Update()
    {
        float rotationStep = rotationSpeed * Time.deltaTime;

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