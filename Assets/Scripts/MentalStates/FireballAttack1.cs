using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveAttack : AttackState
{
    //This is the constructor. At the minimum you need to have (Boss boss, float time) : base (boss, time)
    public ShockwaveAttack(Boss boss, float time) : base(boss, time)
    {
    }

    //This gets called whenever the state begins. Init timers here and such
    public override void BeginState()
    {
        base.BeginState();
        originalDelta = boss.maxDistanceDelta;
    }

    //This gets called whenever the attack timer is up and it switches back to Idle. You don't need this, but it's here if you want it.
    public override void EndState()
    {
        base.EndState();
        boss.maxDistanceDelta = originalDelta;
        launched = false;
    }

    //Called every frame. Wow!
    float originalDelta;
    bool landed;
    bool launched;
    public override void Update()
    {
        base.Update();

        //Move to center
        if (TimeLeft > 27f)
        {
            boss.Move(new Vector3(15f, 10f, 15f));
        }
        //Move up for a bit
        if (27f > TimeLeft && TimeLeft > 25f)
        {
            boss.maxDistanceDelta = 0.02f;

            boss.Move(Vector3.up * 0.1f, true);
        }
        //Slam down and land
        if (25f > TimeLeft && TimeLeft > 19f && !landed)
        {
            boss.maxDistanceDelta = 0.3f;

            if (Physics.Raycast(boss.transform.position, -boss.transform.up, boss.collisionRadius + 1, boss.collisionMask))
                landed = true;

            boss.Move(Vector3.down * 50, true);
        }
        //If never lands, go back to center
        if (19f > TimeLeft && TimeLeft > 16f && !landed)
        {
            boss.maxDistanceDelta = 0.8f;
            boss.Move(new Vector3(15f, 10f, 15f));
            if (!launched)
            {
                launched = true;
                FloatingCapsuleController.instance.rb.AddForce(Vector3.up * 30, ForceMode.VelocityChange);
            }
        }
        //Then after going to center, end state
        if (16f > TimeLeft && !landed)
        {
            EndState();
        }

        if (landed)
        {

        }
    }
}
