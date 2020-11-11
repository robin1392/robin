#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using RandomWarsProtocol;

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

    public class Minion : BaseStat
    {
        public DestroyCallback destroyCallback;
        public delegate void DestroyCallback(Minion minion);

        public DICE_CAST_TYPE castType;
        public bool targetIsEnemy = true;
        public bool isAttacking;
        public bool isPushing;
        public bool isAttackSpeedFactorWithAnimation = true;
        protected float _spawnedTime;
        public float spawnedTime => _spawnedTime;
        private float _pathRefinedTime = 3f;
        private int _pathRefinedCount = 1;
        //private bool _isNexusAttacked;
        protected bool isInvincibility;
        protected bool _isCloacking;
        public bool isCloacking => _isCloacking;
        protected int cloackingCount;
        protected int invincibilityCount;
        private float _originalAttackSpeed;

        [HideInInspector] public int eyeLevel;
        [HideInInspector] public int upgradeLevel;

        private Vector3 _dodgeVelocity;
        protected static readonly int _animatorHashMoveSpeed = Animator.StringToHash("MoveSpeed");
        protected static readonly int _animatorHashIdle = Animator.StringToHash("Idle");
        protected static readonly int _animatorHashAttack = Animator.StringToHash("Attack");
        protected static readonly int _animatorHashSkill = Animator.StringToHash("Skill");

        private Coroutine _crtAttack;
        private Coroutine _crtPush;
        public BehaviourTreeOwner behaviourTreeOwner { get; protected set; }
        protected PoolObjectAutoDeactivate _poolObjectAutoDeactivate;
        public NavMeshAgent agent;
        protected Collider _collider;
        public bool isPolymorph;
        protected int _flagOfWarCount;

        protected static readonly string _scarecrow = "Scarecrow";
        protected static readonly E_BulletType _arrow = E_BulletType.ARROW;
        protected static readonly E_BulletType _spear = E_BulletType.SPEAR;

        protected Dictionary<MAZ, PoolObjectAutoDeactivate> _dicEffectPool = new Dictionary<MAZ, PoolObjectAutoDeactivate>();
        protected Shield _shield;
        protected Coroutine _invincibilityCoroutine;
        protected BaseStat _attackedTarget;

        protected virtual void Awake()
        {
            _poolObjectAutoDeactivate = GetComponent<PoolObjectAutoDeactivate>();
            behaviourTreeOwner = GetComponent<BehaviourTreeOwner>();
            agent = GetComponent<NavMeshAgent>();
            _collider = GetComponentInChildren<Collider>();
        }


        protected virtual void Update()
        {
            _spawnedTime += Time.deltaTime;
            
            if (isPlayable && isPushing == false && isAttacking == false)
            {
                //if (PhotonNetwork.IsConnected && !isMine)
                if(InGameManager.IsNetwork && !isMine && agent.enabled)
                {
                    //rb.position = Vector3.Lerp(rb.position, networkPosition, Time.fixedDeltaTime);
                    if (controller.isMinionAgentMove)
                    {
                        agent.SetDestination(networkPosition);
                    }
                    else
                    {
                        transform.LookAt(networkPosition);
                        transform.position = networkPosition;
                    }
                }

                var velocityMagnitude = rb.velocity.magnitude;
                if (animator != null)
                {
                    //animator.SetFloat(AnimatorHashMoveSpeed, velocityMagnitude);
                    animator.SetFloat(_animatorHashMoveSpeed, agent.velocity.magnitude);
                }


                //if (PhotonNetwork.IsConnected && !isMine) return;
                if (InGameManager.IsNetwork && !isMine) 
                    return;

                if (isAttacking != false || isPushing != false || target == null || !(velocityMagnitude > 0.1f)) 
                    return;
                
                var lTargetDir = rb.velocity;
                lTargetDir.y = 0.0f;
                //transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lTargetDir), Time.fixedDeltaTime * 480f);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lTargetDir), Time.deltaTime * 480f);
            }
        }
        

        public virtual void Initialize(DestroyCallback destroy)
        {
            // if (castType == DICE_CAST_TYPE.HERO)
            // {
            //     power *= Mathf.Pow(1.5f, eyeLevel - 1);
            //     maxHealth *= Mathf.Pow(2f, eyeLevel - 1);
            //     effect *= Mathf.Pow(2f, eyeLevel - 1);
            // }
            
            SetControllEnable(true);
            _dodgeVelocity = Vector3.zero;
            agent.speed = moveSpeed;
            agent.enabled = true;
            agent.isStopped = false;
            agent.updatePosition = true;
            agent.updateRotation = true;
            _collider.enabled = true;
            isPlayable = true;
            isAttacking = false;
            isPolymorph = false;
            animator.gameObject.SetActive(true);
            _spawnedTime = 0;
            _pathRefinedCount = 1;
            target = null;
            _attackedTarget = null;
            _originalAttackSpeed = attackSpeed;
            //_isNexusAttacked = false;
            currentHealth = maxHealth;
            image_HealthBar.fillAmount = 1f;
            switch (NetworkManager.Get().playType)
            {
                case Global.PLAY_TYPE.BATTLE:
                    image_HealthBar.color = isMine ? Color.green : Color.red;
                    break;
                case Global.PLAY_TYPE.COOP:
                    image_HealthBar.color = isBottomPlayer ? Color.green : Color.red;
                    break;
            }
            
            if (animator != null)
            {
                animator.SetFloat(_animatorHashMoveSpeed, 0);

                if (isAttackSpeedFactorWithAnimation)
                {
                    animator.SetFloat("AttackSpeed", 1f / attackSpeed);
                }
            }

            //if (PhotonNetwork.IsConnected)
            if(InGameManager.IsNetwork)
            {
                if (isMine || controller.isPlayingAI)
                {
                    behaviourTreeOwner.behaviour.Resume();
                    //agent.enabled = true;
                }
                else
                {
                    behaviourTreeOwner.behaviour.Pause();
                    //agent.enabled = false;
                }
            }
            
            destroyCallback = null;
            destroyCallback += destroy;
            _pathRefinedTime = Random.Range(2.5f, 3.5f);
            
            cloackingCount = 0;
            Cloacking(false);
            _dicEffectPool.Clear();

            SetColor(isBottomPlayer ? E_MaterialType.BOTTOM : E_MaterialType.TOP);
        }

        public void Heal(float heal)
        {
            if(currentHealth > 0)
            {
                currentHealth += heal;

                if(currentHealth > maxHealth)
                {
                    currentHealth = maxHealth;
                }

                PoolManager.instance.ActivateObject("Effect_Heal", transform.position);
            }

            RefreshHealthBar();
        }

        public override void HitDamage(float damage)
        {
            if (isInvincibility) return;
            if (invincibilityCount > 0)
            {
                invincibilityCount--;
                if (_shield != null && invincibilityCount == 0 && _invincibilityCoroutine == null)
                {
                    _shield.Deactive();
                    _shield = null;
                }
                return;
            }
            
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                //if (PhotonNetwork.IsConnected && !isMine)
                if(InGameManager.IsNetwork && !isMine && controller.isPlayingAI == false)
                    return;

                currentHealth = 0;
                controller.DeathMinion(id);
            }

            RefreshHealthBar();
        }

        public virtual void Death()
        {
            currentHealth = 0;
            SetControllEnable(false);
            isPlayable = false;
            if (animator != null) animator.SetFloat(_animatorHashMoveSpeed, 0);
            StopAllCoroutines();
            InGameManager.Get().RemovePlayerUnit(isBottomPlayer, this);

            destroyCallback(this);
            PoolManager.instance.ActivateObject("Effect_Death", ts_HitPos.position);
            foreach (var autoDeactivate in _dicEffectPool)
            {
                autoDeactivate.Value.Deactive();
            }
            _poolObjectAutoDeactivate.Deactive();
            
            SoundManager.instance.Play(Global.E_SOUND.SFX_MINION_DEATH);
        }

        protected void RefreshHealthBar()
        {
            image_HealthBar.fillAmount = currentHealth / maxHealth;
        }

        public virtual BaseStat SetTarget()
        {
            BaseStat defaultTarget = null;

            switch (NetworkManager.Get().playType)
            {
                case Global.PLAY_TYPE.BATTLE:
                    defaultTarget = controller.targetPlayer;
                    break;
                case Global.PLAY_TYPE.COOP:
                    defaultTarget = controller.coopPlayer;
                    break;
                default:
                    defaultTarget = null;
                    break;
            }
            
            if (isPushing)
            {
                return null;
            }

            if (_attackedTarget != null && _attackedTarget.isAlive)
            {
                var m = _attackedTarget as Minion;
                if (m != null)
                {
                    if (m.isCloacking == false) return _attackedTarget;
                }
                else
                {
                    return _attackedTarget;
                }
            }
            
            var cols = Physics.OverlapSphere(transform.position, searchRange, targetLayer);

            if (cols.Length == 0)
            {
                if (targetMoveType == DICE_MOVE_TYPE.GROUND || targetMoveType == DICE_MOVE_TYPE.ALL)
                {
                    return defaultTarget;
                }
                else
                {
                    return null;
                }
            }

            Collider firstTarget = null;
            var distance = float.MaxValue;
            foreach (var col in cols)
            {
                var bs = col.GetComponentInParent<BaseStat>();
                var m = bs as Minion;

                if (bs == null || bs.isAlive == false || (m != null && m.isCloacking))
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

            if (firstTarget == null && animator != null)
            {
                animator.SetTrigger(_animatorHashIdle);
            }

            return firstTarget ? firstTarget.GetComponentInParent<BaseStat>() : defaultTarget;
        }

        public void ChangeLayer(bool pIsBottomPlayer)
        {
            //isBottomPlayer = string.Compare(layerName, "BottomPlayer", StringComparison.Ordinal) == 0;
            isBottomPlayer = pIsBottomPlayer;
            var layerName = $"{(pIsBottomPlayer ? "BottomPlayer" : "TopPlayer")}{(isFlying ? "Flying" : string.Empty)}"; 
            gameObject.layer = LayerMask.NameToLayer(layerName);

            if (!isBottomPlayer)
            {
                transform.rotation = Quaternion.Euler(0, 180f, 0);
            }

            //if (PhotonNetwork.IsConnected)
            if(InGameManager.IsNetwork)
            {
                //switch (PhotonManager.Instance.playType)
                switch (NetworkManager.Get().playType)
                {
                    case Global.PLAY_TYPE.BATTLE:
                        image_HealthBar.color = isMine ? Color.green : Color.red;
                        break;
                    case Global.PLAY_TYPE.COOP:
                        image_HealthBar.color = Color.green;
                        break;
                }
            }
            InGameManager.Get().AddPlayerUnit(isBottomPlayer, this);
        }

        public virtual void EndGameUnit()
        {
            if (animator != null)
            {
                animator.SetFloat(_animatorHashMoveSpeed, 0);
                animator.SetTrigger(_animatorHashIdle);
            }
            isPlayable = false;
            SetControllEnable(false);
            StopAllCoroutines();
            GetComponent<NodeCanvas.BehaviourTrees.BehaviourTreeOwner>().StopBehaviour();
        }

        public void DamageToTarget(BaseStat m, float delay = 0, float factor = 1f)
        {
            if (m == null || m.isAlive == false) return;

                controller.AttackEnemyMinionOrMagic(m.UID, m.id, power * factor, delay);
                //controller.AttackEnemyMinion(m.id, power * factor, delay);
        }

        public void Push(Vector3 dir, float pushPower)
        {
            //StopAllCoroutines();
            SetControllEnable(false);
            rb.isKinematic = false;
            animator.SetTrigger(_animatorHashIdle);
            rb.velocity = Vector3.zero;
            rb.AddForce(dir.normalized * pushPower, ForceMode.Impulse);
            if(_crtPush != null) StopCoroutine(_crtPush);
            _crtPush = StartCoroutine(PushCoroutine());
        }

        private IEnumerator PushCoroutine()
        {
            yield return new WaitForSeconds(0.5f);

            var vel = rb.velocity.magnitude;
            while (vel > 0.1f)
            {
                yield return new WaitForSeconds(0.1f);
                vel = rb.velocity.magnitude;
            }
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
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
            var ad = PoolManager.instance.ActivateObject<PoolObjectAutoDeactivate>("Effect_Sturn", ts_HitPos.position + Vector3.up * 0.65f);
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

        public void SetNetworkValue(Vector3 position, float hp)//, Quaternion rotation, Vector3 velocity, float health, double sendServerTime)
        {
            networkPosition = position;
            //rb.rotation = rotation;
            //rb.velocity = velocity;
            //agent.velocity = velocity;
            currentHealth = hp;
            RefreshHealthBar();

            //var lag = Mathf.Abs((float)(PhotonNetwork.Time - sendServerTime));
            //networkPosition += rb.velocity * lag;
        }
        

        public virtual void Attack()
        {
            if (isPlayable && isPushing == false)
            {
                _attackedTarget = target;
                if (_crtAttack != null) StopCoroutine(_crtAttack);
                _crtAttack = StartCoroutine(AttackCoroutine());
            }
        }

        private IEnumerator AttackCoroutine()
        {
            //if (target != null && target.id == 0) _isNexusAttacked = true;

            //StartCoroutine(MoveToAttackInnerRanger());
            
            SetControllEnable(false);
            isAttacking = true;
            transform.LookAt(target.transform);
            
            yield return new WaitForSeconds(attackSpeed);
            // while (true)
            // {
            //     yield return null;
            //     if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            //     {
            //         break;
            //     }
            // }
            if (_attackedTarget != null && _attackedTarget.isAlive == false) _attackedTarget = null;
            isAttacking = false;
            SetControllEnable(true);
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

        public void SetVelocityTarget()
        {
            if (controller.isMinionAgentMove)
            {
                if (target != null && isAlive && agent.enabled && agent.updatePosition)
                {
                    Vector3 targetPos = target.transform.position + (target.transform.position - transform.position).normalized * range;
                    agent.SetDestination(targetPos - (targetPos - transform.position).normalized * 0.4f);
                }
            }
            else
            {
                transform.LookAt(networkPosition);
                transform.position = networkPosition;
            }
//            if (isAttacking == false && _spawnedTime > _pathRefinedTime * _pathRefinedCount && targetIsEnemy)// && dodgeVelocity == Vector3.zero)
//            {
//                _pathRefinedCount++;
//#if UNITY_EDITOR
//                Debug.DrawLine(transform.position + Vector3.up * 0.25f + (-transform.right * 0.15f),
//                    (transform.position + Vector3.up * 0.25f + (-transform.right * 0.15f)) + transform.forward * range, Color.red);
//                Debug.DrawLine(transform.position + Vector3.up * 0.25f + (transform.right * 0.15f),
//                    (transform.position + Vector3.up * 0.25f + (transform.right * 0.15f)) + transform.forward * range, Color.red);
//#endif

//                if (transform.position.x > 0)
//                {
//                    if (Physics.Raycast(transform.position + Vector3.up * 0.25f + (-transform.right * 0.15f), transform.forward, range, 1 << gameObject.layer))
//                    {
//                        _dodgeVelocity = transform.right;
//                        StartCoroutine(ResetDodgeVelocityCoroutine());
//                    }
//                    else if (Physics.Raycast(transform.position + Vector3.up * 0.25f + (transform.right * 0.15f), transform.forward, range, 1 << gameObject.layer))
//                    {
//                        _dodgeVelocity = -transform.right;
//                        StartCoroutine(ResetDodgeVelocityCoroutine());
//                    }
//                }
//                else
//                {
//                    if (Physics.Raycast(transform.position + Vector3.up * 0.25f + (transform.right * 0.15f), transform.forward, range, 1 << gameObject.layer))
//                    {
//                        _dodgeVelocity = -transform.right;
//                        StartCoroutine(ResetDodgeVelocityCoroutine());
//                    }
//                    else if (Physics.Raycast(transform.position + Vector3.up * 0.25f + (-transform.right * 0.15f), transform.forward, range, 1 << gameObject.layer))
//                    {
//                        _dodgeVelocity = transform.right;
//                        StartCoroutine(ResetDodgeVelocityCoroutine());
//                    }
//                }
//            }

//            if (target == null) return;

//            Vector3 vel;
//            if (_dodgeVelocity == Vector3.zero)
//            {
//                vel = (target.transform.position - transform.position).normalized * moveSpeed;
//            }
//            else
//            {
//                vel = (transform.forward + _dodgeVelocity).normalized * moveSpeed;
//            }
//            vel.y = 0;
//            rb.velocity = vel;
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
                (transform.position + Vector3.up * 0.1f) + (target.transform.position - transform.position).normalized * range,
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
                (transform.position + Vector3.up * 0.1f) + (target.transform.position - transform.position).normalized * range,
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
//             var hits = new RaycastHit[1];
//             var count = Physics.RaycastNonAlloc(transform.position + Vector3.up * 0.1f, target.transform.position - transform.position, hits, range, friendlyLayer);
// #if UNITY_EDITOR
//             Debug.DrawLine(transform.position + Vector3.up * 0.1f,
//                 (transform.position + Vector3.up * 0.1f) + rb.velocity * range,
//                 Color.yellow);
// #endif
            //return count > 0 ? hits[0].collider.GetComponent<BaseStat>() : null;
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
            attackSpeed = _originalAttackSpeed * factor;
            if (animator != null) animator.SetFloat("AttackSpeed", 1f / attackSpeed);
        }

        protected void SetControllEnable(bool isEnable)
        {
            isPushing = !isEnable;
            isAttacking = !isEnable;
            //rb.isKinematic = isEnable;

            if (isMine || controller.isPlayingAI)
            {
                if (isEnable && agent.enabled == false)
                {
                    agent.enabled = true;
                    agent.isStopped = false;
                    agent.updatePosition = true;
                    agent.updateRotation = true;
                }
                else if (isEnable == false && agent.enabled == true)
                {
                    agent.isStopped = true;
                    agent.updatePosition = false;
                    agent.updateRotation = false;
                    agent.enabled = false;
                }

                if (isEnable == false)
                {
                    rb.velocity = Vector3.zero;
                    agent.velocity = Vector3.zero;
                }
            }
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
                    return targetObject.layer == LayerMask.NameToLayer(isBottomPlayer ? "BottomPlayerFlying" : "TopPlayerFlying");
                case DICE_MOVE_TYPE.ALL:
                    return targetObject.layer ==  LayerMask.NameToLayer(isBottomPlayer ? "BottomPlayer" : "TopPlayer") 
                           || targetObject.layer == LayerMask.NameToLayer(isBottomPlayer ? "BottomPlayerFlying" : "TopPlayerFlying");
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
                    SetColor(isMine ? E_MaterialType.HALFTRANSPARENT : E_MaterialType.TRANSPARENT);
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
                    SetColor(isBottomPlayer ? E_MaterialType.BOTTOM : E_MaterialType.TOP);
                }
            }
        }

        public virtual void SetAnimationTrigger(string triggerName, int targetID)
        {
            var target = InGameManager.Get().GetBaseStatFromId(targetID);
            if (target != null) transform.LookAt(target.transform);
            
            animator.SetTrigger(triggerName);
        }

        public void CancelAttack()
        {
            if (_crtAttack != null) StopCoroutine((_crtAttack));
            if (_attackedTarget != null && _attackedTarget.isAlive == false) _attackedTarget = null;
            isAttacking = false;
            SetControllEnable(true);

            animator.SetTrigger(_animatorHashIdle);
            controller.NetSendPlayer(GameProtocol.SET_MINION_ANIMATION_TRIGGER_RELAY, isMine ? NetworkManager.Get().UserUID : NetworkManager.Get().OtherUID, id, (int)E_AniTrigger.Idle, target.id);
        }

        public NetSyncMinionData GetNetSyncMinionData()
        {
            NetSyncMinionData data = new NetSyncMinionData();

            data.minionId = id;
            data.minionDataId = diceId;
            data.minionHp = currentHealth;
            data.minionMaxHp = maxHealth;
            data.minionPower = power;
            data.minionEffect = effect;
            data.minionEffectUpgrade = effectUpByUpgrade;
            data.minionEffectIngameUpgrade = effectUpByInGameUp;
            data.minionDuration = effectDuration;
            data.minionCooltime = effectCooltime;
            data.minionPos = transform.position;

            return data;
        }

        public void SetNetSyncMinionData(NetSyncMinionData data)
        {
            id = data.minionId;
            if (id > controller.subSpawnCount) controller.subSpawnCount = id + 1;
            diceId = data.minionDataId;
            currentHealth = data.minionHp;
            maxHealth = data.minionMaxHp;
            RefreshHealthBar();
            power = data.minionPower;
            effect = data.minionEffect;
            effectUpByUpgrade = data.minionEffectUpgrade;
            effectUpByInGameUp = data.minionEffectIngameUpgrade;
            effectDuration = data.minionDuration;
            effectCooltime = data.minionCooltime;

            agent.enabled = false;
            transform.position = data.minionPos;
            agent.enabled = true;
        }
    }
}