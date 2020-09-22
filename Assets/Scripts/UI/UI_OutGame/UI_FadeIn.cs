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
    public float fadeinTime = 2f;

    private void Start()
    {
        if (image_Blind != null)
        {
            image_Blind.enabled = true;
            image_Blind.DOFade(0f, fadeinTime).SetEase(Ease.InCirc).OnComplete(() =>
            {
                image_Blind.enabled = false;
            });
        }
    }
}
