using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntermissionScreen : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("How many seconds should the image stay on screen?")]
    public float displayTime = 3.0f;
    
    [Tooltip("The exact name of your main game scene (e.g., '3D' or '2D')")]
    public string nextSceneName = "3D"; 

    void Start()
    {
        // Start the countdown the moment the scene loads
        StartCoroutine(WaitAndLoad());
    }

    IEnumerator WaitAndLoad()
    {
        // Wait for real-world seconds
        yield return new WaitForSeconds(displayTime);
        
        // Load the actual game!
        SceneManager.LoadScene(nextSceneName);
    }
}