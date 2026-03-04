using UnityEngine;

public class TargetSignature : MonoBehaviour
{
    [Header("Intelligence Data")]
    public string codename = "UNKNOWN";
    public string classification = "Hostile"; // e.g., "Cruise Missile", "Drone"
    [TextArea] public string description = "A subsonic cruise missile capable of...";

    [Header("Flight Data (For UI)")]
    public float speed = 450f;     // You can change this on each enemy prefab!
    public float altitude = 15000f; // You can change this on each enemy prefab!

    // In the future, this could hold 3D model references for the visualizer
}