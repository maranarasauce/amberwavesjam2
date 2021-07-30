using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
public class RotateOnJoint : MonoBehaviour
{
    ConfigurableJoint jt;
    // Start is called before the first frame update
    void Start()
    {
        jt = GetComponent<ConfigurableJoint>();
        startRot = jt.transform.rotation;
    }

    public float rotateRate;
    Quaternion startRot;
    public Vector3 rotateAxis;
    // Update is called once per frame
    void Update()
    {
        Quaternion newRot = transform.localRotation;
        Quaternion rotate = Quaternion.AngleAxis(rotateRate, rotateAxis);
        newRot = rotate * newRot;
        jt.SetTargetRotationLocal(newRot, startRot);
    }
}
