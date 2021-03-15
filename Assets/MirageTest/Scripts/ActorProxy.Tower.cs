using ED;
using UnityEngine;

namespace MirageTest.Scripts
{
    public partial class ActorProxy
    {
        void SpawnTower()
        {
            var towerPrefab = Resources.Load<PlayerController>("Tower/Player");
            var playerController = Instantiate(towerPrefab, transform);
            baseStat = playerController;
            baseStat.ActorProxy = this;
            baseStat.id = NetId;
            playerController.isMine = IsLocalPlayerActor;
            playerController.ChangeLayer(IsBottomCamp(), IsLocalPlayerAlly());
            playerController.SetColor(IsBottomCamp() ? E_MaterialType.BOTTOM : E_MaterialType.TOP, IsLocalPlayerAlly());
            isMovable = false;
        }
    }
}