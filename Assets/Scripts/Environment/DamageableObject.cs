using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageableObject : MonoBehaviour, IDamageable
{
    public float Health { get => health; }
    public float LastDamageValue { get => lastDamage; }

    [SerializeField] protected ParticleSystem damagePS;
    [SerializeField] protected ParticleSystem killPS;
    [SerializeField] protected float health;

    public event Action OnDamage;
    public event Action OnKill;

    private float lastDamage;

    protected virtual void Start()
    {
        if(damagePS != null)
            OnDamage += damagePS.Play;
       
        if(killPS != null)
            OnKill   += killPS.Play;
    }

    public void DoDamage(float damage)
    {
        health -= damage;
        lastDamage = damage;

        OnDamage?.Invoke();

        if(health <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        OnKill?.Invoke();
        
        Debug.Log("Killed object: " + gameObject.name);
    }
}
