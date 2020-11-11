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
            _myUID = NetworkManager.Get().CoopUID;
            
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
                    NetSendPlayer(GameProtocol.HIT_DAMAGE_REQ , _myUID, damage);
                }
            }
        }
        protected override void SetColor(E_MaterialType type)
        {
            var mr = GetComponentsInChildren<MeshRenderer>();
            foreach (var m in mr)
            {
                if (m.gameObject.CompareTag("Finish")) continue;

                m.material = arrMaterial[1];
                switch (type)
                {
                    case E_MaterialType.BOTTOM:
                    case E_MaterialType.TOP:
                        Color c = m.material.color;
                        c.a = 1f;
                        m.material.color = c;
                        break;
                    case E_MaterialType.HALFTRANSPARENT:
                    case E_MaterialType.TRANSPARENT:
                        c = m.material.color;
                        c.a = 0.3f;
                        m.material.color = c;
                        break;
                }
            }
        }

        public void Spawn(MsgBossMonster boss = null)
        {
            packetCount = 0;
            
            Debug.LogFormat("COOP_AI_SPAWN : boss={0}", boss != null);
            if (boss != null && boss.DataId > 0)
            {
                Debug.LogFormat("BOSS : hp={0}, id={1}", boss.Hp, boss.DataId);

                var pos = FieldManager.Get().GetTopListPos(0);
                var obj = FileHelper.LoadPrefab(JsonDataManager.Get().dataDiceInfo.dicData[1000].prefabName,
                    Global.E_LOADTYPE.LOAD_MINION);
                var m = CreateMinion(obj, pos, 1, 1);
                var bossData = JsonDataManager.Get().dataBossInfo.dicData[boss.DataId];

                m.id = boss.Id;
                m.maxHealth = ConvertNetMsg.MsgIntToFloat(boss.Hp);
                m.power = ConvertNetMsg.MsgShortToFloat(boss.Atk);
                m.Initialize(MinionDestroyCallback);
            }

            int minionCount = Mathf.Clamp(InGameManager.Get().wave, 1, 15);
            for (int i = 0; i < minionCount; i++)
            {
                var pos = FieldManager.Get().GetTopListPos(i);
                var obj = FileHelper.LoadPrefab(JsonDataManager.Get().dataDiceInfo.dicData[1000].prefabName,
                    Global.E_LOADTYPE.LOAD_MINION);
                var m = CreateMinion(obj, pos, 1, 0);
                
                //m.maxHealth = ConvertNetMsg.MsgIntToFloat(boss.Hp);
                //m.power = ConvertNetMsg.MsgShortToFloat(boss.Atk);
                
                m.targetMoveType = DICE_MOVE_TYPE.ALL;
                m.ChangeLayer(isBottomPlayer);
                m.power = 100f;
                m.maxHealth = 500f;
                m.attackSpeed = 1f;
                m.moveSpeed = 1f;
                m.eyeLevel = 1;
                m.upgradeLevel = 0;
                m.Initialize(MinionDestroyCallback);
            }
        }
    }
}