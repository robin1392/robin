using ED;
using UnityEngine;

namespace MirageTest.Scripts
{
    public partial class ActorProxy
    {
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
                baseStat = minion;
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
            var dicePos = UI_DiceField.Get().arrSlot[spawnSlot].transform.position;
            //로컬플레이어의 진영이 하단에 위치하도록 카메라가 회전된다. 상대방 플레이어의 스폰라인정 시작점은 로컬플레이어 필드 유아이의 정반대로 계산한다. 
            if(!IsLocalPlayerActor)
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
    }
}