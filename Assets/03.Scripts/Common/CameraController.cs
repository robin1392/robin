﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace ED
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : SingletonDestroy<CameraController>
    {
        private Camera camera;
        public Camera camera_UI;
        public Camera camera_Effect;
        public Camera camera_UI_Popup;

        public bool isMoveimmit;
        [SerializeField]
        private Vector3[] arrPos;
        [SerializeField]
        private Vector3[] arrRot;

        public void Start()
        {
            camera = GetComponent<Camera>();

            var split = (Screen.height / (float)Screen.width - 1.777f);
            var height = Mathf.Lerp(-17.09f, -15.77f, split / 0.388f);


            if (NetworkManager.Get() != null && NetworkManager.Get().IsConnect())
            {
                switch (NetworkManager.Get().playType)
                {
                    case Global.PLAY_TYPE.BATTLE:
                        SetPosition(NetworkManager.Get().IsMaster == true, height );
                        break;
                    case Global.PLAY_TYPE.COOP:
                        SetPosition(true, height );
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                SetPosition(true, height);
            }
        }
        

        private void SetPosition(bool isOrigin, float height)
        {
            transform.localPosition = new Vector3(0, height, 0.39f);
            if (!isOrigin)
            {
                transform.parent.rotation = Quaternion.Euler(90f, 0, 180f);
            }
        }

        public void ChangeView(int num)
        {
            float duration = isMoveimmit ? 0 : 1f;
            transform.DOLocalMove(arrPos[num], duration);
            transform.DOLocalRotate(arrRot[num], duration);

            switch(num)
            {
            case 0:
                camera.orthographic = true;
                camera.orthographicSize = 11.7f;
                break;
            case 1:
                camera.orthographic = false;
                break;
            case 2:
                camera.orthographic = true;
                camera.orthographicSize = 14f;
                break;
            case 3:
                camera.orthographic = false;
                break;
            }
        }
    }
}