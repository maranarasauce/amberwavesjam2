using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    public float blastRadius;
    public float damage;
    public float playerDmgReduction;

    public GameObject explosionParticle;
    bool exploded;

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
        Collider[] cols = Physics.OverlapSphere(transform.position, 3);
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

                // @Incomplete: 
                //if (TryGetComponent<PlayerHealth>(out _))
                //    dmg *= playerDmgReduction;

                desc.DoDamage(dmg);
            }

        }
        Destroy(gameObject);
    }

}
