using System;
using MirageTest.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace ED
{
    [System.Serializable]
    public partial class BaseStat : MonoBehaviour
    {
        public ActorProxy ActorProxy;
        public bool isMine;
        public bool isCanBeTarget = true;
        public int diceId;
        public uint id;
        public Animator animator;
        public Material[] arrMaterial;
        public BaseStat target;

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

        [Header("UI Link")] public Image image_HealthBar;
        public Text text_Health;

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
            if (!isAlive)
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

        protected Vector3 networkPosition = Vector3.zero;
        protected MeshRenderer[] arrMeshRenderer;
        protected SkinnedMeshRenderer[] arrSkinnedMeshRenderer;

        public PoolObjectAutoDeactivate _poolObjectAutoDeactivate;

        protected virtual void Awake()
        {
            _poolObjectAutoDeactivate = GetComponent<PoolObjectAutoDeactivate>();
        }

        protected virtual void Start()
        {
        }

        public virtual void OnBaseStatDestroyed()
        {
            StopAllAction();
            _poolObjectAutoDeactivate?.Deactive();
        }

        public void UpdateHealthBar()
        {
            if (image_HealthBar == null)
            {
                return;
            }

            image_HealthBar.color = ActorProxy.IsLocalPlayerAlly ? Color.green : Color.red;
            if (ActorProxy.IsLocalPlayerActor)
            {
                var pos = image_HealthBar.transform.parent.parent.localPosition;
                pos.y *= -1;
                image_HealthBar.transform.parent.parent.localPosition = pos;
            }
        }

        public virtual void ChangeLayer(bool pIsBottomPlayer)
        {
            var layerName = $"{(pIsBottomPlayer ? "BottomPlayer" : "TopPlayer")}{(isFlying ? "Flying" : string.Empty)}";
            gameObject.layer = LayerMask.NameToLayer(layerName);
        }

        public virtual void SetColor(E_MaterialType type, bool isAlly)
        {
            if (arrMeshRenderer == null)
            {
                arrMeshRenderer = GetComponentsInChildren<MeshRenderer>();
            }

            var mat = arrMaterial[isAlly ? 0 : 1];

            foreach (var m in arrMeshRenderer)
            {
                if (m.gameObject.CompareTag("Finish")) continue;

                m.material = mat;

                switch (type)
                {
                    case E_MaterialType.BOTTOM:
                    case E_MaterialType.TOP:
                        Color c = m.material.color;
                        c.a = 1f;
                        m.material.color = c;
                        break;
                    case E_MaterialType.HALFTRANSPARENT:
                        c = m.material.color;
                        c.a = 0.2f;
                        m.material.color = c;
                        break;
                    case E_MaterialType.TRANSPARENT:
                        c = m.material.color;
                        c.a = 0.1f;
                        m.material.color = c;
                        break;
                }
            }

            if (arrSkinnedMeshRenderer == null)
            {
                arrSkinnedMeshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();
            }

            foreach (var m in arrSkinnedMeshRenderer)
            {
                m.material = mat;

                switch (type)
                {
                    case E_MaterialType.BOTTOM:
                    case E_MaterialType.TOP:
                        Color c = m.material.color;
                        c.a = 1f;
                        m.material.color = c;
                        break;
                    case E_MaterialType.HALFTRANSPARENT:
                        c = m.material.color;
                        c.a = 0.2f;
                        m.material.color = c;
                        break;
                    case E_MaterialType.TRANSPARENT:
                        c = m.material.color;
                        c.a = 0.1f;
                        m.material.color = c;
                        break;
                }
            }
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

        public virtual void OnHitDamageOnClient(float damage)
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
    }
}