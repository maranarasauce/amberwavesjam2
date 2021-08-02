using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeFireballAttack : AttackState
{
    //This is the constructor. At the minimum you need to have (Boss boss, float time) : base (boss, time)
    public LargeFireballAttack(Boss boss, float time, float healthCeiling, GameObject fireballPrefab) : base(boss, time, healthCeiling)
    {
        this.grenadePrefab = fireballPrefab;
    }

    GameObject grenadePrefab;
    //This gets called whenever the state begins. Init timers here and such
    public override void BeginState()
    {
        base.BeginState();
        grenadeLaunchTimer = grenadeLaunchDelay;
    }

    public override string GetAttackName()
    {
        return "Big Fireball";
    }

    //This gets called whenever the attack timer is up and it switches back to Idle. You don't need this, but it's here if you want it.
    public override void EndState()
    {
        base.EndState();
    }

    float height = 10.5f;
    public float grenadeLaunchDelay = 7.9f;
    public float grenadeLaunchTimer;
    float lastMove;
    //Called every frame. Wow!
    public override void Update()
    {
        base.Update();
        
        //This moves the Boss to the midpoint between the player and the wall - it also does some sin movement to keep the player on his toes.
        Vector3 directionTowardsPlayer = (playerCamera.position - boss.transform.position);
        Vector3 wallPoint = Vector3.zero;

        Vector3 rayTowardsWall = -directionTowardsPlayer;
        rayTowardsWall.y = 0;
        if (Physics.Raycast(boss.transform.position, rayTowardsWall.normalized, out RaycastHit hitInfo, Mathf.Infinity, boss.collisionMask))
        {
            wallPoint = hitInfo.point;
            wallPoint.y = height;
        }

        Vector3 cameraNoVert = playerCamera.position;
        cameraNoVert.y = height;

        Vector3 midpoint = Vector3.Lerp(wallPoint, cameraNoVert, 0.5f);
        float move = Mathf.Sin(Time.time) * Random.Range(-1, 1);
        move = Mathf.Lerp(move, lastMove, 0.6f);
        midpoint += playerCamera.right * move * 0.5f;
        boss.Move(midpoint);
        lastMove = move;

        //Grenade is launched every 3 seconds, based on the grenadeLaunchDelay variable.
        if (grenadeLaunchTimer <= 0)
            LaunchGrenade(directionTowardsPlayer);
        else grenadeLaunchTimer = Mathf.MoveTowards(grenadeLaunchTimer, 0, Time.deltaTime);
    }

    void LaunchGrenade(Vector3 playerVector)
    {
        grenadeLaunchTimer = grenadeLaunchDelay;
        Vector3 directionTowardsPlayer = playerVector.normalized;
        directionTowardsPlayer.y = -0.6f;
        GameObject grenadeObject = GameObject.Instantiate(grenadePrefab, boss.transform.position + (directionTowardsPlayer * 2f), Quaternion.Euler(directionTowardsPlayer));
        Rigidbody rb = grenadeObject.GetComponent<Rigidbody>();
        rb.AddForce(directionTowardsPlayer * 20f, ForceMode.VelocityChange);
        rb.AddTorque(-boss.transform.right * 10, ForceMode.VelocityChange);
    }

    
}
