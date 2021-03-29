using ED;
using Mirage;
using Mirage.Logging;
using RandomWarsResource.Data;
using UnityEngine;

namespace MirageTest.Scripts
{
    public class BossActorProxy : ActorProxy
    {
        [SyncVar(hook = nameof(SetIsHatched))] public bool isHatched;
        [SyncVar] public int waveSpawned;
        [SyncVar] public int bossIndex;

        private TDataCoopModeBossInfo _bossInfo;
        protected GameObject _egg;

        public TDataCoopModeBossInfo bossInfo
        {
            get
            {
                if (_bossInfo == null)
                {
                    TableManager.Get().CoopModeBossInfo.GetData(dataId, out _bossInfo);
                }

                return _bossInfo;
            }
        }
        
        protected override void OnSpawnActor()
        {
             if (bossInfo == null)
            {
                _logger.LogError($"보스데이터가 없습니다. {dataId}");
                return;
            }

            Vector3 pos = transform.position;
            PoolManager.instance.ActivateObject("particle_necromancer", transform.position);
            var boss =
                PoolManager.instance.ActivateObject<Minion>(bossInfo.prefabName, Vector3.zero, transform);
            if (boss == null)
            {
                PoolManager.instance.AddPool(
                    FileHelper.LoadPrefab(bossInfo.prefabName, Global.E_LOADTYPE.LOAD_COOP_BOSS), 1);
                boss = PoolManager.instance.ActivateObject<Minion>(bossInfo.prefabName, Vector3.zero,
                    transform);
            }

            baseStat = boss;
            boss.ActorProxy = this;
            boss.transform.localPosition = Vector3.zero;
            boss.transform.localRotation = Quaternion.identity;
            boss.SetPathFinding(_seeker, _aiPath);
            boss.Initialize();
            boss.castType = (DICE_CAST_TYPE) bossInfo.castType;
            boss.id = NetId;
            boss.isMine = IsLocalPlayerActor;
            boss.targetMoveType = (DICE_MOVE_TYPE) bossInfo.targetMoveType;
            boss.ChangeLayer(IsBottomCamp());
                
            boss.effectUpgrade = bossInfo.effectUpgrade;
            boss.effectInGameUp = bossInfo.effectInGameUp;
            boss.effectDuration = bossInfo.effectDuration;
            boss.effectCooltime = bossInfo.effectCooltime;
            boss.range = bossInfo.range;
            boss.searchRange = bossInfo.searchRange;
            
            SetIsHatched(isHatched, isHatched);
        }

        void SetIsHatched(bool oldValue, bool newValue)
        {
            //서버에서 생성 할 때 변수에 값을 넣으면 클라이언트에서 hook이 startClient보다 먼저 불린다.
            if (baseStat == null)
            {
                return;
            }
            
            baseStat.animator.gameObject.SetActive(isHatched);
            
            if (newValue == false)
            {
                if (_egg == null)
                {
                    var obj = FileHelper.LoadPrefab(
                        $"{bossInfo.prefabName}_Egg",
                        Global.E_LOADTYPE.LOAD_COOP_BOSS);

                    _egg = Instantiate(obj, transform);
                    _egg.transform.localPosition = Vector3.zero;
                    _egg.transform.localRotation = Quaternion.identity;
                    _egg.transform.localScale = Vector3.one * 100;
                    var layerName = $"{(IsBottomCamp() ? "BottomPlayer" : "TopPlayer")}";
                    gameObject.layer = LayerMask.NameToLayer(layerName);   
                }
            }
            else
            {
                if (_egg != null)
                {
                    Destroy(_egg);
                    _egg = null;
                }
            }
        }
    }
}