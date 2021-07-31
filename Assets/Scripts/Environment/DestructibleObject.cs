using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour, IDamageable
{
    public float Health { get => health; }
    float maxHealth;
    public float LastDamageValue { get => lastDamage; }

    [SerializeField] private ParticleSystem damagePS;
    [SerializeField] private ParticleSystem killPS;
    [SerializeField] private float health;
    MeshRenderer meshRenderer;

    public event Action OnDamage;
    public event Action OnKill;

    private float lastDamage;

    private void Start()
    {
        maxHealth = health;
        OnDamage += damagePS.Play;
        OnKill   += killPS.Play;
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void DoDamage(float damage)
    {
        health -= damage;
        lastDamage = damage;
        meshRenderer.material.SetFloat("_DmgAmount", health / maxHealth);

        OnDamage?.Invoke();

        if(health <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        OnKill?.Invoke();

        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;

        var killDelay = killPS.main.startLifetimeMultiplier;

        Destroy(this.gameObject, killDelay);
        Debug.Log("Destroyed object: " + gameObject.name);
    }
}
