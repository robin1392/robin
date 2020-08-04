﻿#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


#region photon
using Photon.Pun;
#endregion

namespace ED
{
    public class PlayerController : BaseStat, IPunObservable
    {

        #region singleton
        private static PlayerController _instance = null;
        public int instanceID;

        public static PlayerController Get()
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType(typeof(PlayerController)) as PlayerController;
                //if (_instance == null)
                //{
                //}
                //else
                _instance.Init();
            }
        
            return _instance;
        }
        
        protected virtual void Init()
        {
            if (_instance == null)
            {
                instanceID = GetInstanceID();
                _instance = this as PlayerController;
            }

            if (instanceID == 0)
                instanceID = GetInstanceID();
        }
        //public static PlayerController Instance;
        #endregion
        
        #region player variable.
        
        public PlayerController targetPlayer;
        #endregion
        
        #region data variable
        
        // dice
        protected Data_Dice[] _arrDeck;
        public Data_Dice[] arrDeck 
        { 
            get => _arrDeck;
            protected set => _arrDeck = value;
        }
        
        //
        protected Dice[] _arrDice;
        public Dice[] arrDice
        { 
            get => _arrDice;
            protected set => _arrDice = value;
        }
        
        protected int[] _arrUpgradeLevel;
        public int[] arrUpgradeLevel 
        { 
            get => _arrUpgradeLevel;
            protected set => _arrUpgradeLevel = value;
        }
        
        #endregion
        
        #region game variable
        public int spUpgradeLevel
        {
            get; protected set;
        }
        
        public UI_DiceField uiDiceField;
        public GameObject objCollider;
        
        private int _spawnCount = 1;
        
        [SerializeField]
        protected int _sp = 200;
        public virtual int sp 
        { 
            get => _sp;
            protected set
            {
                _sp = value; InGameManager.Get().event_SP_Edit.Invoke(_sp);
            }
        }

        #endregion
        
        #region minion & magic
        
        protected List<Minion> _listMinion = new List<Minion>();

        [SerializeField]
        public List<Minion> listMinion
        {
            get => _listMinion;
            protected set => _listMinion = value;
        }
        
        [SerializeField]
        protected List<Magic> listMagic = new List<Magic>();

        #endregion

        #region unity base
        protected void Awake()
        {
            if (PhotonNetwork.IsConnected)
            {
                if (photonView.IsMine)
                {
                    isMine = true;
                    //Instance = this;
                    Init();
                }
            }
            else
            {
                if (_instance == null)
                {
//                    Instance = this;
                    Init();
                }
                
            }
            //DontDestroyOnLoad(gameObject);

            InitializePlayer();
        }

        protected override void Start()
        {
            if (InGameManager.Get().playType != PLAY_TYPE.CO_OP)
            {
                base.Start();
            }
            
            StartPlayerControll();

        }

        public void OnDestroy()
        {
            DestroyPlayer();
        }

        #endregion
        
        #region init & destroy

        public void InitializePlayer()
        {
            _arrDice = new Dice[15];
            if (arrDeck == null) _arrDeck = new Data_Dice[5];
            _arrUpgradeLevel = new int[5];
        }

        public void DestroyPlayer()
        {
            _arrDice = null;
            _arrDeck = null;
            _arrUpgradeLevel = null;
        }

        public void StartPlayerControll()
        {
            sp = 200;
            currentHealth = maxHealth;
            
            for (var i = 0; i < arrDice.Length; i++)
            {
                arrDice[i] = new Dice {diceFieldNum = i};
            }
            uiDiceField = FindObjectOfType<UI_DiceField>();            
            uiDiceField.SetField(arrDice);

            if (PhotonNetwork.IsConnected)
            {
                //image_HealthBar = photonView.IsMine ? InGameManager.Get().image_BottomHealthBar : InGameManager.Get().image_TopHealthBar;
                //text_Health = photonView.IsMine ? InGameManager.Get().text_BottomHealth : InGameManager.Get().text_TopHealth;
                image_HealthBar = WorldUIManager.Get().GetHealthBar(photonView.IsMine);
                text_Health = WorldUIManager.Get().GetHealthText(photonView.IsMine);
            }
            else
            {
                //image_HealthBar = isBottomPlayer ? InGameManager.Get().image_BottomHealthBar : InGameManager.Get().image_TopHealthBar;
                //text_Health = isBottomPlayer ? InGameManager.Get().text_BottomHealth : InGameManager.Get().text_TopHealth;
                image_HealthBar = WorldUIManager.Get().GetHealthBar(isBottomPlayer);
                text_Health = WorldUIManager.Get().GetHealthText(isBottomPlayer);
            }
            text_Health.text = $"{Mathf.CeilToInt(currentHealth)}";

            InGameManager.Get().AddPlayerUnit(isBottomPlayer, this);

            if (PhotonNetwork.IsConnected)
            {
                if (!photonView.IsMine)
                {
                    transform.parent = isBottomPlayer ? GameObject.Find("Bottom player position").transform : GameObject.Find("Top player position").transform;

                    targetPlayer = InGameManager.Get().playerController;
                    InGameManager.Get().playerController.targetPlayer = this;
                }
            }
            else
            {
                if (InGameManager.Get().playerController != this)
                {
                    targetPlayer = InGameManager.Get().playerController;
                    InGameManager.Get().playerController.targetPlayer = this;
                }
            }

            SetColor();
        }
        #endregion
        
        #region spawn
        
        //[PunRPC]
        public void Spawn()
        {
            var magicCastDelay = 0.05f;
            for (var i = 0; i < arrDice.Length; i++)
            {
                if (arrDice[i].id >= 0 && arrDice[i].data != null && arrDice[i].data.prefab != null)
                {
                    var ts = transform.parent.GetChild(i);

                    switch(arrDice[i].data.castType)
                    {
                        case DICE_CAST_TYPE.MINION:
                            var multiply = arrDice[i].data.spawnMultiply;
                            var upgradeLevel = GetDiceUpgradeLevel(arrDice[i].data);

                            for (var j = 0; j < ((arrDice[i].level + 1) * multiply); j++)
                            {
                                CreateMinion(arrDice[i].data, ts.position, arrDice[i].level + 1, upgradeLevel, magicCastDelay, i);
                            }
                            break;
                        case DICE_CAST_TYPE.HERO:
                            multiply = arrDice[i].data.spawnMultiply;
                            upgradeLevel = GetDiceUpgradeLevel(arrDice[i].data);

                            //for (var j = 0; j < ((arrDice[i].level + 1) * multiply); j++)
                        {
                            CreateMinion(arrDice[i].data, ts.position, arrDice[i].level + 1, upgradeLevel, magicCastDelay, i);
                        }
                            break;
                        case DICE_CAST_TYPE.MAGIC:
                        case DICE_CAST_TYPE.INSTALLATION:
                            for(var j = 0; j < ((arrDice[i].level + 1) * arrDice[i].data.spawnMultiply); j++)
                            {
                                CastMagic(arrDice[i].data, magicCastDelay, i);
                            }
                            break;
                        default:
                            break;
                    }
                    magicCastDelay += 0.1f;
                }
            }
        }
        
        #endregion

        #region game sp & wave
        
        public void AddSp(int add)
        {
            sp += add;
        }

        public void AddSpByWave(int addSp)
        {
            sp += addSp * (50 + 10 * spUpgradeLevel);
        }
        
        public void SP_Upgrade()
        {
            if (spUpgradeLevel < 5)
            {
                sp -= (spUpgradeLevel + 1) * 500;
                spUpgradeLevel++;
                InGameManager.Get().event_SP_Edit.Invoke(sp);
            }
        }
        #endregion
        
        #region minion
        
        public void CreateMinion(Data_Dice data, Vector3 spawnPos, int eyeLevel, int upgradeLevel, float delay, int diceNum)
        {
            StartCoroutine(CreateMinionCoroutine(data, spawnPos, eyeLevel, upgradeLevel, delay, diceNum));
        }

        private IEnumerator CreateMinionCoroutine(Data_Dice data, Vector3 spawnPos, int eyeLevel, int upgradeLevel, float delay, int diceNum)
        {
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);   
            }
            
            
            if (!InGameManager.Get().isGamePlaying) yield break;

            if (uiDiceField != null && isMine && diceNum > 0)
            {
                var setting = uiDiceField.arrSlot[diceNum].ps.main;
                setting.startColor = data.color;
                uiDiceField.arrSlot[diceNum].ps.Play();
            }

            Vector3 dicePos = Vector3.zero;
            if (uiDiceField != null && diceNum > 0)
            {
                dicePos = uiDiceField.arrSlot[diceNum].transform.position;
            }
            else if (diceNum > 0)
            {
                dicePos = InGameManager.Get().playerController.uiDiceField.arrSlot[diceNum].transform.position;
                dicePos.x *= -1f;
                dicePos.z *= -1f;
            }

            if (PhotonNetwork.IsConnected && InGameManager.Get().playType != PLAY_TYPE.CO_OP && !photonView.IsMine)
            {
                dicePos.x *= -1f;
                dicePos.z *= -1f;
            }
            
            spawnPos.x += Random.Range(-0.2f, 0.2f);
            spawnPos.z += Random.Range(-0.2f, 0.2f);
            var m = PoolManager.instance.ActivateObject<Minion>(data.prefab.name, spawnPos, InGameManager.Get().transform);

            if (m == null)
            {
                PoolManager.instance.AddPool(data.prefab, 1);
                Debug.LogFormat("{0} Pool Added 1", data.prefab.name);
                m = PoolManager.instance.ActivateObject<Minion>(data.prefab.name, spawnPos, InGameManager.Get().transform);
            }
            
            if (m != null)
            {
                m.id = _spawnCount++;
                m.controller = this;
                m.isMine = PhotonNetwork.IsConnected ? photonView.IsMine : isMine;
                m.targetMoveType = data.targetMoveType;
                m.ChangeLayer(isBottomPlayer);
                m.power = data.power + (data.powerUpByInGameUp * upgradeLevel);
                m.maxHealth = data.maxHealth + (data.maxHealthUpByInGameUp * upgradeLevel);
                m.effect = data.effect + (data.effectUpByInGameUp * upgradeLevel);
                m.attackSpeed = data.attackSpeed;
                m.moveSpeed = data.moveSpeed;
                m.range = data.range;
                m.eyeLevel = eyeLevel;
                m.upgradeLevel = upgradeLevel;
                m.Initialize(MinionDestroyCallback);
                if (!listMinion.Contains(m)) listMinion.Add(m);
            }

            if (data.castType == DICE_CAST_TYPE.HERO)
            {
                m.power *= arrDice[diceNum].level + 1;
                m.maxHealth *= arrDice[diceNum].level + 1;
                m.effect *= arrDice[diceNum].level + 1;
            }

            if (diceNum > 0)
            {
                var lr = PoolManager.instance.ActivateObject<LineRenderer>("Effect_SpawnLine", Vector3.zero);
                if (lr != null)
                {
                    lr.SetPositions(new Vector3[2] {dicePos, m.hitPos.position});
                    lr.startColor = data.color;
                    lr.endColor = data.color;
                }
            }
        }

        public BaseStat GetBaseStatFromId(int baseStatId)
        {
            return baseStatId == 0 ? (BaseStat) this : listMinion.Find(minion => minion.id == baseStatId);
        }

        public Minion GetRandomMinion()
        {
            return listMinion[Random.Range(0, listMinion.Count)];
        }

        private void MinionDestroyCallback(Minion minion)
        {
            if (PhotonNetwork.IsConnected)
            {
                //photonView.RPC("RemoveMinion", RpcTarget.All, minion.id);
                SendPlayer(RpcTarget.All , E_PTDefine.PT_REMOVEMINION , minion.id);
            }
            else
            {
                RemoveMinion(minion.id);
            }
        }
        
        #endregion
        
        #region magic
        
        private void CastMagic(Data_Dice data, float delay, int diceNum)
        {
            StartCoroutine(CastMagicCoroutine(data, delay, diceNum));
        }

        private IEnumerator CastMagicCoroutine(Data_Dice data, float delay, int diceNum)
        {
            yield return new WaitForSeconds(delay);

            if (!InGameManager.Get().isGamePlaying) yield break;

            if (uiDiceField != null && isMine)
            {
                var setting = uiDiceField.arrSlot[diceNum].ps.main;
                setting.startColor = data.color;
                uiDiceField.arrSlot[diceNum].ps.Play();
            }
            
            Vector3 spawnPos;
            if (uiDiceField != null)
            {
                spawnPos = uiDiceField.arrSlot[diceNum].transform.position;
            }
            else
            {
                spawnPos = InGameManager.Get().playerController.uiDiceField.arrSlot[diceNum].transform.position;
                spawnPos.x *= -1f;
                spawnPos.z *= -1f;
            }

            if (PhotonNetwork.IsConnected && InGameManager.Get().playType != PLAY_TYPE.CO_OP && !photonView.IsMine)
            {
                spawnPos.x *= -1f;
                spawnPos.z *= -1f;
            }

            if (data.prefab != null)
            {
                var m = PoolManager.instance.ActivateObject<Magic>(data.prefab.name, spawnPos,
                    InGameManager.Get().transform);
                if (m != null)
                {
                    m.isMine = PhotonNetwork.IsConnected
                        ? photonView.IsMine
                        : (InGameManager.Get().playerController == this);
                    m.id = _spawnCount++;
                    m.controller = this;
                    m.range = data.range;
                    m.attackSpeed = data.attackSpeed;
                    m.diceFieldNum = diceNum;
                    m.targetMoveType = data.targetMoveType;
                    m.Initialize(isBottomPlayer, data.power, data.moveSpeed);
                    m.SetTarget();
                    listMagic.Add(m);
                }
                else
                {
                    yield break;
                }
            }
            else
            {
                yield break;
            }
        }

        #endregion
        
        #region dice system
        
        public void GetDice()
        {
            var emptyCount = GetDiceFieldEmptySlotCount();

            if (emptyCount == 0)
            {
                Debug.Log("DiceField is Full !!");
                return;
            }
            else
            {
                var emptySlotNum = 0;
                do
                {
                    emptySlotNum = Random.Range(0, arrDice.Length);
                } while (arrDice[emptySlotNum].id >= 0);

                var randomDeckNum = Random.Range(0, arrDeck.Length);
                arrDice[emptySlotNum].Set(arrDeck[randomDeckNum]);
                if (uiDiceField != null)
                {
                    uiDiceField.arrSlot[emptySlotNum].ani.SetTrigger("BBoing");
                    uiDiceField.SetField(arrDice);
                }

                if (PhotonNetwork.IsConnected) 
                    //photonView.RPC("GetDice", RpcTarget.Others, randomDeckNum, emptySlotNum);
                    SendPlayer(RpcTarget.Others , E_PTDefine.PT_GETDICE , randomDeckNum, emptySlotNum);
            }
        }

        
        //[PunRPC]
        public void SetDeck(string deck)
        {
            var splitDeck = deck.Split('/');
            if (arrDeck == null) arrDeck = new Data_Dice[5];

            for (var i = 0; i < splitDeck.Length; i++)
            {
                var num = int.Parse(splitDeck[i]);
                arrDeck[i] = InGameManager.Get().data_AllDice.listDice.Find(data => data.id == num);
                
                // add pool
                //Debug.LogFormat(gameObject.name + " AddPool: " + arrDeck[i].prefab.name);
                if (PoolManager.instance == null) Debug.Log("PoolManager Instnace is null");
                if (arrDeck[i] == null) Debug.LogError(string.Format("{0},i={1}:arrDeck[i] is null", gameObject.name, i));
                if (arrDeck[i].prefab == null) Debug.LogError(string.Format("{0}, arrDeck[{1}].prefab is null", gameObject.name, i));
                PoolManager.instance.AddPool(arrDeck[i].prefab, 50);
            }
        }

        //[PunRPC]
        public void GetDice(int deckNum, int slotNum)
        {
            arrDice[slotNum].Set(arrDeck[deckNum]);
        }

        //[PunRPC]
        public void LevelUpDice(int resetFieldNum, int levelupFieldNum, int levelupDiceId, int level)
        {
            arrDice[resetFieldNum].Reset();
            foreach (var data in InGameManager.Get().data_AllDice.listDice.Where(data => levelupDiceId == data.id))
            {
                arrDice[levelupFieldNum].Set(data, level);
                break;
            }
        }

        public int GetDiceFieldEmptySlotCount()
        {
            return arrDice.Count(dice => dice.id < 0);
        }

        public int DiceUpgrade(int deckNum)
        {
            return ++arrUpgradeLevel[deckNum];
        }

        private int GetDiceUpgradeLevel(Object data)
        {
            var num = 0;
            for (var i = 0; i < arrDeck.Length; i++)
            {
                if (arrDeck[i] != data) continue;
                num = i;
                break;
            }

            return arrUpgradeLevel[num];
        }

        
        #endregion
        
        #region etc system
        
        //[PunRPC]
        public override void HitDamage(float damage, float delay = 0)
        {
            if (currentHealth > 0)
            {
                currentHealth -= damage;

                if (currentHealth <= 0)
                {
                    //InGameManager.Get().obj_Low_HP_Effect.SetActive(false);
                    UI_InGamePopup.Get().ViewLowHP(false);
                    currentHealth = 0;
                    Death();
                }
                else if (((PhotonNetwork.IsConnected && photonView.IsMine) || (!PhotonNetwork.IsConnected && isMine)) 
                         && currentHealth < 1000 && !UI_InGamePopup.Get().GetLowHP())
                {
                    //InGameManager.Get().obj_Low_HP_Effect.SetActive(true);
                    UI_InGamePopup.Get().ViewLowHP(true);
                }
            }

            RefreshHealthBar();
        }

        private void Death()
        {
            if (InGameManager.Get().isGamePlaying)
            {
                if (PhotonNetwork.IsConnected)
                {
                    //InGameManager.Get().photonView.RPC("EndGame", RpcTarget.All);
                    InGameManager.Get().SendBattleManager(RpcTarget.All , E_PTDefine.PT_ENDGAME );
                }
                else
                {
                    InGameManager.Get().EndGame(new PhotonMessageInfo());
                }
            }
        }

        private void RefreshHealthBar()
        {
            image_HealthBar.fillAmount = currentHealth / maxHealth;
            text_Health.text = $"{Mathf.CeilToInt(currentHealth)}";
        }

        //[PunRPC]
        public void ChangeLayer(bool pIsBottomPlayer)
        {
            gameObject.layer = LayerMask.NameToLayer(pIsBottomPlayer ? "BottomPlayer" : "TopPlayer");
            objCollider.layer = LayerMask.NameToLayer(pIsBottomPlayer ? "BottomPlayer" : "TopPlayer");
            this.isBottomPlayer = pIsBottomPlayer;

            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient == false && PhotonManager.Instance.playType == PLAY_TYPE.BATTLE)
            {
                transform.rotation = Quaternion.Euler(0, 180f, 0);
            }

            if (PhotonNetwork.IsConnected && PhotonManager.Instance.playType == PLAY_TYPE.CO_OP)
            {
                if ((PhotonNetwork.IsMasterClient && photonView.IsMine == false)
                    || (PhotonNetwork.IsMasterClient == false && photonView.IsMine == true))
                {
                    GetComponentInChildren<Collider>().enabled = false;
                    transform.GetChild(0).gameObject.SetActive(false);
                    transform.GetChild(1).gameObject.SetActive(false);
                }
            }
        }


        #endregion
        
        #region dice rpc
        
        //////////////////////////////////////////////////////////////////////
        // Dice RPCs
        //////////////////////////////////////////////////////////////////////
        //[PunRPC]
        private void RemoveMinion(int baseStatId)
        {
            listMinion.Remove(listMinion.Find(minion => minion.id == baseStatId));
        }

        public void MagicDestroyCallback(Magic magic)
        {
            if (PhotonNetwork.IsConnected)
            {
                //photonView.RPC("RemoveMagic", RpcTarget.All, magic.id);
                SendPlayer(RpcTarget.All , E_PTDefine.PT_REMOVEMAGIC , magic.id);
            }
            else
            {
                RemoveMagic(magic.id);
            }
        }

        //[PunRPC]
        private void RemoveMagic(int baseStatId)
        {
            listMagic.Remove(listMagic.Find(magic => magic.id == baseStatId));
        }

        
        public void AttackEnemyMinion(int baseStatId, float damage, float delay)
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                //targetPlayer.photonView.RPC("HitDamageMinion", RpcTarget.All, baseStatId, damage, delay);
                targetPlayer.SendPlayer(RpcTarget.All , E_PTDefine.PT_HITMINION , baseStatId, damage, delay);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                targetPlayer.HitDamageMinion(baseStatId, damage, delay);
            }
        }

        //[PunRPC]
        public void HitDamageMinion(int baseStatId, float damage, float delay)
        {
            // baseStatId == 0 => Player tower
            if (baseStatId == 0)
            {
                if (PhotonNetwork.IsConnected)
                {
                    //photonView.RPC("HitDamage", RpcTarget.All, damage, delay);
                    SendPlayer(RpcTarget.All , E_PTDefine.PT_HITDAMAGE , damage, delay);
                }
                else
                {
                    HitDamage(damage, delay);
                }
            }
            else
            {
                listMinion.Find(minion => minion.id == baseStatId)?.HitDamage(damage, delay);
            }
        }

        public void DeathMinion(int baseStatId)
        {
            if (PhotonNetwork.IsConnected)
            {
                //photonView.RPC("DestroyMinion", RpcTarget.All, baseStatId);
                SendPlayer(RpcTarget.All , E_PTDefine.PT_DESTROYMINION , baseStatId);
            }
            else
            {
                DestroyMinion(baseStatId);
            }
        }

        //[PunRPC]
        private void DestroyMinion(int baseStatId)
        {
            listMinion.Find(minion => minion.id == baseStatId)?.Death();
        }

        public void HealMinion(int baseStatId, float heal)
        {
            if (PhotonNetwork.IsConnected)
            {
                //photonView.RPC("HealMinionRpc", RpcTarget.All, baseStatId, heal);
                SendPlayer(RpcTarget.All , E_PTDefine.PT_HEALMINION , baseStatId, heal);
            }
            else
            {
                HealMinionRpc(baseStatId, heal);
            }
        }

        //[PunRPC]
        private void HealMinionRpc(int baseStatId, float heal)
        {
            listMinion.Find(minion => minion.id == baseStatId)?.Heal(heal);
        }

        //[PunRPC]
        public void PushMinion(int baseStatId, Vector3 dir, float pushPower)
        {
            listMinion.Find(minion => minion.id == baseStatId)?.Push(dir, pushPower);
        }

        //[PunRPC]
        public void SturnMinion(int baseStatId, float duration)
        {
            listMinion.Find(minion => minion.id == baseStatId)?.Sturn(duration);
        }

        //[PunRPC]
        public void SetMinionAnimationTrigger(int baseStatId, string trigger)
        {
            listMinion.Find(minion => minion.id == baseStatId)?.animator.SetTrigger(trigger);
        }
        
        

        //////////////////////////////////////////////////////////////////////
        // Unit's RPCs
        //////////////////////////////////////////////////////////////////////
        //[PunRPC]
        public void FireArrow(Vector3 startPos, int targetId, float damage)
        {
            var b = PoolManager.instance.ActivateObject<Bullet>("Bullet", startPos);
            if (b != null)
            {
                b.transform.rotation = Quaternion.identity;
                b.controller = this;
                b.Initialize(targetId, damage, isMine, isBottomPlayer);
            }
        }
        
        //[PunRPC]
        public void FireSpear(Vector3 startPos, int targetId, float damage)
        {
            var b = PoolManager.instance.ActivateObject<Bullet>("Spear", startPos);
            if (b != null)
            {
                b.transform.rotation = Quaternion.identity;
                b.controller = this;
                b.Initialize(targetId, damage, isMine, isBottomPlayer);
            }
        }

        //[PunRPC]
        public void FireCannonBall(Vector3 startPos, Vector3 targetPos, float damage)
        {
            var b = PoolManager.instance.ActivateObject<CannonBall>("CannonBall", startPos);
            if (b != null)
            {
                b.transform.rotation = Quaternion.identity;
                b.controller = this;
                b.Initialize(targetPos, damage, isMine, isBottomPlayer);
            }
        }

        //[PunRPC]
        public void FireballBomb(int baseStatId)
        {
            ((Fireball)listMagic.Find(magic => magic.id == baseStatId))?.Bomb();
        }

        //[PunRPC]
        public void MineBomb(int baseStatId)
        {
            ((Mine)listMagic.Find(magic => magic.id == baseStatId))?.Bomb();
        }

        //[PunRPC]
        public void SetMagicTarget(int baseStatId, int targetId)
        {
            StartCoroutine(SetMagicTargetCoroutine(baseStatId, targetId));
        }

        private IEnumerator SetMagicTargetCoroutine(int baseStatId, int targetId)
        {
            while (listMagic.Find(magic => magic.id == baseStatId) == null) yield return null;
            listMagic.Find(magic => magic.id == baseStatId)?.SetTarget(targetId);
        }

        //[PunRPC]
        public void SetMagicTarget(int baseStatId, float x, float z)
        {
            StartCoroutine(SetMagicTargetCoroutine(baseStatId, x, z));
        }

        private IEnumerator SetMagicTargetCoroutine(int baseStatId, float x, float z)
        {
            while (listMagic.Find(magic => magic.id == baseStatId) == null) yield return null;
            listMagic.Find(magic => magic.id == baseStatId)?.SetTarget(x, z);
        }

        //[PunRPC]
        public void TeleportMinion(int baseStatId, float x, float z)
        {
            listMinion.Find(minion => minion.id == baseStatId).transform.position = new Vector3(x, 0, z);
        }

        //[PunRPC]
        public void FiremanFire(int baseStatId)
        {
            ((Minion_Fireman)listMinion.Find(minion => minion.id == baseStatId))?.Fire();
        }

        //[PunRPC]
        public void SpawnSkeleton(Vector3 pos)
        {
            CreateMinion(InGameManager.Get().data_AllDice.listDice[1], pos, 1, 0, 0, -1);
        }

        //[PunRPC]
        public void SetMinionAttackSpeedFactor(int baseStatId, float factor)
        {
            listMinion.Find(minion => minion.id == baseStatId)?.SetAttackSpeedFactor(factor);
        }
        
        #endregion
        
        #region photon override
        
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(currentHealth);

                stream.SendNext(listMinion.Count);
                foreach (var m in listMinion)
                {
                    stream.SendNext(m.rb.position);
                    stream.SendNext(m.rb.rotation);
                    //stream.SendNext(m.rb.velocity);
                    stream.SendNext(m.agent.velocity);
                    stream.SendNext(m.currentHealth);
                }
            }
            else
            {
                currentHealth = (float)stream.ReceiveNext();

                var loopCount = (int)stream.ReceiveNext();
                for (var i = 0; i < loopCount && i < listMinion.Count; i++)
                {
                    listMinion[i].SetNetworkValue((Vector3)stream.ReceiveNext(), (Quaternion)stream.ReceiveNext(), (Vector3)stream.ReceiveNext(), (float)stream.ReceiveNext(), info.SentServerTime);
                }
            }
        }

        #endregion
        
        #region photon send recv
        public void SendPlayer(RpcTarget target , E_PTDefine ptID , params object[] param)
        {
            photonView.RPC("RecvPlayer", target , ptID , param);
        }

        [PunRPC]
        public void RecvPlayer(E_PTDefine ptID , params object[] param)
        {
            switch (ptID)
            {
                case E_PTDefine.PT_SETDECK:
                    string deck = param[0] as string;
                    SetDeck(deck);
                    break;
                case E_PTDefine.PT_CHANGELAYER:
                    ChangeLayer(param[0] is bool);
                    break;
                case E_PTDefine.PT_REMOVEMINION:
                    int baseID = (int)param[0];
                    RemoveMinion(baseID);
                    break;
                case E_PTDefine.PT_GETDICE:
                    int deckNum = (int)param[0];
                    int slotNum = (int)param[1];
                    GetDice(deckNum, slotNum);
                    break;
                case E_PTDefine.PT_LEVELUPDICE:
                    LevelUpDice((int) param[0], (int) param[1], (int) param[2], (int) param[3]);
                    break;
                case E_PTDefine.PT_REMOVEMAGIC:
                    int magicId = (int)param[0];
                    RemoveMagic(magicId);
                    break;
                case E_PTDefine.PT_HITMINION:
                    int baseIDhit = (int)param[0];
                    int damage = (int)param[1];
                    int delay = (int)param[2];
                    //targetPlayer.HitDamageMinion(baseIDhit, damage, delay);
                    HitDamageMinion(baseIDhit, damage, delay);
                    break;
                case E_PTDefine.PT_HITDAMAGE:
                    int damageH = (int)param[0];
                    int delayH = (int)param[1];
                    HitDamage(damageH, delayH);
                    break;
                case E_PTDefine.PT_DESTROYMINION:
                    int baseIdD = (int)param[0];
                    DestroyMinion(baseIdD);
                    break;
                case E_PTDefine.PT_HEALMINION:
                    int baseId = (int)param[0];
                    int heal = (int)param[1];
                    HealMinionRpc(baseId, heal);
                    break;
                case E_PTDefine.PT_FIREBALLBOMB:
                    FireballBomb((int) param[0]);
                    break;
                case E_PTDefine.PT_MINIONATTACKSPEEDFACTOR:
                    SetMinionAttackSpeedFactor((int) param[0], (float) param[1]);
                    break;
                case E_PTDefine.PT_STURNMINION:
                    SturnMinion((int) param[0], (float) param[1]);
                    break;
                case E_PTDefine.PT_SETMAGICTARGET:
                    if (param[2] != null)
                    {
                        SetMagicTarget((int) param[0], (float) param[1], (float) param[2]);
                    }
                    else
                    {
                        SetMagicTarget((int) param[0], (int) param[1]);
                    }
                    break;
                case E_PTDefine.PT_MINEBOMB:
                    MineBomb((int)param[0]);
                    break;
                case E_PTDefine.PT_FIRECANNONBALL:
                    FireCannonBall((Vector3) param[0], (Vector3) param[1], (float) param[2]);
                    break;
                case E_PTDefine.PT_FIREARROW:
                    FireArrow( (Vector3)param[0] , (int)param[1] , (float)param[2]);
                    break;
                case E_PTDefine.PT_MINIONANITRIGGER:
                    SetMinionAnimationTrigger((int) param[0], (string) param[1]);
                    break;
                case E_PTDefine.PT_FIREMANFIRE:
                    FiremanFire((int) param[0]);
                    break;
                case E_PTDefine.PT_FIRESPEAR:
                    FireSpear((Vector3) param[0], (int) param[1], (float) param[2]);
                    break;
                case E_PTDefine.PT_SPAWNSKELETON:
                    SpawnSkeleton((Vector3) param[0]);
                    break;
                case E_PTDefine.PT_TELEPORTMINION:
                    TeleportMinion((int) param[0],(float)param[1] , (float)param[2]);
                    break;
                
            }
        }
        #endregion
        
        
    }
}