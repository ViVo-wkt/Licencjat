using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Groups")]
    public GameObject mainMenuGroup; // Holds Start, Options, Quit
    public GameObject optionsGroup;  // Holds Back, Volume Knob
    
    // --- NEW SECTION ---
    public GameObject gamemodeGroup; // Holds your new Slider Switch and final Play button
    // -------------------

    [Header("Settings")]
    public string gameSceneName = "3D"; // Type the EXACT name of your main game scene here

    void Start()
    {
        // Ensure we start cleanly on the main menu
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        if (mainMenuGroup != null) mainMenuGroup.SetActive(true);
        if (optionsGroup != null) optionsGroup.SetActive(false);
        if (gamemodeGroup != null) gamemodeGroup.SetActive(false);
    }

    public void OpenOptions()
    {
        if (mainMenuGroup != null) mainMenuGroup.SetActive(false);
        if (optionsGroup != null) optionsGroup.SetActive(true);
        if (gamemodeGroup != null) gamemodeGroup.SetActive(false);
    }

    // --- NEW METHOD ---
    public void OpenGamemodePanel()
    {
        if (mainMenuGroup != null) mainMenuGroup.SetActive(false);
        if (optionsGroup != null) optionsGroup.SetActive(false);
        if (gamemodeGroup != null) gamemodeGroup.SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}