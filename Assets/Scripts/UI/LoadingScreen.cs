using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public float fadeTime = 1f;
    public Image background;


    public void FadeIn()
    {
        SetBG(0);
        StartCoroutine(DoFade(true));
    }

    public void FadeOut()
    {
        SetBG(1);
        StartCoroutine(DoFade(false));
    }

    private IEnumerator DoFade(bool fadeIn)
    {
        int goal = fadeIn ? 1 : 0;
        Color c = background.color;

        while(c.a != goal)
        {
            c.a = Mathf.MoveTowards(c.a, goal, Time.deltaTime / fadeTime);
            background.color = c;

            yield return null;
        }


        //for (float t = 0f; t < 1f; t += Time.deltaTime / fadeTime)
        //{
        //    c.a = Mathf.Lerp(c.a, goal, t);
        //    background.color = c;
            
        //    yield return null;
        //}

        if (!fadeIn)
            gameObject.SetActive(false);
    }
    
    private void SetBG(float x)
    {
        var c = background.color;
        c.a = x;
        background.color = c;
    }
}
