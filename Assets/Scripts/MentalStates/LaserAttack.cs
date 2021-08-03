using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserAttack : AttackState
{
    //This is the constructor. At the minimum you need to have (Boss boss, float time) : base (boss, time)
    public LaserAttack(Boss boss, float time, float healthCeiling) : base(boss, time, healthCeiling)
    {
        laser = boss.transform.Find("ART/Armature/Spine/Laser").gameObject;
        laserOrigin = boss.transform.Find("ART/Armature/Spine/Laser/LaserOrigin");
        laserParticle = laserOrigin.Find("Particle System");
        src = boss.transform.Find("SFX/LAZER").GetComponent<AudioSource>();
    }

    GameObject laser;
    Transform laserOrigin;
    Transform laserParticle;
    AudioSource src;
    float originalDelta;
    //This gets called whenever the state begins. Init timers here and such
    public override void BeginState()
    {
        base.BeginState();
        boss.skin.SetBlendShapeWeight(1, 1);
        playerRb = FloatingCapsuleController.instance.rb;
        originalDelta = boss.maxDistanceDelta;
        boss.maxDistanceDelta = 0.05f * 300f;
        lasered = false;
        laserSound = false;
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
        laser.SetActive(false);
        boss.LookAtPlayer(true);
    }

    Rigidbody playerRb;
    Transform firePoint;
    

    Vector3 currentAim;
    bool lasered;
    float tileDamageMod = 0.01f;
    float damageAmount = 0.1f;

    float lastX;
    bool laserSound;
    //Called every frame. Wow!
    public override void Update()
    {
        base.Update();

        boss.Move(new Vector3(15f, 10f, 15f));

        float timeElapsed = roundDelay - TimeLeft;

        if (2f > timeElapsed && timeElapsed > 1.5f)
        {
            boss.LookAtPlayer(false);
            currentAim = playerCamera.position;
            if (laserSound == false)
            {
                src.Play();
                laserSound = true;
            }
        }
        if (7f > timeElapsed && timeElapsed > 2f)
        {
            
            laserOrigin.LookAt(playerCamera.position);
            Vector3 rot = laserOrigin.localRotation.eulerAngles;

            if (!lasered)
            {
                lasered = true;
                laser.SetActive(true);
                lastX = 45f;
            }

            lastX = Mathf.MoveTowards(lastX, rot.x, 7f * Time.deltaTime);

            rot.x = lastX;
            rot.y = 0;
            rot.z = 0;
            laserOrigin.localRotation = Quaternion.Euler(rot);
            

            if (Physics.Raycast(laserOrigin.position, laserOrigin.forward, out RaycastHit hit, Mathf.Infinity, boss.collisionMask, QueryTriggerInteraction.Collide))
            {
                laserParticle.position = hit.point;
                if (hit.collider.gameObject.TryGetComponent<DamageableObject>(out var desc))
                {
                    float mod = 1f;
                    if (desc.IsTile())
                        mod *= tileDamageMod;
                    desc.DoDamage(damageAmount * mod);
                }
            }

            boss.transform.Rotate(Vector3.up, 360f * Time.deltaTime);
        }

        if (timeElapsed > 7f)
        {
            src.Stop();
            laser.SetActive(false);
            boss.skin.SetBlendShapeWeight(1, 0);
        }
    }
}
