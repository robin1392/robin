using ED;
using UnityEngine;

namespace MirageTest.Scripts
{
    public class TowerActorProxy : ActorProxy
    {
        protected override void OnSpawnActor()
        {
            //KZSee: 검토
            var towerPrefab = Resources.Load<PlayerController>("Tower/Player");
            var playerController = Instantiate(towerPrefab, transform);
            baseStat = playerController;
            baseStat.ActorProxy = this;
            baseStat.id = NetId;
            playerController.isMine = IsLocalPlayerActor;
            playerController.ChangeLayer(IsBottomCamp());
            playerController.SetColor(IsBottomCamp() ? E_MaterialType.BOTTOM : E_MaterialType.TOP, IsLocalPlayerAlly());
            baseStat.SetHealthBarColor();
            isMovable = false;
        }

        protected override void OnApplyDamageOnServer()
        {
            var server = Server as RWNetworkServer;
            server.serverGameLogic.OnHitDamageTower(this);
        }
    }
}