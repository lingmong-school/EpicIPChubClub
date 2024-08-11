/*
* Author: Rayn Bin Kamaludin
* Date: 7/8/2024
* Description: Singleton GameManager that persists across scenes and controls game flow.
*/

using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this; // Set this as the instance
            DontDestroyOnLoad(gameObject); // Make this object persist across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Destroy the GameManager if the current scene is 0 , 3, or 4
        if (scene.buildIndex == 0 || scene.buildIndex == 3 || scene.buildIndex == 4)
        {
            Destroy(gameObject);
        }
    }
}