using UnityEngine;
using TMPro;

public class RadarUIManager : MonoBehaviour
{
    public static RadarUIManager Instance;

    [Header("UI References")]
    public TMP_Text codeNameText;
    public TMP_Text descriptionText;
    public GameObject panelObject;

    void Awake()
    {
        Instance = this;
    }

    public void ShowTargetInfo(TargetSignature data)
    {
        if (data == null) return;

        panelObject.SetActive(true);
        codeNameText.text = data.codename;
        descriptionText.text = $"CLASS: {data.classification}\n\n{data.description}";
    }
    
    public void ClearInfo()
    {
        codeNameText.text = "NO SIGNAL";
        descriptionText.text = "";
    }
}