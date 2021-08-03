using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public Camera cam;
    
    float shakeCap;
    float shakeCapStart;
    public float shakeDecay;
    float shakeDecayStart;

    public void ShakeScreen(float intensity, float duration)
        {
            //shake cap and shake decay are additive in-case multiple shake events occur at once.
            shakeCap += intensity;
            shakeCapStart = shakeCap;
            shakeDecay += duration;
            shakeDecayStart = shakeDecay;
            StartCoroutine(CameraRoutine());
        }

        IEnumerator CameraRoutine()
        {
            while (shakeDecay > 0.01f)
            {
                if(GameManager.inst.IsPaused) { yield return null; }

                Vector3 rotationAmount;
                rotationAmount = UnityEngine.Random.insideUnitSphere * shakeCap;
                rotationAmount.z = 0;
                var shakePerc = shakeDecay / shakeDecayStart;
                shakeCap = shakeCapStart * shakePerc;
                shakeDecay = Mathf.Lerp(shakeDecay, 0, Time.deltaTime * 5f);
                cam.transform.localRotation = cam.transform.localRotation * Quaternion.Euler(rotationAmount);
                yield return null;
            }
        }
}
