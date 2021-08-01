using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile : DamageableObject
{
    private float maxHealth;
    private MeshRenderer meshRenderer;

    protected override void Start()
    {
        base.Start();

        maxHealth    = Health;
        meshRenderer = GetComponent<MeshRenderer>();

        OnDamage += ChangeTileColor;
        OnKill   += DestroyTile;
    }
 
    private void ChangeTileColor()
    {
        meshRenderer.material.SetFloat("_DmgAmount", Health / maxHealth);
    }

    private void DestroyTile()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;

        var killDelay = killPS.main.startLifetimeMultiplier;

        Destroy(this.gameObject, killDelay);
    }
}