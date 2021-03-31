using Cysharp.Threading.Tasks;
using ED;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MirageTest.Scripts
{
    public class TowerActorProxy : ActorProxy
    {
        protected override void OnSpawnActor()
        {
            Spawn().Forget();
        }

        async UniTask Spawn()
        {
            var go = await Addressables.InstantiateAsync("Player", transform);
            var playerController = go.GetComponent<PlayerController>();
            baseStat = playerController;
            baseStat.ActorProxy = this;
            baseStat.id = NetId;
            playerController.isMine = IsLocalPlayerActor;
            playerController.ChangeLayer(IsBottomCamp());
            playerController.SetColor(IsBottomCamp() ? E_MaterialType.BOTTOM : E_MaterialType.TOP, IsLocalPlayerAlly);
            baseStat.SetHealthBarColor();
            isMovable = false;
            
            var client = Client as RWNetworkClient;
            EnableAI(client.IsPlayingAI);
            RefreshHpUI();
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