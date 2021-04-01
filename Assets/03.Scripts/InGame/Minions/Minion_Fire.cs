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
    
    private bool isBomb;

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

    public override IEnumerator Attack()
    {
        if (Random.value < 0.05f)
        {
            var action = new FireAction();
            RunningAction = action;
            yield return action.ActionWithSync(ActorProxy);
            RunningAction = null;
        }

        yield return base.Attack();
    }

    public void AttackBomb()
    {
        if (isBomb)
        {
            if (target == null || target.ActorProxy == null)
            {
                return;
            }
            
            SetBomb(false);
            // 폭발 이펙트
            PoolManager.Get().ActivateObject(pref_FireBombEffect.name, target.ts_HitPos.position);
            
            // 데미지
            if (ActorProxy.isPlayingAI)
            {
                target.ActorProxy.HitDamage(effect);
            }
        }
    }

    public void SetBomb(bool b)
    {
        isBomb = b;
    }
}

public class FireAction : SyncActionWithoutTarget
{
    public override IEnumerator Action(ActorProxy actorProxy)
    {
        // 트리거 On
        var fire = (Minion_Fire) actorProxy.baseStat;
        fire.SetBomb(true);
        
        yield break;
    }

    public override void OnActionCancel(ActorProxy actorProxy)
    {
        // 트리거 Off
        var fire = (Minion_Fire) actorProxy.baseStat;
        fire.SetBomb(false);
        
        base.OnActionCancel(actorProxy);
    }
}
