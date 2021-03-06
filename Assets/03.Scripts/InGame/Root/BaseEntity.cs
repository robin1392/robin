using System;
using MirageTest.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace ED
{
    //TODO: 비즈니스 로지을 모두 덜어낸다. AI는 볼트로 이전하고, 모델에 붙어있는 위치 트랜스폼이나, 이펙트를 참조하는 역할로만 사용한다. 그리고 ModelReferenceHolder 정도로 리네임
    [System.Serializable]
    public partial class BaseEntity : MonoBehaviour
    {
        public ActorProxy ActorProxy;
        public bool isMine;
        public bool isCanBeTarget = true;
        public uint id;
        public Animator animator;
        public Material[] arrMaterial;
        public BaseEntity target;

        [Header("Base Stat")] public DICE_MOVE_TYPE targetMoveType;

        public float power => ActorProxy.power;
        public float effect => ActorProxy.effect;
        public float effectUpgrade;
        public float effectInGameUp;
        public float effectDuration;
        public float effectCooltime;
        public float range;
        public float searchRange;

        public float attackSpeed => ActorProxy.attackSpeed;
        public float moveSpeed => ActorProxy.moveSpeed;

        public bool isBottomCamp => ActorProxy.IsBottomCamp();

        [Header("Positions")] public Transform ts_ShootingPos;
        public Transform ts_HitPos;
        public Transform ts_TopEffectPosition;
        
        // public Transform ts_TopEffectPosition;

        [Header("UI Link")] public Image image_HealthBar;
        public UI_ObjectHealthBar objectHealthBar;
        public Text text_Health;
        public SpriteRenderer sr_Shadow;

        public RendererEffect RendererEffect;

        public bool IsExtracted => _extractedOnGameEnd;
        private bool _extractedOnGameEnd = false;

        public bool isAlive
        {
            get
            {
                if (ActorProxy == null)
                {
                    return false;
                }

                return ActorProxy.currentHealth > 0;
            }
        }

        public bool IsHpFull
        {
            get
            {
                if (ActorProxy == null)
                {
                    return false;
                }

                return ActorProxy.currentHealth >= ActorProxy.maxHealth;
            }
        }

        private static readonly string GROUND_TAG_NAME = "Minion_Ground";
        private static readonly string FLYING_TAG_NAME = "Minion_Flying";
        public bool isFlying => gameObject.CompareTag(FLYING_TAG_NAME);

        public int targetLayer
        {
            get
            {
                switch (targetMoveType)
                {
                    case DICE_MOVE_TYPE.GROUND:
                        return 1 << LayerMask.NameToLayer(isBottomCamp ? "TopPlayer" : "BottomPlayer");
                    case DICE_MOVE_TYPE.FLYING:
                        return 1 << LayerMask.NameToLayer(isBottomCamp ? "TopPlayerFlying" : "BottomPlayerFlying");
                    case DICE_MOVE_TYPE.ALL:
                        return 1 << LayerMask.NameToLayer(isBottomCamp ? "TopPlayer" : "BottomPlayer")
                               | 1 << LayerMask.NameToLayer(isBottomCamp ? "TopPlayerFlying" : "BottomPlayerFlying");
                    default:
                        return 0;
                }
            }
        }

        public bool CanBeTarget()
        {
            if (isCanBeTarget == false)
            {
                return false;
            }
            
            if (!isAlive)
            {
                return false;
            }
            
            if (ActorProxy == null)
            {
                return false;
            }

            if (this is Minion minion)
            {
                return !minion.isCloacking;
            }

            return true;
        }

        public int friendlyLayer
        {
            get
            {
                switch (targetMoveType)
                {
                    case DICE_MOVE_TYPE.GROUND:
                        return 1 << LayerMask.NameToLayer(isBottomCamp ? "BottomPlayer" : "TopPlayer");
                    case DICE_MOVE_TYPE.FLYING:
                        return 1 << LayerMask.NameToLayer(isBottomCamp ? "BottomPlayerFlying" : "TopPlayerFlying");
                    case DICE_MOVE_TYPE.ALL:
                        return 1 << LayerMask.NameToLayer(isBottomCamp ? "BottomPlayer" : "TopPlayer")
                               | 1 << LayerMask.NameToLayer(isBottomCamp ? "BottomPlayerFlying" : "TopPlayerFlying");
                    default:
                        return 0;
                }
            }
        }

        public virtual float Radius => 0;

        protected MeshRenderer[] arrMeshRenderer;
        protected SkinnedMeshRenderer[] arrSkinnedMeshRenderer;

        public PoolObjectAutoDeactivate _poolObjectAutoDeactivate;

        protected virtual void Awake()
        {
            _poolObjectAutoDeactivate = GetComponent<PoolObjectAutoDeactivate>();
            objectHealthBar = GetComponentInChildren<UI_ObjectHealthBar>();
            RendererEffect = new RendererEffect(gameObject);
        }

        protected virtual void Start()
        {
        }

        public virtual void OnBaseEntityDestroyed()
        {
            StopAllAction();
            _poolObjectAutoDeactivate?.Deactive();
            ActorProxy = null;
        }

        public virtual void ChangeLayer(bool pIsBottomPlayer)
        {
            var layerName = $"{(pIsBottomPlayer ? "BottomPlayer" : "TopPlayer")}{(isFlying ? "Flying" : string.Empty)}";
            gameObject.layer = LayerMask.NameToLayer(layerName);
        }

        public virtual void SetAnimationTrigger(string triggerName, uint targetId)
        {
            var targetTransform = ActorProxy.ClientObjectManager[targetId]?.transform;
            SetAnimationTrigger(triggerName, targetTransform);
        }

        public void SetAnimationTrigger(string triggerName, Transform targetTransform)
        {
            if (targetTransform != null) transform.LookAt(targetTransform);
            animator.SetTrigger(triggerName);
        }

        public virtual void OnHitDamageOnClient(float damage, DamageType damageType)
        {
        }

        protected bool IsTargetLayer(GameObject targetObject)
        {
            switch (targetMoveType)
            {
                case DICE_MOVE_TYPE.GROUND:
                    return targetObject.layer == LayerMask.NameToLayer(isBottomCamp ? "TopPlayer" : "BottomPlayer");
                case DICE_MOVE_TYPE.FLYING:
                    return targetObject.layer ==
                           LayerMask.NameToLayer(isBottomCamp ? "TopPlayerFlying" : "BottomPlayerFlying");
                case DICE_MOVE_TYPE.ALL:
                    return targetObject.layer == LayerMask.NameToLayer(isBottomCamp ? "TopPlayer" : "BottomPlayer")
                           || targetObject.layer ==
                           LayerMask.NameToLayer(isBottomCamp ? "TopPlayerFlying" : "BottomPlayerFlying");
                default:
                    return false;
            }
        }

        public virtual bool OnBeforeHitDamage(float damage)
        {
            var cancel = false;
            return cancel;
        }

        public virtual float ModifyDamage(float damage)
        {
            return damage;
        }

        public void ExtractOnGameSessionEnd()
        {
            _extractedOnGameEnd = true;
            StopAllAction();
            transform.SetParent(null, true);
        }
    }
}