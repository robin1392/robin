﻿#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;

using RandomWarsProtocol;
using RandomWarsProtocol.Msg;

//
using UnityEngine.Serialization;

using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


#region photon
//using Photon.Pun;
#endregion

namespace ED
{
    public class PlayerController : BaseStat
    {

        #region singleton
        
        private static PlayerController _instance = null;
        public int instanceID;

        public static PlayerController Get()
        {
            return _instance;
        }
        
        protected void Init()
        {
            if (_instance == null)
            {
                instanceID = GetInstanceID();
                _instance = this as PlayerController;
            }

            if (instanceID == 0)
                instanceID = GetInstanceID();
        }
        
        #endregion
        
        
        
        #region player variable.
        
        public PlayerController targetPlayer;
        public PlayerController coopPlayer;
        #endregion
        
        
        
        #region data variable

        // new dice info
        protected DiceInfoData[] _arrDiceDeck;
        public DiceInfoData[] arrDiceDeck
        {
            get => _arrDiceDeck;
            protected set => _arrDiceDeck = value;
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
        public ParticleSystem ps_ShieldOff;
        public GameObject pref_Guardian;
        
        [SerializeField]
        public int spawnCount = 1;
        public int subSpawnCount = 1000;
        
        [SerializeField]
        protected int _sp = 0;
        public virtual int sp 
        { 
            get => _sp;
            protected set
            {
                _sp = value;
                if (isMine) InGameManager.Get().event_SP_Edit.Invoke(_sp);
            }
        }

        public int robotPieceCount;
        public int robotEyeTotalLevel;

        #endregion
        
        #region minion & magic
        
        //protected List<Minion> _listMinion = new List<Minion>();

        protected List<Minion> _listMinion = new List<Minion>();
        [SerializeField]
        public List<Minion> listMinion
        {
            get { return _listMinion; }
            protected set { _listMinion = value; }
        }
        // {
        //     get => _listMinion;
        //     protected set => _listMinion = value;
        // }
        
        [SerializeField]
        protected List<Magic> _listMagic = new List<Magic>();
        public List<Magic> listMagic
        {
            get { return _listMagic; }
            protected set { _listMagic = value; }
        }
        private readonly string recvMessage = "RecvPlayer";
        private static readonly int Break = Animator.StringToHash("Break");
        public bool isHalfHealth;
        public bool isPlayingAI { get; protected set; }
        public bool isMinionAgentMove = true;
        protected Coroutine crt_SyncMinion;
        protected Queue<int> queueHitDamage = new Queue<int>();
        protected int myUID;
        public int UID => myUID;

        #endregion

        #region unity base
        protected void Awake()
        {
            InitializePlayer();
        }

        protected override void Start()
        {   
            //
            if (_instance == null && isMine)
            {
                _instance = this;
            }
            instanceID = GetInstanceID();
            
            if (InGameManager.Get().playType != Global.PLAY_TYPE.COOP)
            {
                base.Start();
            }
            
            StartPlayerControll();
        }

        private void Update()
        {
            RefreshHealthBar();

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.P))
            {
                PushEnemyMinions(30f);
                SummonGuardian();
            }
#endif
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
            if (arrDiceDeck == null) _arrDiceDeck = new DiceInfoData[5];
            _arrUpgradeLevel = new int[5];
        }

        public void DestroyPlayer()
        {
            if(NetworkManager.Get() != null && isMine) NetworkManager.Get().event_OtherPause.RemoveListener(OtherPlayerPause);
            _arrDice = null;
            _arrDiceDeck = null;
            _arrUpgradeLevel = null;
        }

        protected virtual void StartPlayerControll()
        {
            myUID = isMine ? NetworkManager.Get().UserUID : NetworkManager.Get().OtherUID;
            
            if (isMine)
            {
                NetworkManager.Get().event_OtherPause.AddListener(OtherPlayerPause);
                StartCoroutine(HitDamageQueueCoroutine());
            }

            isHalfHealth = false;

            if( InGameManager.IsNetwork == false )
                sp = 200;

            maxHealth = ConvertNetMsg.MsgIntToFloat(isMine ? NetworkManager.Get().GetNetInfo().playerInfo.TowerHp : NetworkManager.Get().GetNetInfo().otherInfo.TowerHp);
            
            currentHealth = maxHealth;
            RefreshHealthBar();

            for (var i = 0; i < arrDice.Length; i++)
            {
                arrDice[i] = new Dice {diceFieldNum = i};
            }
            uiDiceField = FindObjectOfType<UI_DiceField>();
            uiDiceField.SetField(arrDice);

            
            // 
            //image_HealthBar = WorldUIManager.Get().GetHealthBar(isBottomPlayer);
            //text_Health = WorldUIManager.Get().GetHealthText(isBottomPlayer);
            //text_Health.text = $"{Mathf.CeilToInt(currentHealth)}";
            
            InGameManager.Get().AddPlayerUnit(isBottomPlayer, this);
            
            SetColor(isBottomPlayer ? E_MaterialType.BOTTOM : E_MaterialType.TOP);

            //
            //StartCoroutine(SyncMinionStatus());
            StartSyncMinion();
        }
        
        protected override void SetColor(E_MaterialType type)
        {
            var mr = GetComponentsInChildren<MeshRenderer>();
            foreach (var m in mr)
            {
                if (m.gameObject.CompareTag("Finish")) continue;

                if (NetworkManager.Get().playType == Global.PLAY_TYPE.BATTLE)
                    m.material = arrMaterial[isMine ? 0 : 1];
                else if (NetworkManager.Get().playType == Global.PLAY_TYPE.COOP)
                    m.material = arrMaterial[isBottomPlayer ? 0 : 1];
                    
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

        #endregion
        
        #region spawn
        
        public void Spawn()
        {
            packetCount = 0;
            var magicCastDelay = 0.05f;
            robotPieceCount = 0;
            robotEyeTotalLevel = 0;

            for (var i = 0; i < arrDice.Length; i++)
            {
                //if (arrDice[i].id >= 0 && arrDice[i].data != null && arrDice[i].data.prefab != null)
                if (arrDice[i].id >= 0 && arrDice[i].diceData != null )
                {
                    //var ts = transform.parent.GetChild(i);
                    Transform ts = isBottomPlayer ? FieldManager.Get().GetBottomListTs(i): FieldManager.Get().GetTopListTs(i);
                    
                    //var upgradeLevel = GetDiceUpgradeLevel(arrDice[i].data);
                    var upgradeLevel = GetDiceUpgradeLevel(arrDice[i].diceData);
                    
                    //var multiply = arrDice[i].data.spawnMultiply;
                    var multiply = arrDice[i].diceData.spawnMultiply;

                    switch(arrDice[i].diceData.castType)
                    {
                    case (int)DICE_CAST_TYPE.MINION:
                        for (var j = 0; j < (arrDice[i].eyeLevel + 1) * multiply; j++)
                        {
                            CreateMinion(arrDice[i].diceData, ts.position, arrDice[i].eyeLevel + 1, upgradeLevel, magicCastDelay, i);
                        }
                        break;
                    case (int)DICE_CAST_TYPE.HERO:
                        CreateMinion(arrDice[i].diceData, ts.position, arrDice[i].eyeLevel + 1, upgradeLevel, magicCastDelay, i);
                        break;
                    case (int)DICE_CAST_TYPE.MAGIC:
                    case (int)DICE_CAST_TYPE.INSTALLATION:
                        CastMagic(arrDice[i].diceData, arrDice[i].eyeLevel + 1, upgradeLevel, magicCastDelay, i);
                        break;

                    }
                    magicCastDelay += 0.066666f;
                }
            }
        }
        
        #endregion

        
        
        #region game sp & wave
        
        public void AddSp(int add)
        {
            sp += add;
        }

        private readonly int[] arrSPUpgradeValue = {10, 15, 20, 25, 30, 35};
        public void AddSpByWave(int addSp)
        {
            sp += 40 + addSp * arrSPUpgradeValue[spUpgradeLevel];
        }

        public void SetSp(int sp)
        {
            this.sp = sp;
        }
        
        public void SP_Upgrade()
        {
            if (spUpgradeLevel < 5)
            {
                sp -= (spUpgradeLevel + 1) * 100;
                spUpgradeLevel++;
                InGameManager.Get().event_SP_Edit.Invoke(sp);
            }
        }

        public void SP_Upgrade(int upgradeLv, int curSp)
        {
            spUpgradeLevel = upgradeLv;
            SetSp(curSp);
            InGameManager.Get().event_SP_Edit.Invoke(sp);
        }

        public void SP_Upgrade(int upgradeLv)
        {
            spUpgradeLevel = upgradeLv;
        }
        
        public void RefreshHealthBar()
        {
            image_HealthBar.fillAmount = currentHealth / maxHealth;
            text_Health.text = $"{Mathf.CeilToInt(currentHealth)}";
        }

        #endregion
        
        #region minion

        public Minion CreateMinion(GameObject pref, Vector3 spawnPos, int eyeLevel, int upgradeLevel, bool isSpawnCountUp = true)
        {
            var m = PoolManager.instance.ActivateObject<Minion>(pref.name, spawnPos, InGameManager.Get().transform);

            if (m == null)
            { 
                PoolManager.instance.AddPool(pref, 1);
                //Debug.LogFormat("{0} Pool Added 1", pref.name);
                m = PoolManager.instance.ActivateObject<Minion>(pref.name, spawnPos, InGameManager.Get().transform);
            }

            if (m != null)
            {
                if (isSpawnCountUp) m.id = myUID * 10000 + subSpawnCount++;
                m.controller = this;
                //m.isMine = PhotonNetwork.IsConnected ? photonView.IsMine : isMine;
                m.isMine = isMine;
                
                if (!listMinion.Contains(m)) 
                    listMinion.Add(m);
            }

            return m;
        }
        
        //public void CreateMinion(Data_Dice data, Vector3 spawnPos, int eyeLevel, int upgradeLevel, float delay, int diceNum)
        public void CreateMinion(DiceInfoData data, Vector3 spawnPos, int eyeLevel, int upgradeLevel, float delay, int diceNum)
        {
            StartCoroutine(CreateMinionCoroutine(data, spawnPos, eyeLevel, upgradeLevel, delay, diceNum));
        }

        //private IEnumerator CreateMinionCoroutine(Data_Dice data, Vector3 spawnPos, int eyeLevel, int upgradeLevel, float delay, int diceNum)
        private IEnumerator CreateMinionCoroutine(DiceInfoData data, Vector3 spawnPos, int eyeLevel, int upgradeLevel, float delay, int diceNum)
        {
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }
            
            if (InGameManager.Get().isGamePlaying == false) yield break;

            if (uiDiceField != null && isMine && diceNum > 0)
            {
                var setting = uiDiceField.arrSlot[diceNum].ps.main;
                setting.startColor = FileHelper.GetColor(data.color);
                uiDiceField.arrSlot[diceNum].ps.Play();
            }

            Vector3 dicePos = Vector3.zero;
            if (diceNum >= 0)
            {
                if (uiDiceField != null)
                {
                    dicePos = uiDiceField.arrSlot[diceNum].transform.position;
                }
                else
                {
                    dicePos = InGameManager.Get().playerController.uiDiceField.arrSlot[diceNum].transform.position;
                    dicePos.x *= -1f;
                    dicePos.z *= -1f; 
                }
            }

            //if (PhotonNetwork.IsConnected && InGameManager.Get().playType != PLAY_TYPE.CO_OP && !photonView.IsMine)
            if (InGameManager.IsNetwork && InGameManager.Get().playType != Global.PLAY_TYPE.COOP && !isMine)
            {
                dicePos.x *= -1f;
                dicePos.z *= -1f;
            }
            
            //Debug.LogFormat("Spawn: {0}", data.prefabName);
            //FileHelper.LoadPrefab(data.prefabName , Global.E_LOADTYPE.LOAD_MINION )
            spawnPos.x += Random.Range(-0.2f, 0.2f);
            spawnPos.z += Random.Range(-0.2f, 0.2f);
            var m = PoolManager.instance.ActivateObject<Minion>( data.prefabName, spawnPos, InGameManager.Get().transform);

            if (m == null)
            {
                //PoolManager.instance.AddPool(data.prefab, 1);
                PoolManager.instance.AddPool(FileHelper.LoadPrefab(data.prefabName , Global.E_LOADTYPE.LOAD_MINION , InGameManager.Get().transform), 1);
                //Debug.LogFormat("{0} Pool Added 1", data.prefabName);
                m = PoolManager.instance.ActivateObject<Minion>(data.prefabName, spawnPos, InGameManager.Get().transform);
            }
            
            if (m != null)
            {
                m.castType = (DICE_CAST_TYPE)data.castType;
                m.id = myUID * 10000 + spawnCount++;
                m.diceId = data.id;
                m.controller = this;
                //m.isMine = PhotonNetwork.IsConnected ? photonView.IsMine : isMine;
                m.isMine = isMine;
                m.targetMoveType = (DICE_MOVE_TYPE)data.targetMoveType;
                m.ChangeLayer(isBottomPlayer);

                // new code - by nevill
                int wave = InGameManager.Get().wave;
                var myInfo = isMine
                    ? NetworkManager.Get().GetNetInfo().playerInfo
                    : NetworkManager.Get().GetNetInfo().otherInfo;
                var arrDiceLevel = myInfo.DiceLevelArray;
                int deckNum = -1;
                for (int i = 0; i < arrDiceDeck.Length; i++)
                {
                    if (arrDiceDeck[i] == data)
                    {
                        deckNum = i;
                        break;
                    }
                }
                m.power = data.power + (data.powerUpgrade * arrDiceLevel[deckNum]) + (data.powerInGameUp * upgradeLevel);
                if (wave > 10)
                {
                    m.power *= Mathf.Pow(2f, wave - 10);
                }
                m.powerUpByUpgrade = data.powerUpgrade;
                m.powerUpByInGameUp = data.powerInGameUp;
                m.maxHealth = data.maxHealth + (data.maxHpUpgrade * arrDiceLevel[deckNum]) + (data.maxHpInGameUp * upgradeLevel);
                m.maxHealthUpByUpgrade = data.maxHpUpgrade;
                m.maxHealthUpByInGameUp = data.maxHpInGameUp;
                m.effect = data.effect + (data.effectUpgrade * arrDiceLevel[deckNum]) + (data.effectInGameUp * upgradeLevel);
                m.effectUpByUpgrade = data.effectUpgrade;
                m.effectUpByInGameUp = data.effectInGameUp;
                m.effectDuration = data.effectDuration;
                m.effectCooltime = data.effectCooltime;
                
                m.attackSpeed = data.attackSpeed;
                if (wave > 10)
                {
                    m.attackSpeed *= Mathf.Pow(0.9f, wave - 10);
                    if (m.attackSpeed < data.attackSpeed * 0.5f) m.attackSpeed = data.attackSpeed * 0.5f;
                }
                m.moveSpeed = data.moveSpeed;
                m.range = data.range;
                m.searchRange = data.searchRange;
                m.eyeLevel = eyeLevel;
                m.upgradeLevel = upgradeLevel;
                
                if ((DICE_CAST_TYPE)data.castType == DICE_CAST_TYPE.HERO)
                {
                    m.power *= arrDice[diceNum].eyeLevel + 1;
                    m.maxHealth *= arrDice[diceNum].eyeLevel + 1;
                    m.effect *= arrDice[diceNum].eyeLevel + 1;
                }

                m.Initialize(MinionDestroyCallback);
                
                if (!listMinion.Contains(m)) 
                    listMinion.Add(m);
            }

            if (diceNum >= 0)
            {
                var lr = PoolManager.instance.ActivateObject<LineRenderer>("Effect_SpawnLine", Vector3.zero);
                if (lr != null)
                {
                    lr.SetPositions(new Vector3[2] {dicePos, m.ts_HitPos.position});
                    lr.startColor = FileHelper.GetColor(data.color);//data.color;
                    lr.endColor = FileHelper.GetColor(data.color);//data.color;
                }
            }

            SoundManager.instance.Play(Global.E_SOUND.SFX_MINION_GENERATE);
        }
        //
        // public BaseStat GetBaseStatFromId(int baseStatId)
        // {
        //     if (baseStatId < 0) return null;
        //     if (baseStatId == 0) return this;
        //
        //     var minion = listMinion.Find(m => m.id == baseStatId);
        //     if (minion != null)
        //     {
        //         return minion;
        //     }
        //     else
        //     {
        //         var magic = _listMagic.Find(m => m.id == baseStatId);
        //         if (magic != null)
        //         {
        //             return magic;
        //         }
        //         else
        //         {
        //             return null;
        //         }
        //     }
        // }

        public Minion GetRandomMinion()
        {
            if (listMinion.Count > 0)
            {
                return listMinion[Random.Range(0, listMinion.Count)];
            }
            else
            {
                return null;
            }
        }

        
        #endregion
        
        #region magic
        
        //private void CastMagic(Data_Dice data, int eyeLevel, int upgradeLevel, float delay, int diceNum)
        private void CastMagic(DiceInfoData data, int eyeLevel, int upgradeLevel, float delay, int diceNum)
        {
            StartCoroutine(CastMagicCoroutine(data, eyeLevel, upgradeLevel, delay, diceNum));
        }

        //private IEnumerator CastMagicCoroutine(Data_Dice data, int eyeLevel, int upgradeLevel, float delay, int diceNum)
        private IEnumerator CastMagicCoroutine(DiceInfoData data, int eyeLevel, int upgradeLevel, float delay, int diceNum)
        {
            yield return new WaitForSeconds(delay);

            //Debug.LogFormat("Spawn: {0}", data.prefabName);
            if (InGameManager.Get().isGamePlaying == false) yield break;

            if (uiDiceField != null && isMine)
            {
                var setting = uiDiceField.arrSlot[diceNum].ps.main;
                setting.startColor = FileHelper.GetColor(data.color);//data.color;
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

            //if (PhotonNetwork.IsConnected && InGameManager.Get().playType != PLAY_TYPE.CO_OP && !photonView.IsMine)
            if (InGameManager.IsNetwork && InGameManager.Get().playType != Global.PLAY_TYPE.COOP && !isMine)
            {
                spawnPos.x *= -1f;
                spawnPos.z *= -1f;
            }

            GameObject loadMagic = FileHelper.LoadPrefab(data.prefabName, Global.E_LOADTYPE.LOAD_MAGIC , InGameManager.Get().transform);
            //if (data.prefab != null)
            if(loadMagic != null )
            {
                var m = PoolManager.instance.ActivateObject<Magic>(data.prefabName, spawnPos, InGameManager.Get().transform);
                if (m != null)
                {
                    //m.isMine = PhotonNetwork.IsConnected ? photonView.IsMine : (InGameManager.Get().playerController == this);
                    m.isMine = InGameManager.IsNetwork ? isMine : (InGameManager.Get().playerController == this);
                    m.id = spawnCount++;
                    m.controller = this;
                    m.diceFieldNum = diceNum;
                    m.targetMoveType = (DICE_MOVE_TYPE)data.targetMoveType;
                    m.castType = (DICE_CAST_TYPE)data.castType;

                    int wave = InGameManager.Get().wave;
                    m.power = (data.power + (data.powerInGameUp * upgradeLevel)) * Mathf.Pow(1.5f, eyeLevel - 1);
                    if (wave > 10)
                    {
                        m.power *= Mathf.Pow(2f, wave - 10);
                    }
                    m.powerUpByUpgrade = data.powerUpgrade;
                    m.powerUpByInGameUp = data.powerInGameUp;
                    m.maxHealth = (data.maxHealth + (data.maxHpInGameUp * upgradeLevel)) * Mathf.Pow(2f, eyeLevel - 1);
                    m.maxHealthUpByUpgrade = data.maxHpUpgrade;
                    m.maxHealthUpByInGameUp = data.maxHpInGameUp;
                    m.effect = (data.effect + (data.effectInGameUp * upgradeLevel)) * Mathf.Pow(1.5f, eyeLevel - 1);
                    m.effectUpByUpgrade = data.effectUpgrade;
                    m.effectUpByInGameUp = data.effectInGameUp;
                    m.effectDuration = data.effectDuration;
                    m.effectCooltime = data.effectCooltime;
                
                    m.attackSpeed = data.attackSpeed;
                    if (wave > 10)
                    {
                        m.attackSpeed *= Mathf.Pow(0.9f, wave - 10);
                        if (m.attackSpeed < data.attackSpeed * 0.5f) m.attackSpeed = data.attackSpeed * 0.5f;
                    }
                    m.moveSpeed = data.moveSpeed;
                    m.range = data.range;
                    m.searchRange = data.searchRange;
                    m.eyeLevel = eyeLevel;
                    m.upgradeLevel = upgradeLevel;
                    
                    m.Initialize(isBottomPlayer);
                    m.SetTarget();
                    
                    _listMagic.Add(m);
                }
            }
        }

        #endregion
        
        
        
        #region net dice system
        public void SetDeck(int[] deck)
        {
            if(arrDiceDeck == null)
                _arrDiceDeck = new DiceInfoData[5];
            
            for (int i = 0; i < deck.Length; i++)
            {
                int num = deck[i];
                
                arrDiceDeck[i] = InGameManager.Get().data_DiceInfo.GetData(num);
                
                if (PoolManager.Get() == null) Debug.Log("PoolManager Instnace is null");
                
                if ((Global.E_LOADTYPE)arrDiceDeck[i].loadType == Global.E_LOADTYPE.LOAD_MINION)
                {
                    PoolManager.instance.AddPool(FileHelper.LoadPrefab(arrDiceDeck[i].prefabName , Global.E_LOADTYPE.LOAD_MINION ), 50);  
                }
                else
                {
                    PoolManager.instance.AddPool(FileHelper.LoadPrefab(arrDiceDeck[i].prefabName , Global.E_LOADTYPE.LOAD_MAGIC ), 50);
                }

            }
        }

        public void GetDice(int diceId , int slotNum , int level = 0)
        {
            arrDice[slotNum].Set(GetArrayDeckDice(diceId));
            
            if (uiDiceField != null)
            {
                uiDiceField.arrSlot[slotNum].ani.SetTrigger("BBoing");
                uiDiceField.SetField(arrDice);
            }
            
            //
            uiDiceField.RefreshField();
        }

        public void OtherGetDice(int diceId, int slotNum)
        {
            arrDice[slotNum].Set(GetArrayDeckDice(diceId));
        }

        public DiceInfoData GetArrayDeckDice(int diceId)
        {
            DiceInfoData dice = null;
            for (int i = 0; i < arrDiceDeck.Length; i++)
            {
                if (arrDiceDeck[i].id == diceId)
                {
                    dice = arrDiceDeck[i];
                    break;
                }
            }
            return dice;
        }
        
        
        public void LevelUpDice(int resetFieldNum, int levelupFieldNum, int levelupDiceId, int level)
        {
            arrDice[resetFieldNum].Reset();
            
            DiceInfoData data = InGameManager.Get().data_DiceInfo.GetData(levelupDiceId);


            if (InGameManager.IsNetwork == true)
            {
                // 서버에서 오는 레벨이 1부터 시작하기때문에 (클라는 0 부터 시작)1을 빼줘야된다
                int serverLevel = level - 1;
                if (serverLevel < 0)
                    serverLevel = 0;
                arrDice[levelupFieldNum].Set(data, serverLevel);    
            }
            else
                arrDice[levelupFieldNum].Set(data, level);
            

            if (InGameManager.IsNetwork && isMine)
            {
                if (uiDiceField != null)
                {
                    uiDiceField.SetField(arrDice);
                    
                }
                
                uiDiceField.RefreshField();
            }
        }
        
        public void InGameDiceUpgrade(int diceId , int upgradeLv)
        {
            for (int i = 0; i < arrDiceDeck.Length; i++)
            {
                if (arrDiceDeck[i].id == diceId)
                {
                    arrUpgradeLevel[i] = upgradeLv;
                    break;
                }
            }
        }
        
        private int GetDiceUpgradeLevel(DiceInfoData data)
        {
            var num = 0;
            for (var i = 0; i < arrDiceDeck.Length; i++)
            {
                if (arrDiceDeck[i] != data) continue;
                num = i;
                break;
            }

            return arrUpgradeLevel[num];
        }
        
        #endregion
        
        
        
        #region net etc system
        
        public void ChangeLayer(bool pIsBottomPlayer)
        {
            gameObject.layer = LayerMask.NameToLayer(pIsBottomPlayer ? "BottomPlayer" : "TopPlayer");
            objCollider.layer = LayerMask.NameToLayer(pIsBottomPlayer ? "BottomPlayer" : "TopPlayer");
            this.isBottomPlayer = pIsBottomPlayer;

            if (InGameManager.IsNetwork == true && this.isBottomPlayer == false && NetworkManager.Get().playType == Global.PLAY_TYPE.BATTLE)
            {
                transform.rotation = Quaternion.Euler(0, 180f, 0);
            }
            
            if(InGameManager.IsNetwork)
            {
                //switch (PhotonManager.Instance.playType)
                switch (NetworkManager.Get().playType)
                {
                    case Global.PLAY_TYPE.BATTLE:
                        image_HealthBar.color = isMine ? Color.green : Color.red;
                        break;
                    case Global.PLAY_TYPE.COOP:
                        image_HealthBar.color = Color.green;
                        break;
                }
            }
            
            /*
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
            */
        }

        public void OtherPlayerPause(bool isPuase)
        {
            targetPlayer.SetPlayingAI(isPuase);
        }

        public void SetPlayingAI(bool isPlayingAI)
        {
            this.isPlayingAI = isPlayingAI;

            foreach (var minion in listMinion)
            {
                if (isPlayingAI) minion.behaviourTreeOwner.behaviour.Resume();
                else minion.behaviourTreeOwner.behaviour.Pause();
            }
            
            if (isPlayingAI) StartSyncMinion();
            else StopSyncMinion();
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

                int randomDeckNum = Random.Range(0, arrDiceDeck.Length);
                arrDice[emptySlotNum].Set(arrDiceDeck[randomDeckNum]);
                
                if (uiDiceField != null)
                {
                    uiDiceField.arrSlot[emptySlotNum].ani.SetTrigger("BBoing");
                    uiDiceField.SetField(arrDice);
                }

                //if (PhotonNetwork.IsConnected)
                    //SendPlayer(RpcTarget.Others , E_PTDefine.PT_GETDICE , randomDeckNum, emptySlotNum);
            }
        }
       
        // public void SetDeck(string deck)
        // {
        //     var splitDeck = deck.Split('/');
        //
        //     if(arrDiceDeck == null)
        //         _arrDiceDeck = new DiceInfoData[5];
        //
        //     for (var i = 0; i < splitDeck.Length; i++)
        //     {
        //         var num = int.Parse(splitDeck[i]);
        //         
        //         arrDiceDeck[i] = InGameManager.Get().data_DiceInfo.GetData(num);
        //         
        //         // add pool
        //         if (PoolManager.Get() == null) Debug.Log("PoolManager Instnace is null");
        //         
        //         if ((Global.E_LOADTYPE)arrDiceDeck[i].loadType == Global.E_LOADTYPE.LOAD_MINION)
        //         {
        //             PoolManager.instance.AddPool(FileHelper.LoadPrefab(arrDiceDeck[i].prefabName , Global.E_LOADTYPE.LOAD_MINION ), 50);  
        //         }
        //         else
        //         {
        //             PoolManager.instance.AddPool(FileHelper.LoadPrefab(arrDiceDeck[i].prefabName , Global.E_LOADTYPE.LOAD_MAGIC ), 50);
        //         }
        //         
        //     }
        // }
        
        public void GetDice(int deckNum, int slotNum)
        {
            arrDice[slotNum].Set(arrDiceDeck[deckNum]);
        }

        public int GetDiceFieldEmptySlotCount()
        {
            return arrDice.Count(dice => dice.id < 0);
        }

        public int DiceUpgrade(int deckNum)
        {
            return ++arrUpgradeLevel[deckNum];
        }

        #endregion
        
        
        
        
        
        
        #region etc system

        public void PushEnemyMinions(float pushPower)
        {
            var cols = Physics.OverlapSphere(transform.position, 4f, targetLayer);
            //Debug.Log("PushCount: " + cols.Length);
            foreach (var col in cols)
            {
                var bs = col.GetComponentInParent<BaseStat>();
                if (bs != null)
                {
                    float pp = Mathf.Lerp(pushPower, 0f, Vector3.Distance(col.transform.position, transform.position) / 5f);
                    
                    // nev
                    //targetPlayer.SendPlayer(RpcTarget.All, E_PTDefine.PT_PUSHMINION, bs.id, (col.transform.position - transform.position).normalized, pp);
                    ActionPushMinion(bs.id, (col.transform.position - transform.position).normalized, pp);
                }
            }
        }

        public void SummonGuardian()
        {
            ps_ShieldOff.Play();
            Vector3 pos = isBottomPlayer ? FieldManager.Get().GetBottomListPos(12) : FieldManager.Get().GetTopListPos(12);

            NavMeshHit navMeshHit;
            do
            {
                pos.x += Random.Range(-1f, 1f);
                pos.z += Random.Range(-1f, 1f);
            } while (NavMesh.SamplePosition(pos, out navMeshHit, 0.2f, NavMesh.AllAreas) == false);

            var m = CreateMinion(pref_Guardian, pos, 1, 0);
            m.targetMoveType = DICE_MOVE_TYPE.GROUND;
            m.ChangeLayer(isBottomPlayer);
            m.power = maxHealth / 50f;
            m.maxHealth = maxHealth * 0.3333f;
            m.attackSpeed = 0.8f;
            m.moveSpeed = 0.8f;
            m.range = 0.7f;
            m.eyeLevel = 1;
            m.upgradeLevel = 0;
            m.Initialize(MinionDestroyCallback);
            
            PoolManager.instance.ActivateObject("Effect_Robot_Summon", pos);
        }
        
        
        public override void HitDamage(float damage)
        {
            if (currentHealth > 0)
            {
                currentHealth -= damage;
                if (isHalfHealth == false && currentHealth <= 20000)
                {
                    isHalfHealth = true;
                    animator.SetBool(Break, true);

                    PushEnemyMinions(70f);

                    Invoke("SummonGuardian", 0.5f);
                }

                if (currentHealth <= 0)
                {
                    UI_InGamePopup.Get().ViewLowHP(false);
                    currentHealth = 0;
                    
                    Death();    // 
                }
                //else if (((PhotonNetwork.IsConnected && photonView.IsMine) || (!PhotonNetwork.IsConnected && isMine)) 
                         //&& currentHealth < maxHealth * 0.1f && !UI_InGamePopup.Get().GetLowHP())
                else if( ( (InGameManager.IsNetwork && isMine) || (InGameManager.IsNetwork == false && isMine) ) 
                         && currentHealth < maxHealth * 0.1f && !UI_InGamePopup.Get().GetLowHP())
                {
                    UI_InGamePopup.Get().ViewLowHP(true);
                }
            }
        }

        // 네트워크 변환하면서 필요없어지긴 했으나 
        // 싱글 모드 테스트 일때는 게임 끝내는걸 호출해준다
        // 네트워크 모드에선 게임의 끝은 서버가 판단해준다
        private void Death()
        {
            if (InGameManager.Get().isGamePlaying)
            {
                //SendPlayer(RpcTarget.All, E_PTDefine.PT_ACTIVATEPOOLOBJECT, "Effect_Bomb", transform.position, Quaternion.identity, Vector3.one);
                ActionActivePoolObject("Effect_Bomb", transform.position, Quaternion.identity, Vector3.one);
                animator.gameObject.SetActive(false);
                
                // 연결은 안되었으나 == 싱글모드 일때 && 내 타워라면
                if (InGameManager.IsNetwork == false)
                {
                    //InGameManager.Get().EndGame(!isMine);
                }
                
                /*if (PhotonNetwork.IsConnected)
                {                    
                    InGameManager.Get().SendBattleManager(RpcTarget.All , E_PTDefine.PT_ENDGAME );
                }
                else
                {
                    InGameManager.Get().EndGame(new PhotonMessageInfo());
                }*/
            }
        }

        #endregion
        
        

        #region net minion
        
        
        
        #region minion damage

        protected virtual IEnumerator HitDamageQueueCoroutine()
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
        
        public void HitDamageMinionAndMagic(int baseStatId, float damage )
        {
            // baseStatId == 0 => Player tower
            if (baseStatId == 0)
            {
                //if (PhotonNetwork.IsConnected)
                if( InGameManager.IsNetwork == true && (isMine || isPlayingAI))
                {
                    int convDamage = ConvertNetMsg.MsgFloatToInt( damage );
                    //NetSendPlayer(GameProtocol.HIT_DAMAGE_REQ , myUID, convDamage);
                    queueHitDamage.Enqueue(convDamage);
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
        
        /*public void AttackEnemyMinion(int baseStatId, float damage)
        {
            //if (PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom.PlayerCount > 1)
            HitMinionDamage(true, baseStatId, damage);
            /*targetPlayer.HitDamageMinionAndMagic(baseStatId, damage, delay);
            if(InGameManager.IsNetwork)
            {
                //targetPlayer.SendPlayer(RpcTarget.All , E_PTDefine.PT_HITMINIONANDMAGIC , baseStatId, damage, delay);
                NetSendPlayer(GameProtocol.HIT_DAMAGE_MINION_RELAY , NetworkManager.Get().OtherUID , baseStatId , damage, delay);
            }#1#
        }*/

        public void HitMyMinionDamage(int uid , int minionId , float damage )
        {
            if(minionId == 0)
                print("  id ---- 0 :  "+minionId);
                            
            //if (other == true)
            {
                if (InGameManager.IsNetwork && (isMine || isPlayingAI))
                {
                    NetSendPlayer(GameProtocol.HIT_DAMAGE_MINION_RELAY , uid , minionId , damage, 0);
                }
                HitDamageMinionAndMagic(minionId, damage);
            }
            // else
            // {
            //     if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            //     {
            //         NetSendPlayer(GameProtocol.HIT_DAMAGE_MINION_RELAY , myUID , minionId , damage , 0);
            //     }
            //     HitDamageMinionAndMagic(minionId, damage);
            // }    
        }
    
        public void AttackEnemyMinionOrMagic(int uid, int baseStatId, float damage, float delay)
        {
            //if(baseStatId == 0)
                //print("  id ---- 0 :  "+baseStatId);
            
            StartCoroutine(AttackEnemyMinionOrMagicCoroutine(uid, baseStatId, damage, delay));
        }
        
        IEnumerator AttackEnemyMinionOrMagicCoroutine(int uid, int baseStatId, float damage, float delay)
        {
            if (delay > 0) yield return new WaitForSeconds(delay);

            //if (PhotonNetwork.IsConnected && isMine && PhotonNetwork.CurrentRoom.PlayerCount > 1)
            if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            {
                //targetPlayer.SendPlayer(RpcTarget.All, E_PTDefine.PT_HITMINIONANDMAGIC, baseStatId, damage);
                NetSendPlayer(GameProtocol.HIT_DAMAGE_MINION_RELAY , uid , baseStatId , damage, delay);
            }

            if (targetPlayer.UID == uid)
            {
                targetPlayer.HitDamageMinionAndMagic(baseStatId, damage);
            }
            else if (coopPlayer != null && coopPlayer.UID == uid)
            {
                coopPlayer.HitDamageMinionAndMagic(baseStatId, damage);
            }
        }

        
        
        #endregion
        

        
        
        #region minion target death remove
        public void ActionSetMagicTarget(int baseStatId , params object[] param)
        {
            if (param.Length > 1)
            {
                if (InGameManager.IsNetwork && (isMine || isPlayingAI))
                {
                    //SendPlayer(RpcTarget.All , E_PTDefine.PT_HITDAMAGE , damage);
                    int chX = ConvertNetMsg.MsgFloatToInt((float) param[0]);
                    int chZ = ConvertNetMsg.MsgFloatToInt((float) param[1] );
                    NetSendPlayer(GameProtocol.SET_MAGIC_TARGET_POS_RELAY , myUID , baseStatId , chX , chZ );
                }
                SetMagicTarget(baseStatId, (float) param[0], (float) param[1]);
            }
            else // id 만 들어잇을테니..
            {
                if (InGameManager.IsNetwork && (isMine || isPlayingAI))
                {
                    NetSendPlayer(GameProtocol.SET_MAGIC_TARGET_ID_RELAY , myUID , baseStatId ,(int) param[0]);    
                }
                SetMagicTarget(baseStatId, (int) param[0]);
            }
        }

        public void MinionDestroyCallback(Minion minion)
        {
            // if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            // {
            //     NetSendPlayer(GameProtocol.REMOVE_MINION_RELAY , myUID , minion.id);
            // }
            
            RemoveMinion(minion.id);
            // not use
            /*
            if (PhotonNetwork.IsConnected)
            {
                //photonView.RPC("RemoveMinion", RpcTarget.All, minion.id);
                SendPlayer(RpcTarget.All , E_PTDefine.PT_REMOVEMINION , minion.id);
            }
            else
            {
                RemoveMinion(minion.id);
            }
            */
        }
        public void DeathMinion(int baseStatId)
        {
            if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            {
                NetSendPlayer(GameProtocol.DESTROY_MINION_RELAY , myUID , baseStatId);
            }
            
            DestroyMinion(baseStatId);
            
            
            /*if (PhotonNetwork.IsConnected && isMine)
            {
                //photonView.RPC("DestroyMinion", RpcTarget.All, baseStatId);
                SendPlayer(RpcTarget.All , E_PTDefine.PT_DESTROYMINION , baseStatId);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                DestroyMinion(baseStatId);
            }*/
        }
        
        public void MagicDestroyCallback(Magic magic)
        {
            // if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            // {
            //     NetSendPlayer(GameProtocol.REMOVE_MAGIC_RELAY , myUID , magic.id );
            // }
            RemoveMagic(magic.id);
            
            /*if (PhotonNetwork.IsConnected)
            {
                SendPlayer(RpcTarget.All , E_PTDefine.PT_REMOVEMAGIC , magic.id);
            }
            else
            {
                RemoveMagic(magic.id);
            }*/
        }
        
        public void DeathMagic(int baseStatId)
        {
            if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            {
                NetSendPlayer(GameProtocol.DESTROY_MAGIC_RELAY , myUID , baseStatId );
            }
            DestroyMagic(baseStatId);
            
            /*if (PhotonNetwork.IsConnected && isMine)
            {
                SendPlayer(RpcTarget.All , E_PTDefine.PT_DESTROYMAGIC , baseStatId);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                DestroyMagic(baseStatId);
            }*/
        }

        public void RemoveAllMinionAndMagic()
        {
            var arrMinion = listMinion.ToArray();
            for (int i = 0; i < arrMinion.Length; i++)
            {
                arrMinion[i].Death();
            }

            var arrMagic = _listMagic.ToArray();
            for (int i = 0; i < arrMagic.Length; i++)
            {
                arrMagic[i].Destroy();
            }
        }
        
        #endregion
        
        
        
        public void MinionAniTrigger(int baseStatId , string aniName , int targetId )
        {
            if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            {
                int aniEnum = (int)UnityUtil.StringToEnum<E_AniTrigger>(aniName);
                // 
                NetSendPlayer(GameProtocol.SET_MINION_ANIMATION_TRIGGER_RELAY , myUID , baseStatId , aniEnum , targetId);
            }
            SetMinionAnimationTrigger(baseStatId, aniName , targetId);
        }
        public void ActionSendMsg(int bastStatId, string msgFunc, int targetId = -1)
        {
            if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            {
                int funcEnum = (int) UnityUtil.StringToEnum<E_ActionSendMessage>(msgFunc);
                    
                if (targetId == -1)
                {
                    NetSendPlayer(GameProtocol.SEND_MESSAGE_VOID_RELAY, myUID, bastStatId ,funcEnum );
                }
                else
                {   
                    NetSendPlayer(GameProtocol.SEND_MESSAGE_PARAM1_RELAY, myUID, bastStatId ,funcEnum , targetId );
                }
            }
            MinionSendMessage(bastStatId, msgFunc, targetId);
        }
        public void ActionMinionTarget(int baseStatId, int targetId)
        {
            if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            {
                NetSendPlayer(GameProtocol.SET_MINION_TARGET_RELAY, myUID, baseStatId , targetId );
            }
            SetMinionTarget(baseStatId , targetId);
        }
        
        public void ActionActivePoolObject(string objName , Vector3 startPos , Quaternion rotate , Vector3 scale)
        {
            if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            {
                int enumObj = (int) UnityUtil.StringToEnum<E_PoolName>(objName);
                NetSendPlayer(GameProtocol.ACTIVATE_POOL_OBJECT_RELAY, /*myUID,*/ enumObj , startPos , rotate , scale);
            }
            ActivationPoolObject(objName , startPos , rotate , scale);
        }

        
        
        
        
        
        #region action skill
        public void HealerMinion(int baseStatId, float heal)
        {
            if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            {
                NetSendPlayer(GameProtocol.HEAL_MINION_RELAY , myUID , baseStatId , heal);
            }
            
            HealMinion(baseStatId, heal);
        }
        public void ActionFireBallBomb(int bastStatId)
        {
            if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            {
                NetSendPlayer(GameProtocol.FIRE_BALL_BOMB_RELAY , myUID , bastStatId );
            }
            FireballBomb(bastStatId);
        }

        public void ActionMineBomb(int baseStatId)
        {
            if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            {
                NetSendPlayer(GameProtocol.MINE_BOMB_RELAY , myUID , baseStatId );
            }
            MineBomb(baseStatId);
        }
        public void ActionSturn(bool other , int baseStatId, float duration)
        {
            int chDur = ConvertNetMsg.MsgFloatToInt(duration );
            if (other == true)
            {
                if (InGameManager.IsNetwork && (isMine || isPlayingAI))
                    NetSendPlayer(GameProtocol.STURN_MINION_RELAY, myUID, baseStatId, chDur);
                targetPlayer.SturnMinion(baseStatId , duration);
            }
            else
            {
                if (InGameManager.IsNetwork && (isMine || isPlayingAI))
                    NetSendPlayer(GameProtocol.STURN_MINION_RELAY, myUID, baseStatId, chDur);
                SturnMinion(baseStatId , duration);
            }
        }
        public void ActionRocketBomb(int baseStatId)
        {
            if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            {
                NetSendPlayer(GameProtocol.ROCKET_BOMB_RELAY, myUID, baseStatId);
            }
            RocketBomb(baseStatId);
        }

        public void ActionIceBallBomb(int baseStatId)
        {
            if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            {
                NetSendPlayer(GameProtocol.ICE_BOMB_RELAY, myUID, baseStatId);
            }
            IceballBomb(baseStatId);
        }
        public void ActionFireManFire(int baseStatId)
        {
            if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            {
                NetSendPlayer(GameProtocol.FIRE_MAN_FIRE_RELAY, myUID, baseStatId);
            }
            FiremanFire(baseStatId);
        }
        public void ActionCloacking(int bastStatId, bool isCloacking)
        {
            if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            {
                NetSendPlayer(GameProtocol.MINION_CLOACKING_RELAY, myUID, bastStatId , isCloacking);
            }
            Cloacking(bastStatId, isCloacking);
        }

        public void ActionFlagOfWar(int bastStatId, bool isIn, float factor)
        {
            if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            {
                int convFactor = ConvertNetMsg.MsgFloatToInt(factor);
                NetSendPlayer(GameProtocol.MINION_FLAG_OF_WAR_RELAY, myUID, bastStatId ,convFactor , isIn );
            }
            FlagOfWar(bastStatId , isIn , factor);
        }
        public void ActionMinionScareCrow(bool other , int targetId, float eyeLevel)
        {
            int chEyeLv = ConvertNetMsg.MsgFloatToInt(eyeLevel );
            if (other == true)
            {
                if (InGameManager.IsNetwork && (isMine || isPlayingAI))
                    NetSendPlayer(GameProtocol.SCARECROW_RELAY, myUID, targetId, chEyeLv);
                targetPlayer.ScareCrow(targetId , eyeLevel);
            }
            else
            {
                if (InGameManager.IsNetwork && (isMine || isPlayingAI))
                    NetSendPlayer(GameProtocol.SCARECROW_RELAY, myUID, targetId, chEyeLv);
                ScareCrow(targetId , eyeLevel);
            }
        }
        public void ActionLayzer(int baseStatId, int[] arrTarget)
        {
            if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            {   
                NetSendPlayer(GameProtocol.LAYZER_TARGET_RELAY, myUID, baseStatId , arrTarget );
            }
                
            LayzerMinion(baseStatId, arrTarget);
        }

        public void ActionInvincibility(int baseStatId, float time)
        {
            int convTime = ConvertNetMsg.MsgFloatToInt(time );
            if (InGameManager.IsNetwork && (isMine || isPlayingAI))
                NetSendPlayer(GameProtocol.MINION_INVINCIBILITY_RELAY, myUID, baseStatId , convTime );
            
            SetMinionInvincibility(baseStatId, time);
        }

        public void ActionPushMinion(int baseStatId, Vector3 dir, float pushPower)
        {
            // 상대방 미니언을 푸쉬한다..
            if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            {
                int x = ConvertNetMsg.MsgFloatToInt(dir.x );
                int y = ConvertNetMsg.MsgFloatToInt(dir.y );
                int z = ConvertNetMsg.MsgFloatToInt(dir.z );
                
                int convPush  = ConvertNetMsg.MsgFloatToInt(pushPower );
                
                NetSendPlayer(GameProtocol.PUSH_MINION_RELAY, myUID, baseStatId ,x, y, z, convPush);
            }
            targetPlayer.PushMinion(baseStatId , dir , pushPower);
        }
        #endregion
        
        #endregion
        
        
        #region list remove & destroy
        //
        private void RemoveMinion(int baseStatId)
        {
            listMinion.Remove(listMinion.Find(minion => minion.id == baseStatId));
        }
        private void RemoveMagic(int baseStatId)
        {
            _listMagic.Remove(_listMagic.Find(magic => magic.id == baseStatId));
        }
        private void DestroyMinion(int baseStatId)
        {
            listMinion.Find(minion => minion.id == baseStatId)?.Death();
        }
        private void DestroyMagic(int baseStatId)
        {
            _listMagic.Find(magic => magic.id == baseStatId)?.Destroy();
        }
        
        #endregion

        
        
        
        
        #region list skill to do
        public void HealMinion(int baseStatId, float heal)
        {
            listMinion.Find(minion => minion.id == baseStatId)?.Heal(heal);
        }
        public void FireballBomb(int baseStatId)
        {
            ((Fireball)_listMagic.Find(magic => magic.id == baseStatId))?.Bomb();
        }
        public void MineBomb(int baseStatId)
        {
            ((Mine)_listMagic.Find(magic => magic.id == baseStatId))?.Bomb();
        }
        public void SturnMinion(int baseStatId, float duration)
        {
            listMinion.Find(minion => minion.id == baseStatId)?.Sturn(duration);
        }
        public void RocketBomb(int baseStatId)
        {
            ((Rocket)_listMagic.Find(magic => magic.id == baseStatId))?.Bomb();
        }
        public void IceballBomb(int baseStatId)
        {
            ((Iceball)_listMagic.Find(magic => magic.id == baseStatId))?.Bomb();
        }
        public void FiremanFire(int baseStatId)
        {
            var m = listMinion.Find(minion => minion.id == baseStatId);
            if (m != null)
            {
                var fireman = m as Minion_Fireman;
                if (fireman != null)
                {
                    fireman.Fire();
                }
            }
        }
        public void Cloacking(int baseStatId , bool isCloack)
        {
            listMinion.Find(m => m.id == baseStatId)?.Cloacking(isCloack);
        }
        public void FlagOfWar(int bastStatId , bool isIn , float factor)
        {
            listMinion.Find(m => m.id == bastStatId )?.SetFlagOfWar(isIn, factor);
        }
        public void ScareCrow(int targetId , float eyeLevel )
        {
            listMinion.Find(m => m.id == targetId)?.Scarecrow(eyeLevel);
        }
        public void LayzerMinion(int bastStatId , int[] arrTarget)
        {
            var m_layzer = listMinion.Find(minion => minion.id == bastStatId);
            if (m_layzer != null)
            {
                var layzer = m_layzer as Minion_Layzer;
                if (layzer != null)
                {
                    layzer.SetTargetList(arrTarget);
                }
            }
        }
        
        

        #endregion





        #region list minion target magic

        public void SetMagicTarget(int baseStatId, int targetId)
        {
            StartCoroutine(SetMagicTargetCoroutine(baseStatId, targetId));
        }
        private IEnumerator SetMagicTargetCoroutine(int baseStatId, int targetId)
        {
            while (_listMagic.Find(magic => magic.id == baseStatId) == null) 
                yield return null;
            _listMagic.Find(magic => magic.id == baseStatId)?.SetTarget(targetId);
        }

        public void SetMagicTarget(int baseStatId, float x, float z)
        {
            StartCoroutine(SetMagicTargetCoroutine(baseStatId, x, z));
        }
        private IEnumerator SetMagicTargetCoroutine(int baseStatId, float x, float z)
        {
            while (_listMagic.Find(magic => magic.id == baseStatId) == null)
                yield return null;
            _listMagic.Find(magic => magic.id == baseStatId)?.SetTarget(x, z);
        }

        public void SetMinionTarget(int bastStatId , int targetId )
        {
            listMinion.Find(minion => minion.id == bastStatId).target = InGameManager.Get().GetBaseStatFromId(targetId);
        }

        public void SetMinionInvincibility(int bastStatId ,float time)
        {
            listMinion.Find(m => m.id == bastStatId)?.Invincibility(time);
        }

        
        #endregion
        
        
        
        
        
        
        #region minion etc list do it
        
        public void SetMinionAnimationTrigger(int baseStatId, string trigger , int targetId )
        {
            var m = listMinion.Find(minion => minion.id == baseStatId);
            if (m != null && m.animator != null)
            {
                //m.animator.SetTrigger(trigger);
                m.SetAnimationTrigger(trigger, targetId);
            }
        }
        
        public void MinionSendMessage(int bastStatId, string msgFunc , int targetId = -1 )
        {
            if (targetId == -1)
            {
                listMinion.Find(m => m.id == bastStatId)?.SendMessage(msgFunc, SendMessageOptions.DontRequireReceiver);
                _listMagic.Find(m => m.id == bastStatId)?.SendMessage(msgFunc, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                listMinion.Find(m => m.id == bastStatId)?.SendMessage(msgFunc, targetId, SendMessageOptions.DontRequireReceiver);
                _listMagic.Find(m => m.id == bastStatId)?.SendMessage(msgFunc, targetId, SendMessageOptions.DontRequireReceiver);
            }
        }
        public void ActivationPoolObject(string objectName , Vector3 startPos , Quaternion rotate , Vector3 scale )
        {
            Transform ts = PoolManager.instance.ActivateObject(objectName, startPos);
            if (ts != null)
            {
                ts.rotation = rotate;
                ts.localScale = scale;
            }
        }
        
        public void PushMinion(int baseStatId, Vector3 dir, float pushPower)
        {
            listMinion.Find(minion => minion.id == baseStatId)?.Push(dir, pushPower);
        }
        
        #endregion
        
        
        
        
        
        
        #region fire bullet

        public void ActionFireBullet(E_BulletType bulletType, int id, int targetId, float damage, float moveSpeed)
        {
            if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            {
                // int x = ConvertNetMsg.MsgFloatToInt(startPos.x );
                // int y = ConvertNetMsg.MsgFloatToInt(startPos.y );
                // int z = ConvertNetMsg.MsgFloatToInt(startPos.z );
                int chDamage = ConvertNetMsg.MsgFloatToInt(damage );
                int chSpeed = ConvertNetMsg.MsgFloatToInt(moveSpeed );
                
                //NetSendPlayer(GameProtocol.FIRE_ARROW_RELAY , NetworkManager.Get().UserUID , targetId , x, y, z ,chDamage , chSpeed);
                NetSendPlayer(GameProtocol.FIRE_BULLET_RELAY , myUID, id, targetId , chDamage ,chSpeed , (int)bulletType);
            }
            FireBullet(bulletType , id, targetId, damage, moveSpeed);
        }
        
        public void FireBullet(E_BulletType bulletType, int id, int targetId, float damage, float moveSpeed)
        {
            Bullet b = null;
            Vector3 startPos = listMinion.Find(m => m.id == id).ts_ShootingPos.position;
            
            switch (bulletType)
            {
                case E_BulletType.ARROW:
                    b = PoolManager.instance.ActivateObject<Bullet>("Arrow", startPos);
                    break;
                case E_BulletType.SPEAR:
                    b = PoolManager.instance.ActivateObject<Bullet>("Spear", startPos);
                    break;
                case E_BulletType.NECROMANCER:
                    b = PoolManager.instance.ActivateObject<Bullet>("Necromancer_Bullet", startPos);
                    break;
                case E_BulletType.MAGICIAN:
                    b = PoolManager.instance.ActivateObject<Bullet>("Magician_Bullet", startPos);
                    break;
                case E_BulletType.ARBITER:
                    b = PoolManager.instance.ActivateObject<Bullet>("Arbiter_Bullet", startPos);
                    break;
                case E_BulletType.BABYDRAGON:
                    b = PoolManager.instance.ActivateObject<Bullet>("Babydragon_Bullet", startPos);
                    break;
            }
            
            if (b != null)
            {
                b.transform.rotation = Quaternion.identity;
                b.controller = this;
                b.moveSpeed = moveSpeed;
                b.Initialize(targetId, damage, 0, isMine, isBottomPlayer);
            }
        }
        
        /*public void ActionFireArrow(Vector3 startPos , int targetId , float damage , float moveSpeed)
        {
            if (InGameManager.IsNetwork && isMine)
            {
                int x = (int) (startPos.x * Global.g_networkBaseValue);
                int y = (int) (startPos.y * Global.g_networkBaseValue);
                int z = (int) (startPos.z * Global.g_networkBaseValue);
                int chDamage = (int) (damage * Global.g_networkBaseValue);
                int chSpeed = (int) (moveSpeed * Global.g_networkBaseValue);
                
                NetSendPlayer(GameProtocol.FIRE_ARROW_RELAY , NetworkManager.Get().UserUID , targetId , x, y, z ,chDamage , chSpeed);
            }
            FireArrow(startPos, targetId, damage, moveSpeed);
        }
        public void ActionNecroBullet(Vector3 shootPos , int targetId , float damage , float moveSpeed)
        {
            if (InGameManager.IsNetwork && isMine)
            {
                int convDamage = (int) (damage * Global.g_networkBaseValue);
                int convSpeed = (int) (moveSpeed * Global.g_networkBaseValue);
                NetSendPlayer(GameProtocol.NECROMANCER_BULLET_RELAY, NetworkManager.Get().UserUID, shootPos , targetId ,convDamage , convSpeed );
            }
            FireNecromancerBullet(shootPos, targetId, damage, moveSpeed);
        }
        public void ActionFireSpear(Vector3 startPos, int targetId, float damage, float moveSpeed)
        {            
            if (InGameManager.IsNetwork && isMine)
            {
                int chDamage = (int)(damage *  Global.g_networkBaseValue);
                int chSpeed = (int)(moveSpeed *  Global.g_networkBaseValue);
                
                NetSendPlayer(GameProtocol.FIRE_SPEAR_RELAY, NetworkManager.Get().UserUID, startPos , targetId , chDamage , chSpeed);
            }
            FireSpear(startPos, targetId, damage, moveSpeed);

        }*/
        
        #endregion
        
        
        
        
        
        #region fire cannon
        public void ActionFireCannonBall(E_CannonType type, Vector3 shootPos , Vector3 targetPos , float damage , float range )
        {
            if (InGameManager.IsNetwork && (isMine || isPlayingAI))
            {
                int chDamage = ConvertNetMsg.MsgFloatToInt(damage);
                int chRange = ConvertNetMsg.MsgFloatToInt(range);
                MsgVector3 msgShootPos = ConvertNetMsg.Vector3ToMsg(shootPos);
                MsgVector3 msgTargetPos = ConvertNetMsg.Vector3ToMsg(targetPos);
                
                NetSendPlayer(GameProtocol.FIRE_CANNON_BALL_RELAY, myUID, msgShootPos , msgTargetPos , chDamage , chRange , (int)type);
            }
            FireCannonBall(type ,shootPos, targetPos, damage, range);
        }
        
        public void FireCannonBall(E_CannonType type, Vector3 startPos, Vector3 targetPos, float damage, float splashRange)
        {
            CannonBall b = null;
            switch (type)
            {
                case E_CannonType.DEFAULT:
                    b = PoolManager.instance.ActivateObject<CannonBall>("CannonBall", startPos);
                    break;
                case E_CannonType.BOMBER:
                    b = PoolManager.instance.ActivateObject<CannonBall>("Bomber_Bullet", startPos);
                    break;
            }

            if (b != null)
            {
                b.transform.rotation = Quaternion.identity;
                b.controller = this;
                b.Initialize(targetPos, damage, splashRange, isMine, isBottomPlayer);
            }
        }
        
        /*public void FireCannonBall(Vector3 startPos, Vector3 targetPos, float damage, float splashRange)
        {
            var b = PoolManager.instance.ActivateObject<CannonBall>("CannonBall", startPos);
            if (b != null)
            {
                b.transform.rotation = Quaternion.identity;
                b.controller = this;
                b.Initialize(targetPos, damage, splashRange, isMine, isBottomPlayer);
            }
        }*/
        
        #endregion
        
        
        
        #region skill & effect
        
        public void FireArrow(Vector3 startPos, int targetId, float damage, float moveSpeed)
        {
            var b = PoolManager.instance.ActivateObject<Bullet>("Bullet", startPos);
            if (b != null)
            {
                b.transform.rotation = Quaternion.identity;
                b.controller = this;
                b.moveSpeed = moveSpeed;
                b.Initialize(targetId, damage, 0, isMine, isBottomPlayer);
            }
        }
        public void FireSpear(Vector3 startPos, int targetId, float damage, float moveSpeed)
        {
            var b = PoolManager.instance.ActivateObject<Bullet>("Spear", startPos);
            if (b != null)
            {
                b.transform.rotation = Quaternion.identity;
                b.controller = this;
                b.moveSpeed = moveSpeed;
                b.Initialize(targetId, damage, 0, isMine, isBottomPlayer);
            }
        }
        
        public void FireNecromancerBullet(Vector3 startPos, int targetId, float damage, float moveSpeed)
        {
            var b = PoolManager.instance.ActivateObject<Bullet>("Necromancer_Bullet", startPos);
            if (b != null)
            {
                b.transform.rotation = Quaternion.identity;
                b.controller = this;
                b.moveSpeed = moveSpeed;
                b.Initialize(targetId, damage, 0, isMine, isBottomPlayer);
            }
        }

        #endregion
        
        
        #region sync dice field

        public void SetDiceField(MsgGameDice[] arrDiceData)
        {
            if (arrDice != null)
            {
                for(int i = 0 ; i < arrDice.Length ; i++)
                    arrDice[i].Reset();
            }
            else
            {
                arrDice = new Dice[arrDiceData.Length];    
            }
            
            for (int i = 0; i < arrDiceData.Length; i++)
            {
                int servLevel = arrDiceData[i].Level - 1;
                if (servLevel < 0)
                    servLevel = 0;
                
                arrDice[arrDiceData[i].SlotNum] = new Dice
                {
                    diceData = InGameManager.Get().data_DiceInfo.GetData(arrDiceData[i].DiceId),
                    eyeLevel = servLevel,
                    diceFieldNum = arrDiceData[i].SlotNum
                };
                
                if (uiDiceField != null)
                {
                    uiDiceField.arrSlot[arrDiceData[i].SlotNum].ani.SetTrigger("BBoing");
                    uiDiceField.SetField(arrDice);
                }
            }
            
            //
            if(isMine)
                uiDiceField.RefreshField();    
        }
        
        #endregion
        
        
        
        #region sync minion --

        public void StartSyncMinion()
        {
            StopSyncMinion();
            crt_SyncMinion = StartCoroutine(SyncMinionStatus());
        }

        public void StopSyncMinion()
        {
            if (crt_SyncMinion != null)
            {
                StopCoroutine(crt_SyncMinion);
            }
        }
        
        protected Dictionary<GameProtocol, List<object>> _syncDictionary = new Dictionary<GameProtocol, List<object>>();

        protected int packetCount;
        public IEnumerator SyncMinionStatus()
        {
            if (InGameManager.IsNetwork == false)
                yield break;

            //while (InGameManager.Get().isGamePlaying)
            while (true)
            {
                yield return new WaitForSeconds(0.2f);

                if (InGameManager.Get().isGamePlaying && InGameManager.Get().wave > 0 && InGameManager.IsNetwork && (isMine || isPlayingAI))
                {
                    //if (listMinion.Count > 0 || _syncDictionary.Keys.Count > 0)
                    {
                        byte minionCount = (byte) listMinion.Count;
                        MsgVector2[] msgMinPos = new MsgVector2[listMinion.Count];
                        int[] hp = new int[listMinion.Count];
                        MsgMinionStatus relay = new MsgMinionStatus();

                        for (int i = 0; i < listMinion.Count; i++)
                        {
                            msgMinPos[i] = ConvertNetMsg.Vector3ToMsg(new Vector2(listMinion[i].rb.position.x, listMinion[i].rb.position.z));
                            hp[i] = ConvertNetMsg.MsgFloatToInt(listMinion[i].currentHealth);
                        }

                        #if ENABLE_LOG
                        string str = "MINION_STATUS_RELAY -> Dictionary count : " + _syncDictionary.Keys.Count;
                        #endif
                        if (_syncDictionary.Keys.Count > 0)
                        {
                            foreach (var sync in _syncDictionary)
                            {
                                switch (sync.Key)
                                {
                                    case GameProtocol.HIT_DAMAGE_MINION_RELAY:
                                        relay.arrHitDamageMinionRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgHitDamageMinionRelay)element);
                                        break;
                                    case GameProtocol.DESTROY_MINION_RELAY:
                                        relay.arrDestroyMinionRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgDestroyMinionRelay)element);
                                        break;
                                    case GameProtocol.HEAL_MINION_RELAY:
                                        relay.arrHealMinionRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgHealMinionRelay)element);
                                        break;
                                    case GameProtocol.PUSH_MINION_RELAY:
                                        relay.arrPushMinionRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgPushMinionRelay)element);
                                        break;
                                    case GameProtocol.SET_MINION_ANIMATION_TRIGGER_RELAY:
                                        relay.arrMinionAnimationTriggerRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgSetMinionAnimationTriggerRelay)element);
                                        break;
                                    case GameProtocol.FIRE_BALL_BOMB_RELAY:
                                        relay.arrFireballBombRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgFireballBombRelay)element);
                                        break;
                                    case GameProtocol.MINE_BOMB_RELAY:
                                        relay.arrMineBombRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgMineBombRelay)element);
                                        break;
                                    case GameProtocol.DESTROY_MAGIC_RELAY:
                                        relay.arrDestroyMagicRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgDestroyMagicRelay)element);
                                        break;
                                    case GameProtocol.SET_MAGIC_TARGET_ID_RELAY:
                                        relay.arrMagicTargetIdRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgSetMagicTargetIdRelay)element);
                                        break;
                                    case GameProtocol.SET_MAGIC_TARGET_POS_RELAY:
                                        relay.arrMagicTargetRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgSetMagicTargetRelay)element);
                                        break;
                                    case GameProtocol.STURN_MINION_RELAY:
                                        relay.arrSturnMinionRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgSturnMinionRelay)element);
                                        break;
                                    case GameProtocol.ROCKET_BOMB_RELAY:
                                        relay.arrRocketBombRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgRocketBombRelay)element);
                                        break;
                                    case GameProtocol.ICE_BOMB_RELAY:
                                        relay.arrIceBombRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgIceBombRelay)element);
                                        break;
                                    case GameProtocol.FIRE_CANNON_BALL_RELAY:
                                        relay.arrFireCannonBallRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgFireCannonBallRelay)element);
                                        break;
                                    case GameProtocol.FIRE_MAN_FIRE_RELAY:
                                        relay.arrFireManFireRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgFireManFireRelay)element);
                                        break;
                                    case GameProtocol.ACTIVATE_POOL_OBJECT_RELAY:
                                        relay.arrActivatePoolObjectRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgActivatePoolObjectRelay)element);
                                        break;
                                    case GameProtocol.MINION_CLOACKING_RELAY:
                                        relay.arrMinionCloackingRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgMinionCloackingRelay)element);
                                        break;
                                    case GameProtocol.MINION_FLAG_OF_WAR_RELAY:
                                        relay.arrMinionFlagOfWarRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgMinionFlagOfWarRelay)element);
                                        break;
                                    case GameProtocol.SEND_MESSAGE_VOID_RELAY:
                                        relay.arrSendMessageVoidRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgSendMessageVoidRelay)element);
                                        break;
                                    case GameProtocol.SEND_MESSAGE_PARAM1_RELAY:
                                        relay.arrSendMessageParam1Relay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgSendMessageParam1Relay)element);
                                        break;
                                    case GameProtocol.SET_MINION_TARGET_RELAY:
                                        relay.arrMinionTargetRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgSetMinionTargetRelay)element);
                                        break;
                                    case GameProtocol.SCARECROW_RELAY:
                                        relay.arrScarercrowRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgScarecrowRelay)element);
                                        break;
                                    case GameProtocol.LAYZER_TARGET_RELAY:
                                        relay.arrLayzerTargetRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgLayzerTargetRelay)element);
                                        break;
                                    case GameProtocol.FIRE_BULLET_RELAY:
                                        relay.arrFireBulletRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgFireBulletRelay)element);
                                        break;
                                    case GameProtocol.MINION_INVINCIBILITY_RELAY:
                                        relay.arrMinionInvincibilityRelay = Array.ConvertAll(sync.Value.ToArray(), element => (MsgMinionInvincibilityRelay)element);
                                        break;
                                }

                                #if ENABLE_LOG
                                // Log
                                str += string.Format("\n{0} -> List count : {1}", sync.Key, sync.Value.Count);
                                switch (sync.Key)
                                {
                                    case GameProtocol.HIT_DAMAGE_MINION_RELAY:
                                        foreach (var value in sync.Value)
                                        {
                                            MsgHitDamageMinionRelay msg = (MsgHitDamageMinionRelay) value;
                                            str += string.Format("\n      UID: {0},  ID:{1}, DMG:{2}", msg.PlayerUId,
                                                msg.Id, msg.Damage);
                                        }
                                        break;
                                    case GameProtocol.HEAL_MINION_RELAY:
                                        foreach (var value in sync.Value)
                                        {
                                            MsgHealMinionRelay msg = (MsgHealMinionRelay) value;
                                            str += string.Format("\n      UID: {0},  ID:{1}, HEAL:{2}", msg.PlayerUId,
                                                msg.Id, msg.Heal);
                                        }
                                        break;
                                    case GameProtocol.DESTROY_MINION_RELAY:
                                        foreach (var value in sync.Value)
                                        {
                                            MsgDestroyMinionRelay msg = (MsgDestroyMinionRelay) value;
                                            str += string.Format("\n      UID: {0},  ID:{1}", msg.PlayerUId,
                                                msg.Id);
                                        }
                                        break;
                                    case GameProtocol.DESTROY_MAGIC_RELAY:
                                        foreach (var value in sync.Value)
                                        {
                                            MsgDestroyMagicRelay msg = (MsgDestroyMagicRelay) value;
                                            str += string.Format("\n      UID: {0},  ID:{1}", msg.PlayerUId,
                                                msg.BaseStatId);
                                        }
                                        break;
                                    case GameProtocol.ACTIVATE_POOL_OBJECT_RELAY:
                                        foreach (var value in sync.Value)
                                        {
                                            MsgActivatePoolObjectRelay msg = (MsgActivatePoolObjectRelay) value;
                                            str += string.Format("\n      POOL: {0}", ((E_PoolName)msg.PoolName).ToString());
                                        }
                                        break;
                                }
                                #endif
                            }
                        }
                        
                        #if ENABLE_LOG
                        UnityUtil.Print(string.Format("SEND [{0}][{1}] : ", myUID, InGameManager.Get().wave * 10000 + (isPlayingAI ? targetPlayer.packetCount : packetCount)), str, "red");
                        #endif
                        NetSendPlayer(GameProtocol.MINION_STATUS_RELAY, myUID, minionCount , msgMinPos, hp, relay, InGameManager.Get().wave * 10000 + (isPlayingAI ? targetPlayer.packetCount : packetCount));
                        _syncDictionary.Clear();
                        packetCount++;
                    }
                }
            }
        }

        public void SyncMinion(byte minionCount , MsgVector2[] msgPoss, int[] minionHP, MsgMinionStatus relay, int packetCount)
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
                        targetPlayer.NetRecvPlayer(msg.Key, obj);
                    }
                }
            }
            
            #if ENABLE_LOG
            UnityUtil.Print(string.Format("RECV [{0}][{1}] : ", myUID, packetCount), str, "green");
            #endif
        }

        Dictionary<GameProtocol, List<object>> MsgMinionStatusToDictionary(MsgMinionStatus msg)
        {
            Dictionary<GameProtocol, List<object>> dic = new Dictionary<GameProtocol, List<object>>();

            if (msg.arrHitDamageMinionRelay != null) dic.Add(GameProtocol.HIT_DAMAGE_MINION_RELAY, new List<object>(msg.arrHitDamageMinionRelay));
            if (msg.arrFireballBombRelay != null) dic.Add(GameProtocol.FIRE_BALL_BOMB_RELAY, new List<object>(msg.arrFireballBombRelay));
            if (msg.arrHealMinionRelay != null) dic.Add(GameProtocol.HEAL_MINION_RELAY, new List<object>(msg.arrHealMinionRelay));
            if (msg.arrMineBombRelay != null) dic.Add(GameProtocol.MINE_BOMB_RELAY, new List<object>(msg.arrMineBombRelay));
            if (msg.arrSturnMinionRelay != null) dic.Add(GameProtocol.STURN_MINION_RELAY, new List<object>(msg.arrSturnMinionRelay));
            if (msg.arrRocketBombRelay != null) dic.Add(GameProtocol.ROCKET_BOMB_RELAY, new List<object>(msg.arrRocketBombRelay));
            if (msg.arrIceBombRelay != null) dic.Add(GameProtocol.ICE_BOMB_RELAY, new List<object>(msg.arrIceBombRelay));
            if (msg.arrFireManFireRelay != null) dic.Add(GameProtocol.FIRE_MAN_FIRE_RELAY, new List<object>(msg.arrFireManFireRelay));
            if (msg.arrMinionCloackingRelay != null) dic.Add(GameProtocol.MINION_CLOACKING_RELAY, new List<object>(msg.arrMinionCloackingRelay));
            if (msg.arrMinionFlagOfWarRelay != null) dic.Add(GameProtocol.MINION_FLAG_OF_WAR_RELAY, new List<object>(msg.arrMinionFlagOfWarRelay));
            if (msg.arrScarercrowRelay != null) dic.Add(GameProtocol.SCARECROW_RELAY, new List<object>(msg.arrScarercrowRelay));
            if (msg.arrLayzerTargetRelay != null) dic.Add(GameProtocol.LAYZER_TARGET_RELAY, new List<object>(msg.arrLayzerTargetRelay));
            if (msg.arrMinionInvincibilityRelay != null) dic.Add(GameProtocol.MINION_INVINCIBILITY_RELAY, new List<object>(msg.arrMinionInvincibilityRelay));
            if (msg.arrFireBulletRelay != null) dic.Add(GameProtocol.FIRE_BULLET_RELAY, new List<object>(msg.arrFireBulletRelay));
            if (msg.arrFireCannonBallRelay != null) dic.Add(GameProtocol.FIRE_CANNON_BALL_RELAY, new List<object>(msg.arrFireCannonBallRelay));
            if (msg.arrMinionAnimationTriggerRelay != null) dic.Add(GameProtocol.SET_MINION_ANIMATION_TRIGGER_RELAY, new List<object>(msg.arrMinionAnimationTriggerRelay));
            if (msg.arrMagicTargetIdRelay != null) dic.Add(GameProtocol.SET_MAGIC_TARGET_ID_RELAY, new List<object>(msg.arrMagicTargetIdRelay));
            if (msg.arrMagicTargetRelay != null) dic.Add(GameProtocol.SET_MAGIC_TARGET_POS_RELAY, new List<object>(msg.arrMagicTargetRelay));
            if (msg.arrActivatePoolObjectRelay != null) dic.Add(GameProtocol.ACTIVATE_POOL_OBJECT_RELAY, new List<object>(msg.arrActivatePoolObjectRelay));
            if (msg.arrSendMessageVoidRelay != null) dic.Add(GameProtocol.SEND_MESSAGE_VOID_RELAY, new List<object>(msg.arrSendMessageVoidRelay));
            if (msg.arrSendMessageParam1Relay != null) dic.Add(GameProtocol.SEND_MESSAGE_PARAM1_RELAY, new List<object>(msg.arrSendMessageParam1Relay));
            if (msg.arrMinionTargetRelay != null) dic.Add(GameProtocol.SET_MINION_TARGET_RELAY, new List<object>(msg.arrMinionTargetRelay));
            if (msg.arrPushMinionRelay != null) dic.Add(GameProtocol.PUSH_MINION_RELAY, new List<object>(msg.arrPushMinionRelay));
            if (msg.arrDestroyMinionRelay != null) dic.Add(GameProtocol.DESTROY_MINION_RELAY, new List<object>(msg.arrDestroyMinionRelay));
            if (msg.arrDestroyMagicRelay != null) dic.Add(GameProtocol.DESTROY_MAGIC_RELAY, new List<object>(msg.arrDestroyMagicRelay));

            return dic;
        }

        public void SyncMinionResume()
        {
            isMinionAgentMove = false;

            StartCoroutine(ResumeCoroutine(1f));
        }

        IEnumerator ResumeCoroutine(float time)
        {
            yield return new WaitForSecondsRealtime(time);
            
            isMinionAgentMove = true;
        }
        
        #endregion


        #region net packet player

        public void NetSendPlayer(GameProtocol protocol, params object[] param)
        {
            if (InGameManager.Get().isGamePlaying == false)
                return;
            
            if (NetworkManager.Get().isReconnect == true)
                return;

            if (protocol > GameProtocol.BEGIN_RELAY)
            {
                if (protocol == GameProtocol.MINION_STATUS_RELAY)
                {
                    if (InGameManager.IsNetwork == true)
                    {
                        NetworkManager.Get().Send(protocol, param);
                    }
                }
                else
                {
                    if (_syncDictionary.ContainsKey(protocol) == false)
                    {
                        _syncDictionary.Add(protocol, new List<object>());
                    }

                    switch (protocol)
                    {
                        case GameProtocol.HIT_DAMAGE_MINION_RELAY:
                        {
                            var msg = _syncDictionary[protocol].Find(m =>
                                (((MsgHitDamageMinionRelay) m).PlayerUId == (int) param[0] &&
                                 ((MsgHitDamageMinionRelay) m).Id == (int) param[1]));

                            if (msg != null)
                            {
                                ((MsgHitDamageMinionRelay) msg).Damage += ConvertNetMsg.MsgFloatToInt((float)param[2]);
                            }
                            else
                            {
                                _syncDictionary[protocol]
                                    .Add(ConvertNetMsg.GetHitDamageMinionRelayMsg((int) param[0], (int) param[1],
                                        (float) param[2]));
                            }
                        }
                            break;
                        case GameProtocol.DESTROY_MINION_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetDestroyMinionRelayMsg((int)param[0], (int)param[1]));
                            break;
                        case GameProtocol.DESTROY_MAGIC_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetDestroyMagicRelayMsg((int)param[0], (int)param[1]));
                            break;
                        case GameProtocol.FIRE_BALL_BOMB_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetFireballBombRelayMsg((int)param[0], (int)param[1]));
                            break;
                        case GameProtocol.HEAL_MINION_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetHealMinionRelayMsg((int)param[0], (int)param[1], (float)param[2]));
                            break;
                        case GameProtocol.MINE_BOMB_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetMineBombRelayMsg((int)param[0], (int)param[1]));
                            break;
                        case GameProtocol.STURN_MINION_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetSturnMinionRelayMsg((int)param[0], (int)param[1], (int)param[2]));
                            break;
                        case GameProtocol.ROCKET_BOMB_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetRocketBombRelayMsg((int)param[0], (int)param[1]));
                            break;
                        case GameProtocol.ICE_BOMB_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetIceBombRelayMsg((int)param[0], (int)param[1]));
                            break;
                        case GameProtocol.FIRE_MAN_FIRE_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetFireManFireRelayMsg((int)param[0], (int)param[1]));
                            break;
                        case GameProtocol.MINION_CLOACKING_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetMinionCloackingRelayMsg((int)param[0], (int)param[1], (bool)param[2]));
                            break;
                        case GameProtocol.MINION_FLAG_OF_WAR_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetMinionFlagOfWarRelayMsg((int)param[0], (int)param[1], (int)param[2], (bool)param[3]));
                            break;
                        case GameProtocol.SCARECROW_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetScarecrowRelayMsg((int)param[0], (int)param[1], (int)param[2]));
                            break;
                        case GameProtocol.LAYZER_TARGET_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetLayzerTargetRelayMsg((int)param[0], (int)param[1], (int[])param[2]));
                            break;
                        case GameProtocol.MINION_INVINCIBILITY_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetMinionInvincibilityRelayMsg((int)param[0], (int)param[1], (int)param[2]));
                            break;
                        case GameProtocol.FIRE_BULLET_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetFireBulletRelayMsg((int)param[0], (int)param[1], (int)param[2], (int)param[3], (int)param[4], (int)param[5]));
                            break;
                        case GameProtocol.FIRE_CANNON_BALL_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetFireCannonBallRelayMsg((int)param[0], (MsgVector3)param[1], (MsgVector3)param[2], (int)param[3], (int)param[4], (int)param[5]));
                            break;
                        case GameProtocol.SET_MINION_ANIMATION_TRIGGER_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetMinionAnimationTriggerRelayMsg((int)param[0],(int)param[1], (int)param[2], (int)param[3]));
                            break;
                        case GameProtocol.SET_MAGIC_TARGET_ID_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetMagicTargetIDRelayMsg((int)param[0],(int)param[1], (int)param[2]));
                            break;
                        case GameProtocol.SET_MAGIC_TARGET_POS_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetMagicTargetPosRelayMsg((int)param[0],(int)param[1], (int)param[2], (int)param[3]));
                            break;
                        case GameProtocol.ACTIVATE_POOL_OBJECT_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetActivatePoolObjectRelayMsg((int)param[0], (Vector3)param[1], (Quaternion)param[2], (Vector3)param[3]));
                            break;
                        case GameProtocol.SEND_MESSAGE_VOID_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetSendMessageVoidRelayMsg((int)param[0],(int)param[1], (int)param[2]));
                            break;
                        case GameProtocol.SEND_MESSAGE_PARAM1_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetSendMessageParam1RelayMsg((int)param[0],(int)param[1], (int)param[2], (int)param[3]));
                            break;
                        case GameProtocol.SET_MINION_TARGET_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetMinionTargetRelayMsg((int)param[0],(int)param[1], (int)param[2]));
                            break;
                        case GameProtocol.PUSH_MINION_RELAY:
                            _syncDictionary[protocol].Add(ConvertNetMsg.GetPushMinionRelayMsg((int)param[0], (int)param[1], (int)param[2], (int)param[3], (int)param[4], (int)param[5]));
                            break;
                    }
                }
            }
            else
            {
                if (InGameManager.IsNetwork == true)
                {
                    NetworkManager.Get().Send(protocol, param);
                }
            }
        }
        
        public void NetRecvPlayer(GameProtocol protocol, params object[] param)
        {
            //Debug.Log("RECV : " + protocol.ToString());
            
            if (NetworkManager.Get().isReconnect == true)
                return;
            
            
            switch (protocol)
            {
                case GameProtocol.HIT_DAMAGE_ACK:
                {
                    // 기본적으로 타워가 맞은것을 상대방이 맞앗다고 보내는거니까...
                    MsgHitDamageAck damageack = (MsgHitDamageAck) param[0];

                    float calDamage = ConvertNetMsg.MsgIntToFloat(damageack.Damage );
                    float calCurrentHP = ConvertNetMsg.MsgIntToFloat(damageack.CurrentHp);
                    //targetPlayer.HitDamage(calDamage);
                    if (NetworkManager.Get().UserUID == damageack.PlayerUId)
                    {
                        currentHealth = calCurrentHP;
                        RefreshHealthBar();
                        HitDamage(0);
                    }
                    else if (NetworkManager.Get().OtherUID == damageack.PlayerUId )
                    {
                        targetPlayer.currentHealth = calCurrentHP;
                        targetPlayer.RefreshHealthBar();
                        targetPlayer.HitDamage(0);
                    }
                    else if (NetworkManager.Get().CoopUID == damageack.PlayerUId )
                    {
                        coopPlayer.currentHealth = calCurrentHP;
                        coopPlayer.RefreshHealthBar();
                        coopPlayer.HitDamage(0);
                    }
                    
                    break;
                }
                case GameProtocol.HIT_DAMAGE_NOTIFY:
                {
                    MsgHitDamageNotify damagenoti = (MsgHitDamageNotify) param[0];

                    float calDamage = ConvertNetMsg.MsgIntToFloat(damagenoti.Damage );
                    float calCurrentHP = ConvertNetMsg.MsgIntToFloat(damagenoti.CurrentHp);
                    
                    if (NetworkManager.Get().UserUID == damagenoti.PlayerUId)
                    {
                        currentHealth = calCurrentHP;
                        RefreshHealthBar();
                        HitDamage(0);
                    }
                    else if (NetworkManager.Get().OtherUID == damagenoti.PlayerUId )
                    {
                        targetPlayer.currentHealth = calCurrentHP;
                        targetPlayer.RefreshHealthBar();
                        targetPlayer.HitDamage(0);
                    }
                    else if (NetworkManager.Get().CoopUID == damagenoti.PlayerUId )
                    {
                        coopPlayer.currentHealth = calCurrentHP;
                        coopPlayer.RefreshHealthBar();
                        coopPlayer.HitDamage(0);
                    }
                    
                    break;
                }
                case GameProtocol.HIT_DAMAGE_MINION_RELAY:
                {
                    MsgHitDamageMinionRelay hitminion = (MsgHitDamageMinionRelay) param[0];
                    //Debug.LogFormat("HIT_DAMAGE_MINION_RELAY = UID:{0}, ID:{1}, DMG:{2}", hitminion.PlayerUId, hitminion.Id, hitminion.Damage);
                    float damage = ConvertNetMsg.MsgIntToFloat(hitminion.Damage );
                    //float delay = hitminion.Delay / Global.g_networkBaseValue;
                    
                    if (NetworkManager.Get().UserUID == hitminion.PlayerUId)
                        targetPlayer.HitDamageMinionAndMagic(hitminion.Id, damage);
                    else if (NetworkManager.Get().OtherUID == hitminion.PlayerUId )
                        HitDamageMinionAndMagic(hitminion.Id, damage);
                    else if (NetworkManager.Get().CoopUID == hitminion.PlayerUId )
                        coopPlayer.HitDamageMinionAndMagic(hitminion.Id, damage);
                    
                    break;
                }
                
                case GameProtocol.DESTROY_MINION_RELAY:
                {
                    MsgDestroyMinionRelay destrelay = (MsgDestroyMinionRelay) param[0];
                    
                    //UnityEngine.Debug.Log(NetworkManager.Get().UserUID  + "   destroy id " + destrelay.Id);
                    if (NetworkManager.Get().UserUID == destrelay.PlayerUId)
                        DestroyMinion(destrelay.Id);
                    else if (NetworkManager.Get().OtherUID == destrelay.PlayerUId )
                        targetPlayer.DestroyMinion(destrelay.Id);
                    else if (NetworkManager.Get().CoopUID == destrelay.PlayerUId )
                        coopPlayer.DestroyMinion(destrelay.Id);
                    
                    break;
                }
                
                case GameProtocol.DESTROY_MAGIC_RELAY:
                {
                    MsgDestroyMagicRelay desmagic = (MsgDestroyMagicRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == desmagic.PlayerUId)
                        DestroyMagic(desmagic.BaseStatId);
                    else if (NetworkManager.Get().OtherUID == desmagic.PlayerUId )
                        targetPlayer.DestroyMagic(desmagic.BaseStatId);
                    else if (NetworkManager.Get().CoopUID == desmagic.PlayerUId )
                        coopPlayer.DestroyMagic(desmagic.BaseStatId);
                    
                    break;
                }
                
                
                
                
                
                
                case GameProtocol.FIRE_BALL_BOMB_RELAY:
                {
                    MsgFireballBombRelay fbrelay = (MsgFireballBombRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == fbrelay.PlayerUId)
                        FireballBomb(fbrelay.Id);
                    else if (NetworkManager.Get().OtherUID == fbrelay.PlayerUId )
                        targetPlayer.FireballBomb(fbrelay.Id);
                    else if (NetworkManager.Get().CoopUID == fbrelay.PlayerUId )
                        coopPlayer.FireballBomb(fbrelay.Id);
                    
                    break;
                }
                case GameProtocol.HEAL_MINION_RELAY:
                {
                    MsgHealMinionRelay healrelay = (MsgHealMinionRelay) param[0];

                    float serverHealVal =  ConvertNetMsg.MsgIntToFloat(healrelay.Heal);
                    if (NetworkManager.Get().UserUID == healrelay.PlayerUId)
                        HealMinion(healrelay.Id, serverHealVal);
                    else if (NetworkManager.Get().OtherUID == healrelay.PlayerUId )
                        targetPlayer.HealMinion(healrelay.Id, serverHealVal);
                    else if (NetworkManager.Get().CoopUID == healrelay.PlayerUId )
                        coopPlayer.HealMinion(healrelay.Id, serverHealVal);
                    
                    break;
                }
                case GameProtocol.MINE_BOMB_RELAY:
                {
                    MsgMineBombRelay mbrelay = (MsgMineBombRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == mbrelay.PlayerUId)
                        MineBomb(mbrelay.Id);
                    else if (NetworkManager.Get().OtherUID == mbrelay.PlayerUId )
                        targetPlayer.MineBomb(mbrelay.Id);
                    else if (NetworkManager.Get().CoopUID == mbrelay.PlayerUId )
                        coopPlayer.MineBomb(mbrelay.Id);
                    break;
                }
                case GameProtocol.STURN_MINION_RELAY:
                {
                    MsgSturnMinionRelay sturelay = (MsgSturnMinionRelay) param[0];
                    
                    float chDur = ConvertNetMsg.MsgIntToFloat(sturelay.SturnTime );
                    
                    if (NetworkManager.Get().UserUID == sturelay.PlayerUId)
                        SturnMinion(sturelay.Id, chDur);
                    else if (NetworkManager.Get().OtherUID == sturelay.PlayerUId )
                        targetPlayer.SturnMinion(sturelay.Id, chDur);
                    else if (NetworkManager.Get().CoopUID == sturelay.PlayerUId )
                        coopPlayer.SturnMinion(sturelay.Id, chDur);
                    
                    break;
                }
                case GameProtocol.ROCKET_BOMB_RELAY:
                {
                    MsgRocketBombRelay rockrelay = (MsgRocketBombRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == rockrelay.PlayerUId)
                        DestroyMagic(rockrelay.Id);
                    else if (NetworkManager.Get().OtherUID == rockrelay.PlayerUId )
                        targetPlayer.DestroyMagic(rockrelay.Id);
                    else if (NetworkManager.Get().CoopUID == rockrelay.PlayerUId )
                        coopPlayer.DestroyMagic(rockrelay.Id);
                    
                    break;
                }
                case GameProtocol.ICE_BOMB_RELAY:
                {
                    MsgIceBombRelay icerelay = (MsgIceBombRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == icerelay.PlayerUId)
                        IceballBomb(icerelay.Id);
                    else if (NetworkManager.Get().OtherUID == icerelay.PlayerUId )
                        targetPlayer.IceballBomb(icerelay.Id);
                    else if (NetworkManager.Get().CoopUID == icerelay.PlayerUId )
                        coopPlayer.IceballBomb(icerelay.Id);
                    
                    break;
                }
                case GameProtocol.FIRE_MAN_FIRE_RELAY:
                {
                    MsgFireManFireRelay firerelay = (MsgFireManFireRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == firerelay.PlayerUId)
                        FiremanFire(firerelay.Id);
                    else if (NetworkManager.Get().OtherUID == firerelay.PlayerUId )
                        targetPlayer.FiremanFire(firerelay.Id);
                    else if (NetworkManager.Get().CoopUID == firerelay.PlayerUId )
                        coopPlayer.FiremanFire(firerelay.Id);
                    
                    break;
                }
                case GameProtocol.MINION_CLOACKING_RELAY:
                {
                    MsgMinionCloackingRelay cloackrelay = (MsgMinionCloackingRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == cloackrelay.PlayerUId)
                        Cloacking(cloackrelay.Id, cloackrelay.IsCloacking);
                    else if (NetworkManager.Get().OtherUID == cloackrelay.PlayerUId )
                        targetPlayer.Cloacking(cloackrelay.Id, cloackrelay.IsCloacking);
                    else if (NetworkManager.Get().CoopUID == cloackrelay.PlayerUId )
                        coopPlayer.Cloacking(cloackrelay.Id, cloackrelay.IsCloacking);

                    break;
                }
                case GameProtocol.MINION_FLAG_OF_WAR_RELAY:
                {
                    MsgMinionFlagOfWarRelay flagrelay = (MsgMinionFlagOfWarRelay) param[0];
                    
                    float convFactor = ConvertNetMsg.MsgIntToFloat(flagrelay.Effect );
                    
                    if (NetworkManager.Get().UserUID == flagrelay.PlayerUId)
                        FlagOfWar(flagrelay.BaseStatId , flagrelay.IsFogOfWar , convFactor);
                    else if (NetworkManager.Get().OtherUID == flagrelay.PlayerUId )
                        targetPlayer.FlagOfWar(flagrelay.BaseStatId , flagrelay.IsFogOfWar , convFactor);
                    else if (NetworkManager.Get().CoopUID == flagrelay.PlayerUId )
                        coopPlayer.FlagOfWar(flagrelay.BaseStatId , flagrelay.IsFogOfWar , convFactor);
                    
                    break;
                }
                case GameProtocol.SCARECROW_RELAY:
                {
                    MsgScarecrowRelay scarelay = (MsgScarecrowRelay) param[0];
                    
                    float chEyeLv = ConvertNetMsg.MsgIntToFloat(scarelay.EyeLevel );
                    if (NetworkManager.Get().UserUID == scarelay.PlayerUId)
                        ScareCrow(scarelay.BaseStatId , chEyeLv);
                    else if (NetworkManager.Get().OtherUID == scarelay.PlayerUId )
                        targetPlayer.ScareCrow(scarelay.BaseStatId , chEyeLv);
                    else if (NetworkManager.Get().CoopUID == scarelay.PlayerUId )
                        coopPlayer.ScareCrow(scarelay.BaseStatId , chEyeLv);
                    
                    break;
                }
                case GameProtocol.LAYZER_TARGET_RELAY:
                {
                    MsgLayzerTargetRelay lazerrelay = (MsgLayzerTargetRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == lazerrelay.PlayerUId)
                        LayzerMinion(lazerrelay.Id, ConvertNetMsg.MsgUshortArrToIntArr(lazerrelay.TargetIdArray));
                    else if (NetworkManager.Get().OtherUID == lazerrelay.PlayerUId )
                        targetPlayer.LayzerMinion(lazerrelay.Id, ConvertNetMsg.MsgUshortArrToIntArr(lazerrelay.TargetIdArray));
                    else if (NetworkManager.Get().CoopUID == lazerrelay.PlayerUId )
                        coopPlayer.LayzerMinion(lazerrelay.Id, ConvertNetMsg.MsgUshortArrToIntArr(lazerrelay.TargetIdArray));
                    
                    break;
                }
                case GameProtocol.MINION_INVINCIBILITY_RELAY:
                {
                    MsgMinionInvincibilityRelay inrelay = (MsgMinionInvincibilityRelay) param[0];
                    
                    float convTime = ConvertNetMsg.MsgIntToFloat(inrelay.Time );
                    
                    if (NetworkManager.Get().UserUID == inrelay.PlayerUId)
                        SetMinionInvincibility(inrelay.Id, convTime);
                    else if (NetworkManager.Get().OtherUID == inrelay.PlayerUId )
                        targetPlayer.SetMinionInvincibility(inrelay.Id, convTime);
                    else if (NetworkManager.Get().CoopUID == inrelay.PlayerUId )
                        coopPlayer.SetMinionInvincibility(inrelay.Id, convTime);
                    
                    break;
                }


                case GameProtocol.FIRE_BULLET_RELAY:
                {
                    MsgFireBulletRelay arrrelay = (MsgFireBulletRelay) param[0];
                    
                    //Dir Damage MoveSpeed
                    float calDamage = ConvertNetMsg.MsgIntToFloat(arrrelay.Damage );
                    float calSpeed = ConvertNetMsg.MsgIntToFloat(arrrelay.MoveSpeed );
                    E_BulletType bulletType = (E_BulletType) arrrelay.Type;
                    
                    //NetSendPlayer(GameProtocol.FIRE_BULLET_RELAY , myUID, id, targetId , chDamage ,chSpeed , (int)bulletType);
                    
                    if (NetworkManager.Get().UserUID == arrrelay.PlayerUId)
                        FireBullet(bulletType , arrrelay.Id , arrrelay.targetId, calDamage , calSpeed);
                    else if (NetworkManager.Get().OtherUID == arrrelay.PlayerUId )
                        targetPlayer.FireBullet(bulletType , arrrelay.Id , arrrelay.targetId, calDamage , calSpeed);
                    else if (NetworkManager.Get().CoopUID == arrrelay.PlayerUId )
                        coopPlayer.FireBullet(bulletType , arrrelay.Id , arrrelay.targetId, calDamage , calSpeed);
                    
                    break;
                }
                
                case GameProtocol.FIRE_ARROW_RELAY:
                {
                    MsgFireArrowRelay arrrelay = (MsgFireArrowRelay) param[0];
                    
                    //Dir Damage MoveSpeed
                    Vector3 sPos = ConvertNetMsg.MsgToVector3(arrrelay.Dir);
                    
                    float calDamage = ConvertNetMsg.MsgIntToFloat(arrrelay.Damage );
                    float calSpeed = ConvertNetMsg.MsgIntToFloat(arrrelay.MoveSpeed );
                    
                    if (NetworkManager.Get().UserUID == arrrelay.PlayerUId)
                        FireArrow(sPos , arrrelay.Id, calDamage , calSpeed);
                    else if (NetworkManager.Get().OtherUID == arrrelay.PlayerUId )
                        targetPlayer.FireArrow(sPos , arrrelay.Id, calDamage , calSpeed);
                    else if (NetworkManager.Get().CoopUID == arrrelay.PlayerUId )
                        coopPlayer.FireArrow(sPos , arrrelay.Id, calDamage , calSpeed);
                    break;
                }
                case GameProtocol.FIRE_SPEAR_RELAY:
                {
                    MsgFireSpearRelay spearrelay = (MsgFireSpearRelay) param[0];

                    Vector3 startPos = ConvertNetMsg.MsgToVector3(spearrelay.ShootPos);
                    float chDamage = ConvertNetMsg.MsgIntToFloat(spearrelay.Power );
                    float chSpeed = ConvertNetMsg.MsgIntToFloat(spearrelay.MoveSpeed );
                    
                    if (NetworkManager.Get().UserUID == spearrelay.PlayerUId)
                        FireSpear(startPos, spearrelay.TargetId, chDamage, chSpeed);
                    else if (NetworkManager.Get().OtherUID == spearrelay.PlayerUId )
                        targetPlayer.FireSpear(startPos, spearrelay.TargetId, chDamage, chSpeed);
                    else if (NetworkManager.Get().CoopUID == spearrelay.PlayerUId )
                        coopPlayer.FireSpear(startPos, spearrelay.TargetId, chDamage, chSpeed);
                    
                    break;
                }
                case GameProtocol.NECROMANCER_BULLET_RELAY:
                {
                    MsgNecromancerBulletRelay necrorelay = (MsgNecromancerBulletRelay) param[0];

                    Vector3 shootPos = ConvertNetMsg.MsgToVector3(necrorelay.ShootPos);
                        
                    if (NetworkManager.Get().UserUID == necrorelay.PlayerUId)
                        FireNecromancerBullet(shootPos , necrorelay.TargetId , necrorelay.Power , necrorelay.BulletMoveSpeed );
                    else if (NetworkManager.Get().OtherUID == necrorelay.PlayerUId )
                        targetPlayer.FireNecromancerBullet(shootPos , necrorelay.TargetId , necrorelay.Power , necrorelay.BulletMoveSpeed );
                    else if (NetworkManager.Get().CoopUID == necrorelay.PlayerUId )
                        coopPlayer.FireNecromancerBullet(shootPos , necrorelay.TargetId , necrorelay.Power , necrorelay.BulletMoveSpeed );
                    
                    break;
                }
                case GameProtocol.FIRE_CANNON_BALL_RELAY:
                {
                    MsgFireCannonBallRelay fcannonrelay = (MsgFireCannonBallRelay) param[0];

                    Vector3 startPos = ConvertNetMsg.MsgToVector3(fcannonrelay.ShootPos);
                    Vector3 targetPos = ConvertNetMsg.MsgToVector3(fcannonrelay.TargetPos);
                    float chDamage = ConvertNetMsg.MsgIntToFloat(fcannonrelay.Power );
                    float chRange = ConvertNetMsg.MsgIntToFloat(fcannonrelay.Range );
                    E_CannonType cannonType = (E_CannonType) fcannonrelay.Type;
        
                    if (NetworkManager.Get().UserUID == fcannonrelay.PlayerUId)
                        FireCannonBall(cannonType , startPos, targetPos, chDamage, chRange);
                    else if (NetworkManager.Get().OtherUID == fcannonrelay.PlayerUId )
                        targetPlayer.FireCannonBall(cannonType ,startPos, targetPos, chDamage, chRange);
                    else if (NetworkManager.Get().CoopUID == fcannonrelay.PlayerUId )
                        coopPlayer.FireCannonBall(cannonType ,startPos, targetPos, chDamage, chRange);
                    
                    break;
                }
                
                
                
                case GameProtocol.SET_MINION_ANIMATION_TRIGGER_RELAY:
                {
                    MsgSetMinionAnimationTriggerRelay anirelay = (MsgSetMinionAnimationTriggerRelay) param[0];
                    
                    //UnityEngine.Debug.Log(anirelay.Trigger);
                    string aniName = ((E_AniTrigger)anirelay.Trigger).ToString();
                    
                    //Debug.LogFormat("ANI TRIGGER = UID:{0}, ID:{1}, TRIGGER:{2}, TARGET:{3}", anirelay.PlayerUId, anirelay.Id, aniName, anirelay.TargetId);
                    
                    if (NetworkManager.Get().UserUID == anirelay.PlayerUId)
                        SetMinionAnimationTrigger(anirelay.Id, aniName , anirelay.TargetId);
                    else if (NetworkManager.Get().OtherUID == anirelay.PlayerUId )
                        targetPlayer.SetMinionAnimationTrigger(anirelay.Id, aniName ,anirelay.TargetId);
                    else if (NetworkManager.Get().CoopUID == anirelay.PlayerUId )
                        coopPlayer.SetMinionAnimationTrigger(anirelay.Id, aniName ,anirelay.TargetId);
                    break;
                }
                case GameProtocol.SET_MAGIC_TARGET_ID_RELAY:
                {
                    MsgSetMagicTargetIdRelay smtidrelay = (MsgSetMagicTargetIdRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == smtidrelay.PlayerUId)
                        SetMagicTarget(smtidrelay.Id, smtidrelay.TargetId);
                    else if (NetworkManager.Get().OtherUID == smtidrelay.PlayerUId )
                        targetPlayer.SetMagicTarget(smtidrelay.Id, smtidrelay.TargetId);
                    else if (NetworkManager.Get().CoopUID == smtidrelay.PlayerUId )
                        coopPlayer.SetMagicTarget(smtidrelay.Id, smtidrelay.TargetId);
                    
                    break;
                }
                case GameProtocol.SET_MAGIC_TARGET_POS_RELAY:
                {
                    MsgSetMagicTargetRelay smtrelay = (MsgSetMagicTargetRelay) param[0];
                    
                    float chX =  ConvertNetMsg.MsgIntToFloat(smtrelay.X );
                    float chZ =  ConvertNetMsg.MsgIntToFloat(smtrelay.Z );
                    
                    if (NetworkManager.Get().UserUID == smtrelay.PlayerUId)
                        SetMagicTarget(smtrelay.Id, chX , chZ);
                    else if (NetworkManager.Get().OtherUID == smtrelay.PlayerUId )
                        targetPlayer.SetMagicTarget(smtrelay.Id, chX , chZ);
                    else if (NetworkManager.Get().CoopUID == smtrelay.PlayerUId )
                        coopPlayer.SetMagicTarget(smtrelay.Id, chX , chZ);
                    
                    break;
                }
                case GameProtocol.ACTIVATE_POOL_OBJECT_RELAY:
                {
                    MsgActivatePoolObjectRelay actrelay = (MsgActivatePoolObjectRelay) param[0];
                    
                    Vector3 stPos = ConvertNetMsg.MsgToVector3(actrelay.HitPos);
                    Vector3 localScale = ConvertNetMsg.MsgToVector3(actrelay.LocalScale);
                    Quaternion rotate = ConvertNetMsg.MsgToQuaternion(actrelay.Rotation);
                    
                    string strObjName = ((E_PoolName) actrelay.PoolName).ToString();

                    //if (NetworkManager.Get().UserUID == actrelay.PlayerUId)
                        ActivationPoolObject(strObjName, stPos, rotate, localScale);
                    //else if (NetworkManager.Get().OtherUID == actrelay.PlayerUId )
//                        targetPlayer.ActivationPoolObject(strObjName, stPos, rotate, localScale);

                    break;
                }
                case GameProtocol.SEND_MESSAGE_VOID_RELAY:
                {
                    MsgSendMessageVoidRelay voidmsg = (MsgSendMessageVoidRelay) param[0];
                    
                    string msgFunc = ((E_ActionSendMessage)voidmsg.Message).ToString();
                    
                    if (NetworkManager.Get().UserUID == voidmsg.PlayerUId)
                        MinionSendMessage(voidmsg.Id , msgFunc);
                    else if (NetworkManager.Get().OtherUID == voidmsg.PlayerUId )
                        targetPlayer.MinionSendMessage(voidmsg.Id , msgFunc);
                    else if (NetworkManager.Get().CoopUID == voidmsg.PlayerUId )
                        coopPlayer.MinionSendMessage(voidmsg.Id , msgFunc);
                    
                    break;
                }
                case GameProtocol.SEND_MESSAGE_PARAM1_RELAY:
                {
                    MsgSendMessageParam1Relay paramrelay = (MsgSendMessageParam1Relay) param[0];
                    
                    string msgFunc = ((E_ActionSendMessage)paramrelay.Message).ToString();
                    
                    if (NetworkManager.Get().UserUID == paramrelay.PlayerUId)
                        MinionSendMessage(paramrelay.Id , msgFunc , paramrelay.TargetId);
                    else if (NetworkManager.Get().OtherUID == paramrelay.PlayerUId )
                        targetPlayer.MinionSendMessage(paramrelay.Id , msgFunc , paramrelay.TargetId);
                    else if (NetworkManager.Get().CoopUID == paramrelay.PlayerUId )
                        coopPlayer.MinionSendMessage(paramrelay.Id , msgFunc , paramrelay.TargetId);
                    
                    break;
                }
                case GameProtocol.SET_MINION_TARGET_RELAY:
                {
                    MsgSetMinionTargetRelay targetrelay = (MsgSetMinionTargetRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == targetrelay.PlayerUId)
                        SetMinionTarget(targetrelay.Id , targetrelay.TargetId);
                    else if (NetworkManager.Get().OtherUID == targetrelay.PlayerUId )
                        targetPlayer.SetMinionTarget(targetrelay.Id , targetrelay.TargetId);
                    else if (NetworkManager.Get().CoopUID == targetrelay.PlayerUId )
                        coopPlayer.SetMinionTarget(targetrelay.Id , targetrelay.TargetId);
                    
                    break;
                }
                case GameProtocol.PUSH_MINION_RELAY:
                {
                    MsgPushMinionRelay pushrelay = (MsgPushMinionRelay) param[0];

                    Vector3 conVecDir = ConvertNetMsg.MsgToVector3(pushrelay.Dir);
                    float convPower = ConvertNetMsg.MsgIntToFloat(pushrelay.PushPower );
                    
                    
                    if (NetworkManager.Get().UserUID == pushrelay.PlayerUId)
                        PushMinion(pushrelay.Id ,conVecDir , convPower );
                    else if (NetworkManager.Get().OtherUID == pushrelay.PlayerUId )
                        targetPlayer.PushMinion(pushrelay.Id ,conVecDir , convPower );
                    else if (NetworkManager.Get().CoopUID == pushrelay.PlayerUId )
                        coopPlayer.PushMinion(pushrelay.Id ,conVecDir , convPower );
                    
                    break;
                }
                
                
                case GameProtocol.MINION_STATUS_RELAY:
                {
                    MsgMinionStatusRelay statusrelay = (MsgMinionStatusRelay) param[0];
                    

                    if (NetworkManager.Get().OtherUID == statusrelay.PlayerUId)
                        targetPlayer.SyncMinion(statusrelay.PosIndex, statusrelay.Pos, statusrelay.Hp, statusrelay.Relay, statusrelay.packetCount);
                    else if (NetworkManager.Get().UserUID == statusrelay.PlayerUId)
                        SyncMinion(statusrelay.PosIndex, statusrelay.Pos, statusrelay.Hp, statusrelay.Relay, statusrelay.packetCount);
                    else if (NetworkManager.Get().CoopUID == statusrelay.PlayerUId)
                        coopPlayer.SyncMinion(statusrelay.PosIndex, statusrelay.Pos, statusrelay.Hp, statusrelay.Relay, statusrelay.packetCount);
                    
                    break;
                }
                
            }
        }

        #endregion
        
        
        
        
        
        
        #region not use old code
        /*
        #region dice rpc
        
        //////////////////////////////////////////////////////////////////////
        // Dice RPCs
        //////////////////////////////////////////////////////////////////////

        

        //////////////////////////////////////////////////////////////////////
        // Unit's RPCs
        //////////////////////////////////////////////////////////////////////
        public void TeleportMinion(int baseStatId, float x, float z)
        {
            Transform ts = listMinion.Find(minion => minion.id == baseStatId).transform;
            PoolManager.instance.ActivateObject("Effect_Ninja", ts.position);
            ts.position = new Vector3(x, 0, z);
            PoolManager.instance.ActivateObject("Effect_Ninja", ts.position);
        }

        public void SpawnSkeleton(Vector3 pos)
        {
            //CreateMinion(InGameManager.Get().data_AllDice.listDice[1], pos, 1, 0, 0, -1);
            CreateMinion(InGameManager.Get().data_DiceInfo.GetData(1), pos, 1, 0, 0, -1);
        }

        #endregion
        */
        
        
        
        
        // photon remove
        /*
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
                    //stream.SendNext(m.rb.rotation);
                    //stream.SendNext(m.rb.velocity);
                    //stream.SendNext(m.agent.velocity);
                    //stream.SendNext(m.currentHealth);
                }
            }
            else
            {
                currentHealth = (float)stream.ReceiveNext();

                var loopCount = (int)stream.ReceiveNext();
                for (var i = 0; i < loopCount && i < listMinion.Count; i++)
                {
                    // listMinion[i].SetNetworkValue((Vector3)stream.ReceiveNext(), (Quaternion)stream.ReceiveNext(),
                    //     (Vector3)stream.ReceiveNext(), (float)stream.ReceiveNext(), info.SentServerTime);
                    listMinion[i].SetNetworkValue((Vector3)stream.ReceiveNext());
                }
            }
        }

        #endregion
        
        
        #region photon send recv
        public void SendPlayer(RpcTarget target , E_PTDefine ptID , params object[] param)
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC(recvMessage, target, ptID, param);
            }
            else
            {
                RecvPlayer(ptID, param);
            }
        }

        [PunRPC]
        public void RecvPlayer(E_PTDefine ptID , params object[] param)
        {
            switch (ptID)
            {
                case E_PTDefine.PT_SETDECK:            //
                    string deck = param[0] as string;
                    SetDeck(deck);
                    break;
                case E_PTDefine.PT_CHANGELAYER:            //
                    ChangeLayer((bool) param[0]);
                    break;
                case E_PTDefine.PT_REMOVEMINION:             //
                    int baseID = (int) param[0];
                    RemoveMinion(baseID);
                    break;
                case E_PTDefine.PT_GETDICE:            //
                    int deckNum = (int) param[0];
                    int slotNum = (int) param[1];
                    GetDice(deckNum, slotNum);
                    break;
                case E_PTDefine.PT_LEVELUPDICE:            //
                    LevelUpDice((int) param[0], (int) param[1], (int) param[2], (int) param[3]);
                    break;
                case E_PTDefine.PT_REMOVEMAGIC:            //
                    int magicId = (int) param[0];
                    RemoveMagic(magicId);
                    break;
                case E_PTDefine.PT_HITMINIONANDMAGIC:            //
                    int baseIDhit = (int) param[0];
                    float damage = (float) param[1];
<<<<<<< HEAD
                    //targetPlayer.HitDamageMinion(baseIDhit, damage, delay);
                    HitDamageMinionAndMagic(baseIDhit, damage);
=======
                    float delay = (float) param[2];
                    HitDamageMinionAndMagic(baseIDhit, damage, delay);
>>>>>>> ServerPacket
                    break;
                case E_PTDefine.PT_HITDAMAGE:            //
                    float damageH = (float) param[0];
                    HitDamage(damageH);
                    break;
                case E_PTDefine.PT_DESTROYMINION:            //
                    int baseIdD = (int) param[0];
                    DestroyMinion(baseIdD);
                    break;
                case E_PTDefine.PT_DESTROYMAGIC:            //
                    baseIdD = (int) param[0];
                    DestroyMagic(baseIdD);
                    break;
                case E_PTDefine.PT_HEALMINION:            //
                    int baseId = (int) param[0];
                    float heal = (float) param[1];
                    HealMinion(baseId, heal);
                    break;
                case E_PTDefine.PT_FIREBALLBOMB:            //
                    FireballBomb((int) param[0]);
                    break;
                case E_PTDefine.PT_ICEBALLBOMB:            //
                    IceballBomb((int) param[0]);
                    break;
                case E_PTDefine.PT_ROCKETBOMB:            //
                    RocketBomb((int) param[0]);
                    break;
                case E_PTDefine.PT_MINIONATTACKSPEEDFACTOR:
                    listMinion.Find(minion => minion.id == (int) param[0])?.SetAttackSpeedFactor((float) param[1]);
                    break;
                case E_PTDefine.PT_STURNMINION:            //
                    SturnMinion((int) param[0], (float) param[1]);
                    break;
                case E_PTDefine.PT_SETMAGICTARGET:            //
                    if (param.Length > 2)
                    {
                        SetMagicTarget((int) param[0], (float) param[1], (float) param[2]);
                    }
                    else
                    {
                        SetMagicTarget((int) param[0], (int) param[1]);
                    }

                    break;
                case E_PTDefine.PT_MINEBOMB:            //
                    MineBomb((int) param[0]);
                    break;
<<<<<<< HEAD
                case E_PTDefine.PT_FIRECANNONBALL:
                    FireCannonBall((E_CannonType)param[0], (Vector3) param[1], (Vector3) param[2], (float) param[3], (float) param[4]);
                    break;
                case E_PTDefine.PT_FIREBULLET:
                    FireBullet((E_BulletType)param[0], (Vector3) param[1], (int) param[2], (float) param[3], (float) param[4]);
=======
                case E_PTDefine.PT_FIRECANNONBALL:            //
                    FireCannonBall((Vector3) param[0], (Vector3) param[1], (float) param[2], (float) param[3]);
                    break;
                case E_PTDefine.PT_FIREARROW:            //
                    FireArrow((Vector3) param[0], (int) param[1], (float) param[2], (float) param[3]);
                    break;
                case E_PTDefine.PT_FIRESPEAR:            //
                    FireSpear((Vector3) param[0], (int) param[1], (float) param[2], (float) param[3]);
>>>>>>> ServerPacket
                    break;
                case E_PTDefine.PT_MINIONANITRIGGER:            //
                    SetMinionAnimationTrigger((int) param[0], (string) param[1]);
                    break;
                case E_PTDefine.PT_FIREMANFIRE:            //
                    FiremanFire((int) param[0]);
                    break;
                case E_PTDefine.PT_SPAWNSKELETON:
                    SpawnSkeleton((Vector3) param[0]);
                    break;
                case E_PTDefine.PT_TELEPORTMINION:
                    TeleportMinion((int) param[0], (float) param[1], (float) param[2]);
                    break;
                case E_PTDefine.PT_SPAWN:            //
                    Spawn();
                    break;
                case E_PTDefine.PT_LAYZERTARGET:
                    var m_layzer = listMinion.Find(minion => minion.id == (int) param[0]);
                    if (m_layzer != null)
                    {
                        var layzer = m_layzer as Minion_Layzer;
                        if (layzer != null)
                        {
                            layzer.SetTargetList((int[]) param[1]);
                        }
                    }
                    break;
                case E_PTDefine.PT_MINIONINVINCIBILITY:
                    listMinion.Find(m => m.id == (int) param[0])?.Invincibility((float) param[1]);
                    break;
                case E_PTDefine.PT_SCARECROW:
                    listMinion.Find(m => m.id == (int) param[0])?.Scarecrow((float) param[1]);
                    break;
                case E_PTDefine.PT_ACTIVATEPOOLOBJECT:            //
                    var ts = PoolManager.instance.ActivateObject((string) param[0], (Vector3) param[1]);
                    if (ts != null)
                    {
                        ts.rotation = (Quaternion) param[2];
                        ts.localScale = (Vector3) param[3];
                    }
                    break;
                case E_PTDefine.PT_MINIONCLOACKING:            //
                    listMinion.Find(m => m.id == (int)param[0])?.Cloacking((bool)param[1]);
                    break;
                case E_PTDefine.PT_MINIONFOGOFWAR:            //
                    listMinion.Find(m => m.id == (int)param[0])?.SetFlagOfWar((bool)param[1], (float)param[2]);
                    break;
                case E_PTDefine.PT_SENDMESSAGEVOID:            //
                    listMinion.Find(m => m.id == (int)param[0])?.SendMessage((string)param[1], SendMessageOptions.DontRequireReceiver);
                    listMagic.Find(m => m.id == (int)param[0])?.SendMessage((string)param[1], SendMessageOptions.DontRequireReceiver);
                    break;
                case E_PTDefine.PT_SENDMESSAGEPARAM1:            //
                    listMinion.Find(m => m.id == (int)param[0])?.SendMessage((string)param[1], param[2], SendMessageOptions.DontRequireReceiver);
                    listMagic.Find(m => m.id == (int)param[0])?.SendMessage((string)param[1], param[2], SendMessageOptions.DontRequireReceiver);
                    break;
<<<<<<< HEAD
                case E_PTDefine.PT_SETMINIONTARGET:
                    listMinion.Find(minion => minion.id == (int) param[0]).target =
                        targetPlayer.GetBaseStatFromId((int) param[1]);
=======
                case E_PTDefine.PT_NECROMANCERBULLET:            //
                    FireNecromancerBullet((Vector3)param[0] , (int)param[1] , (float)param[2], (float)param[3]);
                    break;
                case E_PTDefine.PT_SETMINIONTARGET:            //
                    listMinion.Find(minion => minion.id == (int) param[0]).target = targetPlayer.GetBaseStatFromId((int) param[1]);
>>>>>>> ServerPacket
                    break;
                case E_PTDefine.PT_PUSHMINION:
                    listMinion.Find(minion => minion.id == (int)param[0])?.Push((Vector3)param[1], (float)param[2]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ptID), ptID, null);
            }
        }
        #endregion
        */
        #endregion
    }
}
