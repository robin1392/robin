using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ED;
using Quantum;
using RandomWarsResource.Data;
using UnityEngine;
using Dice = Quantum.Dice;

public class DiceActorView : QuantumCallbacks
{
    public EntityView EntityView;

    private bool _initializing = false;
    private bool _initialized = false;

    void Init(QuantumGame game)
    {
        
        // if (_initializing)
        // {
        //     // return;
        // }
        //
        // _initializing = true;
        //
        // try
        // {
        //     var f = game.Frames.Verified;
        //     var actor = f.Get<Actor>(EntityView.EntityRef);
        //     var localPlayer = game.GetLocalPlayers()[0];
        //     var isEnemy = f.Global->Players[localPlayer].Team == actor.Team;
        //     var isLocalPlayerActor = actor.Owner == localPlayer;
        //
        //     var dice = f.Get<Dice>(EntityView.EntityRef);
        //     TableManager.Get().DiceInfo.GetData(dice.DiceInfoId, out var diceInfo);
        //     // diceInfo.prefabName
        //
        //     if (isLocalPlayerActor)
        //     {
        //         var setting = UI_DiceField.Get().arrSlot[dice.FieldIndex].ps.main;
        //         setting.startColor = FileHelper.GetColor(diceInfo.color);
        //         UI_DiceField.Get().arrSlot[dice.FieldIndex].ps.Play();
        //     }
        //
        //     if (diceInfo.castType == (int) DICE_CAST_TYPE.MINION || diceInfo.castType == (int) DICE_CAST_TYPE.HERO)
        //     {
        //         await SpawnMinionOrHero(diceInfo);
        //     }
        //     else if (diceInfo.castType == (int) DICE_CAST_TYPE.MAGIC ||
        //              diceInfo.castType == (int) DICE_CAST_TYPE.INSTALLATION)
        //     {
        //         // await SpawnMagicAndInstallation();
        //     }
        //
        //     _initialized = true;
        //     _initializing = false;
        // }
        // catch (System.Exception)
        // {
        //     return;
        // }
    }

    async UniTask SpawnMinionOrHero(TDataDiceInfo diceInfo)
    {
        PoolManager.instance.ActivateObject("particle_necromancer", transform.position);
        

        // if (minion != null)
        // {
        //     minion.transform.localPosition = Vector3.zero;
        //     minion.transform.localRotation = Quaternion.identity;
        //     minion.Initialize();
        //     minion.castType = (DICE_CAST_TYPE) diceInfo.castType;
        //     minion.id = NetId;
        //     minion.isMine = IsLocalPlayerActor;
        //     minion.targetMoveType = (DICE_MOVE_TYPE) diceInfo.targetMoveType;
        //     minion.ChangeLayer(IsBottomCamp());
        // }
        //
        // if (client.enableUI)
        // {
        //     ShowSpawnLine(minion);
        // }

        SoundManager.instance.Play(Global.E_SOUND.SFX_MINION_GENERATE);
    }

    void ShowSpawnLine(Minion minion)
    {
        // if (ownerTag == GameConstants_Mirage.ServerTag)
        // {
        //     return;
        // }
        //
        // var dicePos = UI_DiceField.Get().arrSlot[spawnSlot].transform.position;
        // if ((CameraController.Get().IsBottomOrientation && team == GameConstants_Mirage.TopCamp) ||
        //     CameraController.Get().IsBottomOrientation == false && team == GameConstants_Mirage.BottomCamp)
        // {
        //     dicePos.x *= -1f;
        //     dicePos.z *= -1f;
        // }
        //
        // var lr = PoolManager.instance.ActivateObject<LineRenderer>("Effect_SpawnLine", Vector3.zero);
        // if (lr == null)
        // {
        //     var pool = PoolManager.instance.data.listPool.Find(data => data.obj.name == "Effect_SpawnLine");
        //     PoolManager.instance.AddPool(pool.obj, 1);
        //     lr = PoolManager.instance.ActivateObject<LineRenderer>("Effect_SpawnLine", Vector3.zero);
        // }
        //
        // if (lr != null)
        // {
        //     lr.SetPositions(new Vector3[2] {dicePos, minion.ts_HitPos.position});
        //     lr.startColor = FileHelper.GetColor(diceInfo.color);
        //     lr.endColor = FileHelper.GetColor(diceInfo.color);
        // }
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
    // public override void OnUpdateView(QuantumGame game)
    // {
    //     var f = game.Frames.Verified;
    //     if (EntityView.EntityRef == EntityRef.None)
    //     {
    //         return;
    //     }
    //
    //     if (_initialized == false && _initializing == false)
    //     {
    //         Init(game).Forget();
    //         return;
    //     }
    //
    //     var unitFields = f.Get<UnitFields>(EntityView.EntityRef);
    //
    //     if (!unitFields.IsOnBoard || !f.Global->Players[unitFields.OwnerID].Fighting)
    //     {
    //         if (_unitUIView.gameObject.activeSelf)
    //         {
    //             _unitUIView.gameObject.SetActive(false);
    //         }
    //
    //         return;
    //     }
    //
    //     if (!_unitUIView.gameObject.activeSelf)
    //     {
    //         _unitUIView.gameObject.SetActive(true);
    //     }
    //
    //     var unitSpec = f.FindAsset<UnitSpec>(unitFields.Spec.Id);
    //     var healthAmount = (float) unitFields.CurrentHealth / unitSpec.Levels[unitFields.Level].MaxHealth;
    //     _unitUIView.SetupHealthBar(healthAmount);
    // }
}