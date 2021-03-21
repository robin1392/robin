using ED;
using Mirage;
using Mirage.Logging;
using UnityEngine;
using Debug = ED.Debug;

namespace MirageTest.Scripts
{
    public partial class ActorProxy
    {
        void SpawnGuardian()
        {
            if (TableManager.Get().GuardianInfo.GetData(dataId, out var dataGuardianInfo) == false)
            {
                _logger.LogError($"수호자데이터가 없습니다. {dataId}");
                return;
            }

            Vector3 pos = transform.position;
            var prefab = FileHelper.LoadPrefab(dataGuardianInfo.prefabName, Global.E_LOADTYPE.LOAD_GUARDIAN);
            PoolManager.instance.AddPool(prefab, 1);
            if (prefab != null)
            {
                PoolManager.instance.ActivateObject("particle_necromancer", transform.position);
                var guadian = PoolManager.instance.ActivateObject<Minion>(dataGuardianInfo.prefabName, Vector3.zero, transform);
                if (guadian == null)
                {
                    PoolManager.instance.AddPool(
                        FileHelper.LoadPrefab(dataGuardianInfo.prefabName, Global.E_LOADTYPE.LOAD_MINION), 1);
                    guadian = PoolManager.instance.ActivateObject<Minion>(dataGuardianInfo.prefabName, Vector3.zero, transform);
                }
                
                baseStat = guadian;
                guadian.ActorProxy = this;
                guadian.transform.localPosition = Vector3.zero;
                guadian.transform.localRotation = Quaternion.identity;
                guadian.SetPathFinding(_seeker, _aiPath);
                guadian.Initialize();
                guadian.castType = (DICE_CAST_TYPE) gudianInfo.castType;
                guadian.id = NetId;
                guadian.isMine = IsLocalPlayerActor;
                guadian.targetMoveType = (DICE_MOVE_TYPE) gudianInfo.targetMoveType;
                guadian.ChangeLayer(IsBottomCamp());
                
            }

            var client = Client as RWNetworkClient;
            var tower = client.GetTower(ownerTag);
            if (tower != null)
            {
                tower.ps_ShieldOff.Play();
                tower.animator.SetBool(AnimationHash.Break, true);
            }

            PoolManager.instance.ActivateObject("Effect_Robot_Summon", pos);
        }
    }
}