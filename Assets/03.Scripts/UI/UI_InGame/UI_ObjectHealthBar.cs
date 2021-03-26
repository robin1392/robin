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
        // if (camera_UI == null)
        // {
        //     camera_UI = FindObjectOfType<CameraController>().camera_UI;
        // }
    
        //canvas.worldCamera = camera_UI;

        rts_HPBar = transform.GetChild(0) as RectTransform;
        bs = GetComponentInParent<BaseStat>();
        //ts_OldParent = transform.parent;
        camera_UI = CameraController.Get().camera_UI;
    }

    private void OnEnable()
    {
        rts_HPBar.gameObject.SetActive(true);
        if (rts_HPBar.parent == transform)
        {
            rts_HPBar.SetParent(WorldUIManager.Get().canvas_UnitHPBar.transform, true);
            rts_HPBar.localRotation = Quaternion.identity;
            rts_HPBar.localScale = Vector3.one;
        }
    }

    private void OnDisable()
    {
        //rts_HPBar.parent = transform;
        if (rts_HPBar != null)
        {
            rts_HPBar.gameObject.SetActive(false);
        }
    }

    private void LateUpdate()
    {
        //transform.forward = camera_UI.transform.forward;
        rts_HPBar.anchoredPosition = camera_UI.WorldToScreenPoint(transform.position);
    }
}
