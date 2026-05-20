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

        // Check if the player has pulled the switch to the right
        bool isArmed = modeSwitch.isOnRightSide;

        // Turn the button's ability to be clicked ON or OFF
        finalPlayButton.enabled = isArmed;

        // --- 3D MATERIAL SWAPPING ---
        // Setting '.material' automatically targets Element 0 of the Mesh Renderer!
        
        if (armedLightRenderer != null)
        {
            // If armed is true, use active material. Otherwise, use inactive.
            armedLightRenderer.material = isArmed ? activeMaterial : inactiveMaterial;
        }

        if (lockedLightRenderer != null)
        {
            // If armed is true, the locked light should be OFF. Otherwise, it should be ON.
            lockedLightRenderer.material = isArmed ? inactiveMaterial : activeMaterial;
        }
    }
}