using Maranara.SVR.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace Maranara.InputShell
{
    public class PCInputPlugin : MonoBehaviour
    {
        void Awake()
        {
            mainCam = Camera.main.transform;
            Camera cam = mainCam.GetComponent<Camera>();
            cam.fieldOfView = 96;
            mainCam.localPosition = new Vector3(0, height, 0);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        InputManager manager;

        PlayerInput input;
        Transform mainCam;
        public float height;
        private void Start()
        {
            manager = InputManager.instance;
            input = manager.input;

            
        }

        public float Sensitivity
        {
            get { return sensitivity; }
            set { sensitivity = value; }
        }
        [Range(0.1f, 9f)] [SerializeField] float sensitivity = 2f;
        [Tooltip("Limits vertical camera rotation. Prevents the flipping that happens when rotation goes above 90.")]
        [Range(0f, 90f)] [SerializeField] float yRotationLimit = 88f;

        Vector2 rotation = Vector2.zero;
        const string xAxis = "Mouse X"; //Strings in direct code generate garbage, storing and re-using them creates no garbage
        const string yAxis = "Mouse Y";
        void Update()
        {
            Vector2 joystick = new Vector2(Input.GetAxis("Horizontal") * 2, Input.GetAxis("Vertical") * 2);
            input.leftJoystick.Update(joystick);
            input.buttonA.Update(Input.GetKey(KeyCode.Space));

            bool crouching = Input.GetKey(KeyCode.LeftShift);
            int crouchInt = (crouching ? -1 : 0);

            Vector2 crouch = new Vector2(0, crouchInt);

            input.rightJoystick.Update(crouch);

            rotation.x += Input.GetAxis(xAxis) * sensitivity;
            rotation.y += Input.GetAxis(yAxis) * sensitivity;
            rotation.y = Mathf.Clamp(rotation.y, -yRotationLimit, yRotationLimit);
            var xQuat = Quaternion.AngleAxis(rotation.x, Vector3.up);
            var yQuat = Quaternion.AngleAxis(rotation.y, Vector3.left);

            mainCam.localRotation = xQuat * yQuat;

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                Cursor.visible = !Cursor.visible;
                if (Cursor.visible)
                    Cursor.lockState = CursorLockMode.None;
                else Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

}
