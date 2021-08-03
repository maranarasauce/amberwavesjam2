using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Maranara.InputShell;

public class GrappleHook : MonoBehaviour
{
    public UIController gunUI;
    private Rigidbody playerRB;
    private InputManager inputManager;
    private FloatingCapsuleController floatingCapsule;
    private ConfigurableJoint joint;
    private LineRenderer lr;
    
    public ScreenShake shake;

    public bool CanGrapple { get => canGrapple; }

    private bool canGrapple;

    public AudioSource grappleSrc;
    public AudioClip launchClip;
    public AudioClip pullClip;
    public AudioClip reloadClip;

    [Header("Grapple Settings")]
    public float grappleMaxDistance;
    public float grappleForce;
    public float coolDown;
    public GameObject ropeTarget;


    public JointDrive newDrive;

    // Chromum wtf there's spaghetti on my screen
    // -parz

    public void Awake()
    {
        inputManager = InputManager.instance;
        playerRB = gameObject.GetComponent<Rigidbody>();
        floatingCapsule = gameObject.GetComponent<FloatingCapsuleController>();
        joint = gameObject.GetComponent<ConfigurableJoint>();
        lr = gameObject.GetComponent<LineRenderer>();
        newDrive = new JointDrive { maximumForce = 0f };

        canGrapple = true;
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
                shake.ShakeScreen(1f, 0.1f);
                DrawRope(hitInfo.point);
                StartCoroutine(WaitToGrapple(hitInfo));
            }
        }
    }

    public void Grapple(RaycastHit hitInfo)
    {
        canGrapple = false;
        floatingCapsule.enabled = false;
        joint.xDrive = newDrive;
        joint.yDrive = newDrive;
        joint.zDrive = newDrive;
        Vector3 pullForce = (hitInfo.point - transform.position) * grappleForce;
        float clamp = 60000f;
        pullForce.y = Mathf.Clamp(pullForce.y, -clamp, clamp);
        playerRB.AddForce(pullForce);
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
        shake.ShakeScreen(0.6f, 1f);
        lr.positionCount = 1;
        floatingCapsule.enabled = true;
        yield return new WaitForSeconds(coolDown);
        grappleSrc.clip = reloadClip;
        grappleSrc.Play();
        canGrapple = true;
        //gunUI.ToggleGrapple(false);
    }

    public IEnumerator WaitToGrapple(RaycastHit hitInfo)
    {
        gunUI.GrappleReset();
        //gunUI.ToggleGrapple(true);
        grappleSrc.clip = launchClip;
        grappleSrc.Play();
        yield return new WaitForSeconds(0.1f);
        grappleSrc.clip = pullClip;
        grappleSrc.Play();
        Grapple(hitInfo);
    }

}
