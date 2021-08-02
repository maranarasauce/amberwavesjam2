using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallClose : AttackState
{
    bool isClosed;

    GameObject[] walls;
    //This is the constructor. At the minimum you need to have (Boss boss, float time) : base (boss, time)
    public WallClose(Boss boss, float time, float healthCeiling, ScreenShake shake) : base(boss, time, healthCeiling)
    {
        moveSound = Resources.Load<AudioClip>("SFX/wallMove");
        walls = GameObject.FindGameObjectsWithTag("Wall");
        this.shake = shake;
    }

    bool isShaking;
    ScreenShake shake;
    AudioClip moveSound;
    GameObject grenadePrefab;
    //This gets called whenever the state begins. Init timers here and such
    public override void BeginState()
    {
        base.BeginState();
        wallTimer = wallTimerDelay;
        grenadeLaunchTimer = grenadeLaunchDelay;
        boss.PlaySFX(moveSound, 0.34f, false);
        isShaking = true;
    }

    public override string GetAttackName()
    {
        return "WallClose";
    }

    //This gets called whenever the attack timer is up and it switches back to Idle. You don't need this, but it's here if you want it.
    public override void EndState()
    {
        base.EndState();
    }

    float height = 10.5f;
    public float grenadeLaunchDelay = 3.9f;
    public float grenadeLaunchTimer;
    float lastMove;
    //Called every frame. Wow!
    public override void Update()
    {
        base.Update();
        
        if (isShaking)
        {
            shake.ShakeScreen(1.4f, 10f);
            isShaking = false;
        }

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

        // doo doo ca ca
        MoveWalls();
    }

    public float wallTimerDelay = 5f;
    public float wallTimer;
    void MoveWalls()
    {
        if (wallTimer > 0)
        {
            wallTimer = Mathf.MoveTowards(wallTimer, 0, Time.deltaTime);
            if (!isClosed)
            {
                foreach (var wall in walls)
                    wall.transform.position += wall.transform.right * 1f * Time.deltaTime;
            } else
            {
                foreach (var wall in walls)
                    wall.transform.position -= wall.transform.right * 1f * Time.deltaTime;
            }
        } else
        {
            isClosed = isClosed ? false : true;
            wallTimer = wallTimerDelay;
            EndState();
        }
    }
}
