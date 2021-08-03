using System;
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
    // slowly becoming messy...
    // - parz

    public bool IsLoading { get => isLoading; }
    public bool IsPaused { get => isPaused; }
    public Resolution MaxResolution { get => Screen.resolutions[Screen.resolutions.Length - 1]; }
    public SceneIndex LastActiveScene { get => lastActiveScene; }
    public SceneIndex NextActiveScene { get => nextActiveScene; }
    public PlayerHealth Player { get => GetPlayer(); }


    public static GameManager inst;

    public event Action OnLoadStart;
    public event Action OnLoad;
    public event Action OnLoadEnd;
    
    [SerializeField] private LoadingScreen loadingScreen;
    [SerializeField] private GameObject pauseScreen;

    private List<AsyncOperation> sceneLoading = new List<AsyncOperation>();
    private bool isLoading = false;
    private bool isPaused = false;

    private PCInputPlugin inputPlugin;
    private PlayerHealth player;

    private float globalMaxVolume;
    private RenderTexture mainRenderTex;

    private SceneIndex lastActiveScene;
    private SceneIndex nextActiveScene = SceneIndex.TitleScreen;

    private void Awake()
    {
        #region Singleton Crap
        if (inst == null)
        {
            inst = this;
        }
        else
        {
            Debug.LogWarning("GameManager already exists!");
            Destroy(this);
        }
        #endregion

        SetGlobalVolume(AudioListener.volume);
        SetRenderResolution(MaxResolution.width, MaxResolution.height);

        // Load into TitleScreen
        sceneLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndex.TitleScreen, LoadSceneMode.Additive));
        SceneIndex[] toLoad = { SceneIndex.TitleScreen };
        StartCoroutine(WaitForLoadCoroutine());
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

    #region load initiators
    /// <summary>
    /// Unloads the ActiveScene and Loads <paramref name="sceneToLoad"/>
    /// </summary>
    /// <param name="sceneToLoad"></param>
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
        
        SceneIndex[] toLoad = { SceneIndex.TitleScreen }; 
        Scene[] toUnload    = { SceneManager.GetActiveScene() };

        Time.timeScale = 0;

        StartCoroutine(LoadScenesCoroutine(toLoad, toUnload, SceneIndex.TitleScreen));
    }
    #endregion

    private IEnumerator LoadSceneCoroutine(SceneIndex scene)
    {
        LoadStart(scene);

        yield return new WaitForSecondsRealtime(loadingScreen.fadeTime);

        sceneLoading.Add(SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene()));
        sceneLoading.Add(SceneManager.LoadSceneAsync((int)scene, LoadSceneMode.Additive));

        StartCoroutine(WaitForLoadCoroutine());
    }


    // here for redundancy
    private IEnumerator LoadScenesCoroutine(SceneIndex[] toLoad, Scene[] toUnload, SceneIndex _nextActiveScene)
    {
        LoadStart(_nextActiveScene);

        yield return new WaitForSecondsRealtime(loadingScreen.fadeTime);

        foreach(var scene in toUnload)
        {
            sceneLoading.Add(SceneManager.UnloadSceneAsync(scene));
        }

        foreach (var scene in toLoad)
        {
            sceneLoading.Add(SceneManager.LoadSceneAsync((int)scene, LoadSceneMode.Additive));
        }

        StartCoroutine(WaitForLoadCoroutine());
    }


    private IEnumerator WaitForLoadCoroutine()
    {
        OnLoad?.Invoke();

        for (int i = 0; i < sceneLoading.Count; i++)
        {
            while (!sceneLoading[i].isDone)
            {
                yield return null;
            }
        }

        isLoading = false;
        OnLoadEnd?.Invoke();

        Time.timeScale = 1;

        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex((int)nextActiveScene));

        sceneLoading.Clear();
    }
    
    private void LoadStart(SceneIndex nextActive)
    {
        isLoading = true;
        
        lastActiveScene = (SceneIndex)SceneManager.GetActiveScene().buildIndex;
        nextActiveScene = nextActive;

        OnLoadStart?.Invoke();
    }
    #endregion

    private PlayerHealth GetPlayer()
    {
        if(SceneManager.GetActiveScene().buildIndex == (int)SceneIndex.Arena)
            player = FindObjectOfType<PlayerHealth>();

        return player ?? null;
    }

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
