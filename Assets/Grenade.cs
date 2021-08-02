using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float blastRadius;
    public float damage;
    public float playerDmgReduction;
    public Rigidbody rb;

    public GameObject explosionParticle;
    bool exploded;

    public bool isSpin;

    private void Update()
    {
        if (isSpin)
            transform.rotation = Quaternion.LookRotation(rb.velocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
    }

    public void Explode()
    {
        if (exploded)
            return;
        exploded = true;

        GameObject.Instantiate(explosionParticle, transform.position, Quaternion.identity);
        Collider[] cols = Physics.OverlapSphere(transform.position, blastRadius);
        foreach (Collider col in cols)
        {
            if (col.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                rb.AddForce((transform.position - rb.position).normalized * -1000, ForceMode.Impulse);
            }

            if (col.gameObject.TryGetComponent<DamageableObject>(out var desc))
            {
                if (desc == null)
                    continue;

                var dmg = damage;

                if (col.gameObject.TryGetComponent<PlayerHealth>(out _))
                   dmg *= playerDmgReduction;

                desc.DoDamage(dmg);
            }

        }
        Destroy(gameObject);
    }

}
