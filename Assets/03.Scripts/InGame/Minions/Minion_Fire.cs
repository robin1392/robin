using System.Collections;
using System.Collections.Generic;
using ED;
using Mirage;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using UnityEngine;

public class Minion_Fire : Minion
{
    public GameObject pref_FireBombEffect;

    protected override void Awake()
    {
        base.Awake();
        
        PoolManager.Get().AddPool(pref_FireBombEffect, 1);
    }

    public override void Initialize()
    {
        base.Initialize();

        _animationEvent.event_Attack -= AttackBomb;
        _animationEvent.event_Attack += AttackBomb;
    }

    public void AttackBomb()
    {
        if (ActorProxy.isPlayingAI == false)
        {
            return;
        }
        
        if (Random.value < 0.05f)
        {
            Vector3 pos = transform.position;
            ActorProxy.PlayEffectWithRelay("Effect_FireBomb", pos);
            
            var cols = Physics.OverlapSphere(pos, (ActorProxy as DiceActorProxy).diceInfo.effectRangeValue, targetLayer);
            foreach (var col in cols)
            {
                var bs = col.GetComponentInParent<BaseEntity>();
                if (bs != null && bs.CanBeTarget())
                {
                    bs.ActorProxy?.HitDamage(ActorProxy.effect);
                }
            }
        }
    }
}