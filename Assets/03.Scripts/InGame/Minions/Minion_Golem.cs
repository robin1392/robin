using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ED
{
    public class Minion_Golem : Minion
    {
        public GameObject pref_MiniGolem;

        [Header("AudioClip")]
        public AudioClip clip_Attack;
        public AudioClip clip_Exposion;

        public override IEnumerator Attack()
        {
            SoundManager.instance?.Play(clip_Attack);
            yield return base.Attack();
        }

        public override BaseStat SetTarget()
        {
            return ActorProxy.GetEnemyTower();
        }

        public override void Death()
        {
            SoundManager.instance?.Play(clip_Exposion);
            
            for (int i = 0; i < 2; i++)
            {
                // Spawn
                var m = controller.CreateMinion(pref_MiniGolem,
                    transform.position + Vector3.right * Random.Range(-0.5f, 0.5f) + Vector3.forward * Random.Range(-0.5f, 0.5f));

                m.targetMoveType = DICE_MOVE_TYPE.GROUND;
                m.ChangeLayer(isBottomPlayer);
                m.power = effect;
                // m.maxHealth = maxHealth * eyeLevel * 0.1f;
                m.attackSpeed = attackSpeed;
                m.moveSpeed = moveSpeed;
                // m.range = range;
                m.eyeLevel = eyeLevel;
                m.ingameUpgradeLevel = ingameUpgradeLevel;
                m.Initialize(destroyCallback);
            }

            base.Death();
        }
    }
}