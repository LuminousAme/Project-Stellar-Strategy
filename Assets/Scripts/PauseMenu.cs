using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    bool paused = false;
    [SerializeField] GameObject pauseMenuObject;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] SceneTransition transition;
    [SerializeField] CamController camController;
    bool inputActive = true;
    bool inputwasActive = true;
    float timeScale = 0.0f;

    private void Start()
    {
        paused = false;
        pauseMenuObject.SetActive(false);
        timeScale = Time.timeScale;
        inputActive = true;
        inputwasActive = true;
        playerInput.actions.FindActionMap("Movement").Enable();
    }

    public void TooglePaused()
    {
        paused = !paused;
        pauseMenuObject.SetActive(!pauseMenuObject.activeSelf);

        if (paused)
        {
            Time.timeScale = 0.0f;
            camController.enabled = false;
        }
        else
        {
            Time.timeScale = timeScale;
            camController.enabled = true;
        }
    }

    private void Update()
    {
        if(inputActive != inputwasActive)
        {
            inputwasActive = inputActive;
            if(inputActive) playerInput.actions.FindActionMap("Movement").Enable();
            else playerInput.actions.FindActionMap("Movement").Disable();
        }

        for(int i = 0; i < SceneManager.sceneCount; i++)
        {
            if(SceneManager.GetSceneAt(i).name == "Options")
            {
                inputActive = false;
                return;
            }
        }
        inputActive = true;
    }

    public void OpenOptions()
    {
        SceneManager.LoadSceneAsync("Options", LoadSceneMode.Additive);
    }
    
    public void ReturnToMenu()
    {
        transition.beginTransition("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
