using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public GameObject damagableObject;
    private IDamageable idam;
    public Slider slider;

    public bool followPlayer = true;


    public void Start()
    {
        idam = damagableObject.GetComponent<IDamageable>();
        slider.minValue = 0f;
        slider.maxValue = idam.Health;

    }


    public void Update()
    {
        slider.value = idam.Health;
        if(followPlayer)
            gameObject.transform.eulerAngles = Camera.main.transform.eulerAngles;
    }
}
