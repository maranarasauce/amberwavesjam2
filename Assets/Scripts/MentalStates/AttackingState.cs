using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : MentalState
{
    AttackState[] attackStates;
    public AttackingState(Boss boss, AttackState[] attackStates) : base(boss)
    {
        this.attackStates = attackStates;
    }

    public override void BeginState()
    {
        roundTimer = 0.5f;
    }

    float roundTimer;
    public override void Update()
    {
        base.Update();

        if (roundTimer <= 0)
            EndState();
        else roundTimer = Mathf.MoveTowards(roundTimer, 0, Time.deltaTime);
    }

    public override void EndState()
    {
        base.EndState();
        AttackState entry = attackStates.RandomEntry<AttackState>();
        boss.SwitchState(entry);
    }
}
