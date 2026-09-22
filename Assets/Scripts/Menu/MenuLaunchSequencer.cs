using UnityEngine;

public class MenuLaunchSequencer : MonoBehaviour
{
    [Header("Hardware")]
    public SliderSwitch modeSwitch;
    
    [Tooltip("Drag your final Play button here!")]
    public MenuButton finalPlayButton; 

    [Header("3D Light Renderers")]
    [Tooltip("The 3D mesh of the light that should glow when ARMED (Right).")]
    public Renderer armedLightRenderer;
    
    [Tooltip("The 3D mesh of the light that should glow when LOCKED (Left).")]
    public Renderer lockedLightRenderer;

    [Header("Materials")]
    [Tooltip("The glowing material to apply when a light is ON.")]
    public Material activeMaterial;
    
    [Tooltip("The dark/glass material to apply when a light is OFF.")]
    public Material inactiveMaterial;

    void Update()
    {
        if (modeSwitch == null || finalPlayButton == null) return;

        bool isArmed = modeSwitch.isOnRightSide;

        finalPlayButton.enabled = isArmed;

        
        if (armedLightRenderer != null)
        {
            armedLightRenderer.material = isArmed ? activeMaterial : inactiveMaterial;
        }

        if (lockedLightRenderer != null)
        {
            lockedLightRenderer.material = isArmed ? inactiveMaterial : activeMaterial;
        }
    }
}