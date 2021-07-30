using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ElectronicJoint : MonoBehaviour
{
    public ElectronicInput input;
    public ConfigurableJoint jt;

    [SerializeField] bool changeTargetPos;
    [SerializeField] bool changeTargetVelocity;
    [SerializeField] bool changeMotionDrives;
    [SerializeField] bool changeTargetRot;
    [SerializeField] bool changeTargetAngularVelocity;
    [SerializeField] bool changeAngularDrives;


    public SerializedJoint lerpJt1;
    public SerializedJoint lerpJt2;

    private void Start()
    {
        OnVoltageChange(0);
       
    }

    private void OnEnable()
    {
        input.onVoltageChange += OnVoltageChange;
    }

    private void OnDisable()
    {
        input.onVoltageChange -= OnVoltageChange;
    }

    JointDrive driveX;
    JointDrive driveY;
    JointDrive driveZ;
    JointDrive angularDriveX;
    JointDrive angularDriveYZ;
    private void OnVoltageChange(float voltage)
    {
        float lerpVoltage = (voltage + 1) / 2;

        if (changeTargetPos)
            jt.targetPosition = Vector3.Lerp(lerpJt1.targetPosition, lerpJt2.targetPosition, lerpVoltage);
        if (changeTargetVelocity)
            jt.targetVelocity = Vector3.Lerp(lerpJt1.targetVelocity, lerpJt2.targetVelocity, lerpVoltage);
        if (changeTargetRot)
            jt.targetRotation = Quaternion.Lerp(lerpJt1.targetRotation, lerpJt2.targetRotation, lerpVoltage);
        if (changeTargetAngularVelocity)
            jt.targetAngularVelocity = Vector3.Lerp(lerpJt1.targetAngularVelocity, lerpJt2.targetAngularVelocity, lerpVoltage);
        if (changeMotionDrives)
        {
            SerializedJoint.DeserializeDrive(Vector3.Lerp(lerpJt1.driveX, lerpJt2.driveX, lerpVoltage), ref driveX);
            SerializedJoint.DeserializeDrive(Vector3.Lerp(lerpJt1.driveY, lerpJt2.driveY, lerpVoltage), ref driveY);
            SerializedJoint.DeserializeDrive(Vector3.Lerp(lerpJt1.driveZ, lerpJt2.driveZ, lerpVoltage), ref driveZ);
            jt.xDrive = driveX;
            jt.yDrive = driveY;
            jt.zDrive = driveZ;
        }
        if (changeAngularDrives)
        {
            SerializedJoint.DeserializeDrive(Vector3.Lerp(lerpJt1.angularDriveX, lerpJt2.driveX, lerpVoltage), ref angularDriveX);
            SerializedJoint.DeserializeDrive(Vector3.Lerp(lerpJt1.angularDriveYZ, lerpJt2.angularDriveYZ, lerpVoltage), ref angularDriveYZ);
            jt.angularXDrive = angularDriveX;
            jt.angularYZDrive = angularDriveYZ;
        }
    }
}

[Serializable]
public struct SerializedJoint
{
    public Vector3 targetPosition;
    public Vector3 targetVelocity;
    public Vector3 driveX;
    public Vector3 driveY;
    public Vector3 driveZ;
    public Quaternion targetRotation;
    public Vector3 targetAngularVelocity;
    public Vector3 angularDriveX;
    public Vector3 angularDriveYZ;

    public SerializedJoint(ConfigurableJoint jt)
    {
        targetPosition = jt.targetPosition;
        targetVelocity = jt.targetVelocity;
        driveX = new Vector3(jt.xDrive.positionSpring, jt.xDrive.positionDamper, jt.xDrive.maximumForce);
        driveY = new Vector3(jt.yDrive.positionSpring, jt.yDrive.positionDamper, jt.yDrive.maximumForce);
        driveZ = new Vector3(jt.xDrive.positionSpring, jt.xDrive.positionDamper, jt.xDrive.maximumForce);
        targetRotation = jt.targetRotation;
        targetAngularVelocity = jt.targetAngularVelocity;
        angularDriveX = new Vector3(jt.angularXDrive.positionSpring, jt.angularXDrive.positionDamper, jt.angularXDrive.maximumForce);
        angularDriveYZ = new Vector3(jt.angularYZDrive.positionSpring, jt.angularYZDrive.positionDamper, jt.angularYZDrive.maximumForce);
    }

    public static void DeserializeDrive(Vector3 serialized, ref JointDrive drive)
    {
        drive.positionSpring = serialized.x;
        drive.positionDamper = serialized.y;
        drive.maximumForce = serialized.z;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ElectronicJoint))]
public class ElectronicJointEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Register current joint as Joint 1"))
        {
            ElectronicJoint electronic = (ElectronicJoint)target;
            electronic.lerpJt1 = new SerializedJoint(electronic.jt);
        }
        if (GUILayout.Button("Register current joint as Joint 2"))
        {
            ElectronicJoint electronic = (ElectronicJoint)target;
            electronic.lerpJt2 = new SerializedJoint(electronic.jt);
        }
    }
}
#endif