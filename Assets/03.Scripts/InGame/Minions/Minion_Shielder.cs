using System.Collections;
using System.Collections.Generic;
using Mirage;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using UnityEngine;

namespace ED
{
    public class Minion_Shielder : Minion
    {
        [Header("AudioClip")]
        public AudioClip clip_ShieldMode;
        
        private float skillCastedTime;
        

        public override void Initialize()
        {
            base.Initialize();
            skillCastedTime = -effectCooltime;
        }

        public void Skill()
        {
            if (_spawnedTime >= skillCastedTime + effectCooltime)
            {
                skillCastedTime = _spawnedTime;
                ActorProxy.AddBuff(BuffInfos.HalfDamage, effectDuration);

                var action = new ShielderAction();
                RunningAction = action;
                RunLocalAction(action.ActionWithSync(ActorProxy), true);
            }
        }

      
        public override void OnHitDamageOnClient(float damage, DamageType damageType)
        {
            Skill();
        }

        public override BaseEntity SetTarget()
        {
            return ActorProxy.GetEnemyTowerOrBossEgg();
        }
    }
}
