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
    public class TowerActorView : QuantumCallbacks
    {
        public EntityView EntityView;

        private bool _initializing = false;
        private bool _initialized = false;

        private Image _healthBarImage;
        private Text _healthBarText;

        async UniTask Init(QuantumGame game)
        {
            _initializing = true;
        
            try
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
                
                var go = await ResourceManager.LoadGameObjectAsync(isEnemy ? "TowerRed" : "TowerBlue", Vector3.zero, Quaternion.identity);
                go.transform.SetParent(transform, false);

                _initialized = true;
                _initializing = false;
            }
            catch (System.Exception)
            {
                return;
            }
        }
        
        public override void OnUpdateView(QuantumGame game)
        {
            var f = game.Frames.Verified;
            if (EntityView.EntityRef == EntityRef.None)
            {
                return;
            }

            if (_initialized == false && _initializing == false)
            {
                Init(game).Forget();
                return;
            }

            if (_initialized == false)
            {
                return;
            }

            var e = EntityView.EntityRef;
            var actor = f.Get<Actor>(e);
            var ratio = (actor.Health / actor.MaxHealth).AsFloat;
            _healthBarImage.fillAmount = ratio;
            _healthBarText.text = $"{FPMath.CeilToInt(actor.Health)}";

            var simulatedTr2D = f.Get<Transform2D>(e);
            var tr = transform;
            tr.position = new Vector3(simulatedTr2D.Position.X.AsFloat, 0, simulatedTr2D.Position.Y.AsFloat);
            tr.eulerAngles = new Vector3(0, simulatedTr2D.Rotation.AsFloat, 0);
            
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
    }
}