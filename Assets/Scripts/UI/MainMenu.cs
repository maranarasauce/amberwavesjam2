using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        StartCoroutine(GameManager.instance.LoadSceneCoroutine(SceneIndex.Arena));
    }
    public void ExitGame()
    {
        Application.Quit();
    }

    public void ToGoMenu()
    {
        StartCoroutine(GameManager.instance.LoadSceneCoroutine(SceneIndex.TitleScreen));
    }
}

