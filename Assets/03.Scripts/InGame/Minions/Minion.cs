#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using Mirage;
using MirageTest.Scripts;
using NodeCanvas.BehaviourTrees;
using Pathfinding;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using RandomWarsProtocol;
using Debug = ED.Debug;

namespace ED
{
    public enum MAZ
    {
        NONE,
        STURN,
        FREEZE,
    }

    public enum TARGET_ORDER
    {
        NONE,
        SHORT_DISTANCE,
        LONG_DISTANCE,
        RANDOM
    }

    public partial class Minion : BaseStat
    {
        public DestroyCallback destroyCallback;

        public delegate void DestroyCallback(Minion minion);

        public DICE_CAST_TYPE castType;
        public bool isPushing;
        public bool isAttackSpeedFactorWithAnimation = true;
        protected float _spawnedTime;

        public float spawnedTime => _spawnedTime;

        //private bool _isNexusAttacked;
        protected bool isInvincibility;
        protected bool _isCloacking;
        public bool isCloacking => _isCloacking;
        protected int cloackingCount;
        protected int invincibilityCount;
        private float _originalAttackSpeed;

        [HideInInspector] public int eyeLevel=> ActorProxy.diceScale;
        [HideInInspector] public int ingameUpgradeLevel=>ActorProxy.ingameUpgradeLevel;

        private Vector3 _dodgeVelocity;
        protected static readonly int _animatorHashMoveSpeed = Animator.StringToHash("MoveSpeed");
        protected static readonly int _animatorHashIdle = Animator.StringToHash("Idle");
        protected static readonly int _animatorHashAttack = Animator.StringToHash("Attack");
        protected static readonly int _animatorHashAttack1 = Animator.StringToHash("Attack1");
        protected static readonly int _animatorHashAttack2 = Animator.StringToHash("Attack2");
        protected static readonly int _animatorHashAttackReady = Animator.StringToHash("AttackReady");
        protected static readonly int _animatorHashSkill = Animator.StringToHash("Skill");
        protected static readonly int _animatorHashSkillLoop = Animator.StringToHash("SkillLoop");

        private Coroutine _crtAttack;
        private Coroutine _crtPush;
        public BehaviourTreeOwner behaviourTreeOwner { get; protected set; }
        public PoolObjectAutoDeactivate _poolObjectAutoDeactivate;
        protected Collider _collider;
        public bool isPolymorph;
        protected int _flagOfWarCount;

        protected static readonly string _scarecrow = "Scarecrow";

        protected Dictionary<MAZ, PoolObjectAutoDeactivate> _dicEffectPool =
            new Dictionary<MAZ, PoolObjectAutoDeactivate>();

        protected Shield _shield;
        protected Coroutine _invincibilityCoroutine;
        protected BaseStat _attackedTarget;
        protected MinionAnimationEvent _animationEvent;
        
        private bool _destroyed; 
        public bool Destroyed => _destroyed;

        protected virtual void Awake()
        {
            _poolObjectAutoDeactivate = GetComponent<PoolObjectAutoDeactivate>();
            // behaviourTreeOwner = GetComponent<BehaviourTreeOwner>();
            _collider = GetComponentInChildren<Collider>();
            _animationEvent = animator.GetComponent<MinionAnimationEvent>();
        }


        protected virtual void Update()
        {
            // if (currentHealth <= 0 && ((InGameManager.IsNetwork && !isMine) || controller.isPlayingAI))
            // {
            //     Death();
            //     return;
            // }
            //
            // _spawnedTime += Time.deltaTime;
            //
            // if (isPlayable && isPushing == false && isAttacking == false)
            // {
            //     float distance = Vector3.Magnitude(networkPosition - transform.position);
            //     //if (PhotonNetwork.IsConnected && !isMine)
            //     if(InGameManager.IsNetwork && !isMine)// && agent.enabled)
            //     {
            //         //rb.position = Vector3.Lerp(rb.position, networkPosition, Time.fixedDeltaTime);
            //         if (controller.isMinionAgentMove)
            //         {
            //             //agent.SetDestination(networkPosition);
            //             if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack") == false
            //             && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") == false
            //             && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2") == false)
            //             {
            //                 // transform.rotation = Quaternion.RotateTowards(transform.rotation,
            //                 //     Quaternion.LookRotation(networkPosition - transform.position), Time.deltaTime * 480f);
            //             }
            //
            //             // transform.position =
            //             //     Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * moveSpeed);
            //             transform.position = Vector3.Lerp(transform.position, networkPosition, 0.5f);//Time.deltaTime * moveSpeed);
            //             // _seeker.StartPath(transform.position, networkPosition);
            //         }
            //         else
            //         {
            //             //transform.LookAt(networkPosition);
            //             transform.position = Vector3.Lerp(transform.position, networkPosition, Time.deltaTime * moveSpeed * 2f);
            //         }
            //     }
            //
            //     //var velocityMagnitude = rb.velocity.magnitude;
            //     if (animator != null)
            //     {
            //         //animator.SetFloat(AnimatorHashMoveSpeed, velocityMagnitude);
            //         if (isMine)
            //             animator.SetFloat(_animatorHashMoveSpeed, _aiPath.velocity.magnitude);//agent.velocity.magnitude);
            //         else
            //             animator.SetFloat(_animatorHashMoveSpeed, distance * 5);
            //     }


            //if (PhotonNetwork.IsConnected && !isMine) return;
            // if (InGameManager.IsNetwork && !isMine) 
            //     return;

            // if (isAttacking != false || isPushing != false || target == null || !(velocityMagnitude > 0.1f)) 
            //     return;

            // var lTargetDir = rb.velocity;
            // lTargetDir.y = 0.0f;
            // //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lTargetDir), Time.fixedDeltaTime * 480f);
            // transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lTargetDir), Time.deltaTime * 480f);
            // }
        }


        public virtual void Initialize(DestroyCallback destroy)
        {
            SetControllEnable(true);
            _dodgeVelocity = Vector3.zero;
            _collider.enabled = true;
            isPolymorph = false;
            animator.gameObject.SetActive(true);
            _spawnedTime = 0;
            target = null;
            _attackedTarget = null;
            _originalAttackSpeed = attackSpeed;
            image_HealthBar.fillAmount = 1f;
            
            RefreshHealthBarColor();

            if (animator != null)
            {
                animator.SetFloat(_animatorHashMoveSpeed, 0);

                if (isAttackSpeedFactorWithAnimation)
                {
                    animator.SetFloat("AttackSpeed", 1f / attackSpeed);
                }
            }

            if (destroy != null)
            {
                destroyCallback = null;
                destroyCallback += destroy;
            }

            cloackingCount = 0;
            Cloacking(false);
            _dicEffectPool.Clear();

            SetColor(isBottomPlayer ? E_MaterialType.BOTTOM : E_MaterialType.TOP, ActorProxy.IsLocalPlayerAlly());
        }
        
        public void Heal(float heal)
        {
            if (ActorProxy.currentHealth > 0)
            {
                ActorProxy.currentHealth += heal;

                //KZSee:
                // if(currentHealth > maxHealth)
                // {
                //     currentHealth = maxHealth;
                // }

                PoolManager.instance.ActivateObject("Effect_Heal", transform.position);
            }

            RefreshHealthBar();
        }

        public override void HitDamage(float damage)
        {
            // if (isInvincibility) return;
            // if (invincibilityCount > 0)
            // {
            //     invincibilityCount--;
            //     if (_shield != null && invincibilityCount == 0 && _invincibilityCoroutine == null)
            //     {
            //         _shield.Deactive();
            //         _shield = null;
            //     }
            //
            //     return;
            // }
            //
            // ActorProxy.currentHealth -= damage;
            // RefreshHealthBar();
            //
            // if (ActorProxy.currentHealth <= 0)
            // {
            //     //if (PhotonNetwork.IsConnected && !isMine)
            //     if (InGameManager.IsNetwork && !isMine && controller.isPlayingAI == false)
            //         return;
            //
            //     ActorProxy.currentHealth = 0;
            //     controller.DeathMinion(id);
            // }
        }

        public virtual void Death()
        {
            SetControllEnable(false);
            if (animator != null) animator.SetFloat(_animatorHashMoveSpeed, 0);
            StopAllCoroutines();

            destroyCallback(this);
            PoolManager.instance.ActivateObject("Effect_Death", ts_HitPos.position);
            foreach (var autoDeactivate in _dicEffectPool)
            {
                autoDeactivate.Value.Deactive();
            }

            _poolObjectAutoDeactivate.Deactive();

            SoundManager.instance?.Play(Global.E_SOUND.SFX_MINION_DEATH);
        }

        protected void RefreshHealthBar()
        {
            image_HealthBar.fillAmount = ActorProxy.currentHealth / ActorProxy.maxHealth;
        }
        
        protected bool CanAttackTarget()
        {
            if (target == null || !target.isAlive)
            {
                return false;
            }

            if (target is Minion minion)
            {
                return !minion.isCloacking;
            }

            return true;
        }

        public virtual BaseStat SetTarget()
        {
            if (_attackedTarget != null && _attackedTarget.CanBeTarget())
            {
                return _attackedTarget;
            }

            var cols = Physics.OverlapSphere(transform.position, searchRange, targetLayer);

            Collider firstTarget = null;
            var distance = float.MaxValue;
            
            Debug.Log(cols.Length);
            foreach (var col in cols)
            {
                var bs = col.GetComponentInParent<BaseStat>();
                if (bs == null || !bs.CanBeTarget())
                {
                    continue;
                }

                var sqr = Vector3.SqrMagnitude(transform.position - col.transform.position);

                if (sqr < distance)
                {
                    distance = sqr;
                    firstTarget = col;
                }
            }

            if (firstTarget != null)
            {
                return firstTarget.GetComponentInParent<BaseStat>();
            }

            if (targetMoveType == DICE_MOVE_TYPE.GROUND || targetMoveType == DICE_MOVE_TYPE.ALL)
            {
                return ActorProxy.GetEnemyTower();
            }

            return null;
        }

        public void ChangeLayer(bool pIsBottomPlayer)
        {
            isBottomPlayer = pIsBottomPlayer;
            var layerName = $"{(pIsBottomPlayer ? "BottomPlayer" : "TopPlayer")}{(isFlying ? "Flying" : string.Empty)}";
            gameObject.layer = LayerMask.NameToLayer(layerName);

            if (!isBottomPlayer)
            {
                ActorProxy.transform.rotation = Quaternion.Euler(0, 180f, 0);
            }
        }

        public virtual void EndGameUnit()
        {
            if (animator != null)
            {
                animator.SetFloat(_animatorHashMoveSpeed, 0);
                animator.SetTrigger(_animatorHashIdle);
            }

            SetControllEnable(false);
            StopAllCoroutines();
            // GetComponent<NodeCanvas.BehaviourTrees.BehaviourTreeOwner>().StopBehaviour();
        }

        public void DamageToTarget(BaseStat m, float delay = 0, float factor = 1f)
        {
            if (m == null || m.isAlive == false) return;

            controller.AttackEnemyMinionOrMagic(m.UID, m.id, power * factor, delay);
            //controller.AttackEnemyMinion(m.id, power * factor, delay);
            controller.SetMaxDamageWithDiceID(diceId, power * factor);
        }

        public void Push(Vector3 dir, float pushPower)
        {
            //StopAllCoroutines();
            SetControllEnable(false);
            animator.SetTrigger(_animatorHashIdle);
            throw new NotImplementedException("리지드바디를 사용하지 않고 Push구현");
            // rb.AddForce(dir.normalized * pushPower, ForceMode.Impulse);
            if (_crtPush != null) StopCoroutine(_crtPush);
            _crtPush = StartCoroutine(PushCoroutine());
        }

        private IEnumerator PushCoroutine()
        {
            yield return new WaitForSeconds(0.5f);

            //KZSee : 마찰때문에 줄어드는 속도를 강제로 유지시키는 코드인듯 하다. 에드님께 물어볼 것.
            // var vel = rb.velocity.magnitude;
            // while (vel > 0.1f)
            // {
            //     yield return new WaitForSeconds(0.1f);
            //     vel = rb.velocity.magnitude;
            // }
            // rb.velocity = Vector3.zero;
            // rb.isKinematic = true;
            SetControllEnable(true);
        }

        public virtual void Sturn(float duration)
        {
            //StopAllCoroutines();

            if (_dicEffectPool.ContainsKey(MAZ.STURN))
            {
                _dicEffectPool[MAZ.STURN].Deactive();
                _dicEffectPool.Remove(MAZ.STURN);
            }

            _crtPush = StartCoroutine(SturnCoroutine(duration));
        }

        private IEnumerator SturnCoroutine(float duration)
        {
            var ad = PoolManager.instance.ActivateObject<PoolObjectAutoDeactivate>("Effect_Sturn",
                ts_HitPos.position + Vector3.up * 0.65f);
            _dicEffectPool.Add(MAZ.STURN, ad);
            //rb.velocity = Vector3.zero;
            //rb.isKinematic = true;
            SetControllEnable(false);
            if (animator != null) animator.SetTrigger(_animatorHashIdle);
            yield return new WaitForSeconds(duration);
            SetControllEnable(true);
            ad.Deactive();
            _dicEffectPool.Remove(MAZ.STURN);
            //rb.isKinematic = false;
        }

        // 무
        public void Invincibility(float time)
        {
            if (_invincibilityCoroutine != null) StopCoroutine(_invincibilityCoroutine);
            _invincibilityCoroutine = StartCoroutine(InvincibilityCoroutine(time));
        }

        public void Invincibility(int addCount)
        {
            invincibilityCount += addCount;
            if (_shield == null)
            {
                _shield = PoolManager.instance.ActivateObject<Shield>("Shield", transform);
                _shield.transform.localPosition = ts_HitPos.localPosition;
            }
        }

        private IEnumerator InvincibilityCoroutine(float time)
        {
            isInvincibility = true;
            if (_shield == null)
            {
                _shield = PoolManager.instance.ActivateObject<Shield>("Shield", transform);
                _shield.transform.localPosition = ts_HitPos.localPosition;
            }

            yield return new WaitForSeconds(time);
            isInvincibility = false;
            if (_shield != null && invincibilityCount == 0)
            {
                _shield.Deactive();
                _shield = null;
            }
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

        public virtual void SetVelocityTarget()
        {
            if (controller.isMinionAgentMove)
            {
                if (target != null && isAlive)
                {
                    Vector3 targetPos = target.transform.position +
                                        (target.transform.position - transform.position).normalized * range;
                    Seeker.StartPath(transform.position, targetPos);
                }
            }
        }

        private IEnumerator ResetDodgeVelocityCoroutine()
        {
            yield return new WaitForSeconds(1f);

            _dodgeVelocity = Vector3.zero;
        }

        public bool IsTargetInnerRange()
        {
#if UNITY_EDITOR
            Debug.DrawLine(transform.position + Vector3.up * 0.1f,
                (transform.position + Vector3.up * 0.1f) +
                (target.transform.position - transform.position).normalized * range,
                Color.yellow);
#endif
            //return Vector3.Distance(transform.position, target.transform.position) < range + 0.1f;

            if (target != null)
            {
                if (Vector3.Distance(transform.position, target.transform.position) <= range)
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
                        if (hit.collider.GetComponentInParent<BaseStat>() == target)
                        {
                            rtn = true;
                        }
                    }

                    return rtn;
                }
            }

            return false;
        }

        public bool IsTargetInnerRange(BaseStat bs)
        {
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

        public void SetFlagOfWar(bool isIn, float factor)
        {
            _flagOfWarCount += isIn ? 1 : -1;

            if (_flagOfWarCount > 0)
            {
                SetAttackSpeedFactor(factor);
            }
            else
            {
                _flagOfWarCount = 0;
                SetAttackSpeedFactor(1f);
            }
        }

        public void SetAttackSpeedFactor(float factor)
        {
            //KZSee:
            // attackSpeed = _originalAttackSpeed * factor;
            if (animator != null) animator.SetFloat("AttackSpeed", 1f / attackSpeed);
        }

        public void SetControllEnable(bool isEnable)
        {
            isPushing = !isEnable;
            AiPath.isStopped = !isEnable;
        }

        public void Scarecrow(float duration)
        {
            //StopAllCoroutines();
            StartCoroutine(ScarecrowCoroutine(duration));
        }

        IEnumerator ScarecrowCoroutine(float duration)
        {
            isPolymorph = true;
            SetControllEnable(false);
            animator.gameObject.SetActive(false);
            var ad = PoolManager.instance.ActivateObject<PoolObjectAutoDeactivate>(_scarecrow, transform.position);
            ad.Deactive(duration);

            yield return new WaitForSeconds(duration);

            isPolymorph = false;
            SetControllEnable(true);
            animator.gameObject.SetActive(true);
        }

        protected bool IsFriendlyLayer(GameObject targetObject)
        {
            switch (targetMoveType)
            {
                case DICE_MOVE_TYPE.GROUND:
                    return targetObject.layer == LayerMask.NameToLayer(isBottomPlayer ? "BottomPlayer" : "TopPlayer");
                case DICE_MOVE_TYPE.FLYING:
                    return targetObject.layer ==
                           LayerMask.NameToLayer(isBottomPlayer ? "BottomPlayerFlying" : "TopPlayerFlying");
                case DICE_MOVE_TYPE.ALL:
                    return targetObject.layer == LayerMask.NameToLayer(isBottomPlayer ? "BottomPlayer" : "TopPlayer")
                           || targetObject.layer ==
                           LayerMask.NameToLayer(isBottomPlayer ? "BottomPlayerFlying" : "TopPlayerFlying");
                default:
                    return false;
            }
        }

        public void Cloacking(bool isCloacking)
        {
            if (isCloacking)
            {
                PoolManager.instance.ActivateObject("Effect_Cloaking", ts_HitPos.position);
                cloackingCount++;
                if (cloackingCount >= 1)
                {
                    this._isCloacking = true;
                    //_collider.enabled = false;
                    SetColor(isMine ? E_MaterialType.HALFTRANSPARENT : E_MaterialType.TRANSPARENT, ActorProxy.IsLocalPlayerAlly());
                }
            }
            else
            {
                cloackingCount--;
                if (cloackingCount <= 0)
                {
                    cloackingCount = 0;
                    this._isCloacking = false;
                    //_collider.enabled = true;
                    SetColor(isBottomPlayer ? E_MaterialType.BOTTOM : E_MaterialType.TOP, ActorProxy.IsLocalPlayerAlly());
                }
            }
        }

        public void CancelAttack()
        {
            if (_crtAttack != null) StopCoroutine((_crtAttack));
            if (_attackedTarget != null && _attackedTarget.isAlive == false) _attackedTarget = null;
            SetControllEnable(true);

            animator.SetTrigger(_animatorHashIdle);
            controller.NetSendPlayer(GameProtocol.SET_MINION_ANIMATION_TRIGGER_RELAY,
                isMine ? NetworkManager.Get().UserUID : NetworkManager.Get().OtherUID, id, (int) E_AniTrigger.Idle,
                target.id);
        }

        public void OnDeath()
        {
            _destroyed = true;
            SoundManager.instance?.Play(Global.E_SOUND.SFX_MINION_DEATH);
            PoolManager.instance.ActivateObject("Effect_Death", ts_HitPos.position);
            foreach (var autoDeactivate in _dicEffectPool)
            {
                autoDeactivate.Value.Deactive();
            }
            
            _poolObjectAutoDeactivate.Deactive();
        }
    }
}