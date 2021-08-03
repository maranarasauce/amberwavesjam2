using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : MonoBehaviour, IDamageable
{
    public float Health { get => health; }
    public bool IsDead  { get => isDead; }
    public float LastDamageValue { get => lastDamage; }

    [SerializeField] protected ParticleSystem damagePS;
    [SerializeField] protected ParticleSystem killPS;
    [SerializeField] protected float health;

    public event Action OnDamage;
    public event Action OnKill;

    private bool isDead = false;
    private float lastDamage;

    protected virtual void Start()
    {
        if(damagePS != null)
            OnDamage += damagePS.Play;
       
        if(killPS != null)
            OnKill   += killPS.Play;
    }

    public void DoDamage(float damage, bool dontKill)
    {
        if (dontKill && ((health - damage) <= 0))
        {
            return;
        }

        health -= damage;
        lastDamage = damage;

        OnDamage?.Invoke();

        if(health <= 0 && !isDead)
        {
            Kill();
        }
    }

    public void DoDamage(float damage)
    {
        DoDamage(damage, false);
    }

    public void Kill()
    {
        isDead = true;
        OnKill?.Invoke();
    }
}
