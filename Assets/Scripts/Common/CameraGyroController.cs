﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraGyroController : SingletonDestroy<CameraGyroController>
{
    private float _x;
    private float _y;
    private Vector3 originPos;
    private Quaternion originAngle;
    
    void Start()
    {
        //Input.gyro.enabled = true;
        originPos = transform.position;
        originAngle = transform.localRotation;
        transform.position += new Vector3(0, 8f, 0);
        transform.rotation = Quaternion.Euler(60f, 0, 0);
        transform.DOMove(originPos, 3f, false).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            Input.gyro.enabled = true;
        });
        transform.DORotate(new Vector3(35f, 0, 0), 3f).SetEase(Ease.OutQuint);
    }

    // Update is called once per frame
    void Update()
    {
        //transform.Rotate(-Input.gyro.rotationRateUnbiased.x, -Input.gyro.rotationRateUnbiased.y, 0);

        if (Input.gyro.rotationRateUnbiased.x < 0.5f && Input.gyro.rotationRateUnbiased.x > -0.5f &&
            Input.gyro.rotationRateUnbiased.y < 0.5f && Input.gyro.rotationRateUnbiased.y > -0.5f)
        {
            transform.localRotation =
                Quaternion.Lerp(transform.localRotation, originAngle, Time.deltaTime);
        }
        else
        {
            _x = -Input.gyro.rotationRateUnbiased.x * 2f;
            _y = -Input.gyro.rotationRateUnbiased.y * 2f;
            
            _x = Mathf.Clamp(_x, -5f, 5f);
            _y = Mathf.Clamp(_y, -5f, 5f);
            //Debug.LogFormat("X:{0}, Y:{1}", _x, _y);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(39f + _x, _y, 0),
                Time.deltaTime);
        }
    }

    private void OnDestroy()
    {
        Input.gyro.enabled = false;
    }

    public void FocusIn()
    {
        transform.DOKill();
        Input.gyro.enabled = false;
        
        var targetPos = originPos + transform.forward * 5f;
        transform.DOMove(targetPos, 1f).SetEase(Ease.OutQuart);
    }

    public void FocusOut()
    {
        transform.DOMove(originPos, 1f).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            Input.gyro.enabled = true;
        });
    }
}
