using Maranara.InputShell;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingCapsuleController : MonoBehaviour
{
    public float ridecastLength;
    float realRidecastLength;
    float ridecastModifier;
    [NonSerialized] public float ridecastFloor;
    float rideHeight;
    public float defaultRideHeight;
    float rideHeightModifier;
    [NonSerialized] public float rideHeightFloor;
    public float jumpForce;
    public LayerMask rideMask;
    Rigidbody rb;
    ConfigurableJoint jt;

    public bool grounded;

    public float maxSpeed;
    public float moveMaxForce;
    public float moveSpring;
    public float moveDamper;
    public AnimationCurve deccelCurve;

    JointDrive yDrive;
    JointDrive xDrive;
    JointDrive zDrive;
    Ray rideRay;

    PlayerInput input;
    public Transform cameraTransform;

    private void Start()
    {
        input = InputManager.instance.input;

        footstepSrc = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        jt = GetComponent<ConfigurableJoint>();
        yDrive = jt.yDrive;
        xDrive = jt.xDrive;
        zDrive = jt.zDrive;

        rideRay = new Ray()
        {
            direction = -transform.up,
            origin = transform.position
        };

        realRidecastLength = ridecastLength;

        jumpTime = maxJumpTime;

        rideHeight = defaultRideHeight;
    }

    private void OnEnable()
    {
        if (input == null)
            input = InputManager.instance.input;
        input.buttonA.onStateDown += ReadyJump;
        input.buttonA.onStateUp += ReleaseJump;
    }


    private void OnDisable()
    {
        input.buttonA.onStateDown -= ReadyJump;
        input.buttonA.onStateUp -= ReleaseJump;
    }

    Vector3 targetPosition;
    void FixedUpdate()
    {
        targetPosition = Vector3.zero;

        xDrive.positionDamper = moveDamper;
        xDrive.positionSpring = moveSpring;
        zDrive.positionDamper = moveDamper;
        zDrive.positionSpring = moveSpring;

        Float();

        JointMove();

        jt.xDrive = xDrive;
        jt.yDrive = yDrive;
        jt.zDrive = zDrive;
        jt.targetPosition = targetPosition;
    }

    private void Update()
    {
        rideHeightModifier = 1;
        ridecastModifier = 1;
        Crouch();
        UpdateJump();
        Step();
    }

    void Float()
    {
        jt.connectedAnchor = Vector3.zero;

        Debug.DrawRay(rideRay.origin, rideRay.direction * realRidecastLength);

        rideRay.origin = transform.position;

        rideHeight = (rideHeightModifier * defaultRideHeight) + rideHeightFloor;
        realRidecastLength = (ridecastModifier * ridecastLength) + ridecastFloor;

        if (Physics.Raycast(rideRay, out RaycastHit hit, realRidecastLength, rideMask))
        {
            float x = hit.distance - rideHeight;

            yDrive.maximumForce = 100000f;
            targetPosition += transform.up * x;

            grounded = true;

            Rigidbody hitRb = hit.rigidbody;
            if (hitRb != null)
            {
                Vector3 carriedVel = hitRb.velocity * Time.fixedDeltaTime;
                carriedVel.y = 0;
                targetPosition -= carriedVel;
            }
        }
        else
        {
            yDrive.maximumForce = 0;

            grounded = false;
        }
    }

    float stepX;
    [Header("Footsteps")]
    [SerializeField] float stepSpeed = 500;
    [SerializeField] float stepPeak = 10;
    AudioSource footstepSrc;
    [SerializeField] AudioClip[] footstepSfxs;
    [SerializeField] AudioClip[] jumpSfxs;
    bool stepped;
    void Step()
    {
        if (grounded)
        {
            stepX += rb.velocity.sqrMagnitude * Time.unscaledDeltaTime;
            float stepY = Mathf.Sin((stepX * Mathf.PI) / stepSpeed);
            if (stepY < -0.9f && !stepped)
            {
                footstepSrc.clip = footstepSfxs.RandomEntry();
                footstepSrc.Play();
                stepped = true;
            }
            else if (stepY >= -0.9f)
            {
                stepped = false;
            }
        }
    }

    void JointMove()
    {
        Vector3 moveInput = new Vector3(input.leftJoystick.value.x, 0, input.leftJoystick.value.y).normalized;
        xDrive.maximumForce = deccelCurve.Evaluate(Mathf.Abs(moveInput.x)) * moveMaxForce;
        zDrive.maximumForce = deccelCurve.Evaluate(Mathf.Abs(moveInput.z)) * moveMaxForce;
        targetPosition -= cameraTransform.TransformDirection(moveInput) * maxSpeed;
    }

    [Header("Jump")]
    float jumpTime;
    float readyJumpTime;
    public float maxJumpTime;
    public float jumpTuckMultiplier = 0.5f;
    bool readyingJump;
    public AnimationCurve jumpTuckCurve;
    void ReadyJump()
    {
        if (!readyingJump)
        {
            readyingJump = true;
            readyJumpTime = 0.3f;
        }
    }

    private void ReleaseJump()
    {
        if (readyingJump)
        {
            readyingJump = false;
            readyJumpTime = 0;

            jumpTime = 0;

            if (grounded && Physics.Raycast(rideRay, ridecastLength))
            {
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
                footstepSrc.clip = jumpSfxs.RandomEntry();
                footstepSrc.Play();
                stepped = true;
            }
        }
    }
    
    void UpdateJump()
    {
        if (jumpTime < maxJumpTime)
        {
            jumpTime += Time.deltaTime;
            float m = (1 - jumpTuckMultiplier) / -maxJumpTime;
            float calc = (-m * jumpTime) + jumpTuckMultiplier;
            ridecastModifier *= calc;
        }
        else
        {
            realRidecastLength = ridecastLength;
            jumpTime = maxJumpTime;
        }

        if (readyingJump)
        {
            if (readyJumpTime > 0)
            {
                readyJumpTime -= Time.unscaledDeltaTime;
                rideHeightModifier *= jumpTuckCurve.Evaluate(readyJumpTime);
            } else
            {
                rideHeightModifier *= jumpTuckCurve.Evaluate(0);
            }
        }
        
    }

    public AnimationCurve crouchRideCurve;
    public AnimationCurve crouchRayCurve;
    void Crouch()
    {
        float crouchValue = input.rightJoystick.value.y;
        rideHeightModifier *= crouchRideCurve.Evaluate(crouchValue);
        ridecastModifier *= crouchRayCurve.Evaluate(crouchValue);
    }

    public void Teleport(Transform target)
    {
        rb.velocity = Vector3.zero;
        targetPosition = Vector3.zero;
        xDrive.positionDamper = 0;
        xDrive.positionSpring = 0;
        zDrive.positionDamper = 0;
        zDrive.positionSpring = 0;

        Vector3 offset = transform.root.position - transform.position;
        transform.root.position = target.position + offset;
    }
}
