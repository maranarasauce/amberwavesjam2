using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateHardMode : MonoBehaviour
{
    public void Activate()
    {
        if (HardMode.instance != null)
        {
            return;
        }

        GameObject hardGO = new GameObject("HardMode");
        HardMode hard = hardGO.AddComponent<HardMode>();
        HardMode.instance = hard;

        GameManager.instance.SwapActiveScene(SceneIndex.Arena);
    }
}
