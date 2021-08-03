using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigeonAttack : AttackState
{
    //This is the constructor. At the minimum you need to have (Boss boss, float time) : base (boss, time)
    public PigeonAttack(Boss boss, float time, float healthCeiling) : base(boss, time, healthCeiling)
    {
        bossBullet = Resources.Load<GameObject>("Prefabs/BossBullet");
        fireSfx = Resources.Load<AudioClip>("SFX/bossGunFire");
        firePoint = boss.gunFirePoint;
    }

    //This gets called whenever the state begins. Init timers here and such
    public override void BeginState()
    {
        base.BeginState();
        boss.Speak(boss.attack);
        playerRb = FloatingCapsuleController.instance.rb;
        LaunchPlayer();
        boss.AnimateGun(true, playerRb.transform);
        originalDelta = boss.maxDistanceDelta;
        boss.maxDistanceDelta = 0.05f;
    }

    public override string GetAttackName()
    {
        return "Pigeon";
    }

    //This gets called whenever the attack timer is up and it switches back to Idle. You don't need this, but it's here if you want it.
    public override void EndState()
    {
        base.EndState();
        boss.maxDistanceDelta = originalDelta;
        boss.AnimateGun(false, null);
    }

    Rigidbody playerRb;
    Transform firePoint;
    GameObject bossBullet;
    AudioClip fireSfx;
    float fireDelay = 0.2f;
    float fireTimer;
    float originalDelta;
    //Called every frame. Wow!
    public override void Update()
    {
        base.Update();

        boss.Move(new Vector3(15f, 10f, 15f));

        if (Mathf.Sin(TimeLeft * 1.5f) > 0)
        {
            if (fireTimer <= 0)
            {
                fireTimer = fireDelay;
                FireBullet();
            }
            else fireTimer = Mathf.MoveTowards(fireTimer, 0, Time.deltaTime);
        }
    }

    Vector3 lastVel = Vector3.zero;
    void FireBullet()
    {
        boss.PlaySFX(fireSfx, 0.2f, true);

        Vector3 targetPos = playerRb.transform.position;

        float dist = Vector3.Distance(targetPos, firePoint.position);
        float timeToHit = dist / 40;
        Vector3 speed = playerRb.velocity;
        lastVel = playerRb.velocity;

        speed *= timeToHit;

        targetPos += speed;

        Vector3 direction = (targetPos - firePoint.position);

        float healthPercentage = (boss.HealthPercent * 0.4f) + 0.13f;
        Vector3 fireDir = direction.normalized + (Random.insideUnitSphere * healthPercentage);

        GameObject.Instantiate(bossBullet, firePoint.position, Quaternion.LookRotation(fireDir));
    }

    void LaunchPlayer()
    {
        Vector3 launchVector = new Vector3(0, 1f, 0);
        playerRb.AddForce(launchVector * 23f, ForceMode.VelocityChange);
    }

    
}
