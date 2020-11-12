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
        [Header("Egg")]
        public Transform ts_EggParent;
        public GameObject obj_Egg;

        private MsgBossMonster msgBoss;
        
        protected override void StartPlayerControll()
        {
            _myUID = NetworkManager.Get().CoopUID;
            id = myUID * 10000;
            
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
                    NetSendPlayer(GameProtocol.HIT_DAMAGE_REQ , _myUID, id, damage);
                }
            }
        }
        protected override void SetColor(E_MaterialType type) { }

        public void Spawn()
        {
            packetCount = 0;
            
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
                m.power = 100f + 10f * InGameManager.Get().wave;
                m.maxHealth = 500f + 50f * InGameManager.Get().wave;
                m.attackSpeed = 1f;
                m.moveSpeed = 1f;
                m.eyeLevel = 1;
                m.upgradeLevel = 0;
                m.Initialize(MinionDestroyCallback);
            }
        }
        
        public override void SpawnMonster(MsgBossMonster boss)
        {
            if (boss != null && boss.DataId > 0)
            {
                var obj = FileHelper.LoadPrefab($"{JsonDataManager.Get().dataBossInfo.dicData[boss.DataId].unitPrefabName}_Egg",
                    Global.E_LOADTYPE.LOAD_COOP_BOSS);

                if (ts_EggParent.childCount > 0)
                {
                    DestroyImmediate(ts_EggParent.GetChild(0).gameObject);
                }

                var egg = Instantiate(obj, ts_EggParent);
                egg.transform.localPosition = Vector3.zero;
                egg.transform.localRotation = Quaternion.identity;
                egg.transform.localScale = Vector3.zero;

                msgBoss = boss;
            }
        }

        private void SpawnBoss()
        {
            if (msgBoss != null)
            {
                var obj = FileHelper.LoadPrefab($"{JsonDataManager.Get().dataBossInfo.dicData[msgBoss.DataId].unitPrefabName}_Egg",
                    Global.E_LOADTYPE.LOAD_COOP_BOSS);

                var m = CreateMinion(obj, transform.position, 1, 0);

                m.id = msgBoss.Id;
                m.maxHealth = ConvertNetMsg.MsgIntToFloat(msgBoss.Hp);
                m.power = ConvertNetMsg.MsgShortToFloat(msgBoss.Atk);
                m.effect = ConvertNetMsg.MsgShortToFloat(msgBoss.SkillAtk);
                m.effectDuration = ConvertNetMsg.MsgShortToFloat(msgBoss.SkillInterval);
                m.effectCooltime = ConvertNetMsg.MsgShortToFloat(msgBoss.SkillCoolTime);
                //m.moveSpeed = ConvertNetMsg.MsgShortToFloat(boss.MoveSpeed);
                m.Initialize(MinionDestroyCallback);

                msgBoss = null;
            }
        }
    }
}