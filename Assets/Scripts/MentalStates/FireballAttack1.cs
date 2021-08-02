using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveAttack : AttackState
{
    //This is the constructor. At the minimum you need to have (Boss boss, float time) : base (boss, time)
    public ShockwaveAttack(Boss boss, float time, float healthCeiling) : base(boss, time, healthCeiling)
    {
        impactSound = Resources.Load<AudioClip>("SFX/shockwaveImpact");
        riseSound = Resources.Load<AudioClip>("SFX/shockwaveRise");
    }

    //This gets called whenever the state begins. Init timers here and such
    public override void BeginState()
    {
        base.BeginState();
        float randomX = Random.Range(10f, 20f);
        float randomZ = Random.Range(10f, 20f);
        centerPoint = new Vector3(randomX, 10f, randomZ);
        originalDelta = boss.maxDistanceDelta;
    }

    AudioClip impactSound;
    AudioClip riseSound;
    AudioClip fallSound;
    public override string GetAttackName()
    {
        return "Shockwave";
    }

    //This gets called whenever the attack timer is up and it switches back to Idle. You don't need this, but it's here if you want it.
    public override void EndState()
    {
        base.EndState();
        boss.maxDistanceDelta = originalDelta;
        landed = false;
        landedTime = 0;
        launched = false;
        damageableObjects.Clear();
    }

    //Called every frame. Wow!
    float originalDelta;
    bool landed;
    bool launched;
    float landedTime;
    Vector3 centerPoint;
    Vector3 landPoint;
    HashSet<Collider> damageableObjects = new HashSet<Collider>();
    public override void Update()
    {
        base.Update();

        //Move to center
        if (TimeLeft > 13f)
        {
            boss.Move(centerPoint);
        }
        //Move up for a bit
        if (13f > TimeLeft && TimeLeft > 10f)
        {
            if (!boss.fxSrc.isPlaying)
                boss.PlaySFX(riseSound, 0.5f, true);

            boss.maxDistanceDelta = 0.02f;

            boss.Move(Vector3.up * 0.1f, true);
        }
        //Slam down and land
        if (10f > TimeLeft && TimeLeft > 4f && !landed)
        {
            boss.maxDistanceDelta = 0.3f;

            if (Physics.Raycast(boss.transform.position, -boss.transform.up, boss.collisionRadius + 1, boss.collisionMask))
            {
                boss.PlaySFX(impactSound, 0.5f, false);
                landPoint = boss.transform.position;

                Collider[] cols = Physics.OverlapSphere(boss.transform.position, 5f, boss.collisionMask);
                foreach (Collider col in cols)
                {
                    if (col.gameObject.TryGetComponent<DamageableObject>(out var desc))
                    {
                        desc.DoDamage(2, true);
                    }
                }
                landed = true;
                landedTime = TimeLeft;
            }
                

            boss.Move(Vector3.down * 50, true);
        }

        #region DidntLand
        //If never lands, go back to center
        if (4f > TimeLeft && TimeLeft > 1f && !landed)
        {
            boss.maxDistanceDelta = 0.8f;
            boss.Move(centerPoint);
            if (!launched)
            {
                launched = true;
                FloatingCapsuleController.instance.rb.AddForce(Vector3.up * 10, ForceMode.VelocityChange);
            }
        }
        //Then after going to center, end state
        if (1f > TimeLeft && !landed)
        {
            EndState();
        }
        #endregion

        if (landed)
        {
            float delta = landedTime - TimeLeft;

            if (delta > 2.75f)
            {
                Collider[] cols = Physics.OverlapSphere(landPoint, ((delta - 2.75f) * 26f), boss.collisionMask);
                foreach (Collider col in cols)
                {
                    if (damageableObjects.Contains(col))
                        continue;
                    damageableObjects.Add(col);

                    if (col.gameObject.TryGetComponent<DamageableObject>(out var desc))
                    {
                        desc.DoDamage(1, true);
                    }
                }
            }

            boss.maxDistanceDelta = 0.05f;

            boss.Move(Vector3.up * 0.1f, true);

            if (delta > 5.75f)
            {
                EndState();
            }
        }
    }
}
