using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballAttack : AttackState
{
    public FireballAttack(Boss boss, float time, GameObject fireballPrefab) : base(boss, time)
    {
        roundDelay = time;
        this.grenadePrefab = fireballPrefab;
    }

    GameObject grenadePrefab;
    public override void BeginState()
    {
        base.BeginState();
        height = 10.5f;
        grenadeLaunchTimer = grenadeLaunchDelay;
    }

    float height;
    public float grenadeLaunchDelay = 3f;
    public float grenadeLaunchTimer;
    float lastMove;
    public override void Update()
    {
        base.Update();

        //TODO: get distance from player and distance behind boss, find medium between two. if wall is too close, go behind player
        Vector3 directionTowardsPlayer = (playerCamera.position - boss.transform.position);
        Vector3 wallPoint = Vector3.zero;

        float distanceFromPlayer = directionTowardsPlayer.magnitude;
        float distanceFromWall = 0;

        Vector3 rayTowardsWall = -directionTowardsPlayer;
        rayTowardsWall.y = 0;
        if (Physics.Raycast(boss.transform.position, rayTowardsWall.normalized, out RaycastHit hitInfo, Mathf.Infinity, boss.collisionMask))
        {
            wallPoint = hitInfo.point;
            wallPoint.y = height;
            distanceFromWall = hitInfo.distance;
        }

        Vector3 cameraNoVert = playerCamera.position;
        cameraNoVert.y = height;

        Vector3 midpoint = Vector3.Lerp(wallPoint, cameraNoVert, 0.5f);
        float move = Mathf.Sin(Time.time) * Random.Range(-1, 1);
        move = Mathf.Lerp(move, lastMove, 0.6f);
        midpoint += playerCamera.right * move * 0.5f;
        boss.Move(midpoint);
        lastMove = move;

        if (grenadeLaunchTimer <= 0)
            LaunchGrenade(directionTowardsPlayer);
        else grenadeLaunchTimer = Mathf.MoveTowards(grenadeLaunchTimer, 0, Time.deltaTime);
    }

    void LaunchGrenade(Vector3 playerVector)
    {
        grenadeLaunchTimer = grenadeLaunchDelay;
        Vector3 directionTowardsPlayer = playerVector.normalized;
        GameObject grenadeObject = GameObject.Instantiate(grenadePrefab, boss.transform.position + (directionTowardsPlayer * 1.3f), Quaternion.Euler(directionTowardsPlayer));
        Rigidbody rb = grenadeObject.GetComponent<Rigidbody>();
        rb.AddForce(directionTowardsPlayer * 20f, ForceMode.VelocityChange);
        rb.AddTorque(-boss.transform.right * 10, ForceMode.VelocityChange);
    }
}
