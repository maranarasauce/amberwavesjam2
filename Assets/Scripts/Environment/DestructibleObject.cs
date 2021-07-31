using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour, IDamageable
{
    public float Health { get => health; }

    [SerializeField] private float killDelay = 1f;
    [SerializeField] private float health;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            DoDamage(1);
    }

    public void DoDamage(float damage)
    {
        health -= damage;

        if(health <= 0)
        {
            Kill();
        }
    }

    public void Kill()
    {
        Debug.Log("Destroyed tile: " + gameObject.name);
        Destroy(this.gameObject, killDelay);
    }
}
