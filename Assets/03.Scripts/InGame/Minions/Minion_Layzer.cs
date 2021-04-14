using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;

namespace ED
{
    public class Minion_Layzer : Minion
    {
        public GameObject obj_LineStart;
        public LineRenderer[] arrLineRenderer;
        public GameObject[] arrObj_LineEnd;

        private HashSet<BaseEntity> _listTarget = new HashSet<BaseEntity>();

        private void LateUpdate()
        {
            bool isLayzerOn = false;
            for (int index = 0; index < arrLineRenderer.Length; index++)
            {
                arrLineRenderer[index].gameObject.SetActive(false);
                arrObj_LineEnd[index].SetActive(false);
            }

            int i = 0;
            foreach (var target in _listTarget)
            {
                if (target != null && target.isAlive)
                {
                    isLayzerOn = true;
                    arrLineRenderer[i].gameObject.SetActive(true);
                    arrLineRenderer[i].SetPositions(new Vector3[2] { ts_ShootingPos.position, target.ts_HitPos.position });
                    arrObj_LineEnd[i].SetActive(true);
                    arrObj_LineEnd[i++].transform.position = target.ts_HitPos.position;
                }
            }

            obj_LineStart.SetActive(isLayzerOn);
        }

        public override void Initialize()
        {
            base.Initialize();
            
            _listTarget.Clear();
            
            obj_LineStart.SetActive(false);
            for (int i = 0; i < arrLineRenderer.Length; i++)
            {
                arrLineRenderer[i].gameObject.SetActive(false);
                arrObj_LineEnd[i].SetActive(false);
            }
        }

        public override BaseEntity SetTarget()
        {
            SetMultiTarget();
            
            if (_listTarget.Count == 0)
                return base.SetTarget();
            else
            {
                return _listTarget.First();
            }
        }

        public void SetMultiTarget()
        {
            _attackedTarget = target;
            var cols = Physics.OverlapSphere(transform.position, range, targetLayer);
            var bsCols = cols.Select(c => c.GetComponentInParent<BaseEntity>()).Where(bs => bs != null && bs.isAlive);
            
            var n = bsCols.Take(ActorProxy.diceScale + 1).ToHashSet();
            if (!n.SetEquals(_listTarget))
            {
                ActorProxy.SyncMultiTarget(ActorProxy.Client.Player.Identity.NetId, n.Select(d => d.id).ToArray());
            }

            _listTarget = n;
        }

        public override IEnumerator Attack()
        {
            if (ActorProxy.isPlayingAI)
            {
                foreach (var baseStat in _listTarget)
                {
                    _attackedTarget = baseStat;
                    baseStat.ActorProxy.HitDamage(power);
                }
                
                if (_attackedTarget != null && _attackedTarget.isAlive == false) _attackedTarget = null;
            }
            
            yield break;
        }

        public void SetTargetList(uint[] arr)
        {
            _listTarget.Clear();
            
            if (arr != null)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    _listTarget.Add(ActorProxy.GetBaseStatWithNetId(arr[i]));
                }
            }
        }
    }
}