using Cysharp.Threading.Tasks;
using ED;
using Quantum;
using RandomWarsResource.Data;
using UnityEngine;
using Dice = Quantum.Dice;

namespace _Scripts.Views
{
    public class DiceActorView : ActorView
    {
        protected override async UniTask OnInit(QuantumGame game)
        {
            var f = game.Frames.Verified;
            var actor = f.Get<Actor>(EntityView.EntityRef);
            var localPlayer = game.GetLocalPlayers()[0];
            var isEnemy = f.AreEachOtherEnemy(actor, localPlayer);
            var isLocalPlayerActor = actor.Owner == localPlayer;
        
            var dice = f.Get<Dice>(EntityView.EntityRef);
            TableManager.Get().DiceInfo.GetData(dice.DiceInfoId, out var diceInfo);

            if (isLocalPlayerActor)
            {
                var setting = UI_DiceField.Get().arrSlot[dice.FieldIndex].ps.main;
                setting.startColor = FileHelper.GetColor(diceInfo.color);
                UI_DiceField.Get().arrSlot[dice.FieldIndex].ps.Play();
            }
        
            if (diceInfo.castType == (int) DICE_CAST_TYPE.MINION || diceInfo.castType == (int) DICE_CAST_TYPE.HERO)
            {
                await SpawnMinionOrHero(diceInfo, isLocalPlayerActor, dice.FieldIndex, actor.Team, !isEnemy);
            }
            else if (diceInfo.castType == (int) DICE_CAST_TYPE.MAGIC ||
                     diceInfo.castType == (int) DICE_CAST_TYPE.INSTALLATION)
            {
                // await SpawnMagicAndInstallation();
            }
        }

        private async UniTask SpawnMinionOrHero(TDataDiceInfo diceInfo, bool isLocalPlayerActor, int fieldIndex, int team, bool isAlly)
        {
            var assetName = "particle_necromancer";
            var spawnEffectGo = await ResourceManager.LoadGameObjectAsync(assetName, transform.position, Quaternion.identity);
            var spawnEffectPo = spawnEffectGo.GetComponent<PoolableObject>();
            spawnEffectPo.AssetName = assetName;
            spawnEffectPo.ReservePushBack();

            ActorModel = await ResourceManager.LoadMonobehaviourAsync<ActorModel>(diceInfo.prefabName, Vector3.zero, Quaternion.identity);
            ActorModel.Initialize(isAlly);
            ActorModel.transform.SetParent(transform, false);

            if (isLocalPlayerActor)
            {
                await ShowSpawnLine(diceInfo, ActorModel, fieldIndex, team);
            }

            SoundManager.instance.Play(Global.E_SOUND.SFX_MINION_GENERATE);
        }

        async UniTask ShowSpawnLine(TDataDiceInfo diceInfo, ActorModel actorModel, int fieldIndex, int team)
        {
            var dicePos = UI_DiceField.Get().arrSlot[fieldIndex].transform.position;
            if ((CameraController.Get().IsBottomOrientation && team == GameConstants.TopCamp) ||
                CameraController.Get().IsBottomOrientation == false && team == GameConstants.BottomCamp)
            {
                dicePos.x *= -1f;
                dicePos.z *= -1f;
            }

            var assetName = "Effect_SpawnLine";
            var lrGo = await ResourceManager.LoadGameObjectAsync(assetName, Vector3.zero, Quaternion.identity);
            var poolable = lrGo.GetComponent<PoolableObject>();
            poolable.AssetName = assetName;
            poolable.ReservePushBack();
        
            var lr = lrGo.GetComponent<LineRenderer>();
            if (lr != null)
            {
                lr.SetPositions(new Vector3[2] {dicePos, actorModel.HitPosition.position});
                lr.startColor = FileHelper.GetColor(diceInfo.color);
                lr.endColor = FileHelper.GetColor(diceInfo.color);
            }
        }
    

        // void SpawnMagicAndInstallation()
        // {
        //     var client = Client as RWNetworkClient;
        //     var magicSpawnPosition = GetMagicSpawnPosition(diceInfo.enableDice);
        //     var magic = PoolManager.instance.ActivateObject<Magic>(diceInfo.prefabName, Vector3.zero, transform);
        //     if (magic == null)
        //     {
        //         GameObject loadMagic = FileHelper.LoadPrefab(diceInfo.prefabName, Global.E_LOADTYPE.LOAD_MAGIC);
        //         PoolManager.instance.AddPool(loadMagic, 1);
        //         magic = PoolManager.instance.ActivateObject<Magic>(diceInfo.prefabName, magicSpawnPosition, transform);
        //     }
        //
        //     if (magic != null)
        //     {
        //         baseEntity = magic;
        //         magic.ActorProxy = this;
        //         if (diceInfo.castType == (int) DICE_CAST_TYPE.MAGIC)
        //         {
        //             transform.position = magicSpawnPosition;
        //             magic.transform.localPosition = Vector3.zero;
        //             magic.transform.localRotation = Quaternion.identity;
        //         }
        //         //TODO: 아래 동작의 타입을 정의하고 데이터 컬럼을 추가해서 제어한다.
        //         //지뢰의 경우 서버에서 설치위치를 설정한다. 이동 전에 서버에서 설정한 설치위치를 저장
        //         else if (diceInfo.id == 1005 || diceInfo.id == 1013)
        //         {
        //             magic.targetPos = transform.position;
        //             magic.startPos = magicSpawnPosition;
        //             transform.position = magicSpawnPosition;
        //         }
        //
        //         magic.transform.localPosition = Vector3.zero;
        //         magic.transform.localRotation = Quaternion.identity;
        //
        //         magic.ActorProxy = this;
        //         magic.isMine = IsLocalPlayerActor;
        //         magic.targetMoveType = (DICE_MOVE_TYPE) diceInfo.targetMoveType;
        //         magic.id = NetId;
        //         magic.SetColor(IsBottomCamp());
        //         magic.ChangeLayer(IsBottomCamp());
        //         magic.Initialize(IsBottomCamp());
        //     }
        //     else
        //     {
        //         _logger.LogError($"리소스가 존재하지 않습니다. - {diceInfo.prefabName} - id:{dataId}");
        //     }
        //
        //     isMovable = false;
        // }
        //
        // public Vector3 GetMagicSpawnPosition(bool isEnableDice)
        // {
        //     if (isEnableDice == false) return transform.position;
        //
        //     Vector3 spawnPosition = UI_DiceField.Get().arrSlot[spawnSlot].transform.position;
        //     if ((Client as RWNetworkClient).enableUI)
        //     {
        //         if (IsLocalPlayerAlly == false)
        //         {
        //             spawnPosition.x *= -1f;
        //             spawnPosition.z *= -1f;
        //         }
        //     }
        //     else if (team != GameConstants_Mirage.BottomCamp)
        //     {
        //         spawnPosition.x *= -1f;
        //         spawnPosition.z *= -1f;
        //     }
        //
        //     return spawnPosition;
        // }
        //

        protected override void OnUpdateViewAfterInit(QuantumGame game)
        {
            var f = game.Frames.Verified;
            var e = EntityView.EntityRef;
            var actor = f.Get<Actor>(e);
            ActorModel.image_HealthBar.fillAmount = (actor.Health / actor.MaxHealth).AsFloat;
        }
    }
}