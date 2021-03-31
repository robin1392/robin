using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;

public class Minion_Wind : Minion
{
    public GameObject pref_Bullet;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();

        _animationEvent.event_FireArrow -= FireArrow;
        _animationEvent.event_FireArrow += FireArrow;
    }

    public void FireArrow()
    {
        //TODO: 빼도 되지 않을까? 고민해보자
        if (target == null || IsTargetInnerRange() == false)
        {
            animator.SetTrigger(AnimationHash.Idle);
            return;
        }

        if (ActorProxy.isPlayingAI)
        {
            ActorProxy.FireBulletWithRelay(E_BulletType.WIND_BULLET, target, power, 10f);
        }
    }
}
