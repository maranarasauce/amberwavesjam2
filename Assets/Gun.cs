using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public AudioSource src;
    public AudioSource reloadSrc;
    public Animator animator;
    public AnimEvent reloadEvent;

    enum Animation
    {
        Idle = 0,
        Fire = 1,
        Reload = 2
    }

    Transform camera;
    private void Start()
    {
        camera = Camera.main.transform;
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
            
        }
    }

    float timeTillFire;
    int ammo;
    public float fireRate;
    public int maxAmmo;
    bool reloading;
    void Fire()
    {
        if (timeTillFire > 0)
        {
            timeTillFire -= Time.unscaledDeltaTime;
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
        src.Stop();
        src.pitch = UnityEngine.Random.Range(0.95f, 1.05f);
        src.volume = UnityEngine.Random.Range(0.93f, 1.01f);
        src.Play();
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
