using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JostleAttack : AttackState
{
    //This is the constructor. At the minimum you need to have (Boss boss, float time) : base (boss, time)
    public JostleAttack(Boss boss, float time, float healthCeiling) : base(boss, time, healthCeiling)
    {
        punchClip = Resources.Load<AudioClip>("SFX/punch");
    }

    //This gets called whenever the state begins. Init timers here and such
    public override void BeginState()
    {
        base.BeginState();
        playerRb = FloatingCapsuleController.instance.rb;
        boss.AnimateGun(true, playerRb.transform);
    }

    public override string GetAttackName()
    {
        return "Jostle";
    }

    //This gets called whenever the attack timer is up and it switches back to Idle. You don't need this, but it's here if you want it.
    public override void EndState()
    {
        base.EndState();
        boss.AnimateGun(false, null);
    }

    Rigidbody playerRb;
    public float grenadeLaunchTimer;
    float lastMove;
    AudioClip punchClip;
    //Called every frame. Wow!
    public override void Update()
    {
        base.Update();
        
        //Grenade is launched every 3 seconds, based on the grenadeLaunchDelay variable.
        if (grenadeLaunchTimer <= 0)
            LaunchPlayer();
        else grenadeLaunchTimer = Mathf.MoveTowards(grenadeLaunchTimer, 0, Time.deltaTime);
    }

    void LaunchPlayer()
    {
        boss.PlaySFX(punchClip, 0.25f);
        grenadeLaunchTimer = UnityEngine.Random.Range(0.3f, 1f);
        Vector3 launchVector = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-0.2f, 0.4f), UnityEngine.Random.Range(-1f, 1f));
        playerRb.AddForce(launchVector * 50f, ForceMode.VelocityChange);
    }

    
}
