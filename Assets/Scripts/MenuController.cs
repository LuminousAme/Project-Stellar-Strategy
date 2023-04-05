using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [SerializeField]



    public void PlayGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void OpenSettings()
    {
        SceneManager.LoadScene("Options");

    }

    public void OpenCredits()
    {
        SceneManager.LoadScene("Credits");

    }

    public void QuitGame()
    {
        Application.Quit();
    }
}