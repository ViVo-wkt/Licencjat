using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSelector : MonoBehaviour
{
    public enum WeaponType { SemiActive, Active, AutoTarget }

    [Header("Current State")]
    public WeaponType currentWeapon = WeaponType.SemiActive;

    [Header("Button Setup")]
    public SpriteRenderer[] buttonRenderers; 
    public Collider2D[] buttonColliders;     

    [Header("2D Visuals")]
    public Sprite unpressedSprite;
    public Sprite pressedSprite;

    // --- UPDATED SECTION ---
    [Header("3D Visuals (Optional)")]
    [Tooltip("Drag your new 3D button models here in the exact same order as the 2D renderers!")]
    public Renderer[] buttonMeshRenderers; 
    
    [Tooltip("Drag your raw .jpg / .png textures here!")]
    public Texture2D unpressedTexture;
    public Texture2D pressedTexture;
    // -----------------------

    [Header("Indicators")]
    public GameObject arhIndicator;
    public GameObject sarhIndicatorVisual; 

    [Header("Audio")]
    [Tooltip("Leave blank to use the default click, or drag a custom SFX here!")]
    public AudioClip customButtonSound;

    void Start() { UpdateVisuals(); }

    void Update()
    {
        if (Mouse.current == null) return;
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            for (int i = 0; i < buttonColliders.Length; i++)
            {
                if (buttonColliders[i] != null && buttonColliders[i].OverlapPoint(mousePos))
                {
                    SelectWeapon(i);
                    break;
                }
            }
        }
    }

    public void SelectWeapon(int index)
    {
        currentWeapon = (WeaponType)index;
        UpdateVisuals();

        if (AudioManager.Instance != null) AudioManager.Instance.PlayClickSound(customButtonSound);
    }

    void UpdateVisuals()
    {
        for (int i = 0; i < buttonRenderers.Length; i++)
        {
            // 1. Update old 2D Sprites
            if (buttonRenderers[i] != null)
                buttonRenderers[i].sprite = (i == (int)currentWeapon) ? pressedSprite : unpressedSprite;
                
            // 2. Update new 3D Meshes with raw Textures!
            if (buttonMeshRenderers != null && i < buttonMeshRenderers.Length && buttonMeshRenderers[i] != null)
            {
                // We access the mainTexture property of the model's underlying material directly
                Texture2D targetTex = (i == (int)currentWeapon) ? pressedTexture : unpressedTexture;
                buttonMeshRenderers[i].material.mainTexture = targetTex;
            }
        }

        if (arhIndicator != null) arhIndicator.SetActive(currentWeapon == WeaponType.Active);
        if (sarhIndicatorVisual != null) sarhIndicatorVisual.SetActive(currentWeapon == WeaponType.SemiActive);
    }
}