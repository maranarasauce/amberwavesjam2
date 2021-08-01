using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxController : MonoBehaviour
{
    public int rotationSpeed = 5;

    private Material skybox;

    private void Start()
    {
        skybox = RenderSettings.skybox;
    }

    private void Update()
    {
        var currentRot = skybox.GetFloat("_Rotation");

        currentRot = Mathf.Repeat(currentRot + rotationSpeed * Time.deltaTime, 360);

        skybox.SetFloat("_Rotation", currentRot);
    }
}
