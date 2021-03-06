using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_Indicator : MonoBehaviour
{
    public Image image_Blind;
    public float fadeTime = 2f;
    public GameObject obj_Indicator;

    private void OnEnable()
    {
        image_Blind.enabled = true;
        var c = image_Blind.color;
        c.a = 0;
        image_Blind.color = c;
        
        if (image_Blind != null)
        {
            image_Blind.DOFade(0.6f, fadeTime).SetDelay(0.2f).SetUpdate(true);
        }

        //Invoke("OnIndicator", fadeTime);
        StartCoroutine(OnIndicator());
    }

    private void OnDisable()
    {
        CancelInvoke("OnIndicator");
        image_Blind.enabled = false;
        obj_Indicator.SetActive(false);
    }

    private IEnumerator OnIndicator()
    {
        yield return new WaitForSecondsRealtime(fadeTime);
        
        if (obj_Indicator != null)
        {
            obj_Indicator.transform.localScale = Vector3.zero;
            obj_Indicator.SetActive(true);
            obj_Indicator.transform.DOScale(Vector3.one, 0.5f).SetDelay(0.25f).SetEase(Ease.InCirc).SetUpdate(true);
        }
    }
}
