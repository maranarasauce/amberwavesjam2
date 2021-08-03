using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HardMode : MonoBehaviour
{
    public static HardMode instance;
    // Start is called before the first frame update
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneManager_sceneLoaded;
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        Boss boss = FindObjectOfType<Boss>();
        if (boss != null)
        {
            boss.hard = true;
        }
    }
}
