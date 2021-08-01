using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text ammoCounter;
    [SerializeField] private Image grenadeIcon;
    [SerializeField] private Image grappleIcon;

    [SerializeField] private RectMask2D grenadeMask;
    [SerializeField] private RectMask2D grappleMask;

    private Gun gun;
    private GrappleHook grapple;
    private float grappleTimer;

    public void Start()
    {
        gun = GameObject.FindObjectOfType<Gun>();
        grapple = GameObject.FindObjectOfType<GrappleHook>();
        grappleTimer = grapple.coolDown;
    }

    public void Update()
    {
        ammoCounter.text = gun.ammo + "/" + gun.maxAmmo;

        if (!gun.GrenadeReady)
        {
            Fill(grenadeMask, 1 - (gun.GrenadeCooldownRate / gun.grenadeCooldownTime));
        }

        if (!grapple.CanGrapple)
        {
            Fill(grappleMask, 1 - (grappleTimer / grapple.coolDown));
        }

        grappleTimer = Mathf.MoveTowards(grappleTimer, 0, Time.deltaTime);
    }



    public void GrenadeReset() => ResetCooldown(grenadeMask);
    public void GrappleReset()
    {
        grappleTimer = grapple.coolDown;
        ResetCooldown(grappleMask);
    }

    private void ResetCooldown(RectMask2D mask)
    {
        mask.padding = new Vector4(0, 0, 0, mask.rectTransform.rect.height);
    }

    public void Fill(RectMask2D mask, float value)
    {
        mask.padding = new Vector4(0, 0, 0, Mathf.Lerp(mask.rectTransform.rect.height, 0, value));
    }

    public void ToggleGrenade(bool i)
    {
        var color = grenadeIcon.color;
        color.a = (i) ? 0.4f : 1f;
        grenadeIcon.color = color;
    }

    public void ToggleGrapple(bool i)
    {
        var color = grappleIcon.color;
        color.a = (i) ? 0.4f : 1f;
        grappleIcon.color = color;
    }
}
