#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using MirageTest.Scripts;
using UnityEngine;

namespace ED
{
    public enum TARGET_ORDER
    {
        NONE,
        SHORT_DISTANCE,
        LONG_DISTANCE,
        RANDOM
    }

    public partial class Minion : BaseEntity
    {
        public DICE_CAST_TYPE castType;
        public bool isAttackSpeedFactorWithAnimation = true;
        protected float _spawnedTime;
        
        public bool isCloacking => ActorProxy.isClocking;

        public Collider collider;
        public bool isPolymorph;

        public Dictionary<BuffType, PoolObjectAutoDeactivate> _dicEffectPool =
            new Dictionary<BuffType, PoolObjectAutoDeactivate>();
        
        protected BaseEntity _attackedTarget;
        protected MinionAnimationEvent _animationEvent;
        
        protected bool _destroyed; 
        public bool Destroyed => _destroyed;

        public override float Radius
        {
            get
            {
                if (collider is SphereCollider sphereCollider)
                {
                    return sphereCollider.radius;
                }

                return 0;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            collider = GetComponentInChildren<Collider>();
            _animationEvent = animator.GetComponent<MinionAnimationEvent>();
        }
        
        protected virtual void Update()
        {
            if (ActorProxy == null)
            {
                return;
            }
            
            if (ActorProxy.isPlayingAI == false)
            {
                return;
            }
            
            _spawnedTime += Time.deltaTime;
            if (animator != null)
            {
                animator.SetFloat(AnimationHash.MoveSpeed, ActorProxy._aiPath.velocity.magnitude);
            }
        }

        public virtual void Initialize()
        {
            _destroyed = false;
            collider.enabled = true;
            isPolymorph = false;
            animator.gameObject.SetActive(true);
            _spawnedTime = 0;
            target = null;
            _attackedTarget = null;
            image_HealthBar.fillAmount = 1f;
            RendererEffect.Reset();

            if (animator != null)
            {
                animator.SetFloat(AnimationHash.MoveSpeed, 0);

                var client = ActorProxy.Client as RWNetworkClient;
                var factor = client.GameState.factor;

                if (isAttackSpeedFactorWithAnimation)
                {
                    animator.SetFloat("AttackSpeed", 1f / attackSpeed / factor);
                }

                animator.speed = factor;
            }

            _dicEffectPool.Clear();
            objectHealthBar?.SetColor(ActorProxy.IsLocalPlayerAlly);
        }

        public void ResetAttackedTarget()
        {
            _attackedTarget = null;
        }
        
        public void SetAttackedTarget(BaseEntity target)
        {
            _attackedTarget = target;
        }

        public override void OnHitDamageOnClient(float damage, DamageType damageType)
        {
            if (damageType == DamageType.Fire)
            {
                PlayRendererHitEffect();
            }   
        }

        public virtual BaseEntity SetTarget()
        {
            if (_attackedTarget != null && _attackedTarget.CanBeTarget())
            {
                return _attackedTarget;
            }
            else if(ActorProxy != null && ActorProxy.isTaunted)
            {
                ActorProxy.DisableBuffEffect(BuffType.Taunted);
            }

            if (ActorProxy == null)
            {
                return null;
            }
            
            if (ActorProxy.isInAllyCamp)
            {
                var position = transform.position;
                
                var target = ActorProxy.GetEnemies().Where(e => e.ActorProxy.isInEnemyCamp)
                    .OrderBy(e => (e.transform.position - position).sqrMagnitude).FirstOrDefault();

                if (target != null)
                {
                    return target;
                }
            }

            var cols = Physics.OverlapSphere(transform.position, searchRange, targetLayer);
            
            Minion closestTarget = null;
            var distance = float.MaxValue;
            
            foreach (var col in cols)
            {
                var bs = col.GetComponentInParent<Minion>();
                if (bs == null || !bs.CanBeTarget())
                {
                    continue;
                }

                var sqr = Vector3.SqrMagnitude(transform.position - col.transform.position);

                if (sqr < distance)
                {
                    distance = sqr;
                    closestTarget = bs;
                }
            }

            if (closestTarget != null)
            {
                return closestTarget;
            }

            if (targetMoveType == DICE_MOVE_TYPE.GROUND || targetMoveType == DICE_MOVE_TYPE.ALL)
            {
                return ActorProxy.GetEnemyTowerOrBossEgg();
            }

            return null;
        }
        
        private IEnumerator MoveToAttackInnerRanger()
        {
            Vector3 startPos = transform.position;
            Vector3 targetPos = target.transform.position;
            if (Vector3.Distance(startPos, targetPos) > range)
            {
                float t = 0;
                Vector3 endPos = targetPos + (startPos - targetPos).normalized * range;
                while (t < 0.5f)
                {
                    transform.position = Vector3.Lerp(startPos, endPos, t * 2f);
                    t += Time.deltaTime;
                    yield return null;
                }

                transform.position = endPos;
            }
        }

        public bool IsTargetAlive()
        {
            return target != null && target.isAlive;
        }
        
        public bool IsTargetInnerRange()
        {
            if (ActorProxy == null)
            {
                return false;
            }
            
            if (target == null || target.isAlive == false)
            {
                return false;
            }
            
#if UNITY_EDITOR
            if (target != null)
            {
                Debug.DrawLine(transform.position + Vector3.up * 0.1f,
                    (transform.position + Vector3.up * 0.1f) +
                    (target.transform.position - transform.position).normalized * range,
                    Color.yellow);   
            }
#endif
            //return Vector3.Distance(transform.position, target.transform.position) < range + 0.1f;

            if (target != null)
            {
                if (Vector3.Distance(transform.position, target.transform.position) <= range + target.Radius + Radius)
                {
                    return true;
                }
                else
                {
                    var hits = Physics.RaycastAll(transform.position + Vector3.up * 0.1f,
                        (target.transform.position - transform.position).normalized, range, targetLayer);

                    bool rtn = false;
                    foreach (var hit in hits)
                    {
                        if (hit.collider.GetComponentInParent<BaseEntity>() == target)
                        {
                            rtn = true;
                        }
                    }

                    return rtn;
                }
            }

            return false;
        }

        public bool IsTargetInnerRange(BaseEntity bs)
        {
            if (ActorProxy == null)
            {
                return false;
            }
#if UNITY_EDITOR
            Debug.DrawLine(transform.position + Vector3.up * 0.1f,
                (transform.position + Vector3.up * 0.1f) +
                (target.transform.position - transform.position).normalized * range,
                Color.yellow);
#endif

            if (bs != null)
            {
                return Vector3.Distance(transform.position, bs.transform.position) <= range;
            }

            return false;
        }

        public bool IsFriendlyTargetInnerRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) <= range;
        }

      

        public void SetAttackSpeedFactor(float factor)
        {
            //KZSee:
            // attackSpeed = _originalAttackSpeed * factor;
            if (animator != null) animator.SetFloat("AttackSpeed", 1f / attackSpeed);
        }
        
        protected bool IsFriendlyLayer(GameObject targetObject)
        {
            switch (targetMoveType)
            {
                case DICE_MOVE_TYPE.GROUND:
                    return targetObject.layer == LayerMask.NameToLayer(isBottomCamp ? "BottomPlayer" : "TopPlayer");
                case DICE_MOVE_TYPE.FLYING:
                    return targetObject.layer ==
                           LayerMask.NameToLayer(isBottomCamp ? "BottomPlayerFlying" : "TopPlayerFlying");
                case DICE_MOVE_TYPE.ALL:
                    return targetObject.layer == LayerMask.NameToLayer(isBottomCamp ? "BottomPlayer" : "TopPlayer")
                           || targetObject.layer ==
                           LayerMask.NameToLayer(isBottomCamp ? "BottomPlayerFlying" : "TopPlayerFlying");
                default:
                    return false;
            }
        }

        public void ApplyCloacking(bool isCloacking)
        {
            var isAlly = ActorProxy.IsLocalPlayerAlly;
            if (isCloacking)
            {
                PoolManager.instance.ActivateObject("Effect_Cloaking", ts_HitPos.position);
                Debug.LogError("URP로 변경되면서 은신 효과가 사라진 상태입니다. 구현이 필요합니다.");
            }
            else
            {
                Debug.LogError("URP로 변경되면서 은신 효과가 사라진 상태입니다. 구현이 필요합니다.");
            }
        }

        public override void OnBaseEntityDestroyed()
        {
            if (animator != null) animator.SetFloat(AnimationHash.MoveSpeed, 0);
            _destroyed = true;
            SoundManager.instance.Play(Global.E_SOUND.SFX_MINION_DEATH);
            
            PoolManager.instance.ActivateObject(ActorProxy.IsBottomCamp() ? "Effect_Death_blue" : "Effect_Death_red", ts_HitPos.position);
            foreach (var autoDeactivate in _dicEffectPool)
            {
                autoDeactivate.Value.Deactive();
            }
            
            base.OnBaseEntityDestroyed();
        }

        public void PlayRendererHitEffect()
        {
            var amount = 0.0f;
            var targetValue = 0.7f;
            var duration = 0.2f;
            Tweener tweener = DOTween.To(() => amount, x => amount = x, targetValue, duration);
            tweener.OnUpdate(() => { RendererEffect.SetTintColor(Color.red, amount); });
            tweener.SetLoops(2, LoopType.Yoyo).SetEase(Ease.InOutQuad);
        }
    }
}