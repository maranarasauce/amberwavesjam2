using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    public int rotationSpeed = 5;

    private void Update()
    {
        if (RenderSettings.skybox != null)
        {
            var currentRot = RenderSettings.skybox.GetFloat("_Rotation");

            currentRot = Mathf.Repeat(currentRot + rotationSpeed * Time.deltaTime, 360);

            RenderSettings.skybox.SetFloat("_Rotation", currentRot);
        }
    }
}
