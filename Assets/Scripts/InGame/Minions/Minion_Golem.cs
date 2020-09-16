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

            base.Attack();
            controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
        }

        public override BaseStat SetTarget()
        {
            return controller.targetPlayer;
        }

        public override void Death()
        {
            for (int i = 0; i < 2; i++)
            {
                // Spawn
                var m = controller.CreateMinion(pref_MiniGolem,
                    transform.position + Vector3.right * Random.Range(-0.5f, 0.5f) +
                    Vector3.forward * Random.Range(-0.5f, 0.5f), 1, 0);

                m.targetMoveType = DICE_MOVE_TYPE.GROUND;
                m.ChangeLayer(isBottomPlayer);
                m.power = effect;
                m.maxHealth = maxHealth * eyeLevel * 0.1f;
                m.attackSpeed = attackSpeed;
                m.moveSpeed = moveSpeed;
                m.range = range;
                m.eyeLevel = eyeLevel;
                m.upgradeLevel = upgradeLevel;
                m.Initialize(destroyCallback);
            }

            base.Death();
        }
    }
}