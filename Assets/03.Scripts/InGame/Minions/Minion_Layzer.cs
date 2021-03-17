using System;
using System.Collections;
using System.Collections.Generic;
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
                if (i < _listTarget.Count && i < ActorProxy.diceScale && _listTarget[i] != null && _listTarget[i].isAlive)
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

        public override BaseStat SetTarget()
        {
            SetMultiTarget();
            
            if (_listTarget.Count == 0)
                return base.SetTarget();
            else
            {
                return _listTarget[0];
            }
        }

        public void SetMultiTarget()
        {
            List<BaseStat> beforeList = new List<BaseStat>(_listTarget);
            _attackedTarget = target;
            var cols = Physics.OverlapSphere(transform.position, range, targetLayer);
            _listTarget.Clear();
            for (int i = 0 ; i < cols.Length && _listTarget.Count < ActorProxy.diceScale; ++i)
            {
                var bs = cols[i].GetComponentInParent<BaseStat>();
                var m = bs as Minion;
                if (bs != null && bs.isAlive)
                {
                    if (m != null && m.isCloacking) continue;
                    
                    _listTarget.Add(bs);
                }
            }
            
            if(ActorProxy.isPlayingAI && _listTarget.Equals(beforeList) == false)
            {
                List<uint> list = new List<uint>();
                foreach (var baseStat in _listTarget)
                {
                    list.Add(baseStat.ActorProxy.NetId);
                }
                ActorProxy.SyncMultiTarget(ActorProxy.NetId, list.ToArray());
            }
        }

        public override IEnumerator Attack()
        {
            if (ActorProxy.isPlayingAI)
            {
                foreach (var baseStat in _listTarget)
                {
                    _attackedTarget = baseStat;
                    baseStat.ActorProxy.HitDamage(1f);
                }
                
                if (_attackedTarget != null && _attackedTarget.isAlive == false) _attackedTarget = null;
            }
            
            yield break;
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