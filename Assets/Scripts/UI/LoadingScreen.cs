using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    public float fadeTime = 1f;
    public Image background;
    public GameObject text;

    // Some of this also feels stupid
    // - parz

    private void Start()
    {
        GameManager.instance.OnLoadStart += FadeIn;
        GameManager.instance.OnLoadEnd   += FadeOut;
    }

    public void FadeIn()
    {
        SetBG(0);
        StartCoroutine(DoFade(true));
    }

    public void FadeOut()
    {
        SetBG(1);
        ShowLoadingAnim(false);
        StartCoroutine(DoFade(false));
    }

    private IEnumerator DoFade(bool fadeIn)
    {
        int goal = fadeIn ? 1 : 0;
        Color c = background.color;

        while(c.a != goal)
        {
            c.a = Mathf.MoveTowards(c.a, goal, Time.unscaledDeltaTime / fadeTime);
            background.color = c;

            yield return null;
        }

        if (fadeIn)
            ShowLoadingAnim(true);
    }

    public void ShowLoadingAnim(bool b)
    {
        text.SetActive(b);
    }

    private void SetBG(float x)
    {
        var c = background.color;
        c.a = x;
        background.color = c;
    }
}
