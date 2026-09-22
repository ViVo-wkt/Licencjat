using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Menu Groups")]
    public GameObject mainMenuGroup;
    public GameObject optionsGroup;
    
    public GameObject gamemodeGroup;

    [Header("Settings")]
    public string gameSceneName = "3D";

    void Start()
    {
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