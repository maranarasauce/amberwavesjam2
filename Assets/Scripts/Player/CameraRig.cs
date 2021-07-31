using Maranara.InputShell;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maranara.SVR.Player
{
    

    public class CameraRig : MonoBehaviour
    {
        PlayerInput input;
        public Transform playerCamera;
        public Transform playerCameraDir;
        public Transform headHolder;
        public MeshRenderer blackOutRenderer;
        public Transform leftControllerOffset;
        public Transform rightControllerOffset;
        public Transform bodyToFollow;
        public Rigidbody rbToFollow;
        public FloatingCapsuleController controller;
        Vector3 LastCamPos;

        public float heightGoal;
        public bool skipHeightCheck;
        public float standingHeight;

        public GameObject heightCalibrationGraphic;
        private void Start()
        {
            input = InputManager.instance.input;
            rbToFollow.gameObject.SetActive(false);

            if (skipHeightCheck)
            {
                float diff = heightGoal / standingHeight;
                transform.localScale = Vector3.one * diff;

                rbToFollow.gameObject.SetActive(true);
            }
        }

        private void RegisterHeight()
        {
            standingHeight = playerCamera.transform.localPosition.y;
            float diff = heightGoal / standingHeight;
            transform.localScale = Vector3.one * diff;

            rbToFollow.gameObject.SetActive(true);
        }

        private void OnEnable()
        {
            input = InputManager.instance.input;

            input.buttonX.onStateDown += RegisterHeight;
        }

        private void OnDisable()
        {
            input.buttonX.onStateDown -= RegisterHeight;
        }

        private void Update()
        {
            Vector3 lockedDir = playerCamera.rotation.eulerAngles;
            lockedDir.x = 0;
            lockedDir.z = 0;
            playerCameraDir.transform.rotation = Quaternion.Euler(lockedDir);

            //Calculate camera's current velocity
            Vector3 cameraPosVelocity = ((playerCamera.localPosition) - (LastCamPos));

            //Set velocity on Y axis to 0, so ball doesnt go up infinitely
            cameraPosVelocity.y = 0;
            //Move locosphere the same amount the camera has moved, using the velocity value
            rbToFollow.MovePosition(rbToFollow.position + headHolder.TransformDirection(cameraPosVelocity));

            //Cancel out the camera's velocity by setting the local position of the camera's parent (HeadHolder) to the INVERSE of the camera's local position
            Vector3 mod = new Vector3(-playerCamera.localPosition.x, 0, -playerCamera.localPosition.z);
            playerCamera.parent.localPosition = mod;

            //register the camera's last position for velocity calculation
            LastCamPos = playerCamera.localPosition;
            playerCameraDir.position = playerCamera.transform.position;

            //Move the entire CameraRig's localposition to that of the locosphere's (may need to offset it on the y axis a little bit so you're actually on the floor instead of the middle of the locosphere)
            Vector3 locoPosition = bodyToFollow.transform.position;
            headHolder.position = locoPosition;

            //Floating Capsule Controller, lift or sink depending on how crouched the player is
            float heightRatio = (standingHeight / (playerCamera.localPosition.y * transform.localScale.y)) - 1f;
            heightRatio *= -1f;
            controller.rideHeightFloor = heightRatio;
            controller.ridecastFloor = heightRatio;
        }
    }
}
