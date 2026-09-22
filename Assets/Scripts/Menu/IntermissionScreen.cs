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
        StartCoroutine(WaitAndLoad());
    }

    IEnumerator WaitAndLoad()
    {
        yield return new WaitForSeconds(displayTime);
        
        SceneManager.LoadScene(nextSceneName);
    }
}