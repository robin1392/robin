using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace ED
{
    public partial class Magic : BaseStat
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

        public DICE_CAST_TYPE castType => (DICE_CAST_TYPE)ActorProxy.diceInfo.castType;

        public Vector3 targetPos;
        public Vector3 startPos;
        
        public int eyeLevel => ActorProxy.diceScale;
        public int upgradeLevel => ActorProxy.ingameUpgradeLevel;
        
        protected Coroutine destroyRoutine;
        
        public int diceFieldNum => ActorProxy.spawnSlot;

        public virtual void Initialize(bool pIsBottomPlayer)
        {
            SetHealthBarColor();
        }

        public void SetColor(bool isBottomCamp)
        {
            var mr = GetComponentsInChildren<MeshRenderer>(true);
            foreach (var m in mr)
            {
                m.material = arrMaterial[isBottomCamp ? 0 : 1];
            }
            var smr = GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var m in smr)
            {
                m.material = arrMaterial[isBottomCamp ? 0 : 1];
            }
        }

        public virtual void Destroy(float delay = 0)
        {
            if (destroyRoutine != null) StopCoroutine(destroyRoutine);
            destroyRoutine = StartCoroutine(DestroyCoroutine(delay));
        }

        private IEnumerator DestroyCoroutine(float delay = 0)
        {
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }

            // poolDeactive.Deactive();
        }

        // public void SetNetworkValue(Vector3 position, Vector3 velocity, double sentServerTime)
        // {
        //     networkPosition = position;
        //     if (rb != null) rb.velocity = velocity;
        //
        //     var lag = Mathf.Abs((float)(PhotonNetwork.Time - sentServerTime));
        //     networkPosition += rb.velocity * lag;
        // }

        public virtual void SetTarget() { SetTargetBaseStat(); }

        private void SetTargetBaseStat()
        {
            //if (PhotonNetwork.IsConnected && isMine)
            if(InGameManager.IsNetwork && (isMine || controller.isPlayingAI) )
            {
                target = InGameManager.Get().GetRandomPlayerUnit(!isBottomCamp);
                if (target != null)
                {
                    //controller.SendPlayer(RpcTarget.Others, E_PTDefine.PT_SETMAGICTARGET, id, target.id);
                    controller.ActionSetMagicTarget(id, target.id);
                    
                    StartCoroutine(Activate());
                }
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false)
            {
                target = InGameManager.Get().GetRandomPlayerUnit(!isBottomCamp);
                if (target != null)
                {
                    StartCoroutine(Activate());
                }
            }
        }

        protected void SetTargetPosition()
        {
            //if (PhotonNetwork.IsConnected && isMine)
            if(InGameManager.IsNetwork && (isMine || controller.isPlayingAI))
            {
                targetPos = InGameManager.Get().GetRandomPlayerFieldPosition(isBottomCamp);
                
                //controller.SendPlayer(RpcTarget.Others , E_PTDefine.PT_SETMAGICTARGET,id, targetPos.x, targetPos.z);
                controller.ActionSetMagicTarget(id, targetPos.x, targetPos.z);
                
                StartCoroutine(Activate());
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false)
            {
                targetPos = InGameManager.Get().GetRandomPlayerFieldPosition(isBottomCamp);
                StartCoroutine(Activate());
            }
        }

        public void SetTarget(uint id)
        {
            target = ActorProxy.GetBaseStatWithNetId(id);
            StartCoroutine(Activate());
        }

        public void SetTarget(float x, float z)
        {
            this.targetPos = new Vector3(x, 0, z);
            StartCoroutine(Activate());
        }

        protected virtual IEnumerator Activate() { yield return null; }
        
        public void DamageToTarget(BaseStat m, float delay = 0, float factor = 1f)
        {
            if (m == null) return;
            
            controller.AttackEnemyMinionOrMagic(m.UID, m.id, power * factor, delay);
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
                    return targetObject.layer == LayerMask.NameToLayer(isBottomCamp ? "BottomPlayer" : "TopPlayer");
                case DICE_MOVE_TYPE.FLYING:
                    return targetObject.layer == LayerMask.NameToLayer(isBottomCamp ? "BottomPlayerFlying" : "TopPlayerFlying");
                case DICE_MOVE_TYPE.ALL:
                    return targetObject.layer ==  LayerMask.NameToLayer(isBottomCamp ? "BottomPlayer" : "TopPlayer") 
                           || targetObject.layer == LayerMask.NameToLayer(isBottomCamp ? "BottomPlayerFlying" : "TopPlayerFlying");
                default:
                    return false;
            }
        }

        public override void HitDamage(float damage)
        {
            
            if (ActorProxy.currentHealth <= 0)
            {
                //if (PhotonNetwork.IsConnected && !isMine) return;
                if (InGameManager.IsNetwork && !isMine) return;

                ActorProxy.currentHealth = 0;
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
                
                //KZSee: health와 라이프타임을 조합해야 할 듯 하다. 계속 체력이 줄기때문에
                // currentHealth -= (maxHealth / lifeTime) * Time.deltaTime;
                // if (currentHealth <= 0)
                {
                    EndLifetime();
                    break;
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