using UnityEngine;

public class TargetSignature : MonoBehaviour
{
    [Header("Intelligence Data")]
    public string codename = "UNKNOWN";
    public string classification = "Hostile"; // e.g., "Cruise Missile", "Drone"
    [TextArea] public string description = "A subsonic cruise missile capable of...";
    
    // In the future, this could hold 3D model references for the visualizer
}