using UnityEngine;

namespace ED
{
    public partial class Magic : BaseEntity
    {
        public enum SPAWN_TYPE
        {
            RANDOM_FIELD,
            SPAWN_POINT,
        }

        [SerializeField] public Collider collider;
        [SerializeField] protected Collider _hitCollider;

        public Vector3 targetPos;
        public Vector3 startPos;

        public int eyeLevel => ActorProxy.diceScale;

        protected const float magicLifeTime = 45.0f;
        
        protected float elapsedTime
        {
            get
            {
                if (ActorProxy == null)
                {
                    return 0;
                }
                
                return (float)ActorProxy.NetworkTime.Time - spawnTime;        
            }
        }
        protected float spawnTime;

        public virtual void Initialize(bool pIsBottomPlayer)
        {
            spawnTime = ActorProxy.spawnTime;
            if (_hitCollider != null)
            {
                var layerName =
                    $"{(pIsBottomPlayer ? "BottomPlayer" : "TopPlayer")}{(isFlying ? "Flying" : string.Empty)}";
                _hitCollider.gameObject.layer = LayerMask.NameToLayer(layerName);
            }

            objectHealthBar?.SetColor(ActorProxy.IsLocalPlayerAlly);
        }

        public void SetColor(bool isBottomCamp)
        {
            if (UI_Main.isTeamColorMode == false)
            {
                return;
            }

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
    }
}