using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ElectronicInput : MonoBehaviour
{
    public UnityEvent OnClickDown;
    public UnityEvent OnClickUp;
    public delegate void ElectricUpdate(float voltage);
    public event ElectricUpdate onVoltageChange;
    internal ElectricUpdate voltageChangeHandler;
    public float voltage;
    internal float lastVoltage;

    private void Awake()
    {
        voltageChangeHandler = onVoltageChange;
    }
}
