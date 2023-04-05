using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] LeanTweenHelper transitionTween;
    UnityEngine.UI.Image transitionImage;
    public UnityEngine.UI.Image GetImage() => transitionImage;
    string sceneToLoad = "";

    private void OnEnable()
    {
        LeanTweenHelper.onTweenComplete += ProcessTweenFinish;
    }

    private void OnDisable()
    {
        LeanTweenHelper.onTweenComplete -= ProcessTweenFinish;
    }

    void ProcessTweenFinish(LeanTweenHelper tween, int index)
    {
        if (tween != transitionTween) return;

        if(index == 0)
        {
            transitionImage.maskable = false;
            transitionImage.raycastTarget = false;
        }
        else if (index == 1 && sceneToLoad != null && sceneToLoad != "")
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    void Start()
    {
        transitionImage = transitionTween.GetComponent<UnityEngine.UI.Image>();
        transitionTween.BeginTween(0);
        transitionImage.maskable = true;
        transitionImage.raycastTarget = true;
    }

    public void beginTransition(string scene)
    {
        sceneToLoad = scene;
        transitionTween.BeginTween(1);
    }
}