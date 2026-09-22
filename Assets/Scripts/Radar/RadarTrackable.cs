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
        float ratio = oldScale / newScale;

        transform.position = transform.position * ratio;
    }
}