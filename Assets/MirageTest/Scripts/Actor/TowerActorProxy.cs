using System;
using ED;
using Pathfinding;
using UnityEngine;

namespace MirageTest.Scripts
{
    public class TowerActorProxy : ActorProxy
    {
        protected override void OnSpawnActor()
        {
            //KZSee: 검토
            var towerPrefab = Resources.Load<Tower>("Tower/Tower");
            var playerController = Instantiate(towerPrefab, transform);
            baseEntity = playerController;
            baseEntity.ActorProxy = this;
            baseEntity.id = NetId;
            playerController.isMine = IsLocalPlayerActor;
            playerController.ChangeLayer(IsBottomCamp());
            playerController.SetColor(IsBottomCamp() ? E_MaterialType.BOTTOM : E_MaterialType.TOP, IsLocalPlayerAlly);
            baseEntity.SetHealthBarColor();

            var client = Client as RWNetworkClient;
            if (client.enableUI)
            {
                if (IsLocalPlayerAlly)
                {
                    UI_InGame.Get().image_BottomTowerHealthBar.transform.parent.gameObject.SetActive(true);
                    playerController.image_HealthBar = UI_InGame.Get().image_BottomTowerHealthBar;
                    playerController.text_Health = UI_InGame.Get().text_BottomTowerHealthBar;
                }
                else
                {
                    UI_InGame.Get().image_TopTowerHealthBar.transform.parent.gameObject.SetActive(true);
                    playerController.image_HealthBar = UI_InGame.Get().image_TopTowerHealthBar;
                    playerController.text_Health = UI_InGame.Get().text_TopTowerHealthBar;
                }
            }

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