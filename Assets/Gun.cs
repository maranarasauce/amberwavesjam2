using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maranara.InputShell;

public class Gun : MonoBehaviour
{
    public UIController gunUI;
    public AudioSource src;
    public AudioSource reloadSrc;
    public AudioSource grenadeSrc;
    public AudioClip grenadeFireClip;
    public AudioClip grenadeReloadedClip;
    public Animator animator;
    public AnimEvent reloadEvent;
    public Transform quadFlare;
    public LayerMask gunFireMask;

    public ScreenShake shake;

    public Transform gunTip;
    public float Damage;

    public bool GrenadeReady { get => grenadeReady; }
    public float GrenadeCooldownRate { get => grenadeCooldownRate; }

    public GameObject grenadePrefab;
    public float grenadeLobForce = 20;
    public float grenadeCooldownTime;
    float grenadeCooldownRate;
    float timeTillFire;
    bool grenadeReady = true;
    
    [NonSerialized] public int ammo;
    public float fireRate;
    public int maxAmmo;
    bool reloading;
    private PlayerInput input;

    enum Animation
    {
        Idle = 0,
        Fire = 1,
        Reload = 2
    }

    Transform playCam;
    private void Start()
    {
        playCam = Camera.main.transform;
        ammo = maxAmmo;
        reloadEvent.executedAction += FinishReload;
        
        input = InputManager.instance.input;
        input.leftTriggerClick.onStateUp += OnTriggerRelease;
        input.rightTriggerClick.onStateDown += GrenadeLaunch;
    }

    private void Update()
    {
        if (input.leftTriggerClick.value && !reloading)
        {
            Fire();
        }
        
        
        if (grenadeCooldownRate <= 0 && !grenadeReady)
        {
            //gunUI.ToggleGrenade(false);
            grenadeSrc.clip = grenadeReloadedClip;
            grenadeSrc.Play();
            grenadeReady = true;
            grenadeCooldownRate = 0;
        }
        else
        {
            grenadeCooldownRate = Mathf.MoveTowards(grenadeCooldownRate, 0, Time.deltaTime);
        }
    }

    


    void Fire()
    {
        shake.ShakeScreen(0.05f, 0.1f);

        if (timeTillFire > 0)
        {
            timeTillFire -= Time.deltaTime;
            return;
        }
        timeTillFire = fireRate;

        if (ammo <= 0)
        {
            Reload();
            return;
        }
        SetAnim(Animation.Fire);
        ammo--;
        float randomRot = UnityEngine.Random.Range(0f, 360f);
        quadFlare.localRotation = Quaternion.Euler(new Vector3(0, 0, randomRot));

        RaycastHit hitInfo;
        if(Physics.Raycast(gunTip.position, gunTip.forward, out hitInfo, 10000f, gunFireMask))
        {
            if(hitInfo.transform.TryGetComponent<IDamageable>(out IDamageable objectHit))
            {
                objectHit.DoDamage(Damage);
            }
            if (hitInfo.transform.TryGetComponent<Grenade>(out Grenade grenadeHit))
            {
                grenadeHit.Explode();
            }
        }


        src.Stop();
        src.pitch = UnityEngine.Random.Range(0.97f, 1.02f);
        src.volume = UnityEngine.Random.Range(0.95f, 1.05f) * src.volume;
        src.Play();
    }

    void GrenadeLaunch()
    {
        if(!grenadeReady) { return; }

        shake.ShakeScreen(3f, 0.1f);
        //gunUI.ToggleGrenade(true);
        gunUI.GrenadeReset();
        grenadeReady = false;
        grenadeCooldownRate = grenadeCooldownTime;
        grenadeSrc.clip = grenadeFireClip;
        grenadeSrc.Play();
        GameObject grenadeObject = GameObject.Instantiate(grenadePrefab, playCam.position + (playCam.forward * 1.1f), playCam.rotation);
        Rigidbody rb = grenadeObject.GetComponent<Rigidbody>();
        rb.AddForce(playCam.forward * 20f, ForceMode.VelocityChange);
        rb.AddTorque(-playCam.right * 10, ForceMode.VelocityChange);
    }

    void Reload()
    {
        SetAnim(Animation.Reload);
        reloading = true;
        reloadSrc.Play();
    }

    void FinishReload()
    {
        SetAnim(Animation.Idle);
        ammo = maxAmmo;
        reloading = false;
    }

    private void OnTriggerRelease()
    {
        timeTillFire = 0;
        if (!reloading)
            SetAnim(Animation.Idle);
    }

    void SetAnim(Animation state)
    {
        animator.SetInteger("State", (int)state);
    }
}
