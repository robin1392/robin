using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;

namespace ED
{
    public class Magic : BaseStat
    {
        public enum SPAWN_TYPE
        {
            RANDOM_FIELD,
            SPAWN_POINT,
        }

        [SerializeField]
        protected Collider _collider;
        [SerializeField]
        protected Collider _hitCollider;

        public DICE_CAST_TYPE castType;
        public SPAWN_TYPE spawnType;
        //public DICE_MOVE_TYPE targetMoveType;
        //public bool isMine;
        //public int id;
        //public PlayerController controller;
        //public Rigidbody rb;
        public DestroyCallback destroyCallback;
        public delegate void DestroyCallback(Magic magic);

        // protected int targetLayer
        // {
        //     get
        //     {
        //         switch (targetMoveType)
        //         {
        //             case DICE_MOVE_TYPE.GROUND:
        //                 return 1 << LayerMask.NameToLayer(isBottomPlayer ? "TopPlayer" : "BottomPlayer");
        //             case DICE_MOVE_TYPE.FLYING:
        //                 return 1 << LayerMask.NameToLayer(isBottomPlayer ? "TopPlayerFlying" : "BottomPlayerFlying");
        //             case DICE_MOVE_TYPE.ALL:
        //                 return 1 << LayerMask.NameToLayer(isBottomPlayer ? "TopPlayer" : "BottomPlayer") 
        //                        | 1 << LayerMask.NameToLayer(isBottomPlayer ? "TopPlayerFlying" : "BottomPlayerFlying");
        //             default:
        //                 return 0;
        //         }
        //     }
        // }
        public Vector3 targetPos;
        // public float power;
        // public float powerUpByUpgrade;
        // public float powerUpByInGameUp;
        // public float maxHealth;
        // public float currentHealth;
        // public float maxHealthUpByUpgrade;
        // public float maxHealthUpByInGameUp;
        // public float effect;
        // public float effectUpByUpgrade;
        // public float effectUpByInGameUp;
        // public float effectDuration;
        // public float effectCooltime;
        // public float attackSpeed;
        // public float moveSpeed = 2f;
        // public float range;
        // public float searchRange;
        // protected bool isBottomPlayer;
        
        public int eyeLevel;
        public int upgradeLevel;
        
        protected Coroutine destroyRoutine;
        //private Vector3 networkPosition;
        private PoolObjectAutoDeactivate poolDeactive;

        //public Material[] arrMaterial;

        [HideInInspector] public int diceFieldNum;

        protected virtual void Awake()
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

        protected void Update()
        {
            if (image_HealthBar != null)
            {
                image_HealthBar.fillAmount = currentHealth / maxHealth;
            }
        }

        public virtual void Initialize(bool pIsBottomPlayer)
        {
            currentHealth = maxHealth;
            isBottomPlayer = pIsBottomPlayer;

            destroyCallback = null;
            destroyCallback += controller.MagicDestroyCallback;

            if (image_HealthBar != null)
            {
                image_HealthBar.enabled = true;
                image_HealthBar.fillAmount = 1f;
                
                if (_hitCollider != null)
                {
                    var layerName = $"{(pIsBottomPlayer ? "BottomPlayer" : "TopPlayer")}{(isFlying ? "Flying" : string.Empty)}"; 
                    _hitCollider.gameObject.layer = LayerMask.NameToLayer(layerName);
                }
                
                StartCoroutine(LifetimeCoroutine());
            }

            if (image_HealthBar != null)
            {
                image_HealthBar.fillAmount = 1f;
                image_HealthBar.color = isMine ? Color.green : Color.red;
            }
        }

        protected void SetColor()
        {
            var isBlue = isMine;
            if (InGameManager.Get().playType == Global.PLAY_TYPE.CO_OP)
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

        public virtual void Destroy(float delay = 0)
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
            //if (PhotonNetwork.IsConnected && isMine)
            if(InGameManager.IsNetwork && isMine )
            {
                target = InGameManager.Get().GetRandomPlayerUnit(!isBottomPlayer);
                if (target != null)
                {
                    //controller.SendPlayer(RpcTarget.Others, E_PTDefine.PT_SETMAGICTARGET, id, target.id);
                    controller.ActionSetMagicTarget(id, target.id);
                    
                    StartCoroutine(Move());
                }
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false || controller.isPlayingAI)
            {
                target = InGameManager.Get().GetRandomPlayerUnit(!isBottomPlayer);
                if (target != null)
                {
                    StartCoroutine(Move());
                }
            }
        }

        protected void SetTargetPosition()
        {
            //if (PhotonNetwork.IsConnected && isMine)
            if(InGameManager.IsNetwork && isMine )
            {
                targetPos = InGameManager.Get().GetRandomPlayerFieldPosition(isBottomPlayer);
                
                //controller.SendPlayer(RpcTarget.Others , E_PTDefine.PT_SETMAGICTARGET,id, targetPos.x, targetPos.z);
                controller.ActionSetMagicTarget(id, targetPos.x, targetPos.z);
                
                StartCoroutine(Move());
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false || controller.isPlayingAI)
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
            
            controller.AttackEnemyMinionOrMagic(m.id, power * factor, delay);
        }

        // protected bool IsTargetLayer(GameObject targetObject)
        // {
        //     switch (targetMoveType)
        //     {
        //         case DICE_MOVE_TYPE.GROUND:
        //             return targetObject.layer == LayerMask.NameToLayer(isBottomPlayer ? "TopPlayer" : "BottomPlayer");
        //         case DICE_MOVE_TYPE.FLYING:
        //             return targetObject.layer == LayerMask.NameToLayer(isBottomPlayer ? "TopPlayerFlying" : "BottomPlayerFlying");
        //         case DICE_MOVE_TYPE.ALL:
        //             return targetObject.layer ==  LayerMask.NameToLayer(isBottomPlayer ? "TopPlayer" : "BottomPlayer") 
        //                    || targetObject.layer == LayerMask.NameToLayer(isBottomPlayer ? "TopPlayerFlying" : "BottomPlayerFlying");
        //         default:
        //             return false;
        //     }
        // }
        
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

        public override void HitDamage(float damage)
        {
            currentHealth -= damage;

            if (currentHealth <= 0)
            {
                //if (PhotonNetwork.IsConnected && !isMine) return;
                if (InGameManager.IsNetwork && !isMine) return;

                currentHealth = 0;
                EndLifetime();
            }
        }
        
        protected IEnumerator LifetimeCoroutine()
        {
            float t = 0;
            float lifeTime = InGameManager.Get().spawnTime; 
            while (t < lifeTime)
            {
                yield return null;
                t += Time.deltaTime;
                
                currentHealth -= (maxHealth / lifeTime) * Time.deltaTime;
                if (currentHealth <= 0)
                {
                    EndLifetime();
                    yield break;
                }
            }
            
            if (InGameManager.Get().isGamePlaying == false) yield break;

            EndLifetime();
        }

        protected virtual void EndLifetime()
        {
            StopAllCoroutines();
            controller.DeathMagic(id);
        }
    }
}