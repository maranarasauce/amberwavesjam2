using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maranara.InputShell
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager instance;
        public PlayerInput input;

        public void Awake()
        {
            instance = this;
            input = new PlayerInput();
        }
    }

}
