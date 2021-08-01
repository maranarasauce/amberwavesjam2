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
        Preparing,
        Attacking,
        Recovering,
        Dying
    }

    Dictionary<State, MentalState> mentalStates = new Dictionary<State, MentalState>();

    public enum AttackState
    {
        Fireball,
        Uzi,
        Canonball
    }

    [Serializable]
    public class Dialogue
    {
        public AudioClip clip;
        public string subtitle;
    }

    AttackState currentAttack;
    MentalState currentState;

    public TextMeshProUGUI dialogueString;
    public AudioSource src;

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
        SwitchState(State.Idling);
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
    }

    #region Damage
    private void Boss_OnDamage()
    {
        if (LastDamageValue >= 3)
        {
            RandomSpeak(hurtBIG, 2);
        } else
        {
            RandomSpeak(hurt, 5);
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
    void SwitchState(MentalState state)
    {
        if (currentState != null)
            currentState.EndState();
        currentState = state;
        currentState.BeginState();
    }

    void SwitchState(State state)
    {
        SwitchState(mentalStates[state]);
    }
    #endregion

    #region "Physics"
    [Header("'Physics'")]
    public float collisionRadius;
    public LayerMask collisionMask;
    public void Move(Vector3 position)
    {
        Vector3 direction = (transform.position - position).normalized;
        Vector3 origin = transform.position + (-direction * collisionRadius);
        Debug.DrawLine(origin, position);
        if (Physics.Linecast(origin, position, out RaycastHit hitInfo, collisionMask))
        {
            Vector3 calcPoint = hitInfo.point + (-hitInfo.normal * collisionRadius);
            Vector3.Lerp(transform.position, calcPoint, 0.5f);
        } else
        {
            transform.position = position;
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
