using System;
using UnityEngine;

    public class Projectile : MonoBehaviour
    {
        public float constantSpeed = 1;
        public int damageAmount;
        [SerializeField] LayerMask hitMask;
        [NonSerialized] public Vector3 forward;

        public void Start()
        {
            forward = transform.forward;
        }

        public bool updateForwardAxis;
        private void FixedUpdate()
        {
            if (updateForwardAxis)
                forward = transform.forward;

            float speed = constantSpeed * Time.fixedDeltaTime;
            Vector3 velocity = forward * speed;
            transform.position += velocity;

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, speed, hitMask, QueryTriggerInteraction.Collide))
            {
                Hit(hit);
            }
        }

        void Hit(RaycastHit hit)
        {
        Destroy(this);
        }
}