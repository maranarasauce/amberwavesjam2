using Maranara.SVR.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsHandJoint : PhysicsHand
{
    public ConfigurableJoint jt;
    Quaternion startRot;

    private void Start()
    {
        linearDrive = jt.xDrive;
        angularDrive = jt.angularXDrive;
        emptyDrive = new JointDrive()
        {
            maximumForce = 0
        };
        startRot = transform.rotation;
        maxForce = jt.xDrive.maximumForce;
    }

    float maxForce;
    Vector3 lastTargetPos;
    public override void AddForceTowardsTarget()
    {
        if (solve)
        {
            linearDrive.maximumForce = (maxForce * forceMultiplier) + 1;
            angularDrive.maximumForce = (maxForce * torqueMultiplier) + 1;
            SetDrives();
        } else
        {
            linearDrive.maximumForce = 1;
            angularDrive.maximumForce = 1;
            SetDrives();
        }
        

        Vector3 targetVel = (target.position - lastTargetPos) * Time.fixedDeltaTime;
        Vector3 targetPos = playerRB.transform.InverseTransformPoint(target.position);
        jt.targetPosition = targetPos;
        jt.targetVelocity = targetVel;
        jt.SetTargetRotationLocal(Quaternion.Inverse(target.rotation), startRot);
        lastTargetPos = target.position;
    }

    JointDrive linearDrive;
    JointDrive angularDrive;
    JointDrive emptyDrive;
    public void SetDrives()
    {
        jt.xDrive = linearDrive;
        jt.yDrive = linearDrive;
        jt.zDrive = linearDrive;
        jt.angularXDrive = angularDrive;
        jt.angularYZDrive = angularDrive;
    }
    public override void Solve(bool solve)
    {
            rigidbody.isKinematic = !solve;
    }

    public override void PushMask(Collision col)
    {
    }

}
