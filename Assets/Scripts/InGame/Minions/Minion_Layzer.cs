﻿using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace ED
{
    public class Minion_Layzer : Minion
    {
        public LineRenderer[] arrLineRenderer;

        private List<BaseStat> _listTarget = new List<BaseStat>();

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            for (int i = 0; i < arrLineRenderer.Length; i++)
            {
                if (i < _listTarget.Count && i < eyeLevel && _listTarget[i] != null && _listTarget[i].isAlive)
                {
                    arrLineRenderer[i].gameObject.SetActive(true);
                    arrLineRenderer[i].SetPositions(new Vector3[2] { shootingPos.position, _listTarget[i].hitPos.position });
                    arrLineRenderer[i].startColor = isMine ? Color.blue : Color.red;
                    arrLineRenderer[i].endColor = arrLineRenderer[i].startColor;
                }
                else
                {
                    arrLineRenderer[i].gameObject.SetActive(false);
                }
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
                _listTarget.Add(m);
                intList.Add(m.id);

                controller.AttackEnemyMinion(m.id, power, 0f);
            }
            
            controller.SendPlayer(RpcTarget.Others, E_PTDefine.PT_LAYZERTARGET, id, intList.ToArray());
        }

        public override void Death()
        {
            base.Death();

            for (int i = 0; i < arrLineRenderer.Length; i++)
            {
                arrLineRenderer[i].gameObject.SetActive(false);
            }
        }

        public void SetTargetList(int[] arr)
        {
            _listTarget.Clear();
            if (arr != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    _listTarget.Add(controller.targetPlayer.listMinion.Find(minion => minion.id == arr[i]));
                }
            }
        }
    }
}