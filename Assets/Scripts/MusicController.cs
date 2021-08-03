using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicController : MonoBehaviour
{

    // Currently hardcoded for arena

    public AudioSource arenaAudio;
    public AudioClip[] arenaMusiksPool;
    List<AudioClip> arenaMusiks = new List<AudioClip>();

    private void Start()
    {
        GameManager.inst.OnLoadEnd += ControlArenaAudio;
        arenaMusiks.AddRange(arenaMusiksPool);
    }

    private void ControlArenaAudio()
    {
        if(GameManager.inst.NextActiveScene == SceneIndex.Arena)
        {
            if (!arenaAudio.isPlaying)
            {
                PlayRandomSong();    
            }
        }
        else
        {
            arenaAudio.Stop();
        }

    }

    public void Update()
    {
        if (!arenaAudio.isPlaying)
            PlayRandomSong();
    }

    public void PlayRandomSong()
    {
        if (arenaMusiks.Count == 0)
            arenaMusiks.AddRange(arenaMusiksPool);

        AudioClip clip = arenaMusiks[UnityEngine.Random.Range(0, arenaMusiks.Count + 1)];
        arenaMusiks.Remove(clip);
        arenaAudio.clip = clip;
        arenaAudio.Play();
    }
}
