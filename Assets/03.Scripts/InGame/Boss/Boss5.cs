using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ED;
using ED.Boss;
using Microsoft.Win32.SafeHandles;
using RandomWarsProtocol;
using UnityEngine;

public class Boss5 : BossBase
{
    [Header("Effect")]
    public GameObject obj_Attack;
    public GameObject obj_AttackHit;
    public GameObject obj_Skill;
    public GameObject obj_SkillHit;
    
    private float _skillCastedTime;

    protected override void Start()
    {
        base.Start();
        
        PoolManager.Get().AddPool(obj_Attack, 3);
        PoolManager.Get().AddPool(obj_AttackHit, 3);
        PoolManager.Get().AddPool(obj_Skill, 20);
        PoolManager.Get().AddPool(obj_SkillHit, 20);
        if (_animationEvent == null) _animationEvent = GetComponentInChildren<MinionAnimationEvent>();
    }

    public override void Initialize()
    {
        base.Initialize();
        _skillCastedTime = -effectCooltime;
        if (_animationEvent == null) _animationEvent = GetComponentInChildren<MinionAnimationEvent>();
        _animationEvent.event_Attack -= Callback_OnAttackAnimationEvent;
        _animationEvent.event_Attack += Callback_OnAttackAnimationEvent;
    }

    public void Callback_OnAttackAnimationEvent()
    {
        if (target != null)
        {
            if (ActorProxy.isPlayingAI)
            {
                // controller.NetSendPlayer(GameProtocol.SEND_MESSAGE_PARAM1_RELAY, id, E_ActionSendMessage.FireBullet, target.id);
            }
            
            var attack = PoolManager.Get().ActivateObject(obj_Attack.name, ts_ShootingPos.position);
            if (attack != null)
            {
                var ts_hit = target.ts_HitPos;
                if (ts_hit == null) ts_hit = target.transform;
                attack.DOMove(ts_hit.position, 0.5f).OnComplete(() =>
                {
                    attack.GetComponent<PoolObjectAutoDeactivate>().Deactive();
                    // if (ActorProxy.isPlayingAI) DamageToTarget(target, 0);
                    PoolManager.Get().ActivateObject(obj_AttackHit.name, ts_hit.position);
                });
            }
        }
    }

    // call relay
    public void FireBullet(uint targetId)
    {
        target = ActorProxy.GetBaseStatWithNetId(targetId);

        if (target != null)
        {
            var attack = PoolManager.Get().ActivateObject(obj_Attack.name, ts_ShootingPos.position);
            if (attack != null)
            {
                var ts_hit = target.ts_HitPos;
                if (ts_hit == null) ts_hit = target.transform;
                attack.DOMove(ts_hit.position, 0.5f).OnComplete(() =>
                {
                    attack.GetComponent<PoolObjectAutoDeactivate>().Deactive();
                    PoolManager.Get().ActivateObject(obj_AttackHit.name, ts_hit.position);
                });
            }
        }
    }

    public void Skill()
    {
        if (_spawnedTime >= _skillCastedTime + effectCooltime)
        {
            _skillCastedTime = _spawnedTime;

            animator.SetTrigger(AnimationHash.Skill);
            // controller.NetSendPlayer(GameProtocol.SEND_MESSAGE_VOID_RELAY, id, E_ActionSendMessage.DropBullet);
            DropBullet();
        }
    }

    // IEnumerator SkillCoroutine()
    // {
    //     
    // }

    public void DropBullet()
    {
        animator.SetTrigger(AnimationHash.Skill);
        StartCoroutine(DropBulletCoroutine());
    }

    IEnumerator DropBulletCoroutine()
    {
        yield return new WaitForSeconds(1f);
        
        for (int i = 0; i < 20; i++)
        {
            float x = Random.Range(-3.5f, 3.5f);
            float z = Random.Range(-5.5f, 5.5f);
            Vector3 startPos = new Vector3(x, 20f, z);
            var bullet = PoolManager.Get().ActivateObject(obj_Skill.name, startPos);
            if (bullet != null)
            {
                bullet.DOMoveY(0, 0.5f).OnComplete(() =>
                {
                    PoolManager.Get().ActivateObject(obj_SkillHit.name, bullet.position);
                    bullet.GetComponent<PoolObjectAutoDeactivate>().Deactive();

                    if (ActorProxy.isPlayingAI)
                    {
                        //KZSee:
                        // var minions = ActorProxy.GetEnemies();;
                        // for (int j = 0; j < minions.Length; j++)
                        // {
                        //     DamageToTarget(minions[j], 0, 0.1f);
                        // }
                    }
                });
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
}
