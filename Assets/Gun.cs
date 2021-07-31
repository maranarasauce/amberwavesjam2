using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public AudioSource src;
    public AudioClip fireClip;
    public AudioClip reloadClip;

    Transform camera;
    private void Start()
    {
        camera = Camera.main.transform;
        ammo = maxAmmo;
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
        }
        if (Input.GetMouseButtonDown(1))
        {
            ammo = maxAmmo;
            reloading = false;
            src.clip = reloadClip;
            src.Play();
            src.clip = fireClip;
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
        ammo--;
        src.Stop();
        src.pitch = UnityEngine.Random.Range(0.99f, 1.01f);
        src.volume = UnityEngine.Random.Range(0.99f, 1.01f);
        src.Play();
    }

    void Reload()
    {
        reloading = true;
    }
}
