using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : MentalState
{
    public DeathState(Boss boss) : base(boss)
    {
    }

    public override void BeginState()
    {
        boss.AnimateGun(false, null);
        boss.animator.Play("SatanDeath");
        boss.deathParticleSystem.Play();
        boss.Speak(boss.death);
        boss.LookAtPlayer(false);

        boss.dead = true;
    }

    public override void Update()
    {
        base.Update();
        boss.maxDistanceDelta = 0.02f;
        boss.Move(Vector3.down * 2f * Time.deltaTime, true);
        Collider[] cols = Physics.OverlapSphere(boss.transform.position, 5f, boss.collisionMask);
        foreach (Collider col in cols)
        {
            if (col.gameObject.TryGetComponent<Tile>(out var desc))
            {
                desc.DoDamage(1, false);
            }
        }
        List<Collider> oldCol = new List<Collider>();
        oldCol.AddRange(oldCol);

        Collider[] outerCols = Physics.OverlapSphere(boss.transform.position, 10f, boss.collisionMask);
        foreach (Collider col in outerCols)
        {
            if (oldCol.Contains(col))
                continue;
            if (col.gameObject.TryGetComponent<Tile>(out var desc))
            {
                desc.DoDamage(3, true);
            }
        }

        if (boss.transform.position.y < -1f && !ended)
        {
            EndState();
        }
    }

    bool ended;
    public override void EndState()
    {
        base.EndState();
        ended = true;
        GameManager.inst.SwapActiveScene(SceneIndex.Win);
    }
}
