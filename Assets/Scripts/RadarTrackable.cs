using UnityEngine;

public class RadarTrackable : MonoBehaviour
{
    void OnEnable()
    {
        RadarZoomSystem.OnZoomChanged += HandleZoomChange;
    }

    void OnDisable()
    {
        RadarZoomSystem.OnZoomChanged -= HandleZoomChange;
    }

    void HandleZoomChange(float oldScale, float newScale)
    {
        // Calculate the ratio
        float ratio = oldScale / newScale;

        // Apply to position
        // Example: Zooming OUT (1 -> 2). Ratio is 0.5. Object moves closer to center.
        transform.position = transform.position * ratio;
    }
}