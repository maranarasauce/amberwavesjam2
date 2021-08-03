using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FartAttack : AttackState
{
    //This is the constructor. At the minimum you need to have (Boss boss, float time) : base (boss, time)
    public FartAttack(Boss boss, float time, float healthCeiling) : base(boss, time, healthCeiling)
    {
        fartSound = Resources.Load<AudioClip>("SFX/fart");
    }

    //This gets called whenever the state begins. Init timers here and such
    public override void BeginState()
    {
        base.BeginState();
        float randomX = Random.Range(10f, 20f);
        float randomZ = Random.Range(10f, 20f);
        centerPoint = new Vector3(randomX, 3f, randomZ);
        originalDelta = boss.maxDistanceDelta;
    }

    AudioClip fartSound;
    public override string GetAttackName()
    {
        return "Fart";
    }

    //This gets called whenever the attack timer is up and it switches back to Idle. You don't need this, but it's here if you want it.
    public override void EndState()
    {
        base.EndState();
        boss.maxDistanceDelta = originalDelta;
        fard = false;
        damageableObjects.Clear();
    }

    //Called every frame. Wow!
    float originalDelta;
    bool fard;
    Vector3 centerPoint;
    HashSet<Collider> damageableObjects = new HashSet<Collider>();
    public override void Update()
    {
        base.Update();

        float timeElapsed = roundDelay - TimeLeft;

        //Move to center
        if (timeElapsed < 1f)
        {
            boss.Move(centerPoint);
        }
        //Slam down and land
        if (1f < timeElapsed && timeElapsed > 4f)
        {
            if (!fard)
            {
                fard = true;

                boss.maxDistanceDelta = 0.01f;

                boss.PlaySFX(fartSound, 0.5f, false);

                Collider[] cols = Physics.OverlapSphere(boss.transform.position, 5f, boss.collisionMask);
                foreach (Collider col in cols)
                {
                    if (col.gameObject.TryGetComponent<DamageableObject>(out var desc))
                    {
                        desc.DoDamage(3, true);
                    }
                }
            }
        }

        if (6f < timeElapsed)
        {
            if (boss.transform.position.y < (centerPoint.y + 10f))
                boss.Move(Vector3.up * 0.1f * 2f, true);

            if (timeElapsed > 10f)
            {
                EndState();
            }
        }
    }
}
