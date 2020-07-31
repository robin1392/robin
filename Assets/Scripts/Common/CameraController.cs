using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        private Camera camera;

        private void Awake()
        {
            camera = GetComponent<Camera>();

            var split = (Screen.height / (float)Screen.width - 1.777f);
            var height = Mathf.Lerp(-17.09f, -15.77f, split / 0.388f);

            if (PhotonNetwork.IsConnected)
            {
                SetPosition(PhotonManager.Instance.playType != PLAY_TYPE.BATTLE || PhotonNetwork.IsMasterClient,
                    height);
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