using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRestart : MonoBehaviour
{
    public Boss boss;
    public void PlayTaunt()
    {
        boss.Speak(boss.taunt);
    }

    public void Restart()
    {
        //SceneManager.LoadScene(SceneManager.GetSceneByName("Arena").buildIndex);
        GameManager.inst.SwapActiveScene(SceneIndex.Arena);
    }
} 