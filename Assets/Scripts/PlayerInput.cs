using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maranara.InputShell
{
    /*
     * CONTROLLER LAYOUT - : https://i.imgur.com/zNBGIUB.png : -
     */

    public class PlayerInput
    {
        public PlayerJoystick leftJoystick;
        public PlayerJoystick rightJoystick;

        public PlayerAxis leftTrigger;
        public PlayerDigital leftTriggerClick;
        public PlayerAxis rightTrigger;
        public PlayerDigital rightTriggerClick;

        public PlayerAxis leftGrip;
        public PlayerDigital leftGripClick;
        public PlayerAxis rightGrip;
        public PlayerDigital rightGripClick;

        public PlayerDigital buttonA;
        public PlayerDigital buttonB;
        public PlayerDigital buttonX;
        public PlayerDigital buttonY;

        public PlayerHaptic leftHaptic;
        public PlayerHaptic rightHaptic;

        public PlayerPose leftHandPose;
        public PlayerPose rightHandPose;


        public PlayerInput()
        {
            leftJoystick = new PlayerJoystick("LeftStick");
            rightJoystick = new PlayerJoystick("RightStick");

            leftTrigger = new PlayerAxis("LeftTrigger");
            leftTriggerClick = new PlayerDigital("LeftTriggerClick");
            rightTrigger = new PlayerAxis("RightTrigger");
            rightTriggerClick = new PlayerDigital("RightTriggerClick");

            leftGrip = new PlayerAxis("LeftGrip");
            leftGripClick = new PlayerDigital("LeftGripClick");
            rightGrip = new PlayerAxis("RightGrip");
            rightGripClick = new PlayerDigital("RightGripClick");

            buttonA = new PlayerDigital("A");
            buttonB = new PlayerDigital("B");
            buttonX = new PlayerDigital("X");
            buttonY = new PlayerDigital("Y");

            leftHaptic = new PlayerHaptic("LeftHaptics", Controller.LeftHand);
            rightHaptic = new PlayerHaptic("RightHaptics", Controller.RightHand);

            leftHandPose = new PlayerPose("LeftHandPose");
            rightHandPose = new PlayerPose("RightHandPose");
        }

        public delegate void OnBoolUpdate(bool pressed);
        public delegate void OnFloatUpdate(float value);
        public delegate void OnFloat2Update(Vector2 value);
        public delegate void OnFloatArrayUpdate(float[] value);
        public delegate void OnInputUpdate();
    }

    public class PlayerDigital
    {
        public string name;
        public event PlayerInput.OnBoolUpdate stateChanged;
        public event PlayerInput.OnInputUpdate onStateDown;
        public event PlayerInput.OnInputUpdate onStateUp;
        public bool value;

        public void Update(bool newValue)
        {
            if (value != newValue)
            {
                value = newValue;

                stateChanged?.Invoke(value);
                if (value)
                    onStateDown?.Invoke();
                else onStateUp?.Invoke();

            }
        }

        public PlayerDigital(string name)
        {
            this.name = name;
        }
    }

    public class PlayerAxis
    {
        public string name;
        public event PlayerInput.OnFloatUpdate axisChanged;
        public float value;

        public PlayerAxis(string name)
        {
            this.name = name;
        }

        public void Update(float newValue)
        {
            if (value != newValue)
            {
                value = newValue;
                axisChanged?.Invoke(value);
            }
        }
    }

    public class PlayerPose
    {
        public string name;
        public event PlayerInput.OnFloatArrayUpdate axisChanged;
        public float[] value;

        public PlayerPose(string name)
        {
            this.name = name;
        }

        public void Update(float[] newValue)
        {
            if (value != newValue)
            {
                value = newValue;
                axisChanged?.Invoke(value);
            }
        }
    }

    public class PlayerJoystick
    {
        public string name;
        public PlayerAxis xAxis;
        public PlayerAxis yAxis;
        public event PlayerInput.OnFloat2Update axisChanged;
        public Vector2 value;
        public PlayerJoystick(string name)
        {
            this.name = name;
            xAxis = new PlayerAxis(name + "X");
            yAxis = new PlayerAxis(name + "Y");
        }

        public void Update(Vector2 newValue)
        {
            if (value != newValue)
            {
                value = newValue;
                axisChanged?.Invoke(value);

                float valueX = newValue.x;
                float valueY = newValue.y;

                xAxis?.Update(valueX);
                yAxis?.Update(valueY);
            }
        }

        public void Update(float xValue, float yValue)
        {
            Vector2 newVec = value;
            newVec.x = xValue;
            newVec.y = yValue;

            if (value != newVec)
            {
                value = newVec;
                axisChanged?.Invoke(value);

                xAxis?.Update(xValue);
                yAxis?.Update(yValue);
            }
        }
    }

    public class PlayerHaptic
    {
        public string name;
        public Controller controller;

        public PlayerHaptic(string name, Controller controllerNumber)
        {
            this.name = name;
            this.controller = controllerNumber;
        }

        public Action<Vector2, Controller> vibrate;
        public void Vibrate(float intensity, float duration)
        {
            Vector2 value = new Vector2(intensity, duration);
            vibrate?.Invoke(value, controller);
        }
    }

    public enum Controller
    {
        LeftHand,
        RightHand,
        Chest,
        Head
    }
}
