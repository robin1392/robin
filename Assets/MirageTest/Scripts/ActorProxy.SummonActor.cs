using Cysharp.Threading.Tasks.Triggers;
using ED;
using ED.SummonActor;
using UnityEngine;

namespace MirageTest.Scripts
{
    public partial class ActorProxy
    {
        void SpawnSummonActor()
        {
            var client = Client as RWNetworkClient;
            var summonActorInfo = SummonActorInfos.GetSummonActorInfo(dataId);
            var summoned = PoolManager.instance.ActivateObject<SummonActor>(summonActorInfo.prefab, transform.position, transform);
            if (summoned != null)
            {
                baseStat = summoned;
                summoned.ActorProxy = this;
                var summonedTr = summoned.transform;
                summonedTr.localPosition = Vector3.zero;
                summonedTr.localRotation = Quaternion.identity;
                summoned.Initialize();
                summoned.controller = (Client as RWNetworkClient).GetTower(ownerTag);
                summoned.id = NetId;
                summoned.isMine = IsLocalPlayerActor;
                summoned.targetMoveType = summonActorInfo.targetMoveType;
                summoned.ChangeLayer(IsBottomCamp());
            }
        }

    }
}