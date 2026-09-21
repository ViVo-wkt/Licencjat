using UnityEngine;

public class RadarWarningRing : MonoBehaviour
{
    public static RadarWarningRing Instance;

    [Header("Hardware References")]
    [Tooltip("Assign 12 diode MeshRenderers starting from 0° (top) clockwise: 0°, 30°, 60°, ..., 330°.")]
    public MeshRenderer[] diodeRenderers = new MeshRenderer[12];

    [Header("Materials (Element 0)")]
    public Material unlitMaterial;
    public Material litMaterial;

    [Header("Timing")]
    public float flashDuration = 1.0f;

    private float[] _flashTimers = new float[12];

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        SetAllDiodes(unlitMaterial);
    }

    void Update()
    {
        for (int i = 0; i < _flashTimers.Length; i++)
        {
            if (_flashTimers[i] > 0f)
            {
                _flashTimers[i] -= Time.unscaledDeltaTime;
                if (_flashTimers[i] <= 0f)
                {
                    SetDiodeMaterial(i, unlitMaterial);
                }
            }
        }
    }

    /// <summary>
    /// Flashes the diode closest to the provided world position relative to center (0,0).
    /// </summary>
    public void NotifyThreatSpawn(Vector2 spawnPosition)
    {
        // Bearing in degrees: 0° is North (+Y), 90° is East (+X)
        float bearing = Mathf.Atan2(spawnPosition.x, spawnPosition.y) * Mathf.Rad2Deg;
        if (bearing < 0f) bearing += 360f;

        // Since Diode 0 centers at 15° (covering 0° to 30°),
        // Diode 1 centers at 45° (covering 30° to 60°), etc.,
        // dividing directly by 30 gives the exact diode index:
        int sectorIndex = Mathf.FloorToInt(bearing / 30f) % 12;

        FlashDiode(sectorIndex);
    }

    public void FlashDiode(int index)
    {
        if (index < 0 || index >= diodeRenderers.Length) return;
        if (diodeRenderers[index] == null) return;

        SetDiodeMaterial(index, litMaterial);
        _flashTimers[index] = flashDuration;
    }

    private void SetDiodeMaterial(int index, Material mat)
    {
        if (diodeRenderers[index] == null || mat == null) return;

        Material[] mats = diodeRenderers[index].materials;
        if (mats.Length > 0)
        {
            mats[0] = mat; // Element 0 swap
            diodeRenderers[index].materials = mats;
        }
    }

    private void SetAllDiodes(Material mat)
    {
        for (int i = 0; i < diodeRenderers.Length; i++)
        {
            SetDiodeMaterial(i, mat);
        }
    }
}