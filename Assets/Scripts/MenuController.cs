using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    SceneTransition transition;

    private void Start()
    {
        if (MusicManager.instance.CurrentIndex() != 0)
            MusicManager.instance.FadeTracksIn(0, int.MaxValue, 5f);

        transition = GetComponent<SceneTransition>();
    }

    public void PlayGame()
    {
        transition.beginTransition("SampleScene");
    }

    public void OpenSettings()
    {
        transition.beginTransition("Options");
    }

    public void OpenCredits()
    {
        transition.beginTransition("Credits");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OpenMainMenu()
    {
        transition.beginTransition("MainMenu");
    }
}