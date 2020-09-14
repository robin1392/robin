using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace ED
{
    public class Minion_Layzer : Minion
    {
        public GameObject obj_LineStart;
        public LineRenderer[] arrLineRenderer;
        public GameObject[] arrObj_LineEnd;

        private List<BaseStat> _listTarget = new List<BaseStat>();

        private void LateUpdate()
        {
            bool isLayzerOn = false;
            for (int i = 0; i < arrLineRenderer.Length; i++)
            {
                if (i < _listTarget.Count && i < eyeLevel && _listTarget[i] != null && _listTarget[i].isAlive)
                {
                    isLayzerOn = true;
                    arrLineRenderer[i].gameObject.SetActive(true);
                    arrLineRenderer[i].SetPositions(new Vector3[2] { ts_ShootingPos.position, _listTarget[i].ts_HitPos.position });
                    arrObj_LineEnd[i].SetActive(true);
                    arrObj_LineEnd[i].transform.position = _listTarget[i].ts_HitPos.position;
                    //arrLineRenderer[i].startColor = isMine ? Color.blue : Color.red;
                    //arrLineRenderer[i].endColor = arrLineRenderer[i].startColor;
                }
                else
                {
                    arrLineRenderer[i].gameObject.SetActive(false);
                    arrObj_LineEnd[i].SetActive(false);
                }
            }

            obj_LineStart.SetActive(isLayzerOn);
        }

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);
            
            _listTarget.Clear();
            
            obj_LineStart.SetActive(false);
            for (int i = 0; i < arrLineRenderer.Length; i++)
            {
                arrLineRenderer[i].gameObject.SetActive(false);
                arrObj_LineEnd[i].SetActive(false);
            }
        }

        public override void Attack()
        {
            var cols = Physics.OverlapSphere(transform.position, range, targetLayer);
            _listTarget.Clear();
            List<int> intList = new List<int>();
            foreach (var col in cols)
            {
                var m = col.GetComponentInParent<BaseStat>();
                if (m != null && m.isAlive)
                {
                    _listTarget.Add(m);
                    intList.Add(m.id);

                    controller.AttackEnemyMinion(m.id, power, 0f);
                }
            }

            //if (PhotonNetwork.IsConnected && isMine)
            if(InGameManager.Get().IsNetwork() && isMine)
            {
                //controller.SendPlayer(RpcTarget.Others, E_PTDefine.PT_LAYZERTARGET,id, intList.Count > 0 ? intList.ToArray() : null);
                controller.ActionLayzer(id, intList.Count > 0 ? intList.ToArray() : null);
            }
        }

        public override void Death()
        {
            base.Death();

            obj_LineStart.SetActive(false);
            for (int i = 0; i < arrLineRenderer.Length; i++)
            {
                arrLineRenderer[i].gameObject.SetActive(false);
                arrObj_LineEnd[i].SetActive(false);
            }
        }

        public override void Sturn(float duration)
        {
            base.Sturn(duration);
            
            obj_LineStart.SetActive(false);
            for (int i = 0; i < arrLineRenderer.Length; i++)
            {
                arrLineRenderer[i].gameObject.SetActive(false);
                arrObj_LineEnd[i].SetActive(false);
            }
        }

        public void SetTargetList(int[] arr)
        {
            _listTarget.Clear();
            
            if (arr != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    if (arr[i] == 0) _listTarget.Add(controller.targetPlayer);
                    else _listTarget.Add(controller.targetPlayer.listMinion.Find(minion => minion.id == arr[i]));
                }
            }
        }
    }
}