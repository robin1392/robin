#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomWarsProtocol;

namespace ED
{
    public class Coop_AI : PlayerController
    {
        protected override void StartPlayerControll()
        {
            myUID = NetworkManager.Get().CoopUID;
            
            if (isMine)
            {
                NetworkManager.Get().event_OtherPause.AddListener(OtherPlayerPause);
                StartCoroutine(HitDamageQueueCoroutine());
            }

            isHalfHealth = false;

            if( InGameManager.IsNetwork == false )
                sp = 200;

            maxHealth = ConvertNetMsg.MsgIntToFloat(NetworkManager.Get().GetNetInfo().coopInfo.TowerHp);
            
            currentHealth = maxHealth;
            RefreshHealthBar();

            InGameManager.Get().AddPlayerUnit(isBottomPlayer, this);
            
            SetColor(isBottomPlayer ? E_MaterialType.BOTTOM : E_MaterialType.TOP);

            StartSyncMinion();
        }
        protected override IEnumerator HitDamageQueueCoroutine()
        {
            int damage = 0;
            while (true)
            {
                yield return new WaitForSeconds(0.2f);

                damage = 0;
                while (queueHitDamage.Count > 0)
                {
                    damage += queueHitDamage.Dequeue();
                }

                if (damage > 0)
                {
                    NetSendPlayer(GameProtocol.HIT_DAMAGE_REQ , myUID, damage);
                }
            }
        }
    }
}