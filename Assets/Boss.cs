using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public bool init_NoAI;
    public bool hard;

    Dictionary<State, MentalState> mentalStates = new Dictionary<State, MentalState>();

    [Serializable]
    public class Dialogue
    {
        public AudioClip clip;
        public string subtitle;
    }

    AttackState currentAttack;
    MentalState currentState;

    ScreenShake shake;
    public GameObject grenade;
    public GameObject bigGrenade;
    public GameObject mine;
    public TextMeshProUGUI dialogueString;
    public AudioSource src;
    public AudioSource fxSrc;
    [SerializeField] public SkinnedMeshRenderer skin;
    Transform player;

    [Header("Dialogue Entries")]
    public Dialogue[] attack;
    [SerializeField] Dialogue[] hurt;
    [SerializeField] Dialogue[] hurtBIG;
    public Dialogue[] death;
    [SerializeField] Dialogue[] intro;
    public Dialogue[] taunt;
    
    protected override void Start()
    {
        base.Start();

        base.OnDamage += Boss_OnDamage;
        base.OnKill += Boss_OnKill;
        percentage = 1f;

        if (!init_NoAI)
        {
            LookAtPlayer(true);

            mentalStates.Add(State.Idling, new IdleState(this));
            player = Camera.main.transform;
            shake = FloatingCapsuleController.instance.GetComponent<ScreenShake>();

            if (hard)
                health = 450f;

            //This is the attack array. Set your attack here if you want the boss to use it!!!
            List<AttackStateWeight> attackIndex = new List<AttackStateWeight>()
            {
                //new AttackStateWeight( Attack Constructor, Weight of 0.0 to 1.0),

                new AttackStateWeight( 0.9f , new FireballAttack(this, 16, 0f, grenade)),
                new AttackStateWeight( 0.6f, new LargeFireballAttack(this, 16, 200f, bigGrenade)),
                new AttackStateWeight( 0.3f, new ShockwaveAttack(this, 15, 100f, shake)),
                new AttackStateWeight( 0.4f, new JostleAttack(this, 5, 250f)),
                new AttackStateWeight( 0.5f, new WallClose(this, 9, 350f, shake)),
                new AttackStateWeight( 0.8f, new PigeonAttack(this, 4f, 0f)),
                new AttackStateWeight( 0.01f, new FartAttack(this, 11f, 0f)),
                new AttackStateWeight( 0.6f, new OppressorAttack(this, 10f, 0f) ),
                new AttackStateWeight( 0.4f, new LaserAttack(this, 9f, 0f)),
                new AttackStateWeight( 0.8f, new MineAttack(this, 5f, 0f, mine))
            };

            List<float> weights = new List<float>();
            List<AttackState> attackStates = new List<AttackState>();

            foreach (AttackStateWeight weight in attackIndex)
            {
                weights.Add(weight.weight);

                if (hard)
                    weight.state.SetHealthCeiling(0f);

                attackStates.Add(weight.state);
            }

            mentalStates.Add(State.Attacking, new AttackingState(this, weights.ToArray(), attackStates.ToArray()));
            mentalStates.Add(State.Dying, new DeathState(this));
            SwitchState(State.Idling);
            Speak(intro);
        }

        startingHealth = health;
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

        if (currentState != null)
            currentState.Update();

        skin.SetBlendShapeWeight(0, Mathf.Sin(Time.time * 10) * 100);
        
        if (lookAtPlayer)
        {
            skin.rootBone.transform.LookAt(player);
            Vector3 rootBoneRot = skin.rootBone.rotation.eulerAngles;
            rootBoneRot.x = 0;
            skin.rootBone.rotation = Quaternion.Euler(rootBoneRot);
        }
    }

    #region Damage
    [NonSerialized] public float startingHealth;
    float percentage;
    public float HealthPercent { get => percentage; }
    private void Boss_OnDamage()
    {
        if (LastDamageValue >= 3)
        {
            RandomSpeak(hurtBIG, 2);
        } else
        {
            RandomSpeak(hurt, 15);
        }

        percentage = Health / startingHealth;
    }

    [NonSerialized] public bool dead;
    private void Boss_OnKill()
    {
        if (!dead)
        {
            SwitchState(State.Dying);
        }
        
    }
    #endregion

    #region Dialogue
    bool speaking;
    public void Speak(Dialogue[] dialogueSet)
    {
        if (dead)
            return;
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

    public void PlaySFX(AudioClip clip, float volume, bool spatial)
    {
        fxSrc.spatialBlend = (spatial ? 1f : 0f);
        fxSrc.volume = volume;
        fxSrc.clip = clip;
        fxSrc.Play();
    }

    public void PlaySFX(AudioClip clip, float volume)
    {
        PlaySFX(clip, volume, false);
    }

    public void PlaySFX(AudioClip clip)
    {
        PlaySFX(clip, 1f, false);
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

    #region Animation
    [Header("Animation")]
    [SerializeField] Transform bicepL;
    public Transform gunFirePoint;
    public Animator animator;
    bool usingGun;
    Transform target;
    public ParticleSystem deathParticleSystem;
    public void AnimateGun(bool animateGun, Transform target)
    {
        usingGun = animateGun;
        this.target = target;
        string animate = (usingGun ? "SatanGun" : "SatanIdle");
        animator.Play(animate);
    }

    bool lookAtPlayer;
    public void LookAtPlayer(bool look)
    {
        lookAtPlayer = look;
    }

    public void LateUpdate()
    {
        if (usingGun)
        {
            bicepL.LookAt(target, Vector3.up);
            bicepL.rotation *= Quaternion.FromToRotation(Vector3.up, Vector3.forward);
        }
    }
    #endregion

    #region Math
    public static int GetRandomWeightedIndex(float[] weights)
    {
        if (weights == null || weights.Length == 0) return -1;

        float w;
        float t = 0;
        int i;
        for (i = 0; i < weights.Length; i++)
        {
            w = weights[i];

            if (float.IsPositiveInfinity(w))
            {
                return i;
            }
            else if (w >= 0f && !float.IsNaN(w))
            {
                t += weights[i];
            }
        }

        float r = UnityEngine.Random.value;
        float s = 0f;

        for (i = 0; i < weights.Length; i++)
        {
            w = weights[i];
            if (float.IsNaN(w) || w <= 0f) continue;

            s += w / t;
            if (s >= r) return i;
        }

        return -1;
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

    public void SetHealthCeiling(float onlyBelowHealth)
    {
        this.onlyBelowHealth = onlyBelowHealth;
    }

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

public class AttackStateWeight
{
    public AttackStateWeight(float weight, AttackState state)
    {
        this.state = state;
        this.weight = weight;
    }

    public AttackState state;
    public float weight;
}

