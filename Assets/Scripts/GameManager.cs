using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Maranara.InputShell;


public enum SceneIndex
{
    Manager = 0,
    TitleScreen = 1,
    Arena = 2,
}


public class GameManager : MonoBehaviour
{
    public bool IsLoading { get => isLoading; }
    public bool IsPaused { get => isPaused; }

    public Resolution MaxResolution { get => Screen.resolutions[Screen.resolutions.Length - 1]; }

    public static GameManager instance;
    public LoadingScreen loadingScreen;
    public GameObject pauseScreen;

    private List<AsyncOperation> sceneLoading = new List<AsyncOperation>();
    private bool isLoading = false;
    private bool isPaused = false;
    private PCInputPlugin inputPlugin;

    private float globalMaxVolume;
    private RenderTexture mainRenderTex;

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

        SetRenderResolution(MaxResolution.width, MaxResolution.height);

        SetGlobalVolume(AudioListener.volume);

        sceneLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndex.TitleScreen, LoadSceneMode.Additive));
        SceneLoadProgress(SceneIndex.TitleScreen);
    }

    private void Update()
    {
        #region pausing
        if (isPaused && Input.GetKeyDown(KeyCode.Return))
        {
            isPaused = false;
            pauseScreen.SetActive(false);

            LoadTitleScreen();
        }

        if(!isLoading && Input.GetKeyDown(KeyCode.Escape) && SceneManager.GetActiveScene().buildIndex == (int)SceneIndex.Arena)
        {
            isPaused = !isPaused;

            pauseScreen.SetActive(isPaused);

            Time.timeScale = isPaused ? 0 : 1;

            if (inputPlugin == null) // can't cache on start since the object is in another scene
                inputPlugin = FindObjectOfType<PCInputPlugin>();

            inputPlugin.enabled = !isPaused;

            AudioListener.volume = IsPaused ? globalMaxVolume * 0.5f : globalMaxVolume;
        }
        #endregion

    }

    #region Scene Loading

    // Some of this feels stupid
    // - parz

    public void SwapActiveScene(SceneIndex sceneToLoad)
    {
        if(isLoading) { return; }

        StartCoroutine(LoadSceneCoroutine(sceneToLoad));
    }

    public void LoadGame()
    {
        if(isLoading) { return; }

        // For some dumbass reason, SceneManager cannot return a scene struct if it's not already loaded.
        SceneIndex[] toLoad   = { SceneIndex.Arena }; 
        Scene[] toUnload      = { SceneManager.GetActiveScene() };

        StartCoroutine(LoadScenesCoroutine(toLoad, toUnload, SceneIndex.Arena));
    }

    public void LoadTitleScreen()
    {
        if (isLoading) { return; }

        // For some dumbass reason, SceneManager cannot return a scene struct if it's not already loaded.
        SceneIndex[] toLoad = { SceneIndex.TitleScreen }; 
        Scene[] toUnload    = { SceneManager.GetActiveScene() };

        Time.timeScale = 0;

        StartCoroutine(LoadScenesCoroutine(toLoad, toUnload, SceneIndex.TitleScreen));
    }


    private IEnumerator LoadSceneCoroutine(SceneIndex scene)
    {
        LoadFade();

        yield return new WaitForSecondsRealtime(loadingScreen.fadeTime);

        sceneLoading.Add(SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene()));
        sceneLoading.Add(SceneManager.LoadSceneAsync((int)scene, LoadSceneMode.Additive));

        SceneLoadProgress(scene);
    }

    private IEnumerator LoadScenesCoroutine(SceneIndex[] toLoad, Scene[] toUnload, SceneIndex nextActiveScene)
    {
        LoadFade();

        yield return new WaitForSecondsRealtime(loadingScreen.fadeTime);

        foreach(var scene in toUnload)
        {
            sceneLoading.Add(SceneManager.UnloadSceneAsync(scene));
        }

        foreach (var scene in toLoad)
        {
            sceneLoading.Add(SceneManager.LoadSceneAsync((int)scene, LoadSceneMode.Additive));
        }

        SceneLoadProgress(nextActiveScene);
    }


    private IEnumerator SceneLoadProgress(Scene nextActiveScene)
    {
        if (loadingScreen.isActiveAndEnabled)
            loadingScreen.ShowLoadingAnim(true);

        for (int i = 0; i < sceneLoading.Count; i++)
        {
            while (!sceneLoading[i].isDone)
            {
                yield return null;
            }
        }

        if (loadingScreen.isActiveAndEnabled)
        {
            loadingScreen.ShowLoadingAnim(false);
            loadingScreen.FadeOut(); // fade out to next scene after loading is complete
        }

        Time.timeScale = 1;

        isLoading = false;
        SceneManager.SetActiveScene(nextActiveScene);

        sceneLoading.Clear();
    }

    private void SceneLoadProgress(SceneIndex nextActiveScene)
    {
        Scene s = SceneManager.GetSceneByBuildIndex((int)nextActiveScene);

        StartCoroutine(SceneLoadProgress(s));
    }
    private void LoadFade()
    {
        isLoading = true;
        loadingScreen.gameObject.SetActive(true);
        loadingScreen.FadeIn();
    }
    #endregion


    public void SetGlobalVolume(float volume)
    {
        AudioListener.volume = volume;
        globalMaxVolume = volume;
    }


    public void ScaleRenderResolution(float scalar)
    {
        var w = (int)(MaxResolution.width  * scalar);
        var h = (int)(MaxResolution.height * scalar);

        SetRenderResolution(w, h);
    }

    public void SetRenderResolution(int width, int height)
    {
        if(mainRenderTex != null)
            mainRenderTex.Release();
        mainRenderTex = new RenderTexture(width, height, 24);
        Graphics.SetRenderTarget(mainRenderTex);

        Debug.Log(mainRenderTex.width + "|" + mainRenderTex.height);
    }
}
