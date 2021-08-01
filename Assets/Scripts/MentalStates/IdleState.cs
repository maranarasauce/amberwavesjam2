using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : MentalState
{
    public IdleState(Boss boss) : base(boss)
    {
    }

    public override void BeginState()
    {
        circleCenter = boss.transform.position;
        timeUntilSwitch = UnityEngine.Random.Range(1f, 5f);
    }

    float initial_TimeUntilSwitch;
    float timeUntilSwitch;
    Vector3 circleCenter;
    float circleFrequency = 1f;
    float circleRadius = 2;
    public override void Update()
    {
        base.Update();
        float angle = Time.time * Mathf.PI * circleFrequency;
        Vector3 newPos = new Vector3(Mathf.Cos(angle) * circleRadius, 0, Mathf.Sin(angle) * circleRadius);
        newPos += circleCenter;
        boss.Move(newPos, false);
    }
}
