using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OppressorAttack : AttackState
{
    //This is the constructor. At the minimum you need to have (Boss boss, float time) : base (boss, time)
    public OppressorAttack(Boss boss, float time, float healthCeiling) : base(boss, time, healthCeiling)
    {
        bossBullet = Resources.Load<GameObject>("Prefabs/BossBulletNoTile");
        fireSfx = Resources.Load<AudioClip>("SFX/bossGunFire");
        firePoint = boss.gunFirePoint;
    }

    //This gets called whenever the state begins. Init timers here and such
    public override void BeginState()
    {
        base.BeginState();
        boss.Speak(boss.attack);
        playerRb = FloatingCapsuleController.instance.rb;
        boss.AnimateGun(true, playerRb.transform);
        originalDelta = boss.maxDistanceDelta;
        boss.maxDistanceDelta = 0.05f * 300f;
        fireDelay = (0.1f * boss.HealthPercent) + 0.025f;
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
    float originalDelta;
    float fireTimer;
    float fireDelay = 0.05f;
    Vector3 playerPos;
    Vector3 currentAim;
    //Called every frame. Wow!
    public override void Update()
    {
        base.Update();

        boss.Move(new Vector3(15f, 10f, 15f));

        float timeElapsed = roundDelay - TimeLeft;

        if (1.1f > timeElapsed && timeElapsed > 1f)
        {
            currentAim = playerCamera.position;
        }
        if (timeElapsed > 1.1f)
        {
            playerPos = playerCamera.position;
            currentAim = Vector3.MoveTowards(currentAim, playerPos, 80 / Time.deltaTime);

            Vector3 fireDir = (currentAim - firePoint.position);
            fireDir.y = (Mathf.Sin(timeElapsed * 20) * 6f) - 7f;
            fireDir.x += Random.Range(-1f, 1f);
            if (fireTimer <= 0)
            {
                fireTimer = fireDelay;
                FireBullet(fireDir);
            }
            else fireTimer = Mathf.MoveTowards(fireTimer, 0, Time.deltaTime);
        }
    }

    Vector3 lastVel = Vector3.zero;
    void FireBullet(Vector3 fireDir)
    {
        boss.PlaySFX(fireSfx, 0.2f, true);
        GameObject.Instantiate(bossBullet, firePoint.position, Quaternion.LookRotation(fireDir));
    }


    
}
