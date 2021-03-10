using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Minion_Arbiter : Minion
    {
        public float bulletMoveSpeed = 6f;

        //
        public GameObject pref_Bullet;
        private List<Minion> listCloaking = new List<Minion>();

        protected override void Awake()
        {
            base.Awake();
            
            PoolManager.instance.AddPool(pref_Bullet, 1);
        }

        protected override void Start()
        {
            base.Start();

            var ae = animator.GetComponentInChildren<MinionAnimationEvent>();
            ae.event_FireArrow += FireArrow;
        }

        public override void Death()
        {
            foreach (var minion in listCloaking)
            {
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONCLOACKING, minion.id, false);
                controller.ActionCloacking(minion.id, false);
            }
            listCloaking.Clear();
            
            base.Death();
        }

        public void Skill()
        {
            var cols = Physics.OverlapSphere(transform.position, range, friendlyLayer);
            List<Minion> tempList = new List<Minion>();
            foreach (var col in cols)
            {
                var m = col.GetComponentInParent<Minion>();
                if (m != null)
                {
                    tempList.Add(m);
                }
            }

            foreach (var temp in tempList)
            {
                if (listCloaking.Contains(temp) == false && temp.GetType() != typeof(Minion_Arbiter))
                {
                    listCloaking.Add(temp);
                    //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONCLOACKING, temp.id, true);
                    controller.ActionCloacking(temp.id, true);
                }
            }

            List<Minion> removeList = new List<Minion>();
            foreach (var minion in listCloaking)
            {
                if (tempList.Contains(minion) == false)
                {
                    removeList.Add(minion);
                }
            }

            foreach (var minion in removeList)
            {
                listCloaking.Remove(minion);
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONCLOACKING, minion.id, false);
                controller.ActionCloacking(minion.id, false);
            }
        }
        
        public void FireArrow()
        {
            if (target == null || IsTargetInnerRange() == false)
            {
                animator.SetTrigger(_animatorHashIdle);
                SetControllEnable(true);
                return;
            }

            if (ActorProxy.isPlayingAI)
            {
                ActorProxy.FireBulletWithRelay(E_BulletType.ARBITER , target, power, bulletMoveSpeed);
            }
        }
    }
}
