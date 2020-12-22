using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ED;
using Microsoft.Win32.SafeHandles;
using RandomWarsProtocol;
using UnityEngine;

public class Boss5 : Minion
{
    [Header("Effect")]
    public GameObject obj_Attack;
    public GameObject obj_AttackHit;
    public GameObject obj_Skill;
    public GameObject obj_SkillHit;
    
    private float _skillCastedTime;
    private MinionAnimationEvent _animationEvent;

    protected override void Start()
    {
        base.Start();
        
        PoolManager.Get().AddPool(obj_Attack, 3);
        PoolManager.Get().AddPool(obj_AttackHit, 3);
        PoolManager.Get().AddPool(obj_Skill, 20);
        PoolManager.Get().AddPool(obj_SkillHit, 20);
        if (_animationEvent == null) _animationEvent = GetComponentInChildren<MinionAnimationEvent>();
    }

    public override void Initialize(DestroyCallback destroy)
    {
        base.Initialize(destroy);
        _skillCastedTime = -effectCooltime;
        if (_animationEvent == null) _animationEvent = GetComponentInChildren<MinionAnimationEvent>();
        _animationEvent.event_Attack += Callback_Attack;
    }

    public override void Attack()
    {
        if (target == null || target.isAlive == false || IsTargetInnerRange() == false) return;
        
        if( InGameManager.IsNetwork && (isMine || controller.isPlayingAI) )
        {
            base.Attack();
            controller.MinionAniTrigger(id, "Attack", target.id);
        }
        else if(InGameManager.IsNetwork == false)
        {
            base.Attack();
            animator.SetTrigger(_animatorHashAttack);
        }
    }

    public override void Death()
    {
        base.Death();

        _animationEvent.event_Attack -= Callback_Attack;
    }

    public void Callback_Attack()
    {
        if (target != null)
        {
            if (isMine || controller.isPlayingAI)
            {
                controller.NetSendPlayer(GameProtocol.SEND_MESSAGE_PARAM1_RELAY, id, E_ActionSendMessage.FireBullet, target.id);
            }
            
            var attack = PoolManager.Get().ActivateObject(obj_Attack.name, ts_ShootingPos.position);
            if (attack != null)
            {
                var ts_hit = target.ts_HitPos;
                if (ts_hit == null) ts_hit = target.transform;
                attack.DOMove(ts_hit.position, 0.5f).OnComplete(() =>
                {
                    attack.GetComponent<PoolObjectAutoDeactivate>().Deactive();
                    if (isMine || controller.isPlayingAI) DamageToTarget(target, 0);
                    PoolManager.Get().ActivateObject(obj_AttackHit.name, ts_hit.position);
                });
            }
        }
    }

    // call relay
    public void FireBullet(int targetID)
    {
        target = InGameManager.Get().GetBaseStatFromId(targetID);

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
            
            SetControllEnable(false);
            animator.SetTrigger(_animatorHashSkill);
            controller.NetSendPlayer(GameProtocol.SEND_MESSAGE_VOID_RELAY, id, E_ActionSendMessage.DropBullet);
            DropBullet();
        }
    }

    // IEnumerator SkillCoroutine()
    // {
    //     
    // }

    public void DropBullet()
    {
        animator.SetTrigger(_animatorHashSkill);
        StartCoroutine(DropBulletCoroutine());
    }

    IEnumerator DropBulletCoroutine()
    {
        for (int i = 0; i < 20; i++)
        {
            float x = Random.Range(-3.5f, 3.5f);
            float z = Random.Range(-5.5f, 5.5f);
            Vector3 startPos = new Vector3(x, 5f, z);
            var bullet = PoolManager.Get().ActivateObject(obj_Skill.name, startPos);
            if (bullet != null)
            {
                bullet.DOMoveY(0, 0.5f).OnComplete(() =>
                {
                    PoolManager.Get().ActivateObject(obj_SkillHit.name, bullet.position);
                    bullet.GetComponent<PoolObjectAutoDeactivate>().Deactive();
                });
            }

            yield return new WaitForSeconds(0.1f);
        }
        
        SetControllEnable(true);
    }
}
