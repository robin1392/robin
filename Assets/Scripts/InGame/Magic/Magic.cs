using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;

namespace ED
{
    public class Magic : MonoBehaviourPun
    {
        public enum SPAWN_TYPE
        {
            RANDOM_FIELD,
            SPAWN_POINT,
        }

        public SPAWN_TYPE spawnType;
        public DICE_MOVE_TYPE targetMoveType;
        public bool isMine;
        public int id;
        public PlayerController controller;
        public Rigidbody rb;
        public DestroyCallback destroyCallback;
        public delegate void DestroyCallback(Magic magic);

        protected int targetLayer
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
        protected float moveSpeed = 2f;
        public BaseStat target;
        public Vector3 targetPos;
        protected float damage;
        protected bool isBottomPlayer;
        public float range;
        public float searchRange;
        public float attackSpeed;
        public int eyeLevel;
        public int upgradeLevel;

        protected Coroutine destroyRoutine;
        private Vector3 networkPosition;
        private PoolObjectAutoDeactivate poolDeactive;

        public Material[] arrMaterial;

        [HideInInspector] public int diceFieldNum;

        private void Awake()
        {
            if (rb  == null)
            {
                rb = GetComponent<Rigidbody>();
            }
            if (poolDeactive == null)
            {
                poolDeactive = GetComponent<PoolObjectAutoDeactivate>();
            }
        }

        public virtual void Initialize(bool pIsBottomPlayer, float pDamage, float pMoveSpeed = 1f)
        {
            this.isBottomPlayer = pIsBottomPlayer;
            this.damage = pDamage;
            this.moveSpeed = pMoveSpeed;
            //targetLayer = 1 << LayerMask.NameToLayer(pIsBottomPlayer ? "TopPlayer" : "BottomPlayer");

            destroyCallback = null;
            destroyCallback += controller.MagicDestroyCallback;
        }

        protected void SetColor()
        {
            var isBlue = isMine;
            if (InGameManager.Get().playType == PLAY_TYPE.CO_OP)
            {
                isBlue = isBottomPlayer;
            }

            var mr = GetComponentsInChildren<MeshRenderer>();
            foreach (var m in mr)
            {
                m.material = arrMaterial[isBlue ? 0 : 1];
            }
            var smr = GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (var m in smr)
            {
                m.material = arrMaterial[isBlue ? 0 : 1];
            }
        }

        protected void Destroy(float delay = 0)
        {
            if (destroyRoutine != null) StopCoroutine(destroyRoutine);
            destroyRoutine = StartCoroutine(DestroyCoroutine(delay));
        }

        private IEnumerator DestroyCoroutine(float delay = 0)
        {
            destroyCallback(this);

            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }

            poolDeactive.Deactive();
        }

        public void SetNetworkValue(Vector3 position, Vector3 velocity, double sentServerTime)
        {
            networkPosition = position;
            if (rb != null) rb.velocity = velocity;

            var lag = Mathf.Abs((float)(PhotonNetwork.Time - sentServerTime));
            networkPosition += rb.velocity * lag;
        }

        public virtual void SetTarget() { SetTargetBaseStat(); }

        private void SetTargetBaseStat()
        {
            if (PhotonNetwork.IsConnected && isMine)
            {
                target = InGameManager.Get().GetRandomPlayerUnit(!isBottomPlayer);
                //controller.photonView.RPC("SetMagicTarget", RpcTarget.Others, id, target.id);
                controller.SendPlayer(RpcTarget.Others , E_PTDefine.PT_SETMAGICTARGET,id, target.id);
                StartCoroutine(Move());
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                target = InGameManager.Get().GetRandomPlayerUnit(!isBottomPlayer);
                StartCoroutine(Move());
            }
        }

        protected void SetTargetPosition()
        {
            if (PhotonNetwork.IsConnected && isMine)
            {
                targetPos = InGameManager.Get().GetRandomPlayerFieldPosition(isBottomPlayer);
                //controller.photonView.RPC("SetMagicTarget", RpcTarget.Others, id, targetPos.x, targetPos.z);
                controller.SendPlayer(RpcTarget.Others , E_PTDefine.PT_SETMAGICTARGET,id, targetPos.x, targetPos.z);
                StartCoroutine(Move());
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                targetPos = InGameManager.Get().GetRandomPlayerFieldPosition(isBottomPlayer);
                StartCoroutine(Move());
            }
        }

        public void SetTarget(int id)
        {
            target = controller.targetPlayer.GetBaseStatFromId(id);
            StartCoroutine(Move());
        }

        public void SetTarget(float x, float z)
        {
            this.targetPos = new Vector3(x, 0, z);
            StartCoroutine(Move());
        }

        protected virtual IEnumerator Move() { yield return null; }
        
        public void DamageToTarget(BaseStat m, float delay = 0, float factor = 1f)
        {
            if (m == null) return;
            
            controller.AttackEnemyMinion(m.id, damage * factor, delay);
        }

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
    }
}