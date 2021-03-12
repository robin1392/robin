using ED;
using UnityEngine;

namespace MirageTest.Scripts
{
    public partial class ActorProxy
    {
        void SpawnMagicAndInstallation()
        {
            var spawnPos = UI_DiceField.Get().arrSlot[spawnSlot].transform.position;
            var client = Client as RWNetworkClient;

            if (IsLocalPlayerActor)
            {
                if (client.enableUI)
                {
                    var setting = UI_DiceField.Get().arrSlot[spawnSlot].ps.main;
                    setting.startColor = FileHelper.GetColor(diceInfo.color); //data.color;
                    UI_DiceField.Get().arrSlot[spawnSlot].ps.Play();
                }
            }
            else
            {
                spawnPos.x *= -1f;
                spawnPos.z *= -1f;
            }

            GameObject loadMagic = FileHelper.LoadPrefab(diceInfo.prefabName, Global.E_LOADTYPE.LOAD_MAGIC,
                InGameManager.Get().transform);
            if (loadMagic != null)
            {
                var m = PoolManager.instance.ActivateObject<Magic>(diceInfo.prefabName, spawnPos,
                    InGameManager.Get().transform);
                if (m != null)
                {
                    m.isMine = IsLocalPlayerActor;
                    m.diceFieldNum = spawnSlot;
                    m.targetMoveType = (DICE_MOVE_TYPE) diceInfo.targetMoveType;
                    m.castType = (DICE_CAST_TYPE) diceInfo.castType;
                    m.id = NetId;
                    m.isBottomPlayer = IsBottomCamp();

                    m.Initialize(IsBottomCamp());
                    m.SetTarget();
                }
            }
        }
    }
}