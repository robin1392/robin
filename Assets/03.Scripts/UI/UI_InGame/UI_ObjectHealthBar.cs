using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using UnityEngine.UI;

public class UI_ObjectHealthBar : MonoBehaviour
{
    //public Canvas canvas;
    public static Camera camera_UI;

    private RectTransform rts_HPBar;
    private BaseStat bs;

    private void Awake()
    {
        rts_HPBar = transform.GetChild(0) as RectTransform;
        bs = GetComponentInParent<BaseStat>();
        camera_UI = CameraController.Get().camera_UI;
    }

    private void OnEnable()
    {
        rts_HPBar.gameObject.SetActive(true);
        if (rts_HPBar.parent == transform)
        {
            rts_HPBar.parent = WorldUIManager.Get().canvas_UnitHPBar.transform;
            rts_HPBar.localScale = Vector3.one;
            rts_HPBar.localRotation = Quaternion.identity;
        }
    }

    private void OnDisable()
    {
        if (rts_HPBar != null)
        {
            rts_HPBar.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        rts_HPBar.transform.position = transform.position;
    }
}
