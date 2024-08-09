
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Function to load the first scene (Scene 1)
    public void PlayGame()
    {
        SceneManager.LoadScene(1); // Load Scene 1
    }

    // Function to exit the game
    public void ExitGame()
    {
        Debug.Log("Exit game called"); // This will show in the editor's console
        Application.Quit(); // Quit the application
    }
}