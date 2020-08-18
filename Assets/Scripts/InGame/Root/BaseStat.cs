using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

namespace ED
{
    [System.Serializable]
    public class BaseStat : MonoBehaviourPun
    {
        public bool isMine;
        public int id;
        public PlayerController controller;
        public Rigidbody rb;
        public Animator animator;
        public Material[] arrMaterial;

        [Header("Base Stat")]
        public DICE_MOVE_TYPE targetMoveType;
        public float power;
        public float powerUpByUpgrade;
        public float powerUpByInGameUp;
        public float maxHealth;
        public float currentHealth;
        public float maxHealthUpByUpgrade;
        public float maxHealthUpByInGameUp;
        public float effect;
        public float effectUpByUpgrade;
        public float effectUpByInGameUp;
        public float attackSpeed;
        public float moveSpeed;
        public float range;
        public float searchRange = 2f;
        public bool isBottomPlayer;

        [Header("Positions")] 
        public Transform ts_ShootingPos;
        public Transform ts_HitPos;

        [Header("UI Link")]
        public Image image_HealthBar;
        public Text text_Health;

        public bool isPlayable = true;
        public bool isAlive => currentHealth > 0;

        private static readonly string GROUND_TAG_NAME = "Minion_Ground";
        private static readonly string FLYING_TAG_NAME = "Minion_Flying";
        public bool isFlying => gameObject.CompareTag(FLYING_TAG_NAME);
        
        //public int targetLayer => 1 << LayerMask.NameToLayer(isBottomPlayer ? "TopPlayer" : "BottomPlayer");
        public int targetLayer
        {
            get
            {
                switch (targetMoveType)
                {
                    case DICE_MOVE_TYPE.GROUND:
                        return 1 << LayerMask.NameToLayer(isBottomPlayer ? "TopPlayer" : "BottomPlayer");
                    case DICE_MOVE_TYPE.FLYING:
                        return 1 << LayerMask.NameToLayer(isBottomPlayer ? "TopPlayerFlying" : "BottomPlayerFlying");
                    case DICE_MOVE_TYPE.ALL:
                        return 1 << LayerMask.NameToLayer(isBottomPlayer ? "TopPlayer" : "BottomPlayer") 
                               | 1 << LayerMask.NameToLayer(isBottomPlayer ? "TopPlayerFlying" : "BottomPlayerFlying");
                    default:
                        return 0;
                }
            }
        }

        //public int friendlyLayer => 1 << LayerMask.NameToLayer(isBottomPlayer ? "BottomPlayer" : "TopPlayer");
        public int friendlyLayer
        {
            get
            {
                switch (targetMoveType)
                {
                    case DICE_MOVE_TYPE.GROUND:
                        return 1 << LayerMask.NameToLayer(isBottomPlayer ? "BottomPlayer" : "TopPlayer");
                    case DICE_MOVE_TYPE.FLYING:
                        return 1 << LayerMask.NameToLayer(isBottomPlayer ? "BottomPlayerFlying" : "TopPlayerFlying");
                    case DICE_MOVE_TYPE.ALL:
                        return 1 << LayerMask.NameToLayer(isBottomPlayer ? "BottomPlayer" : "TopPlayer") 
                               | 1 << LayerMask.NameToLayer(isBottomPlayer ? "BottomPlayerFlying" : "TopPlayerFlying");
                    default:
                        return 0;
                }
            }
        }

        protected Vector3 networkPosition = Vector3.zero;

        protected virtual void Start() { }
        
        protected void SetColor(E_MaterialType type)
        {
            var mr = GetComponentsInChildren<MeshRenderer>();
            foreach (var m in mr)
            {
                m.material = arrMaterial[isMine ? 0 : 1];
                switch (type)
                {
                    case E_MaterialType.BOTTOM:
                    case E_MaterialType.TOP:
                        Color c = m.material.color;
                        c.a = 1f;
                        m.material.color = c;
                        break;
                    case E_MaterialType.HALFTRANSPARENT:
                    case E_MaterialType.TRANSPARENT:
                        c = m.material.color;
                        c.a = 0.3f;
                        m.material.color = c;
                        break;
                }
            }
            var smr = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var m in smr)
            {
                m.material = arrMaterial[isMine ? 0 : 1];
                switch (type)
                {
                    case E_MaterialType.BOTTOM:
                    case E_MaterialType.TOP:
                        Color c = m.material.color;
                        c.a = 1f;
                        m.material.color = c;
                        break;
                    case E_MaterialType.HALFTRANSPARENT:
                    case E_MaterialType.TRANSPARENT:
                        c = m.material.color;
                        c.a = 0.3f;
                        m.material.color = c;
                        break;
                }
            }
        }

        //[PunRPC]
        public virtual void HitDamage(float damage, float delay = 0) { }

        protected bool IsTargetLayer(GameObject targetObject)
        {
            switch (targetMoveType)
            {
                case DICE_MOVE_TYPE.GROUND:
                    return targetObject.layer == LayerMask.NameToLayer(isBottomPlayer ? "TopPlayer" : "BottomPlayer");
                case DICE_MOVE_TYPE.FLYING:
                    return targetObject.layer == LayerMask.NameToLayer(isBottomPlayer ? "TopPlayerFlying" : "BottomPlayerFlying");
                case DICE_MOVE_TYPE.ALL:
                    return targetObject.layer ==  LayerMask.NameToLayer(isBottomPlayer ? "TopPlayer" : "BottomPlayer") 
                           || targetObject.layer == LayerMask.NameToLayer(isBottomPlayer ? "TopPlayerFlying" : "BottomPlayerFlying");
                default:
                    return false;
            }
        }
    }
}