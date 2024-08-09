using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
* Author: Rayn Bin Kamaludin
* Date: 9/8/2024
* Description: Pause menu handler
*/

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;

    private PlayerControls controls;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.Pause.performed += ctx => OnPause();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    void Start()
    {
        // Start with the pause menu deactivated
        pauseMenu.SetActive(false);
    }

    void OnPause()
    {
        if (pauseMenu.activeSelf)
        {
            // If the pause menu is active, deactivate it and resume the game
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            // If the pause menu is not active, activate it and pause the game
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(1);
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); // Assumes the main menu is the first scene
    }

    public void QuitGame()
    {
        Debug.Log("Exiting game...");
        Application.Quit();
    }
}