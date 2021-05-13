using _Scripts.Resourcing;
using Cysharp.Threading.Tasks;
using ED;
using Photon.Deterministic;
using Quantum;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.UI;
using Dice = Quantum.Dice;

namespace _Scripts.Views
{
    public class TowerActorView : ActorView
    {
        private Image _healthBarImage;
        private Text _healthBarText;

        protected override void OnInit(QuantumGame game)
        {
            var f = game.Frames.Verified;
            var actor = f.Get<Actor>(EntityView.EntityRef);
            var localPlayer = game.GetLocalPlayers()[0];
            var isEnemy = f.AreEachOtherEnemy(actor, localPlayer);

            if (isEnemy)
            {
                UI_InGame.Get().image_TopTowerHealthBar.transform.parent.gameObject.SetActive(true);
                _healthBarImage = UI_InGame.Get().image_TopTowerHealthBar;
                _healthBarText = UI_InGame.Get().text_TopTowerHealthBar;
            }
            else
            {
                UI_InGame.Get().image_BottomTowerHealthBar.transform.parent.gameObject.SetActive(true);
                _healthBarImage = UI_InGame.Get().image_BottomTowerHealthBar;
                _healthBarText = UI_InGame.Get().text_BottomTowerHealthBar;
            }
                
            ActorModel = PreloadedResourceManager.LoadPoolable<ActorModel>(isEnemy ? AssetNames.TowerRed : AssetNames.TowerBlue, Vector3.zero, Quaternion.identity);
            ActorModel.transform.SetParent(transform, false);
        }

        protected override void OnUpdateViewAfterInit(QuantumGame game)
        {
            var f = game.Frames.Predicted;
            var e = EntityView.EntityRef;
            if(f.TryGet(e, out Health health) == false)
            {
                return;
            }
            
            var actor = f.Get<Actor>(e);
            var ratio = (health.Value / health.MaxValue).AsFloat;
            _healthBarImage.fillAmount = ratio;
            _healthBarText.text = $"{FPMath.CeilToInt(health.Value)}";

            var simulatedTr2D = f.Get<Transform2D>(e);
            var tr = transform;
            tr.position = new Vector3(simulatedTr2D.Position.X.AsFloat, 0, simulatedTr2D.Position.Y.AsFloat);
            tr.eulerAngles = new Vector3(0, simulatedTr2D.Rotation.AsFloat, 0);
            
            //TODO: 이벤트로 뺀다.
            if (f.AreEachOtherEnemy(actor, game.GetLocalPlayers()[0]))
            {
                return;
            }
                
            if (ratio >= 0.1f)
            {
                return;
            }
            
            if (UI_InGamePopup.Get().IsLowHpEffectActiveSelf())
            {
                return;
            }
            
            UI_InGamePopup.Get().ShowLowHPEffect(true);
        }

        protected override void OnEntityDestroyedInternal(QuantumGame game)
        {
            _healthBarImage?.transform.parent.gameObject.SetActive(false);
            _healthBarImage = null;
            _healthBarText = null;
            
            ResourceManager.LoadGameObjectAsyncAndReseveDeacivate("Effect_Bomb", transform.position, Quaternion.identity).Forget();
            ResourceManager.LoadGameObjectAsyncAndReseveDeacivate("Effect_TowerDestroyed", transform.position, Quaternion.identity).Forget();
            
            SoundManager.instance.Play(Global.E_SOUND.SFX_INGAME_TOWER_EXPLOSION);
            base.OnEntityDestroyedInternal(game);
        }
    }
}