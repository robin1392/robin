using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_FadeIn : MonoBehaviour
{
    public Image image_Blind;
    public float fadeTime = 2f;
    public bool isFadeOut;

    private void OnEnable()
    {
        if (image_Blind != null)
        {
            image_Blind.enabled = true;
            var c = image_Blind.color;
            c.a = isFadeOut ? 0 : 1f;
            image_Blind.color = c;
            image_Blind.DOFade(isFadeOut ? 0.8f : 0f, fadeTime).SetEase(Ease.InCirc).OnComplete(() =>
            {
                if (isFadeOut == false) image_Blind.enabled = false;
            });
        }
    }

    private void OnDisable()
    {
        if (image_Blind) image_Blind.enabled = false;
    }
}
