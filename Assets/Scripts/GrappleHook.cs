using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maranara.InputShell;

public class GrappleHook : MonoBehaviour
{
    private Rigidbody playerRB;
    private InputManager inputManager;
    private FloatingCapsuleController floatingCapsule;
    private ConfigurableJoint joint;
    private LineRenderer lr;

    public bool canGrapple;

    [Header("Grapple Settings")]
    public float grappleMaxDistance;
    public float grappleForce;
    public float coolDown;
    public GameObject ropeTarget;



    public JointDrive newDrive;


    public void Start()
    {
        inputManager = InputManager.instance;
        playerRB = gameObject.GetComponent<Rigidbody>();
        floatingCapsule = gameObject.GetComponent<FloatingCapsuleController>();
        joint = gameObject.GetComponent<ConfigurableJoint>();
        lr = gameObject.GetComponent<LineRenderer>();
        newDrive = new JointDrive { maximumForce = 0f };
    }

    public void Update()
    {
        lr.SetPosition(0, ropeTarget.transform.position);
    }

    public void OnEnable()
    {
        inputManager.input.buttonX.onStateDown += ButtonX_onStateDown;
    }

    public void OnDisable()
    {
        inputManager.input.buttonX.onStateDown -= ButtonX_onStateDown;
    }

    private void ButtonX_onStateDown()
    {
        if(canGrapple)
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, grappleMaxDistance))
            {
                DrawRope(hitInfo.point);
                StartCoroutine(wait(hitInfo));
            }
        }
    }

    public void Grapple(RaycastHit hitInfo)
    {
        canGrapple = false;
        print("Tiddies");
        floatingCapsule.enabled = false;
        joint.xDrive = newDrive;
        joint.yDrive = newDrive;
        joint.zDrive = newDrive;
        print("Boobies");
        playerRB.AddForce((hitInfo.point - transform.position) * grappleForce);
        StartCoroutine(waitForSec());
    }

    public void DrawRope(Vector3 targetPos)
    {
        lr.positionCount = 2;
        lr.SetPosition(1, targetPos);

        
    }


    public IEnumerator waitForSec()
    {
        yield return new WaitForSeconds(0.2f);
        lr.positionCount = 1;
        floatingCapsule.enabled = true;
        yield return new WaitForSeconds(coolDown);
        canGrapple = true;
    }

    public IEnumerator wait(RaycastHit hitInfo)
    {
        yield return new WaitForSeconds(0.1f);
        Grapple(hitInfo);
    }

}
