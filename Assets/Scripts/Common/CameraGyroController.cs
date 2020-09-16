using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGyroController : MonoBehaviour
{
    private float _x;
    private float _y;
    private Quaternion originAngle;
    
    void Start()
    {
        Input.gyro.enabled = true;
        originAngle = transform.localRotation;
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
            _x = -Input.gyro.rotationRateUnbiased.x;
            _y = -Input.gyro.rotationRateUnbiased.y;
            
            //_x = Mathf.Clamp(_x, -5f, 5f);
            //_y = Mathf.Clamp(_y, -5f, 5f);
            //Debug.LogFormat("X:{0}, Y:{1}", _x, _y);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(-15f + _x, _y, 0),
                Time.deltaTime);
        }
    }

    private void OnDestroy()
    {
        Input.gyro.enabled = false;
    }
}
