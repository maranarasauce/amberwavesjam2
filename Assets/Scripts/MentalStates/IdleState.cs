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
        roundTimer = UnityEngine.Random.Range(3f, 5f);
    }

    float roundTimerDelay;
    float roundTimer;
    Vector3 circleCenter;
    float circleFrequency = 1.75f;
    float circleRadius = 4f;
    public override void Update()
    {
        base.Update();
        float angle = Time.time * Mathf.PI * circleFrequency;
        Vector3 newPos = new Vector3(Mathf.Cos(angle) * circleRadius, 0, Mathf.Sin(angle) * circleRadius);
        newPos += circleCenter;
        boss.Move(newPos, false);

        if (roundTimer <= 0)
            EndState();
        else roundTimer = Mathf.MoveTowards(roundTimer, 0, Time.deltaTime);
    }

    public override void EndState()
    {
        base.EndState();
        boss.SwitchState(Boss.State.Attacking);
    }
}
