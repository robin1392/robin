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

        private MsgMonster msgBoss;
        private static readonly int Idle = Animator.StringToHash("Idle");
        private static readonly int Incubation = Animator.StringToHash("Incubation");

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
        
        protected override void SetColor(E_MaterialType type) { }

        private void DestroyEgg()
        {
            animator.SetTrigger(Idle);
            if (ts_EggParent.childCount > 0)
            {
                DestroyImmediate(ts_EggParent.GetChild(0).gameObject);
            }
            obj_IncubationParticle.SetActive(false);
        }
        

        #region HitDamage

        public override void HitDamageMinionAndMagic(int baseStatId, float damage )
        {
            if (damage <= 0f) return;
            
            if (baseStatId == id || baseStatId < 10000)
            {
                if( InGameManager.IsNetwork == true && (isMine || isPlayingAI))
                {
                    //int convDamage = ConvertNetMsg.MsgFloatToInt( damage );
                    // 타워가 맞으면 알ID로 교체해서 전송
                    if (baseStatId == myUID * 10000) baseStatId = msgBoss.Id;
                    if (dicHitDamage.ContainsKey(baseStatId) == false)
                    {
                        dicHitDamage.Add(baseStatId, damage);
                    }
                    else
                    {
                        dicHitDamage[baseStatId] += damage;
                    }
                }
                else if (InGameManager.IsNetwork == false)
                {
                    HitDamage(damage);
                }
            }
            else
            {
                var m = listMinion.Find(minion => minion.id == baseStatId);
                if (m != null)
                {
                    m.HitDamage(damage);
                    var obj = PoolManager.instance.ActivateObject("Effect_ArrowHit", m.ts_HitPos.position);
                    obj.rotation = Quaternion.identity;
                    obj.localScale = Vector3.one * 0.6f;
                }
                else
                {
                    var mg = _listMagic.Find(magic => magic.id == baseStatId);
                    if (mg != null)
                    {
                        mg.HitDamage(damage);
                    }
                }
            }
        }

        public override void HitDamage(float damage)
        {
            if (currentHealth > 0)
            {
                currentHealth -= damage;

                if (currentHealth <= 0)
                {
                    currentHealth = 0;

                    // 알이 깨졌을 때.. 
                    DestroyEgg();
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

        public override void HitDamageWithID(int id, float curHP)
        {
            if (this.id == id || (msgBoss != null && msgBoss.Id == id))
            {
                currentHealth = curHP;
                RefreshHealthBar();
                HitDamage(0);
            }
            else
            {
                var minion = listMinion.Find(m => m.id == id);
                if (minion != null)
                {
                    minion.currentHealth = curHP;
                    minion.HitDamage(0);
                }
            }
        }

        #endregion
        
        
        #region Spawn

        public override void Spawn(MsgSpawnMinion[] infos)
        {
            packetCount = 0;

            //int minionCount = Mathf.Clamp(Mathf.FloorToInt(InGameManager.Get().wave * 1.5f), 1, 15);
            //for (int i = 0; i < minionCount; i++)
            //{
            //    var pos = FieldManager.Get().GetTopListPos(i);
            //    var obj = FileHelper.LoadPrefab(JsonDataManager.Get().dataDiceInfo.dicData[1000].prefabName,
            //        Global.E_LOADTYPE.LOAD_MINION);
            //    var m = CreateMinion(obj, pos);

            //    //m.maxHealth = ConvertNetMsg.MsgIntToFloat(boss.Hp);
            //    //m.power = ConvertNetMsg.MsgShortToFloat(boss.Atk);

            //    m.targetMoveType = DICE_MOVE_TYPE.ALL;
            //    m.ChangeLayer(false);
            //    m.power = Mathf.Clamp(100f + 2f * InGameManager.Get().wave, 0, 300f);
            //    m.maxHealth = Mathf.Clamp(300f + 5f * InGameManager.Get().wave, 0, 2000f);
            //    m.attackSpeed = 1f;
            //    m.moveSpeed = 1f;
            //    m.eyeLevel = 1;
            //    m.upgradeLevel = 0;
            //    m.Initialize(MinionDestroyCallback);
            //}

            for(int i = 0; i < infos.Length; i++)
            {
                var pos = FieldManager.Get().GetTopListPos(infos[i].SlotIndex);

                RandomWarsResource.Data.TDataDiceInfo dataDiceInfo;
                if (TableManager.Get().DiceInfo.GetData(infos[i].DiceId, out dataDiceInfo) == false)
                {
                    return;
                }
                var obj = FileHelper.LoadPrefab(dataDiceInfo.prefabName,
                    Global.E_LOADTYPE.LOAD_MINION);

                var m = CreateMinion(obj, pos);

                m.targetMoveType = (DICE_MOVE_TYPE)dataDiceInfo.targetMoveType;
                m.ChangeLayer(false);
                m.power = dataDiceInfo.power + dataDiceInfo.powerUpgrade * infos[i].DiceLevel + dataDiceInfo.powerInGameUp * infos[i].DiceInGameUp;
                m.maxHealth = dataDiceInfo.maxHealth + dataDiceInfo.maxHpUpgrade * infos[i].DiceLevel + dataDiceInfo.maxHpInGameUp * infos[i].DiceInGameUp;
                m.attackSpeed = dataDiceInfo.attackSpeed;
                m.moveSpeed = dataDiceInfo.moveSpeed;
                m.eyeLevel = 1;
                m.ingameUpgradeLevel = infos[i].DiceInGameUp;
                m.Initialize(MinionDestroyCallback);
            }

            if (InGameManager.Get().wave > 0 && InGameManager.Get().wave % 5 == 4)
            {
                StartCoroutine(EggIncubationCoroutine());
            }
        }

        // Spawn Egg
        public override void SpawnMonster(MsgMonster boss)
        {
            if (boss != null && boss.DataId > 0)
            {
                RandomWarsResource.Data.TDataCoopModeBossInfo dataCoopModeBossInfo;
                if (TableManager.Get().CoopModeBossInfo.GetData(boss.DataId, out dataCoopModeBossInfo) == false)
                {
                    return;
                }

                var obj = FileHelper.LoadPrefab(
                    $"{dataCoopModeBossInfo.prefabName}_Egg",
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

                // 알이 있으면 보스 소환
                if (msgBoss != null && InGameManager.Get().wave > 1 && currentHealth > 0) 
                {
                    SpawnBossFromEgg();
                }

                // Set HP
                id = boss.Id;
                maxHealth = ConvertNetMsg.MsgIntToFloat(boss.Hp);
                currentHealth = maxHealth;
                RefreshHealthBar();
                msgBoss = boss;
            }
        }

        private void SpawnBossFromEgg()
        {
            if (msgBoss != null)
            {
                RandomWarsResource.Data.TDataCoopModeBossInfo dataCoopModeBossInfo;
                if (TableManager.Get().CoopModeBossInfo.GetData(msgBoss.DataId, out dataCoopModeBossInfo) == false)
                {
                    return;
                }

                var obj = FileHelper.LoadPrefab(
                    $"{dataCoopModeBossInfo.prefabName}",
                    Global.E_LOADTYPE.LOAD_COOP_BOSS);

                var m = CreateMinion(obj, transform.position, false);

                m.id = msgBoss.Id;
                m.targetMoveType = DICE_MOVE_TYPE.GROUND;
                m.ChangeLayer(false);
                m.maxHealth = ConvertNetMsg.MsgIntToFloat(msgBoss.Hp);
                m.power = ConvertNetMsg.MsgShortToFloat(msgBoss.Power);
                m.effect = ConvertNetMsg.MsgShortToFloat(msgBoss.Effect);
                m.effectDuration = ConvertNetMsg.MsgShortToFloat(msgBoss.Duration);
                m.effectCooltime = ConvertNetMsg.MsgShortToFloat(msgBoss.EffectCoolTime);
                m.moveSpeed = ConvertNetMsg.MsgShortToFloat(msgBoss.MoveSpeed);
                m.attackSpeed = ConvertNetMsg.MsgShortToFloat(msgBoss.AttackSpeed);
                m.isMine = NetworkManager.Get().IsMaster;
                m.eyeLevel = 1;
                m.ingameUpgradeLevel = 0;
                m.Initialize(MinionDestroyCallback);
                
                m.currentHealth = currentHealth;
                m.HitDamage(0);

                msgBoss = null;
            }
        }

        IEnumerator EggIncubationCoroutine()
        {
            float t = 0;
            while (t < 15f)
            {
                t += Time.deltaTime;
                if (isAlive == false)
                {
                    DestroyEgg();
                    yield break;
                }
                yield return null;
            }
            
            animator.SetTrigger(Incubation);
            obj_IncubationParticle.SetActive(true);

            t = 0;
            while (t < 5f)
            {
                t += Time.deltaTime;
                if (isAlive == false)
                {
                    DestroyEgg();
                    yield break;
                }
                yield return null;
            }

            //SpawnBoss();
            obj_SummonParticle.SetActive(true);
            obj_IncubationParticle.SetActive(false);

            t = 0;
            while (t < 3f)
            {
                t += Time.deltaTime;
                yield return null;
            }

            obj_SummonParticle.SetActive(false);
        }

        #endregion
        
        
        #region Network
        
        public override void SyncMinion(MsgMinionInfo[] msgMinionInfos, MsgMinionStatus relay, int packetCount)
        {
            // for (var i = 0; i < minionCount && i < listMinion.Count; i++)
            // {
            //     Vector3 chPos = ConvertNetMsg.MsgToVector3(msgPos[i]);
            //     float chHP = ConvertNetMsg.MsgIntToFloat(minionHP[i]);
            //     listMinion[i].SetNetworkValue(chPos, chHP);
            // }

            for (int i = 0; i < msgMinionInfos.Length; i++)
            {
                var m = listMinion.Find(minion => minion.id == msgMinionInfos[i].Id);
                if (m != null)
                {
                    m.SetNetworkValue(
                        ConvertNetMsg.MsgToVector3(msgMinionInfos[i].Pos),
                        ConvertNetMsg.MsgIntToFloat(msgMinionInfos[i].Hp)
                    );
                }
                else // 유닛이 없을 경우 생성하기
                {
                    
                }
            }

            var dic = MsgMinionStatusToDictionary(relay);

            #if ENABLE_LOG
            string str = "MINION_STATUS_RELAY -> Dictionary count : " + dic.Keys.Count;
            #endif

            foreach (var msg in dic)
            {
#if ENABLE_LOG
                str += string.Format("\n{0} -> List count : {1}", msg.Key, msg.Value.Count);
                switch(msg.Key)
                {
                case GameProtocol.HIT_DAMAGE_MINION_RELAY:
                    foreach(var value in msg.Value)
                    {
                        MsgHitDamageMinionRelay m = (MsgHitDamageMinionRelay)value;
                        str += string.Format("\n      ID:{0}, DMG:{1}", m.Id, m.Damage);
                    }

                    break;
                case GameProtocol.HEAL_MINION_RELAY:
                    foreach(var value in msg.Value)
                    {
                        MsgHealMinionRelay m = (MsgHealMinionRelay)value;
                        str += string.Format("\n      ID:{0}, HEAL:{1}", m.Id, m.Heal);
                    }

                    break;
                case GameProtocol.DESTROY_MINION_RELAY:
                    foreach(var value in msg.Value)
                    {
                        MsgDestroyMinionRelay m = (MsgDestroyMinionRelay)value;
                        str += string.Format("\n      ID:{0}", m.Id);
                    }

                    break;
                case GameProtocol.DESTROY_MAGIC_RELAY:
                    foreach(var value in msg.Value)
                    {
                        MsgDestroyMagicRelay m = (MsgDestroyMagicRelay)value;
                        str += string.Format("\n      ID:{0}", m.BaseStatId);
                    }

                    break;
                case GameProtocol.ACTIVATE_POOL_OBJECT_RELAY:
                    foreach(var value in msg.Value)
                    {
                        MsgActivatePoolObjectRelay m = (MsgActivatePoolObjectRelay)value;
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
                        NetRecvPlayer(msg.Key, obj);
                    }
                }
            }
            
            #if ENABLE_LOG
            UnityUtil.Print(string.Format("RECV [{0}][{1}] : ", _myUID, packetCount), str, "green");
            #endif
        }
        
        #endregion
    }
}