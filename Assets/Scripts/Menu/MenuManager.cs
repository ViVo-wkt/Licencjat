using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Groups")]
    public GameObject mainMenuGroup; // Holds Start, Options, Quit
    public GameObject optionsGroup;  // Holds Back, Volume Knob

    [Header("Settings")]
    public string gameSceneName = "3D"; // Type the EXACT name of your main game scene here

    void Start()
    {
        // Ensure we start on the main menu
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        mainMenuGroup.SetActive(true);
        optionsGroup.SetActive(false);
    }

    public void OpenOptions()
    {
        mainMenuGroup.SetActive(false);
        optionsGroup.SetActive(true);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        Application.Quit();
        
        // This line makes the quit button work while testing inside the Unity Editor!
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}