﻿using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// This script fades in when loaded, and fades out to scene load when called
public class SceneFader : MonoBehaviour
{
    public Image img;
    public AnimationCurve curve;

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    public void FadeTo(string scene)
    {
        StartCoroutine(FadeOut(scene));
    }

    IEnumerator FadeIn()
    {
        float t = 1f;

        while (t > 0f)
        {
            t -= Time.fixedDeltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }
    }

    IEnumerator FadeOut(string scene)
    {
        float t = 0f;
        
        while (t < 1f)
        {
            t += Time.fixedDeltaTime;
            float a = curve.Evaluate(t);
            img.color = new Color(0f, 0f, 0f, a);
            yield return null;
        }

        SceneManager.LoadScene(scene);
    }
}
