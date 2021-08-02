using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : DamageableObject
{
    private void OnEnable()
    {
        OnKill += PlayerHealth_OnKill;
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
