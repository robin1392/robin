using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ED;
using ED.Boss;
using Microsoft.Win32.SafeHandles;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using RandomWarsProtocol;
using UnityEngine;

public class Boss5 : BossBase
{
    [Header("Effect")]
    public GameObject obj_Attack;
    public GameObject obj_AttackHit;
    public GameObject obj_Skill;
    public GameObject obj_SkillHit;

    public float bulletMoveSpeed = 10;
    private float _skillCastedTime;

    protected override void Start()
    {
        base.Start();

        if (_animationEvent == null)
        {
            _animationEvent = GetComponentInChildren<MinionAnimationEvent>();
        }
        
        _animationEvent.event_FireArrow -= FireArrow;
        _animationEvent.event_FireArrow += FireArrow;
        
        PoolManager.Get().AddPool(obj_Attack, 3);
        PoolManager.Get().AddPool(obj_AttackHit, 3);
        PoolManager.Get().AddPool(obj_Skill, 20);
        PoolManager.Get().AddPool(obj_SkillHit, 20);
    }

    void FireArrow()
    {
        if (target == null || IsTargetInnerRange() == false)
        {
            animator.SetTrigger(AnimationHash.Idle);
            return;
        }

        if (ActorProxy.isPlayingAI)
        {
            ActorProxy.FireBulletWithRelay(E_BulletType.BOSS5_BULLET, target, power, bulletMoveSpeed);
        }
    }
    
    protected override IEnumerator Combat()
    {
        while (true)
        {
            yield return Skill();

            if (!IsTargetInnerRange())
            {
                ApproachToTarget();
            }
            else
            {
                break;
            }

            yield return null;

            target = SetTarget();
        }

        StopApproachToTarget();

        if (target == null)
        {
            yield break;
        }

        yield return Attack();
    }

    public IEnumerator Skill()
    {
        if (_spawnedTime >= _skillCastedTime + effectCooltime)
        {
            _skillCastedTime = _spawnedTime;

            var action = new Boss5SkillAction();
            yield return action.ActionWithSync(ActorProxy);
        }
    }
    
    public class Boss5SkillAction : SyncActionWithoutTarget
    {
        public override IEnumerator Action(ActorProxy actorProxy)
        {
            var boss = (Boss5) actorProxy.baseStat;

            boss.animator.SetTrigger(AnimationHash.Skill);

            yield return new WaitForSeconds(1f);
        
            for (int i = 0; i < 20; i++)
            {
                float x = Random.Range(-3.5f, 3.5f);
                float z = Random.Range(-5.5f, 5.5f);
                Vector3 startPos = new Vector3(x, 20f, z);
                var bullet = PoolManager.Get().ActivateObject(boss.obj_Skill.name, startPos);
                if (bullet != null)
                {
                    bullet.DOMoveY(0, 0.5f).OnComplete(() =>
                    {
                        PoolManager.Get().ActivateObject(boss.obj_SkillHit.name, bullet.position);
                        bullet.GetComponent<PoolObjectAutoDeactivate>().Deactive();

                        if (actorProxy.isPlayingAI)
                        {
                            var enemies = actorProxy.GetEnemies();
                            foreach (var enemy in enemies)
                            {
                                enemy.ActorProxy.HitDamage(actorProxy.power * 0.1f);
                            }
                        }
                    });
                }

                yield return new WaitForSeconds(0.1f);
            }
            
            yield return new WaitForSeconds(0.4f);
        }
    }
}


