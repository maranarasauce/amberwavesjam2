using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float constantSpeed = 1;
    public float damageAmount;
    public float tileDamageMod = 1f;
    [SerializeField] LayerMask hitMask;
    [NonSerialized] public Vector3 forward;

    public void Start()
    {
        forward = transform.forward;
    }

    public bool updateForwardAxis;
    private void Update()
    {
        if (updateForwardAxis)
            forward = transform.forward;

        float speed = constantSpeed * Time.deltaTime;
        Vector3 velocity = forward * speed;
        transform.position += velocity;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, speed + 0.1f, hitMask, QueryTriggerInteraction.Collide))
        {
            Hit(hit);
        }
    }

    void Hit(RaycastHit hit)
    {
        if (hit.collider.gameObject.TryGetComponent<DamageableObject>(out var desc))
        {
            float mod = 1f;
            if (desc.IsTile())
                mod *= tileDamageMod;
            desc.DoDamage(damageAmount * mod);
        }

        Destroy(gameObject);
    }
}