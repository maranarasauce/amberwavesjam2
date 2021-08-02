using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingState : MentalState
{
    AttackState[] attackStates;
    float[] weights;
    public AttackingState(Boss boss, float[] weights, AttackState[] attackStates) : base(boss)
    {
        this.weights = weights;
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

    AttackState lastEntry;
    public override void EndState()
    {
        base.EndState();
        int weight = Boss.GetRandomWeightedIndex(weights);
        AttackState entry = attackStates[weight];
        int regen = 0;
        while ((entry.HealthCeiling < boss.Health && entry.HealthCeiling != 0) || entry == lastEntry)
        {
            regen++;
            weight = Boss.GetRandomWeightedIndex(weights);
            
            entry = attackStates[weight];

            if (regen == 5) {
                break;
            }
                
        }
        lastEntry = entry;
        boss.SwitchState(entry);
    }
}
