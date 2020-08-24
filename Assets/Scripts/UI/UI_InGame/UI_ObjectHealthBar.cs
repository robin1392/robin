using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using UnityEngine.UI;

public class UI_ObjectHealthBar : MonoBehaviour
{
    public Canvas canvas;
    public Camera camera_UI;

    private void Awake()
    {
        camera_UI = FindObjectOfType<CameraController>().camera_UI;
        canvas.worldCamera = camera_UI;
    }

    private void LateUpdate()
    {
        transform.forward = camera_UI.transform.forward;
    }
}
