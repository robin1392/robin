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
            var m = PoolManager.instance.ActivateObject<Minion>(diceInfo.prefabName, Vector3.zero, transform);
            if (m == null)
            {
                PoolManager.instance.AddPool(
                    FileHelper.LoadPrefab(diceInfo.prefabName, Global.E_LOADTYPE.LOAD_MINION, transform), 1);
                //Debug.LogFormat("{0} Pool Added 1", data.prefabName);
                m = PoolManager.instance.ActivateObject<Minion>(diceInfo.prefabName, Vector3.zero, transform);
            }
            
            if (m != null)
            {
                m.transform.localPosition = Vector3.zero;
                m.transform.localRotation = Quaternion.identity;
                baseStat = m;
                m.ActorProxy = this;
                m.SetPathFinding(_seeker, _aiPath);
                m.Initialize(null);
                m.controller = (Client as RWNetworkClient).GetTower(ownerTag);
                m.castType = (DICE_CAST_TYPE) diceInfo.castType;
                m.id = NetId;
                m.isMine = IsLocalPlayerActor;
                m.targetMoveType = (DICE_MOVE_TYPE) diceInfo.targetMoveType;
                m.isBottomPlayer = IsBottomCamp();
                m.ChangeLayer(IsBottomCamp());
            }

            if (client.enableUI)
            {
                ShowSpawnLine(m);
            }

            EnableClientCombatLogic(client.IsPlayingAI);
            SoundManager.instance?.Play(Global.E_SOUND.SFX_MINION_GENERATE);
        }

        void ShowSpawnLine(Minion minion)
        {
            var setting = UI_DiceField.Get().arrSlot[spawnSlot].ps.main;
            setting.startColor = FileHelper.GetColor(diceInfo.color);

            var dicePos = UI_DiceField.Get().arrSlot[spawnSlot].transform.position;
            if (IsLocalPlayerActor)
            {
                UI_DiceField.Get().arrSlot[spawnSlot].ps.Play();
            }
            else
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