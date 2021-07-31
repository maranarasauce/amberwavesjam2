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
        tauntSrc.clip = tauntClips[tauntClips.Length - 1];
        tauntSrc.Play();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
