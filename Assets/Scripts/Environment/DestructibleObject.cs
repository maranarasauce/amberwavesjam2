using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class DestructibleObject : MonoBehaviour, IDamageable
{
    public float Health { get => health; }

    [SerializeField] private float killDelay = 0f;
    [SerializeField] private float health;

    public event Action OnDamage;
    public event Action OnKill;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            DoDamage(1);
    }

    public void DoDamage(float damage)
    {
        health -= damage;

        OnDamage?.Invoke();

        if(health <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        OnKill?.Invoke();

        Destroy(this.gameObject, killDelay);
        Debug.Log("Destroyed object: " + gameObject.name);
    }
}
