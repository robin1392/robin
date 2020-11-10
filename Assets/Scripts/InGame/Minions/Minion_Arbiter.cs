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

        public override void Attack()
        {
            if (target == null || target.isAlive == false || IsTargetInnerRange() == false) return;
            
            //if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            if( (InGameManager.IsNetwork && isMine) || InGameManager.IsNetwork == false || controller.isPlayingAI )
            {
                base.Attack();
                //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
                controller.MinionAniTrigger(id, "Attack", target.id);
            }
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
        //
        // private void OnTriggerEnter(Collider other)
        // {
        //     if (IsFriendlyLayer(other.gameObject))
        //     {
        //         var m = other.gameObject.GetComponentInParent<Minion>();
        //         if (m != null && listCloaking.Contains(m) == false && m.GetType() != typeof(Minion_Arbiter))
        //         {
        //             listCloaking.Add(m);
        //             controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONCLOACKING, m.id, true);
        //         }
        //     }
        // }
        //
        // private void OnTriggerExit(Collider other)
        // {
        //     if (IsFriendlyLayer(other.gameObject))
        //     {
        //         var m = other.gameObject.GetComponentInParent<Minion>();
        //         if (m != null && listCloaking.Contains(m))
        //         {
        //             listCloaking.Remove(m);
        //             controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONCLOACKING, m.id, false);
        //         }
        //     }
        // }
        //
        
        public void FireArrow()
        {

            if (target == null || IsTargetInnerRange() == false)
            {
                animator.SetTrigger(_animatorHashIdle);
                isAttacking = false;
                SetControllEnable(true);
                return;
            }
            
            //if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            if( (InGameManager.IsNetwork && isMine) || InGameManager.IsNetwork == false || controller.isPlayingAI )
            {
                controller.ActionFireBullet(E_BulletType.ARBITER , id, target.id, power, bulletMoveSpeed);
            }
           
            /*if (PhotonNetwork.IsConnected && isMine)
            {
                //controller.photonView.RPC("FireArrow", RpcTarget.All, shootingPos.position, target.id, power);
                controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREBULLET, E_BulletType.ARBITER, ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                controller.FireBullet(E_BulletType.ARBITER, ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
            }*/
            
        }
    }
}
