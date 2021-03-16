using ED;
using UnityEngine;

namespace MirageTest.Scripts
{
    public partial class ActorProxy
    {
        void SpawnMagicAndInstallation()
        {
            var client = Client as RWNetworkClient;
            var magicSpawnPosition = GetMagicSpawnPosition();
            var magic = PoolManager.instance.ActivateObject<Magic>(diceInfo.prefabName, Vector3.zero, transform);
            if (magic == null)
            {
                GameObject loadMagic = FileHelper.LoadPrefab(diceInfo.prefabName, Global.E_LOADTYPE.LOAD_MAGIC);
                PoolManager.instance.AddPool(loadMagic, 1);
                magic = PoolManager.instance.ActivateObject<Magic>(diceInfo.prefabName, magicSpawnPosition, transform);
            }

            if (magic != null)
            {
                baseStat = magic;
                
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
            
            isMovable = false;
        }

        public Vector3 GetMagicSpawnPosition()
        {
            Vector3 spawnPosition = UI_DiceField.Get().arrSlot[spawnSlot].transform.position;
            if ((Client as RWNetworkClient).enableUI)
            {
                if(IsLocalPlayerAlly() == false)
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