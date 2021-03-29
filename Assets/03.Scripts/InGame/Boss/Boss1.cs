using System.Collections;
using System.Collections.Generic;
using ED;
using ED.Boss;
using Microsoft.Win32.SafeHandles;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using RandomWarsProtocol;
using UnityEngine;

public class Boss1 : BossBase
{
    public GameObject pref_Dust;

    private float _skillCastedTime;
    private bool _isSkillCasting;

    public override void Initialize()
    {
        base.Initialize();
        _skillCastedTime = -effectCooltime;
        PoolManager.instance.AddPool(pref_Dust, 1);
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

    private Minion GetLongDistanceTarget()
    {
        var minions = ActorProxy.GetEnemies();
        List<BaseStat> list = new List<BaseStat>();
        float min = 0f;
        float max = 0f;

        var position = transform.position;
        foreach (var minion in minions)
        {
            if (minion == null)
            {
                continue;
            }
            
            float sqrDistance = Vector3.SqrMagnitude(minion.transform.position - position);

            if (list.Count == 0)
            {
                list.Add(minion);
                min = sqrDistance;
                max = sqrDistance;
                continue;
            }

            if (sqrDistance > max)
            {
                list.Add(minion);
                max = sqrDistance;
            }
            else if (sqrDistance < min)
            {
                list.Insert(0, minion);
                min = sqrDistance;
            }
        }

        if (list.Count > 0)
            return list[Random.Range(list.Count / 2, list.Count)] as Minion;
        else
            return null;
    }

    public IEnumerator Skill()
    {
        if (_spawnedTime >= _skillCastedTime + effectCooltime)
        {
            var target = GetLongDistanceTarget();
            if (target != null)
            {
                _attackedTarget = target;
                _skillCastedTime = _spawnedTime;
                var action = new JumpSkillAction();
                yield return action.ActionWithSync(ActorProxy, target.ActorProxy);
            }
        }
    }

    public void SplashDamage()
    {
        var targets = Physics.OverlapSphere(ActorProxy.transform.position, 1f, targetLayer);
        for (int i = 0; i < targets.Length; i++)
        {
            var bs = targets[i].GetComponentInParent<BaseStat>();
            if (bs != null && bs.isAlive)
            {
                bs.ActorProxy.HitDamage(power);
            }
        }
    }
}

public class JumpSkillAction : SyncActionWithTarget
{
    public override IEnumerator Action(ActorProxy actorProxy, ActorProxy targetActorProxy)
    {
        var boss = (Boss1) actorProxy.baseStat;
        if (boss == null)
        {
            boss.collider.enabled = true;
            yield break;
        }

        var actorTransform = actorProxy.transform;
        var targetTransform = targetActorProxy.transform;
        boss.animator.SetTrigger(AnimationHash.Skill);

        yield return new WaitForSeconds(1f);

        if (targetTransform == null)
        {
            yield break;
        }
        
        var lookPos = targetTransform.position;
        lookPos.y = 0;
        actorProxy.transform.LookAt(lookPos);

        yield return null;

        var startPos = actorTransform.position;
        var targetPos = targetTransform.position;
        var t = 0f;

        float fV_x;
        float fV_y;
        float fV_z;

        float fg;
        float fEndTime;
        float fMaxHeight = 2f;
        float fHeight;
        float fEndHeight;
        float fTime = 0f;
        float fMaxTime = 0.5f;

        fEndHeight = targetPos.y - startPos.y;
        fHeight = fMaxHeight - startPos.y;
        fg = 2 * fHeight / (fMaxTime * fMaxTime);
        fV_y = Mathf.Sqrt(2 * fg * fHeight);

        float a = fg;
        float b = -2 * fV_y;
        float c = 2 * fEndHeight;

        fEndTime = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);

        fV_x = -(startPos.x - targetPos.x) / fEndTime;
        fV_z = -(startPos.z - targetPos.z) / fEndTime;

        boss.collider.enabled = false;
        var currentPos = new Vector3();
        while (t < fEndTime)
        {
            t += Time.deltaTime;

            currentPos.x = startPos.x + fV_x * t;
            currentPos.y = startPos.y + (fV_y * t) - (0.5f * fg * t * t);
            currentPos.z = startPos.z + fV_z * t;

            actorTransform.position = currentPos;

            yield return null;
        }

        boss.collider.enabled = true;
        var pos = actorTransform.position;
        pos.y = 0;
        actorTransform.position = pos;

        if (actorProxy.isPlayingAI)
        {
            boss.SplashDamage();
        }

        var effect = PoolManager.instance.ActivateObject("Effect_Support", pos);
        effect.rotation = Quaternion.identity;
        effect.localScale = Vector3.one * 0.8f;
    }

    public override void OnActionCancel(ActorProxy actorProxy)
    {
        var boss = (Boss1) actorProxy.baseStat;
        boss.collider.enabled = true;
    }
}