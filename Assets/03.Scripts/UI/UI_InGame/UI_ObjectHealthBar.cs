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
    private BaseEntity bs;

    private void Awake()
    {
        rts_HPBar = transform.GetChild(0) as RectTransform;
        bs = GetComponentInParent<BaseEntity>();
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
