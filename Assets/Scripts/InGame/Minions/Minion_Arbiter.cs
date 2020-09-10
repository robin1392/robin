using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Arbiter : Minion
    {
        private List<Minion> listCloaking = new List<Minion>();
        
        public override void Attack()
        {
            if (target == null) return;
            
            //if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            //{
                base.Attack();
                //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
                controller.MinionAniTrigger(id, "Attack");
            //}
        }

        public override void Death()
        {
            foreach (var minion in listCloaking)
            {
                controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONCLOACKING, minion.id, false);
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
                    controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONCLOACKING, temp.id, true);
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
                controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONCLOACKING, minion.id, false);
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
        public void FireSpear()
        {
            if (target == null) return;
            
            if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            {
                controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIRESPEAR , ts_ShootingPos.position, target.id, power);
            }
        }
        
        public void FireArrow()
        {
            if (target == null) return;
            
            if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            {
                controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREARROW , ts_ShootingPos.position, target.id, power);
            }
        }
    }
}
