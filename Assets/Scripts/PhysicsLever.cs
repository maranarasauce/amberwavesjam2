using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

public class PhysicsLever : ElectronicInput
{
    public ConfigurableJoint jt;
    public float clickThreshold;
    [SerializeField] private AudioSource clickSource;
    float limit;
    bool pressed;

    void Start()
    {
        limit = jt.linearLimit.limit;
    }

    void Update()
    {
        float currentPos = transform.localPosition.x;
        voltage = (currentPos) / (limit);

        /*if (!voltage.FastApproximately(lastVoltage, 0.001f))
        {
            voltageChangeHandler?.Invoke(voltage);
        }
            

        if (Mathf.Abs(voltage).FastApproximately(1, clickThreshold))
        {
            if (pressed)
                return;
            pressed = true;
            if (voltage > 0)
                Click(false);
            else Click(true);
        }
        else pressed = false;*/

        lastVoltage = voltage;
    }

    void Click(bool down)
    {
        if (down)
            OnClickDown?.Invoke();
        else OnClickUp?.Invoke();

        clickSource.Stop();
        clickSource.Play();
    }
}
