using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : DamageableObject
{
    public AudioSource hurtSource;
    public AudioClip[] hurtClips;

    private ScreenShake shake;
    
    protected override void Start()
    {
        shake = gameObject.GetComponent<ScreenShake>();
    }

    private void OnEnable()
    {
        OnDamage += PlayerHealth_OnDamage;
        OnKill += PlayerHealth_OnKill;
    }

    private void OnDisable()
    {
        OnKill -= PlayerHealth_OnKill;
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

    private void PlayerHealth_OnKill()
    {
        Tile[] tiles = FindObjectsOfType<Tile>();
        foreach (Tile tile in tiles)
        {
            tile.Kill();
        }
    }
}
