#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using RandomWarsProtocol;
using RandomWarsResource.Data;
using UnityEngine;

namespace ED
{
    public class Coop_AI : PlayerController
    {
        [Header("Egg")] public Transform ts_EggParent;

        public GameObject obj_IncubationParticle;
        public GameObject obj_SummonParticle;

        private MsgMonster msgBoss;
        

        protected override void StartPlayerControll()
        {
            // throw new NotImplementedException();
            // // _myUID = NetworkManager.Get().CoopUID;
            // // id = myUID * 10000;
            //
            // NetworkManager.Get().event_OtherPause.AddListener(OtherPlayerPauseAI);
            // if (isMine)
            // {
            //     StartCoroutine(HitDamageQueueCoroutine());
            // }
            //
            // isHalfHealth = false;
            //
            // if (InGameManager.IsNetwork == false)
            //     sp = 200;
            //
            // maxHealth = ConvertNetMsg.MsgIntToFloat(NetworkManager.Get().GetNetInfo().coopInfo.TowerHp);
            //
            // currentHealth = maxHealth;
            // RefreshHealthBar();
            //
            // InGameManager.Get().AddPlayerUnit(isBottomPlayer, this);
            //
            // SetColor(isBottomPlayer ? E_MaterialType.BOTTOM : E_MaterialType.TOP);
            //
            // StartSyncMinion();
        }

        private void OtherPlayerPauseAI(bool isPause)
        {
            SetPlayingAI(isPause);
        }

        public override void SetColor(E_MaterialType type, bool isAlly)
        {
        }

        private void DestroyEgg()
        {
            animator.SetTrigger(AnimationHash.Idle);
            if (ts_EggParent.childCount > 0)
            {
                DestroyImmediate(ts_EggParent.GetChild(0).gameObject);
            }

            obj_IncubationParticle.SetActive(false);
        }


        #region HitDamage

        public override void HitDamageMinionAndMagic(uint baseStatId, float damage)
        {
            if (damage <= 0f) return;

            if (baseStatId == id || baseStatId < 10000)
            {
                if (InGameManager.IsNetwork == true && (isMine || isPlayingAI))
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

            for (int i = 0; i < infos.Length; i++)
            {
                var pos = FieldManager.Get().GetTopListPos(infos[i].SlotIndex);

                TDataDiceInfo dataDiceInfo;
                if (TableManager.Get().DiceInfo.GetData(infos[i].DiceId, out dataDiceInfo) == false)
                {
                    return;
                }

                switch (dataDiceInfo.castType)
                {
                    case (int) DICE_CAST_TYPE.MINION:
                    case (int) DICE_CAST_TYPE.HERO:
                    {
                        var obj = FileHelper.LoadPrefab(dataDiceInfo.prefabName,
                            Global.E_LOADTYPE.LOAD_MINION);

                        var m = CreateMinion(obj, pos);

                        m.targetMoveType = (DICE_MOVE_TYPE) dataDiceInfo.targetMoveType;
                        m.ChangeLayer(false);
                        // m.power = dataDiceInfo.power + dataDiceInfo.powerUpgrade * infos[i].DiceLevel +
                        //           dataDiceInfo.powerInGameUp * infos[i].DiceInGameUp;
                        // m.maxHealth = dataDiceInfo.maxHealth + dataDiceInfo.maxHpUpgrade * infos[i].DiceLevel + dataDiceInfo.maxHpInGameUp * infos[i].DiceInGameUp;
                        // m.attackSpeed = dataDiceInfo.attackSpeed;
                        // m.moveSpeed = dataDiceInfo.moveSpeed;
                        // m.eyeLevel = 1;
                        // m.ingameUpgradeLevel = infos[i].DiceInGameUp;
                        m.Initialize();
                    }
                        break;
                    case (int) DICE_CAST_TYPE.MAGIC:
                    case (int) DICE_CAST_TYPE.INSTALLATION:
                    {
                        // CastMagic(dataDiceInfo, objID, arrDice[fieldIndex].eyeLevel + 1, upgradeLevel, magicCastDelay,
                        //     fieldIndex);
                        var obj = FileHelper.LoadPrefab(dataDiceInfo.prefabName,
                            Global.E_LOADTYPE.LOAD_MAGIC);

                        var m = CastMagic(obj, pos);
                        m.targetMoveType = (DICE_MOVE_TYPE) dataDiceInfo.targetMoveType;
                        //m.ChangeLayer(false);
                        // m.power = dataDiceInfo.power + dataDiceInfo.powerUpgrade * infos[i].DiceLevel +
                        //           dataDiceInfo.powerInGameUp * infos[i].DiceInGameUp;
                        // // m.maxHealth = dataDiceInfo.maxHealth + dataDiceInfo.maxHpUpgrade * infos[i].DiceLevel + dataDiceInfo.maxHpInGameUp * infos[i].DiceInGameUp;
                        // m.attackSpeed = dataDiceInfo.attackSpeed;
                        // m.moveSpeed = dataDiceInfo.moveSpeed;
                        
                        //m.ingameUpgradeLevel = infos[i].DiceInGameUp;
                        m.Initialize(false);
                    }
                        break;
                }
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
                TDataCoopModeBossInfo dataCoopModeBossInfo;
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
                //KZSee:
                // if (msgBoss != null && InGameManager.Get().wave > 1 && currentHealth > 0) 
                {
                    SpawnBossFromEgg();
                }

                // Set HP
                id = boss.Id;
                // maxHealth = ConvertNetMsg.MsgIntToFloat(boss.Hp);
                // currentHealth = maxHealth;
                RefreshHealthBar();
                msgBoss = boss;
            }
        }

        private void SpawnBossFromEgg()
        {
            if (msgBoss != null)
            {
                TDataCoopModeBossInfo dataCoopModeBossInfo;
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
                // m.maxHealth = ConvertNetMsg.MsgIntToFloat(msgBoss.Hp);
                // m.power = ConvertNetMsg.MsgIntToFloat(msgBoss.Power);
                // m.effect = ConvertNetMsg.MsgIntToFloat(msgBoss.Effect);
                // m.effectDuration = ConvertNetMsg.MsgShortToFloat(msgBoss.Duration);
                // m.effectCooltime = ConvertNetMsg.MsgShortToFloat(msgBoss.EffectCoolTime);
                // m.moveSpeed = ConvertNetMsg.MsgShortToFloat(msgBoss.MoveSpeed);
                // m.attackSpeed = ConvertNetMsg.MsgShortToFloat(msgBoss.AttackSpeed);
                // m.isMine = NetworkManager.Get().IsMaster;
                // m.eyeLevel = 1;
                // m.ingameUpgradeLevel = 0;
                m.Initialize();

                // m.currentHealth = currentHealth;
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

            animator.SetTrigger(AnimationHash.Incubation);
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
    }
}