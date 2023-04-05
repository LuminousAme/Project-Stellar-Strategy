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
    [SerializeField] LeanTweenHelper tween;
    bool inputActive = true;
    bool inputwasActive = true;
    float timeScale = 0.0f;
    bool loadingOptions = false;
    float elapsed = 0.0f;

    private void Start()
    {
        paused = false;
        pauseMenuObject.SetActive(false);
        timeScale = Time.timeScale;
        inputActive = true;
        inputwasActive = true;
        playerInput.actions.FindActionMap("Movement").Enable();
        loadingOptions = false;
        elapsed = 0.0f;
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
        if(loadingOptions)
        {
            elapsed += Time.unscaledDeltaTime;
            if(elapsed > 1.5f)
            {
                SceneManager.LoadSceneAsync("Options", LoadSceneMode.Additive);
                loadingOptions = false;
            }
        }

        if(inputActive != inputwasActive)
        {
            inputwasActive = inputActive;
            if (inputActive)
            {
                tween.BeginTween(0);
                playerInput.actions.FindActionMap("Movement").Enable();
            }
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
        tween.BeginTween(2);
        loadingOptions = true;
        elapsed = 0.0f;
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
