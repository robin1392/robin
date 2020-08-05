#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Random = UnityEngine.Random;

namespace ED
{
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

        public bool targetIsEnemy = true;
        public bool isAttacking;
        public bool isPushing;
        public BaseStat target;
        protected float _spawnedTime;
        private float _pathRefinedTime = 3f;
        private int _pathRefinedCount = 1;
        //private bool _isNexusAttacked;
        protected bool isInvincibility;
        protected int invincibilityCount;
        private float _originalAttackSpeed;

        [HideInInspector] public int eyeLevel;
        [HideInInspector] public int upgradeLevel;

        private Vector3 _dodgeVelocity;
        protected static readonly int _animatorHashMoveSpeed = Animator.StringToHash("MoveSpeed");
        protected static readonly int _animatorHashIdle = Animator.StringToHash("Idle");
        protected static readonly int _animatorHashAttack = Animator.StringToHash("Attack");
        protected static readonly int _animatorHashSkill = Animator.StringToHash("Skill");

        private Coroutine _crtPush;
        private BehaviourTreeOwner _behaviourTreeOwner;
        protected PoolObjectAutoDeactivate _poolObjectAutoDeactivate;
        public NavMeshAgent agent;
        protected Collider _collider;

        protected void Awake()
        {
            _poolObjectAutoDeactivate = GetComponent<PoolObjectAutoDeactivate>();
            _behaviourTreeOwner = GetComponent<BehaviourTreeOwner>();
            agent = GetComponent<NavMeshAgent>();
            _collider = GetComponentInChildren<Collider>();
        }

        protected virtual void FixedUpdate()
        {
            if (isPlayable && isPushing == false && isAttacking == false)
            {
                if (PhotonNetwork.IsConnected && !isMine)
                {
                    //rb.position = Vector3.Lerp(rb.position, networkPosition, Time.fixedDeltaTime);
                    agent.SetDestination(networkPosition);
                }

                var velocityMagnitude = rb.velocity.magnitude;
                if (animator != null)
                {
                    //animator.SetFloat(AnimatorHashMoveSpeed, velocityMagnitude);
                    animator.SetFloat(_animatorHashMoveSpeed, agent.velocity.magnitude);
                }

                _spawnedTime += Time.fixedDeltaTime;

                if (PhotonNetwork.IsConnected && !isMine) return;

                if (isAttacking != false || isPushing != false || target == null || !(velocityMagnitude > 0.1f)) return;
                var lTargetDir = rb.velocity;
                lTargetDir.y = 0.0f;
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lTargetDir), Time.fixedDeltaTime * 480f);
            }
        }

        public virtual void Initialize(DestroyCallback destroy)
        {
            SetControllEnable(true);
            _dodgeVelocity = Vector3.zero;
            agent.speed = moveSpeed;
            if (animator != null) animator.SetFloat(_animatorHashMoveSpeed, 0);
            isPlayable = true;
            isAttacking = false;
            _spawnedTime = 0;
            _pathRefinedCount = 1;
            target = null;
            _originalAttackSpeed = attackSpeed;
            //_isNexusAttacked = false;
            currentHealth = maxHealth;

            if (PhotonNetwork.IsConnected)
            {
                if (isMine)
                {
                    _behaviourTreeOwner.behaviour.Resume();
                    //agent.enabled = true;
                }
                else
                {
                    _behaviourTreeOwner.behaviour.Pause();
                    //agent.enabled = false;
                }
            }
            
            destroyCallback = null;
            destroyCallback += destroy;
            _pathRefinedTime = Random.Range(2.5f, 3.5f);

            SetColor();
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

        public override void HitDamage(float damage, float delay = 0)
        {
            if (isInvincibility) return;
            if (invincibilityCount > 0)
            {
                invincibilityCount--;
                return;
            }
            
            StartCoroutine(HitDamageCoroutine(damage, delay));
        }

        private IEnumerator HitDamageCoroutine(float damage, float delay)
        {
            if (delay > 0) yield return new WaitForSeconds(delay);

            if (currentHealth > 0)
            {
                currentHealth -= damage;

                if (currentHealth <= 0)
                {
                    if (PhotonNetwork.IsConnected && !isMine) yield break;

                    currentHealth = 0;
                    controller.DeathMinion(id);
                }
            }

            RefreshHealthBar();
        }

        public virtual void Death()
        {
            SetControllEnable(false);
            isPlayable = false;
            if (animator != null) animator.SetFloat(_animatorHashMoveSpeed, 0);
            StopAllCoroutines();
            InGameManager.Instance.RemovePlayerUnit(isBottomPlayer, this);

            destroyCallback(this);
            PoolManager.instance.ActivateObject("Effect_Death", hitPos.position);
            _poolObjectAutoDeactivate.Deactive();
        }

        private void RefreshHealthBar()
        {
            image_HealthBar.fillAmount = currentHealth / maxHealth;
        }

        public virtual BaseStat SetTarget()
        {
            if (isPushing)
            {
                return null;
            }
            
            var cols = Physics.OverlapSphere(transform.position, range * 3f, targetLayer);

            if (cols.Length == 0)
            {
                if (targetMoveType == DICE_MOVE_TYPE.GROUND || targetMoveType == DICE_MOVE_TYPE.ALL)
                {
                    return controller.targetPlayer;
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
                if (!col) continue;
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

            return firstTarget ? firstTarget.GetComponentInParent<BaseStat>() : controller.targetPlayer;
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

            if (PhotonNetwork.IsConnected)
            {
                switch (PhotonManager.Instance.playType)
                {
                    case PLAY_TYPE.BATTLE:
                        image_HealthBar.color = isMine ? Color.green : Color.red;
                        break;
                    case PLAY_TYPE.CO_OP:
                        image_HealthBar.color = Color.green;
                        break;
                    default:
                        break;
                }
            }
            InGameManager.Instance.AddPlayerUnit(isBottomPlayer, this);
        }

        public void EndGameUnit()
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
            if (m == null) return;
            // if (PhotonNetwork.IsConnected && isMine)
            // {
            //     if (m.photonView == null)
            //         controller.AttackEnemyMinion(m.id, power * factor, delay);
            //     else
            //         m.photonView.RPC("HitDamage", RpcTarget.All, power * factor, delay);
            // }
            // else if (PhotonNetwork.IsConnected == false)
            {
                controller.AttackEnemyMinion(m.id, power * factor, delay);
            }
        }

        public void Push(Vector3 dir, float pushPower)
        {
            rb.velocity = Vector3.zero;
            rb.AddForce(dir.normalized * pushPower, ForceMode.Impulse);
            SetControllEnable(false);
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
            SetControllEnable(true);
        }

        public virtual void Sturn(float duration)
        {
            StopAllCoroutines();
            if (animator != null) animator.SetTrigger(_animatorHashIdle);
            
            if(_crtPush != null) StopCoroutine(_crtPush);
            _crtPush = StartCoroutine(SturnCoroutine(duration));
        }

        private IEnumerator SturnCoroutine(float duration)
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            SetControllEnable(false);
            yield return new WaitForSeconds(duration);
            SetControllEnable(true);
            rb.isKinematic = false;
        }

        public void Invincibility(float time)
        {
            StartCoroutine(InvincibilityCoroutine(time));
        }

        public void Invincibility(int addCount)
        {
            invincibilityCount += addCount;
        }

        private IEnumerator InvincibilityCoroutine(float time)
        {
            isInvincibility = true;
            yield return new WaitForSeconds(time);
            isInvincibility = false;
        }

        public void SetNetworkValue(Vector3 position, Quaternion rotation, Vector3 velocity, float health, double sendServerTime)
        {
            networkPosition = position;
            rb.rotation = rotation;
            rb.velocity = velocity;
            //agent.velocity = velocity;
            currentHealth = health;

            var lag = Mathf.Abs((float)(PhotonNetwork.Time - sendServerTime));
            networkPosition += rb.velocity * lag;
        }

        public virtual void Attack()
        {
            if (isPlayable && isPushing == false)
            {
                StartCoroutine(AttackCoroutine());
            }
        }

        private IEnumerator AttackCoroutine()
        {
            //if (target != null && target.id == 0) _isNexusAttacked = true;
            SetControllEnable(false);
            isAttacking = true;
            transform.LookAt(target.transform);
            yield return new WaitForSeconds(attackSpeed);
            isAttacking = false;
            SetControllEnable(true);
            
        }
        
        public bool IsTargetAlive()
        {
            return target != null && target.isAlive;
        }

        public void SetVelocityTarget()
        {
            //if (_agent.velocity.magnitude < 0.1f)
            if (target != null)
            {
                //_agent.isStopped = false;
                //_agent.updatePosition = true;
                //_agent.updateRotation = true;
                agent.SetDestination(target.transform.position + Vector3.right * Random.Range(-0.2f, 0.2f) +
                                      Vector3.forward * Random.Range(-0.2f, 0.2f));
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
            var hits = Physics.RaycastAll(transform.position + Vector3.up * 0.1f,
                (target.transform.position - transform.position).normalized, range, targetLayer);
            foreach (var hit in hits)
            {
                if (hit.collider.GetComponentInParent<BaseStat>() == target) return true;
            }

            return false;
        }

        public BaseStat IsFriendlyTargetInnerRange()
        {
            var hits = new RaycastHit[1];
            var count = Physics.RaycastNonAlloc(transform.position + Vector3.up * 0.1f, target.transform.position - transform.position, hits, range, friendlyLayer);
#if UNITY_EDITOR
            Debug.DrawLine(transform.position + Vector3.up * 0.1f,
                (transform.position + Vector3.up * 0.1f) + rb.velocity * range,
                Color.yellow);
#endif
            return count > 0 ? hits[0].collider.GetComponent<BaseStat>() : null;
        }

        public void SetAttackSpeedFactor(float factor)
        {
            attackSpeed = _originalAttackSpeed / factor;
            if (animator != null) animator.speed = factor;
        }

        protected void SetControllEnable(bool isEnable)
        {
            isPushing = !isEnable;
            //_collider.enabled = isEnable;
            //rb.isKinematic = !isEnable;

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
}