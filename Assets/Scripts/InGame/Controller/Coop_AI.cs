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

        public GameObject obj_IncubationParticle;
        public GameObject obj_SummonParticle;

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

            if (InGameManager.IsNetwork == false)
                sp = 200;

            maxHealth = ConvertNetMsg.MsgIntToFloat(NetworkManager.Get().GetNetInfo().coopInfo.TowerHp);

            currentHealth = maxHealth;
            RefreshHealthBar();

            InGameManager.Get().AddPlayerUnit(isBottomPlayer, this);

            SetColor(isBottomPlayer ? E_MaterialType.BOTTOM : E_MaterialType.TOP);

            StartSyncMinion();
        }

        #region HitDamage

        public override void HitDamage(float damage)
        {
            if (currentHealth > 0)
            {
                currentHealth -= damage;

                if (currentHealth <= 0)
                {
                    currentHealth = 0;

                    // 알이 깨졌을 때.. 
                }

                RefreshHealthBar();
            }
        }

        protected override IEnumerator HitDamageQueueCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.2f);

                if (dicHitDamage.Count > 0)
                {
                    MsgDamage[] msg = new MsgDamage[dicHitDamage.Count];

                    int loopCount = 0;
                    foreach (var dmg in dicHitDamage)
                    {
                        if (dmg.Value > 0f)
                        {
                            msg[loopCount] = new MsgDamage
                            {
                                Id = ConvertNetMsg.MsgIntToUshort(dmg.Key),
                                Damage = ConvertNetMsg.MsgFloatToInt(dmg.Value)
                            };
                        }

                        loopCount++;
                    }

                    NetSendPlayer(GameProtocol.HIT_DAMAGE_REQ, _myUID, msg);

                    dicHitDamage.Clear();
                }
            }
        }

        #endregion

        protected override void SetColor(E_MaterialType type)
        {
        }

        #region Spawn

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

            if (InGameManager.Get().wave > 0 && InGameManager.Get().wave % 5 == 4)
            {
                StartCoroutine(EggIncubationCoroutine());
            }
        }

        public override void SpawnMonster(MsgBossMonster boss)
        {
            if (boss != null && boss.DataId > 0)
            {
                var obj = FileHelper.LoadPrefab(
                    $"{JsonDataManager.Get().dataBossInfo.dicData[boss.DataId].unitPrefabName}_Egg",
                    Global.E_LOADTYPE.LOAD_COOP_BOSS);

                if (obj != null)
                {
                    if (ts_EggParent.childCount > 0)
                    {
                        DestroyImmediate(ts_EggParent.GetChild(0).gameObject);
                    }

                    var egg = Instantiate(obj, ts_EggParent);
                    egg.transform.localPosition = Vector3.zero;
                    egg.transform.localRotation = Quaternion.identity;
                    egg.transform.localScale = Vector3.one;
                }

                // Set HP
                id = boss.Id;
                maxHealth = ConvertNetMsg.MsgIntToFloat(boss.Hp);
                currentHealth = maxHealth;
                RefreshHealthBar();

                msgBoss = boss;
            }
        }

        private void SpawnBoss()
        {
            if (msgBoss != null)
            {
                var obj = FileHelper.LoadPrefab(
                    $"{JsonDataManager.Get().dataBossInfo.dicData[msgBoss.DataId].unitPrefabName}",
                    Global.E_LOADTYPE.LOAD_COOP_BOSS);

                var m = CreateMinion(obj, transform.position, 1, 0, false);

                m.id = msgBoss.Id;
                m.maxHealth = ConvertNetMsg.MsgIntToFloat(msgBoss.Hp);
                m.power = ConvertNetMsg.MsgShortToFloat(msgBoss.Atk);
                m.effect = ConvertNetMsg.MsgShortToFloat(msgBoss.SkillAtk);
                m.effectDuration = ConvertNetMsg.MsgShortToFloat(msgBoss.SkillInterval);
                m.effectCooltime = ConvertNetMsg.MsgShortToFloat(msgBoss.SkillCoolTime);
                //m.moveSpeed = ConvertNetMsg.MsgShortToFloat(boss.MoveSpeed);
                m.isMine = NetworkManager.Get().IsMaster;
                m.ChangeLayer(false);
                m.Initialize(MinionDestroyCallback);
                m.currentHealth = currentHealth;
                m.HitDamage(0);

                msgBoss = null;
            }
        }

        IEnumerator EggIncubationCoroutine()
        {
            yield return new WaitForSeconds(15f);

            animator.SetTrigger("Incubation");
            obj_IncubationParticle.SetActive(true);

            yield return new WaitForSeconds(5f);

            SpawnBoss();
            obj_SummonParticle.SetActive(true);

            yield return new WaitForSeconds(3f);
            obj_IncubationParticle.SetActive(false);
            obj_SummonParticle.SetActive(false);
        }

        #endregion
        
        
        protected override void SyncMinion(int uid, byte minionCount , MsgVector2[] msgPoss, int[] minionHP, MsgMinionStatus relay, int packetCount)
        {
            for (var i = 0; i < minionCount && i < listMinion.Count; i++)
            {
                Vector3 chPos = ConvertNetMsg.MsgToVector3(msgPoss[i]);
                float chHP = ConvertNetMsg.MsgIntToFloat(minionHP[i]);
                listMinion[i].SetNetworkValue(chPos, chHP);
            }

            var dic = MsgMinionStatusToDictionary(relay);

            #if ENABLE_LOG
            string str = "MINION_STATUS_RELAY -> Dictionary count : " + dic.Keys.Count;
            #endif

            foreach (var msg in dic)
            {
                #if ENABLE_LOG
                str += string.Format("\n{0} -> List count : {1}", msg.Key, msg.Value.Count);
                switch (msg.Key)
                {
                    case GameProtocol.HIT_DAMAGE_MINION_RELAY:
                        foreach (var value in msg.Value)
                        {
                            MsgHitDamageMinionRelay m = (MsgHitDamageMinionRelay) value;
                            str += string.Format("\n      UID: {0},  ID:{1}, DMG:{2}", m.PlayerUId,
                                m.Id, m.Damage);
                        }
                        break;
                    case GameProtocol.HEAL_MINION_RELAY:
                        foreach (var value in msg.Value)
                        {
                            MsgHealMinionRelay m = (MsgHealMinionRelay) value;
                            str += string.Format("\n      UID: {0},  ID:{1}, HEAL:{2}", m.PlayerUId,
                                m.Id, m.Heal);
                        }
                        break;
                    case GameProtocol.DESTROY_MINION_RELAY:
                        foreach (var value in msg.Value)
                        {
                            MsgDestroyMinionRelay m = (MsgDestroyMinionRelay) value;
                            str += string.Format("\n      UID: {0},  ID:{1}", m.PlayerUId,
                                m.Id);
                        }
                        break;
                    case GameProtocol.DESTROY_MAGIC_RELAY:
                        foreach (var value in msg.Value)
                        {
                            MsgDestroyMagicRelay m = (MsgDestroyMagicRelay) value;
                            str += string.Format("\n      UID: {0},  ID:{1}", m.PlayerUId,
                                m.BaseStatId);
                        }
                        break;
                    case GameProtocol.ACTIVATE_POOL_OBJECT_RELAY:
                        foreach (var value in msg.Value)
                        {
                            MsgActivatePoolObjectRelay m = (MsgActivatePoolObjectRelay) value;
                            str += string.Format("\n      POOL: {0}", ((E_PoolName)m.PoolName).ToString());
                        }
                        break;
                }
                #endif
                
                if (msg.Value.Count > 0)
                {
                    foreach (var obj in msg.Value)
                    {
                        //if (NetworkManager.Get().OtherUID == uid)
                            //targetPlayer.NetRecvPlayer(msg.Key, obj);
                        //else if (NetworkManager.Get().CoopUID == uid)
//                            coopPlayer.NetRecvPlayer(msg.Key, obj);
                        if (NetworkManager.Get().IsMaster)
                            targetPlayer.NetRecvPlayer(msg.Key, obj);
                        else
                            targetPlayer.targetPlayer.NetRecvPlayer(msg.Key, obj);
                    }
                }
            }
            
            #if ENABLE_LOG
            UnityUtil.Print(string.Format("RECV [{0}][{1}] : ", _myUID, packetCount), str, "green");
            #endif
        }
    }
}