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
            playerController.SetColor(IsBottomCamp() ? E_MaterialType.BOTTOM : E_MaterialType.TOP, IsLocalPlayerAlly);
            baseStat.UpdateHealthBar();
            isMovable = false;
        }

        protected override void OnApplyDamageOnServer()
        {
            var server = Server as RWNetworkServer;
            server.serverGameLogic.OnHitDamageTower(this);
        }

        protected override void OnSetHp()
        {
            base.OnSetHp();
            
            if (currentHealth >= maxHealth * 0.1f)
            {
                return;
            }

            if (IsLocalPlayerAlly == false)
            {
                return;
            }

            var client = Client as RWNetworkClient;
            if (client.enableUI == false)
            {
                return;
            }

            if (UI_InGamePopup.Get().IsLowHpEffectActiveSelf())
            {
                return;
            }
            
            UI_InGamePopup.Get().ShowLowHPEffect(true);
        }
    }
}