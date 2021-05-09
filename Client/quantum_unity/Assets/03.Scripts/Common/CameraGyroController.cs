using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraGyroController : SingletonDestroy<CameraGyroController>
{
    public Transform ts_Target;
    
    private float _x;
    private float _y;
    private Vector3 originPos;
    private Quaternion originAngle;
    private bool isFade;

    void Start()
    {
        //Input.gyro.enabled = true;
        originPos = transform.position;
        originAngle = transform.localRotation;
        transform.position += new Vector3(0, 8f, 0);
        //transform.rotation = Quaternion.Euler(60f, 0, 0);
        transform.LookAt(ts_Target);
        transform.DOMove(originPos, 3f, false).SetEase(Ease.OutQuint).OnComplete(() =>
        {
            Input.gyro.enabled = true;
            originAngle = transform.localRotation;
        });
        //transform.DORotate(new Vector3(35f, 0, 0), 3f).SetEase(Ease.OutQuint);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (isFade == false)
        {
            if (Input.gyro.rotationRateUnbiased.x < 0.5f && Input.gyro.rotationRateUnbiased.x > -0.5f &&
                Input.gyro.rotationRateUnbiased.y < 0.5f && Input.gyro.rotationRateUnbiased.y > -0.5f)
            {
                // transform.localRotation =
                //     Quaternion.Lerp(transform.localRotation, originAngle, Time.deltaTime);
                transform.position = Vector3.Lerp(transform.position, originPos, Time.deltaTime);
            }
            else
            {
                _x = -Input.gyro.rotationRateUnbiased.x;
                _y = -Input.gyro.rotationRateUnbiased.y;

                _x = Mathf.Clamp(_x, -0.5f, 0.5f);
                _y = Mathf.Clamp(_y, -0.5f, 0.5f);
                //Debug.LogFormat("X:{0}, Y:{1}", _x, _y);
                // transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(39f + _x, _y, 0),
                //     Time.deltaTime);
                transform.position = Vector3.Lerp(transform.position,  originPos + new Vector3(_x, _y, 0), Time.deltaTime);
            }
        }
        transform.LookAt(ts_Target);
    }

    private void OnDestroy()
    {
        Input.gyro.enabled = false;
    }

    public void FocusIn()
    {
        isFade = true;
        transform.DOKill();
        Input.gyro.enabled = false;
        
        var targetPos = new Vector3(0, -995.51f, -7.38f);
        transform.DOMove(targetPos, 1f).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            //isFade = false;
            originAngle = transform.localRotation;
        });
    }

    public void FocusOut()
    {
        isFade = true;
        transform.DOMove(originPos, 1f).SetEase(Ease.OutQuart).OnComplete(() =>
        {
            Input.gyro.enabled = true;
            isFade = false;
            originAngle = transform.localRotation;
        });
    }
}
