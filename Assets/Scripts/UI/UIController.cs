using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private Gun gun;
    public Text ammoCounter;
    public Image grenadeIcon;
    public Image grappleIcon;

    public void Start()
    {
        gun = GameObject.FindObjectOfType<Gun>();
    }

    public void Update()
    {
        ammoCounter.text = gun.ammo + "/" + gun.maxAmmo;
    }

    public void toggleGrenade(bool i)
    {
        var color = grenadeIcon.color;
        color.a = (i) ? 0.4f : 1f;
        grenadeIcon.color = color;
    }

    public void toggleGrapple(bool i)
    {
        var color = grappleIcon.color;
        color.a = (i) ? 0.4f : 1f;
        grappleIcon.color = color;
    }
}
