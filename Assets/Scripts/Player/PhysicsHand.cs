using UnityEngine;
using Maranara.SVR.Player;
using System;

namespace Maranara.SVR.Player
{
    public enum HandType
    {
        Left = 0,
        Right = 1
    }
    public class PhysicsHand : MonoBehaviour
    {
        public HandType hand;
        public Transform target;
        public Rigidbody rigidbody;
        
         public float forceMultiplier = 1;
         public float torqueMultiplier = 1;
        public bool solve = true;
        [Header("PID Stuff")]
        [SerializeField] float frequency = 10;
        [SerializeField] float damping = 1;
        [SerializeField] float climbMultiplier = 0.5f;
        Vector3 targetLastPos = Vector3.zero;
        [NonSerialized] public Vector3 desiredVelocity = Vector3.zero;
        bool climbing;
        public Rigidbody playerRB;
        public FloatingCapsuleController controller;
        
        private void Start()
        {
            if (rigidbody == null)
                rigidbody = GetComponent<Rigidbody>();
            rigidbody.maxAngularVelocity = 9999;
        }

        private void FixedUpdate()
        {
            AddForceTowardsTarget();
        }

        public LayerMask handPushMask;
        private void OnCollisionStay(Collision collision)
        {
            PushMask(collision);
        }

        public virtual void Solve(bool solve)
        {
            this.solve = solve;
        }

        public virtual void PushMask(Collision col)
        {
            if (handPushMask == (handPushMask | (1 << col.gameObject.layer)))
            {
                playerRB.AddForce(-desiredVelocity);
                if (!controller.grounded)
                {
                    //haptics?.Execute(0f, 0.001f, 0.001f, 0.1f, SteamVR_Input_Sources.Any);
                    ChangeClimbingState(true);

                }
                else
                {
                    ChangeClimbingState(false);
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            ChangeClimbingState(false);
        }

        public void ChangeClimbingState(bool changeTo)
        {
            climbing = changeTo;
            if (climbing)
            {
                //damping = dragAmount;
            } else
            {
                //damping = originalDamping;
            }
        }
        
        //https://digitalopus.ca/site/pd-controllers/
        public virtual void AddForceTowardsTarget()
        {
            if (!solve)
                return;

            Vector3 Vdes = (targetLastPos - target.position) + Vector3.ClampMagnitude(playerRB.velocity, 20);
            float kp = (6f * frequency) * (6f * frequency) * 0.25f;
            float kd = 4.5f * frequency * damping;
            float dt = Time.fixedDeltaTime;
            float g = 1 / (1 + kd * dt + kp * dt * dt);
            float ksg = kp * g;
            float kdg = (kd + kp * dt) * g;
            Vector3 Pt0 = transform.position;
            Vector3 Vt0 = rigidbody.velocity;
            Vector3 Pdes = target.position;
            Vector3 F = (Pdes - Pt0) * ksg + (Vdes - Vt0) * kdg;
            F = F * forceMultiplier * (climbing ? climbMultiplier : 1);
            rigidbody.AddForce(F);
            desiredVelocity = F;
            targetLastPos = target.position;

            Quaternion desiredRotation = target.rotation;
            Vector3 x;
            float xMag;
            Quaternion q = desiredRotation * Quaternion.Inverse(transform.rotation);
            // Q can be the-long-rotation-around-the-sphere eg. 350 degrees
            // We want the equivalant short rotation eg. -10 degrees
            // Check if rotation is greater than 190 degees == q.w is negative
            if (q.w < 0)
            {
                // Convert the quaterion to eqivalent "short way around" quaterion
                q.x = -q.x;
                q.y = -q.y;
                q.z = -q.z;
                q.w = -q.w;
            }
            q.ToAngleAxis(out xMag, out x);
            x.Normalize();
            x *= Mathf.Deg2Rad;
            Vector3 pidv = kp * x * xMag - kd * rigidbody.angularVelocity;
            Quaternion rotInertia2World = rigidbody.inertiaTensorRotation * transform.rotation;
            pidv = Quaternion.Inverse(rotInertia2World) * pidv;
            pidv.Scale(rigidbody.inertiaTensor);
            pidv = rotInertia2World * pidv * torqueMultiplier;
            rigidbody.AddTorque(pidv);
        }
    }

}
