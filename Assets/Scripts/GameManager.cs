﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public enum SceneIndex
{
    Manager = 0,
    TitleScreen = 1,
    Arena = 2,
    Win = 3
}


public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public LoadingScreen loadingScreen;

    private List<AsyncOperation> sceneLoading = new List<AsyncOperation>();

    private void Awake()
    {
        #region Singleton Crap
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("GameManager already exists!");
            Destroy(this);
        }
        #endregion

        sceneLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndex.TitleScreen, LoadSceneMode.Additive));
        SceneLoadProgress(SceneIndex.TitleScreen);
    }

    bool tempPress = false;
    private void Update()
    {
        //@Refactor: Move somewhere else and clean it up
        if (Input.GetKey(KeyCode.RightAlt) && Input.GetKeyDown(KeyCode.Escape) 
            && !tempPress
            && SceneManager.GetActiveScene().buildIndex != (int)SceneIndex.TitleScreen)
        {
            tempPress = true;
            StartCoroutine(LoadSceneCoroutine(SceneIndex.TitleScreen));
        }

        if (!tempPress && SceneManager.GetActiveScene().buildIndex == (int)SceneIndex.TitleScreen)
            tempPress = false;
    }

    #region Scene Loading logic

    // Some of this feels stupid
    // - parz
    // I tried simplifying it - mara

    public void SwapActiveScene(SceneIndex scene)
    {
        StartCoroutine(LoadSceneCoroutine(scene));
    }
    public IEnumerator LoadSceneCoroutine(SceneIndex scene)
    {
        LoadFade();

        yield return new WaitForSeconds(loadingScreen.fadeTime);

        sceneLoading.Add(SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene()));

        sceneLoading.Add(SceneManager.LoadSceneAsync((int)scene, LoadSceneMode.Additive));
        
        SceneLoadProgress(scene);
    }
    

    private IEnumerator SceneLoadProgress(Scene nextActiveScene)
    {
        if (loadingScreen.isActiveAndEnabled)
            loadingScreen.ShowLoadingAnim(true);

        for (int i = 0; i < sceneLoading.Count; i++)
        {
            while (!sceneLoading[i].isDone)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        if (loadingScreen.isActiveAndEnabled)
        {
            loadingScreen.ShowLoadingAnim(false);
            loadingScreen.FadeOut(); // fade out to next scene after loading is complete
        }

        SceneManager.SetActiveScene(nextActiveScene);
     
        sceneLoading.Clear();
    }
    #endregion

    #region Helper Methods
    private void SceneLoadProgress(SceneIndex nextActiveScene)
    {
        Scene s = SceneManager.GetSceneByBuildIndex((int)nextActiveScene);

        StartCoroutine(SceneLoadProgress(s));
    }
    private void LoadFade()
    {
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.FadeIn();
    }
    #endregion
}
