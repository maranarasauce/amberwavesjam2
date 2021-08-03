using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{

    // Currently hardcoded for arena

    public AudioSource arenaAudio;

    private void Start()
    {
        GameManager.inst.OnLoadEnd += ControlArenaAudio;
    }

    private void ControlArenaAudio()
    {
        if(GameManager.inst.NextActiveScene == SceneIndex.Arena)
        {
            if (!arenaAudio.isPlaying)
                arenaAudio.Play();
        }
        else
        {
            arenaAudio.Stop();
        }

    }

    public void PlayAudio(AudioSource source)
    {
        source.Play();
    }
}
