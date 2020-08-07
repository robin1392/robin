using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Golem : Minion
    {
        public GameObject pref_MiniGolem;

        public override void Attack()
        {
            if (target == null) return;

            if (PhotonNetwork.IsConnected && isMine)
            {
                base.Attack();
                //controller.photonView.RPC("SetMinionAnimationTrigger", RpcTarget.All, id, "Attack");
                controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                base.Attack();
                animator.SetTrigger(_animatorHashAttack);
            }
        }

        public override BaseStat SetTarget()
        {
            return controller.targetPlayer;
        }

        public override void Death()
        {
            for (int i = 0; i < eyeLevel - 1; i++)
            {
                // Spawn
                var m = controller.CreateMinion(pref_MiniGolem, transform.position, 1, 0);
                m.targetMoveType = DICE_MOVE_TYPE.GROUND;
                m.ChangeLayer(isBottomPlayer);
                m.power = power;
                m.maxHealth = maxHealth * eyeLevel * 0.1f;
                m.attackSpeed = attackSpeed;
                m.moveSpeed = moveSpeed;
                m.range = range;
                m.eyeLevel = eyeLevel;
                m.upgradeLevel = upgradeLevel;
                m.Initialize(destroyCallback);
                controller.listMinion.Add(m);
            }

            base.Death();
        }
    }
}