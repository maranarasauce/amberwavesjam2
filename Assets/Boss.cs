using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Boss : DamageableObject
{
    public enum State
    {
        Idling,
        Attacking,
        Dying
    }

    Dictionary<State, MentalState> mentalStates = new Dictionary<State, MentalState>();
    AttackState[] attackStates;

    [Serializable]
    public class Dialogue
    {
        public AudioClip clip;
        public string subtitle;
    }

    AttackState currentAttack;
    MentalState currentState;

    public GameObject grenade;
    public GameObject bigGrenade;
    public TextMeshProUGUI dialogueString;
    public AudioSource src;
    [SerializeField] SkinnedMeshRenderer skin;
    Transform player;

    [Header("Dialogue Entries")]
    [SerializeField] Dialogue[] attack;
    [SerializeField] Dialogue[] hurt;
    [SerializeField] Dialogue[] hurtBIG;
    [SerializeField] Dialogue[] death;
    [SerializeField] Dialogue[] intro;
    public Dialogue[] taunt;

    protected override void Start()
    {
        base.Start();

        base.OnDamage += Boss_OnDamage;
        mentalStates.Add(State.Idling, new IdleState(this));
        player = Camera.main.transform;
        //This is the attack array. Set your attack here if you want the boss to use it!!!
        attackStates = new AttackState[]
        {
            new FireballAttack(this, 16, grenade),
            new LargeFireballAttack(this, 16, bigGrenade),
            new ShockwaveAttack(this, 15, 250)
        };
        mentalStates.Add(State.Attacking, new AttackingState(this, attackStates));
        SwitchState(State.Idling);
        Speak(intro);
    }

    public void Update()
    {
        if (speaking)
        {
            if (!src.isPlaying)
            {
                StartCoroutine(WaitToDisableText());
            }
        }

        currentState.Update();

        skin.SetBlendShapeWeight(0, Mathf.Sin(Time.time * 10) * 100);
        skin.rootBone.transform.LookAt(player);
        Vector3 rootBoneRot = skin.rootBone.rotation.eulerAngles;
        rootBoneRot.x = 0;
        skin.rootBone.rotation = Quaternion.Euler(rootBoneRot);
    }

    #region Damage
    private void Boss_OnDamage()
    {
        if (LastDamageValue >= 3)
        {
            RandomSpeak(hurtBIG, 2);
        } else
        {
            RandomSpeak(hurt, 15);
        }
    }

    
    #endregion

    #region Dialogue
    bool speaking;
    public void Speak(Dialogue[] dialogueSet)
    {
        Dialogue dialogue = dialogueSet.GetRandomValue();
        src.clip = dialogue.clip;
        dialogueString.text = dialogue.subtitle;
        src.Play();
        speaking = true;
    }
    public void RandomSpeak(Dialogue[] dialogueSet, int chance)
    {
        int random = UnityEngine.Random.Range(0, chance - 1);
        if (random == 0)
        {
            Speak(dialogueSet);
        }
    }

    IEnumerator WaitToDisableText()
    {
        speaking = false;
        yield return new WaitForSecondsRealtime(1);
        dialogueString.text = null;
    }
    #endregion

    #region States
    public void SwitchState(MentalState state)
    {
        currentState = state;
        currentState.BeginState();
    }

    public void SwitchState(State state)
    {
        SwitchState(mentalStates[state]);
    }
    #endregion

    #region "Physics"
    [Header("'Physics'")]
    public float collisionRadius;
    public float maxDistanceDelta = 3;
    public LayerMask collisionMask;
    public void Move(Vector3 position)
    {
        Vector3 direction = (position - transform.position);
        Vector3 origin = transform.position;
        float distance = (collisionRadius + 2f);
        Debug.DrawRay(origin, direction.normalized);
        if (Physics.Raycast(origin, direction.normalized, out RaycastHit hitInfo, distance, collisionMask))
        {
            Vector3 calcPoint = hitInfo.point + (hitInfo.normal * collisionRadius * 2);
            transform.position = Vector3.MoveTowards(transform.position, calcPoint, maxDistanceDelta);
        } else
        {
            transform.position = Vector3.MoveTowards(transform.position, position, maxDistanceDelta);
        }
    }

    public void Move(Vector3 position, bool additive)
    {
        if (additive)
            position += transform.position;
        Move(position);
    }
    #endregion
}

public class MentalState
{
    public float initialStateLength;
    public Boss boss;
    public Transform playerCamera;

    public MentalState(Boss boss)
    {
        this.boss = boss;
        playerCamera = Camera.main.transform.parent.Find("CameraDir");
    }

    public virtual void Update()
    {

    }

    public virtual void BeginState()
    {

    }

    public virtual void EndState()
    {

    }
}

public class AttackState : MentalState
{
    public AttackState(Boss boss, float time) : base(boss)
    {
        roundDelay = time;
    }

    public AttackState(Boss boss, float time, float healthCeiling) : base(boss)
    {
        roundDelay = time;
        onlyBelowHealth = healthCeiling;
    }

    public float TimeLeft { get => roundTimer; }
    public float HealthCeiling { get => onlyBelowHealth; }
    float onlyBelowHealth = 0;
    float roundTimer;
    public float roundDelay;

    public virtual string GetAttackName()
    {
        return "Base";
    }

    public override void BeginState()
    {
        base.BeginState();
        roundTimer = roundDelay;
    }

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
        boss.SwitchState(Boss.State.Idling);
    }
}