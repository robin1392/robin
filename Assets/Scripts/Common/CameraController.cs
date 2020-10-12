using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : SingletonDestroy<CameraController>
    {
        private Camera camera;
        public Camera camera_UI;
        public Camera camera_Effect;
        public Camera camera_UI_Popup;

        private void Awake()
        {
            
            
            /*
            if (PhotonNetwork.IsConnected)
            {
                SetPosition(PhotonManager.Instance.playType != PLAY_TYPE.BATTLE || PhotonNetwork.IsMasterClient, height);
            }
            else
            {
                SetPosition(true, height);
            }
            */
        }

        public void Start()
        {
            camera = GetComponent<Camera>();

            var split = (Screen.height / (float)Screen.width - 1.777f);
            var height = Mathf.Lerp(-17.09f, -15.77f, split / 0.388f);


            if (NetworkManager.Get() != null && NetworkManager.Get().IsConnect())
            {
                SetPosition(NetworkManager.Get().playType != Global.PLAY_TYPE.BATTLE || NetworkManager.Get().IsMaster , height );
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
    }
}