using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : DamageableObject
{
    ScreenShake shake;
    public AudioSource hurtSource;
    public AudioClip[] hurtClips;

    private void Start()
    {
        shake = gameObject.GetComponent<ScreenShake>();
    }

    private void OnEnable()
    {
        OnDamage += PlayerHealth_OnDamage;
        OnKill += PlayerHealth_OnKill;
    }

    private void PlayerHealth_OnDamage()
    {
        if (!hurtSource.isPlaying)
        {
            hurtSource.clip = hurtClips.GetRandomValue();
            hurtSource.Play();
        }
        
        shake.ShakeScreen(1f, 0.2f);
    }

    private void OnDisable()
    {
        OnKill -= PlayerHealth_OnKill;
    }

    bool kill;
    private void PlayerHealth_OnKill()
    {
        if (kill)
            return;
        kill = true;

        Tile[] tiles = FindObjectsOfType<Tile>();
        foreach (Tile tile in tiles)
        {
            tile.Kill();
        }
    }
}
