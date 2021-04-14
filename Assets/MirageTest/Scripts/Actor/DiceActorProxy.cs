using ED;
using Mirage.Logging;
using RandomWarsResource.Data;
using UnityEngine;
using Debug = ED.Debug;

namespace MirageTest.Scripts
{
    public class DiceActorProxy : ActorProxy
    {
        private TDataDiceInfo _diceInfo;
        public TDataDiceInfo diceInfo
        {
            get
            {
                if (_diceInfo == null)
                {
                    TableManager.Get().DiceInfo.GetData(dataId, out _diceInfo);
                }

                return _diceInfo;
            }
        }
        
        protected override void OnSpawnActor()
        {
            var client = Client as RWNetworkClient;
            if (client.enableUI && IsLocalPlayerActor)
            {
                var setting = UI_DiceField.Get().arrSlot[spawnSlot].ps.main;
                setting.startColor = FileHelper.GetColor(diceInfo.color);
                UI_DiceField.Get().arrSlot[spawnSlot].ps.Play();
            }

            if (diceInfo.castType == (int) DICE_CAST_TYPE.MINION || diceInfo.castType == (int) DICE_CAST_TYPE.HERO)
            {
                SpawnMinionOrHero();
            }
            else if (diceInfo.castType == (int) DICE_CAST_TYPE.MAGIC ||
                     diceInfo.castType == (int) DICE_CAST_TYPE.INSTALLATION)
            {
                SpawnMagicAndInstallation();
            }
            else
            {
                _logger.LogError($"잘못된 CastType입니다. {diceInfo.castType.ToString()} - id:{dataId}");
                return;
            }

            baseEntity.effectUpgrade = diceInfo.effectUpgrade;
            baseEntity.effectInGameUp = diceInfo.effectInGameUp;
            baseEntity.effectDuration = diceInfo.effectDuration;
            baseEntity.effectCooltime = diceInfo.effectCooltime;
            baseEntity.range = diceInfo.range;
            baseEntity.searchRange = diceInfo.searchRange;
        }
        
        void SpawnMinionOrHero()
        {
            var client = Client as RWNetworkClient;
            PoolManager.instance.ActivateObject("particle_necromancer", transform.position);
            var minion = PoolManager.instance.ActivateObject<Minion>(diceInfo.prefabName, Vector3.zero, transform);
            if (minion == null)
            {
                PoolManager.instance.AddPool(
                    FileHelper.LoadPrefab(diceInfo.prefabName, Global.E_LOADTYPE.LOAD_MINION), 1);
                //Debug.LogFormat("{0} Pool Added 1", data.prefabName);
                //포지션을 원점으로 주고 있지만, StartClient 이후에 포지션 미라지 네트워크아이덴티티의 포지션 동기화가 이루어져 서버상의 위치로 바뀐다. 
                minion = PoolManager.instance.ActivateObject<Minion>(diceInfo.prefabName, Vector3.zero, transform);
            }

            if (minion != null)
            {
                baseEntity = minion;
                minion.ActorProxy = this;
                minion.transform.localPosition = Vector3.zero;
                minion.transform.localRotation = Quaternion.identity;
                minion.SetPathFinding(_seeker, _aiPath);
                minion.Initialize();
                minion.castType = (DICE_CAST_TYPE) diceInfo.castType;
                minion.id = NetId;
                minion.isMine = IsLocalPlayerActor;
                minion.targetMoveType = (DICE_MOVE_TYPE) diceInfo.targetMoveType;
                minion.ChangeLayer(IsBottomCamp());
            }

            if (client.enableUI)
            {
                ShowSpawnLine(minion);
            }
            
            SoundManager.instance.Play(Global.E_SOUND.SFX_MINION_GENERATE);
        }

        void ShowSpawnLine(Minion minion)
        {
            if (ownerTag == GameConstants.ServerTag)
            {
                return;
            }
            
            var dicePos = UI_DiceField.Get().arrSlot[spawnSlot].transform.position;
            if((CameraController.Get().IsBottomOrientation && team == GameConstants.TopCamp) ||
               CameraController.Get().IsBottomOrientation == false && team == GameConstants.BottomCamp)
            {
                dicePos.x *= -1f;
                dicePos.z *= -1f;
            }

            var lr = PoolManager.instance.ActivateObject<LineRenderer>("Effect_SpawnLine", Vector3.zero);
            if (lr == null)
            {
                var pool = PoolManager.instance.data.listPool.Find(data => data.obj.name == "Effect_SpawnLine");
                PoolManager.instance.AddPool(pool.obj, 1);
                lr = PoolManager.instance.ActivateObject<LineRenderer>("Effect_SpawnLine", Vector3.zero);
            }

            if (lr != null)
            {
                lr.SetPositions(new Vector3[2] {dicePos, minion.ts_HitPos.position});
                lr.startColor = FileHelper.GetColor(diceInfo.color);
                lr.endColor = FileHelper.GetColor(diceInfo.color);
            }
        }
        
        void SpawnMagicAndInstallation()
        {
            var client = Client as RWNetworkClient;
            var magicSpawnPosition = GetMagicSpawnPosition(diceInfo.enableDice);
            var magic = PoolManager.instance.ActivateObject<Magic>(diceInfo.prefabName, Vector3.zero, transform);
            if (magic == null)
            {
                GameObject loadMagic = FileHelper.LoadPrefab(diceInfo.prefabName, Global.E_LOADTYPE.LOAD_MAGIC);
                PoolManager.instance.AddPool(loadMagic, 1);
                magic = PoolManager.instance.ActivateObject<Magic>(diceInfo.prefabName, magicSpawnPosition, transform);
            }

            if (magic != null)
            {
                baseEntity = magic;
                
                if (diceInfo.castType == (int) DICE_CAST_TYPE.MAGIC)
                {
                    transform.position = magicSpawnPosition;
                    magic.transform.localPosition = Vector3.zero;
                    magic.transform.localRotation = Quaternion.identity;
                }
                //지뢰의 경우 서버에서 설치위치를 설정한다. 이동 전에 서버에서 설정한 설치위치를 저장
                else if ( diceInfo.id == 1005)
                {
                    magic.targetPos = transform.position;
                    magic.startPos = magicSpawnPosition;
                    transform.position = magicSpawnPosition;
                }
                
                magic.transform.localPosition = Vector3.zero;
                magic.transform.localRotation = Quaternion.identity;
                
                magic.ActorProxy = this;
                magic.isMine = IsLocalPlayerActor;
                magic.targetMoveType = (DICE_MOVE_TYPE) diceInfo.targetMoveType;
                magic.id = NetId;
                magic.SetColor(IsBottomCamp());
                magic.ChangeLayer(IsBottomCamp());
                magic.Initialize(IsBottomCamp());
            }
            else
            {
                _logger.LogError($"리소스가 존재하지 않습니다. - {diceInfo.prefabName} - id:{dataId}");
            }
            
            isMovable = false;
        }

        public Vector3 GetMagicSpawnPosition(bool isEnableDice)
        {
            if (isEnableDice == false) return transform.position;
            
            Vector3 spawnPosition = UI_DiceField.Get().arrSlot[spawnSlot].transform.position;
            if ((Client as RWNetworkClient).enableUI)
            {
                if(IsLocalPlayerAlly == false)
                {
                    spawnPosition.x *= -1f;
                    spawnPosition.z *= -1f;
                }    
            }
            else if(team != GameConstants.BottomCamp)
            {
                spawnPosition.x *= -1f;
                spawnPosition.z *= -1f;
            }

            return spawnPosition;
        }
    }
}