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

    private void Start()
    {
        image_Blind.enabled = true;
        image_Blind.DOFade(0f, 1.5f).OnComplete(() =>
        {
            image_Blind.enabled = false;
        });
    }
}
