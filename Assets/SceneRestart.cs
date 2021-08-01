using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneRestart : MonoBehaviour
{
    public AudioSource tauntSrc;
    public AudioClip[] tauntClips;
    public void PlayTaunt()
    {
        tauntSrc.clip = tauntClips.GetRandomValue<AudioClip>();
        tauntSrc.Play();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

public static class ArrayExtension
{
    public static T GetRandomValue<T>(this T[] array)
    {
        return array[UnityEngine.Random.Range(0, array.Length - 1)];
    }
}