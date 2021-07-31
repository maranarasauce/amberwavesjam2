using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private Gun gun;
    public Text ammoCounter;

    public void Start()
    {
        gun = GameObject.FindObjectOfType<Gun>();
    }

    public void Update()
    {
        ammoCounter.text = gun.ammo + "/" + gun.maxAmmo;
    }
}
