using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Transform gunTip;
    public float Damage;

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
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && !reloading)
        {
            Fire();
        }
        if (Input.GetMouseButtonUp(0))
        {
            timeTillFire = 0;
            if (!reloading)
                SetAnim(Animation.Idle);
        }
        if (Input.GetMouseButtonDown(1))
        {
            if (grenadeReady)
                GrenadeLaunch();
        }
        if (grenadeCooldownRate > 0)
            grenadeCooldownRate -= Time.deltaTime;
        else if (!grenadeReady)
        {
            gunUI.toggleGrenade(false);
            grenadeSrc.clip = grenadeReloadedClip;
            grenadeSrc.Play();
            grenadeReady = true;
            grenadeCooldownRate = 0;
        }
    }

    public GameObject grenadePrefab;
    public float grenadeLobForce = 20;
    bool grenadeReady = true;
    float grenadeCooldownRate;
    float timeTillFire;
    [NonSerialized] public int ammo;
    public float fireRate;
    public float grenadeCooldownTime;
    public int maxAmmo;
    bool reloading;
    void Fire()
    {
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
        }


        src.Stop();
        src.pitch = UnityEngine.Random.Range(0.97f, 1.02f);
        src.volume = UnityEngine.Random.Range(0.93f, 1.01f);
        src.Play();
    }

    void GrenadeLaunch()
    {
        gunUI.toggleGrenade(true);
        grenadeReady = false;
        grenadeCooldownRate = grenadeCooldownTime;
        grenadeSrc.clip = grenadeFireClip;
        grenadeSrc.Play();
        GameObject grenadeObject = GameObject.Instantiate(grenadePrefab, playCam.position + (playCam.forward * 1.1f), playCam.rotation);
        grenadePrefab.transform.position = transform.position;
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

    void SetAnim(Animation state)
    {
        animator.SetInteger("State", (int)state);
    }
}
