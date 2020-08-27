#if UNITY_EDITOR
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
                PlayerController pc = GameObject.FindObjectOfType(typeof(PlayerController)) as PlayerController;
                if (pc != null && pc.photonView.IsMine)
                {
                    _instance = pc;
                    _instance.Init();
                    return _instance;
                }
                else
                {
                    return null;
                }
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
        /*protected Data_Dice[] _arrDeck;
        public Data_Dice[] arrDeck 
        { 
            get => _arrDeck;
            protected set => _arrDeck = value;
        }*/
        
        
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

        public int robotPieceCount;
        public int robotEyeTotalLevel;

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
            if (arrDiceDeck == null) _arrDiceDeck = new DiceInfoData[5];
            _arrUpgradeLevel = new int[5];
        }

        public void DestroyPlayer()
        {
            _arrDice = null;
            _arrDiceDeck = null;
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

            SetColor(isBottomPlayer ? E_MaterialType.BOTTOM : E_MaterialType.TOP);
        }
        #endregion
        
        #region spawn
        
        //[PunRPC]
        public void Spawn()
        {
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
                        for (var j = 0; j < (arrDice[i].level + 1) * multiply; j++)
                        {
                            CreateMinion(arrDice[i].diceData, ts.position, arrDice[i].level + 1, upgradeLevel, magicCastDelay, i);
                        }
                        break;
                    case (int)DICE_CAST_TYPE.HERO:
                        CreateMinion(arrDice[i].diceData, ts.position, arrDice[i].level + 1, upgradeLevel, magicCastDelay, i);
                        break;
                    case (int)DICE_CAST_TYPE.MAGIC:
                        // for(var j = 0; j < (arrDice[i].level + 1) * multiply; j++)
                        // {
                        //     CastMagic(arrDice[i].diceData, arrDice[i].level + 1, upgradeLevel, magicCastDelay, i);
                        // }
                        // break;
                    case (int)DICE_CAST_TYPE.INSTALLATION:
                        CastMagic(arrDice[i].diceData, arrDice[i].level + 1, upgradeLevel, magicCastDelay, i);
                        break;
                        // upgradeLevel = GetDiceUpgradeLevel(arrDice[i].data);
                        // CastMagic(arrDice[i].data, arrDice[i].level, upgradeLevel, magicCastDelay, i);
                        // break;
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

        public Minion CreateMinion(GameObject pref, Vector3 spawnPos, int eyeLevel, int upgradeLevel)
        {
            var m = PoolManager.instance.ActivateObject<Minion>(pref.name, spawnPos, InGameManager.Get().transform);

            if (m == null)
            { 
                PoolManager.instance.AddPool(pref, 1);
                Debug.LogFormat("{0} Pool Added 1", pref.name);
                m = PoolManager.instance.ActivateObject<Minion>(pref.name, spawnPos, InGameManager.Get().transform);
            }

            if (m != null)
            {
                m.id = _spawnCount++;
                m.controller = this;
                m.isMine = PhotonNetwork.IsConnected ? photonView.IsMine : isMine;
                if (!listMinion.Contains(m)) listMinion.Add(m);
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

            if (PhotonNetwork.IsConnected && InGameManager.Get().playType != PLAY_TYPE.CO_OP && !photonView.IsMine)
            {
                dicePos.x *= -1f;
                dicePos.z *= -1f;
            }
            
            Debug.LogFormat("Spawn: {0}", data.prefabName);
            //FileHelper.LoadPrefab(data.prefabName , Global.E_LOADTYPE.LOAD_MINION )
            spawnPos.x += Random.Range(-0.2f, 0.2f);
            spawnPos.z += Random.Range(-0.2f, 0.2f);
            var m = PoolManager.instance.ActivateObject<Minion>( data.prefabName, spawnPos, InGameManager.Get().transform);

            if (m == null)
            {
                //PoolManager.instance.AddPool(data.prefab, 1);
                PoolManager.instance.AddPool(FileHelper.LoadPrefab(data.prefabName , Global.E_LOADTYPE.LOAD_MINION , InGameManager.Get().transform), 1);
                Debug.LogFormat("{0} Pool Added 1", data.prefabName);
                m = PoolManager.instance.ActivateObject<Minion>(data.prefabName, spawnPos, InGameManager.Get().transform);
            }
            
            if (m != null)
            {
                m.castType = (DICE_CAST_TYPE)data.castType;
                m.id = _spawnCount++;
                m.controller = this;
                m.isMine = PhotonNetwork.IsConnected ? photonView.IsMine : isMine;
                m.targetMoveType = (DICE_MOVE_TYPE)data.targetMoveType;
                m.ChangeLayer(isBottomPlayer);
                
                //m.power = data.power + (data.powerUpByInGameUp * upgradeLevel);
                //m.powerUpByUpgrade = data.powerUpByUpgrade;
                //m.powerUpByInGameUp = data.powerUpByInGameUp;
                //m.maxHealth = data.maxHealth + (data.maxHealthUpByInGameUp * upgradeLevel);
                //m.maxHealthUpByUpgrade = data.maxHealthUpByUpgrade;
                //m.maxHealthUpByInGameUp = data.maxHealthUpByInGameUp;
                //m.effect = data.effect + (data.effectUpByInGameUp * upgradeLevel);
                //m.effectUpByUpgrade = data.effectUpByUpgrade;
                //m.effectUpByInGameUp = data.effectUpByInGameUp;
                
                // new code - by nevill
                m.power = data.power + (data.powerInGameUp * upgradeLevel);
                m.powerUpByUpgrade = data.powerUpgrade;
                m.powerUpByInGameUp = data.powerInGameUp;
                m.maxHealth = data.maxHealth + (data.maxHpInGameUp * upgradeLevel);
                m.maxHealthUpByUpgrade = data.maxHpUpgrade;
                m.maxHealthUpByInGameUp = data.maxHpInGameUp;
                m.effect = data.effect + (data.effectInGameUp * upgradeLevel);
                m.effectUpByUpgrade = data.effectUpgrade;
                m.effectUpByInGameUp = data.effectInGameUp;
                m.effectDuration = data.effectDuration;
                m.effectCooltime = data.effectCooltime;
                
                m.attackSpeed = data.attackSpeed;
                m.moveSpeed = data.moveSpeed;
                m.range = data.range;
                m.searchRange = data.searchRange;
                m.eyeLevel = eyeLevel;
                m.upgradeLevel = upgradeLevel;
                m.Initialize(MinionDestroyCallback);
                if (!listMinion.Contains(m)) listMinion.Add(m);
            }

            if ((DICE_CAST_TYPE)data.castType == DICE_CAST_TYPE.HERO)
            {
                m.power *= arrDice[diceNum].level + 1;
                m.maxHealth *= arrDice[diceNum].level + 1;
                m.effect *= arrDice[diceNum].level + 1;
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
        }

        public BaseStat GetBaseStatFromId(int baseStatId)
        {
            if (baseStatId == 0) return this;

            var minion = listMinion.Find(m => m.id == baseStatId);
            if (minion != null)
            {
                return minion;
            }
            else
            {
                var magic = listMagic.Find(m => m.id == baseStatId);
                if (magic != null)
                {
                    return magic;
                }
                else
                {
                    return null;
                }
            }
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
        
        //private void CastMagic(Data_Dice data, int eyeLevel, int upgradeLevel, float delay, int diceNum)
        private void CastMagic(DiceInfoData data, int eyeLevel, int upgradeLevel, float delay, int diceNum)
        {
            StartCoroutine(CastMagicCoroutine(data, eyeLevel, upgradeLevel, delay, diceNum));
        }

        //private IEnumerator CastMagicCoroutine(Data_Dice data, int eyeLevel, int upgradeLevel, float delay, int diceNum)
        private IEnumerator CastMagicCoroutine(DiceInfoData data, int eyeLevel, int upgradeLevel, float delay, int diceNum)
        {
            yield return new WaitForSeconds(delay);

            Debug.LogFormat("Spawn: {0}", data.prefabName);
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

            if (PhotonNetwork.IsConnected && InGameManager.Get().playType != PLAY_TYPE.CO_OP && !photonView.IsMine)
            {
                spawnPos.x *= -1f;
                spawnPos.z *= -1f;
            }

            GameObject loadMagic = FileHelper.LoadPrefab(data.prefabName, Global.E_LOADTYPE.LOAD_MAGIC , InGameManager.Get().transform);
            //if (data.prefab != null)
            if(loadMagic != null )
            {
                //var m = PoolManager.instance.ActivateObject<Magic>(data.prefab.name, spawnPos, InGameManager.Get().transform);
                var m = PoolManager.instance.ActivateObject<Magic>(data.prefabName, spawnPos, InGameManager.Get().transform);
                if (m != null)
                {
                    m.isMine = PhotonNetwork.IsConnected
                        ? photonView.IsMine
                        : (InGameManager.Get().playerController == this);
                    m.id = _spawnCount++;
                    m.controller = this;
                    m.diceFieldNum = diceNum;
                    m.targetMoveType = (DICE_MOVE_TYPE)data.targetMoveType;
                    m.castType = (DICE_CAST_TYPE)data.castType;
                    
                    m.power = (data.power + (data.powerInGameUp * upgradeLevel)) * Mathf.Pow(1.5f, eyeLevel - 1);
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
                    m.moveSpeed = data.moveSpeed;
                    m.range = data.range;
                    m.searchRange = data.searchRange;
                    m.eyeLevel = eyeLevel;
                    m.upgradeLevel = upgradeLevel;
                    
                    m.Initialize(isBottomPlayer);
                    m.SetTarget();
                    listMagic.Add(m);
                }
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

                //var randomDeckNum = Random.Range(0, arrDeck.Length);
                int randomDeckNum = Random.Range(0, arrDiceDeck.Length);
                //arrDice[emptySlotNum].Set(arrDeck[randDeckNum]);
                arrDice[emptySlotNum].Set(arrDiceDeck[randomDeckNum]);
                
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
            //if (arrDeck == null) arrDeck = new Data_Dice[5];
            
            //
            if(arrDiceDeck == null)_arrDiceDeck = new DiceInfoData[5];

            for (var i = 0; i < splitDeck.Length; i++)
            {
                var num = int.Parse(splitDeck[i]);
                
                //arrDeck[i] = InGameManager.Get().data_AllDice.listDice.Find(data => data.id == num);
                arrDiceDeck[i] = InGameManager.Get().data_DiceInfo.GetData(num);
                
                // add pool
                //Debug.LogFormat(gameObject.name + " AddPool: " + arrDeck[i].prefab.name);
                if (PoolManager.Get() == null) Debug.Log("PoolManager Instnace is null");
                
                //if (arrDeck[i] == null) Debug.LogError(string.Format("{0},i={1}:arrDeck[i] is null", gameObject.name, i));
                //if (arrDeck[i].prefab == null) Debug.LogError(string.Format("{0}, arrDeck[{1}].prefab is null", gameObject.name, i));
                //PoolManager.instance.AddPool(arrDeck[i].prefab, 50);
                
                //if (arrDiceDeck[i] == null) Debug.LogError(string.Format("{0},i={1}:arrDiceDeck[i] is null", gameObject.name, i));
                
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

        //[PunRPC]
        public void GetDice(int deckNum, int slotNum)
        {
            arrDice[slotNum].Set(arrDiceDeck[deckNum]);
        }

        //[PunRPC]
        public void LevelUpDice(int resetFieldNum, int levelupFieldNum, int levelupDiceId, int level)
        {
            arrDice[resetFieldNum].Reset();
            /*foreach (var data in InGameManager.Get().data_AllDice.listDice.Where(data => levelupDiceId == data.id))
            {
                arrDice[levelupFieldNum].Set(data, level);
                break;
            }*/
            
            DiceInfoData data = InGameManager.Get().data_DiceInfo.GetData(levelupDiceId);
            arrDice[levelupFieldNum].Set(data, level);
            
            
        }

        public int GetDiceFieldEmptySlotCount()
        {
            return arrDice.Count(dice => dice.id < 0);
        }

        public int DiceUpgrade(int deckNum)
        {
            return ++arrUpgradeLevel[deckNum];
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
                targetPlayer.SendPlayer(RpcTarget.All , E_PTDefine.PT_HITMINIONANDMAGIC , baseStatId, damage, delay);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                targetPlayer.HitDamageMinionAndMagic(baseStatId, damage, delay);
            }
        }

        //[PunRPC]
        public void HitDamageMinionAndMagic(int baseStatId, float damage, float delay)
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
                var m = listMinion.Find(minion => minion.id == baseStatId);
                if (m != null)
                {
                    m.HitDamage(damage, delay);
                }
                else
                {
                    var mg = listMagic.Find(magic => magic.id == baseStatId);
                    if (mg != null)
                    {
                        mg.HitDamage(damage, delay);
                    }
                }
            }
        }

        public void DeathMinion(int baseStatId)
        {
            if (PhotonNetwork.IsConnected && isMine)
            {
                //photonView.RPC("DestroyMinion", RpcTarget.All, baseStatId);
                SendPlayer(RpcTarget.All , E_PTDefine.PT_DESTROYMINION , baseStatId);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                DestroyMinion(baseStatId);
            }
        }

        public void DeathMagic(int baseStatId)
        {
            if (PhotonNetwork.IsConnected && isMine)
            {
                SendPlayer(RpcTarget.All , E_PTDefine.PT_DESTROYMAGIC , baseStatId);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                DestroyMagic(baseStatId);
            }
        }

        //[PunRPC]
        private void DestroyMinion(int baseStatId)
        {
            listMinion.Find(minion => minion.id == baseStatId)?.Death();
        }

        private void DestroyMagic(int baseStatId)
        {
            listMagic.Find(magic => magic.id == baseStatId)?.Destroy();
        }

        public void HealMinion(int baseStatId, float heal)
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
        public void FireArrow(Vector3 startPos, int targetId, float damage, float moveSpeed)
        {
            var b = PoolManager.instance.ActivateObject<Bullet>("Bullet", startPos);
            if (b != null)
            {
                b.transform.rotation = Quaternion.identity;
                b.controller = this;
                b.moveSpeed = moveSpeed;
                b.Initialize(targetId, damage, isMine, isBottomPlayer);
            }
        }
        
        //[PunRPC]
        public void FireSpear(Vector3 startPos, int targetId, float damage, float moveSpeed)
        {
            var b = PoolManager.instance.ActivateObject<Bullet>("Spear", startPos);
            if (b != null)
            {
                b.transform.rotation = Quaternion.identity;
                b.controller = this;
                b.moveSpeed = moveSpeed;
                b.Initialize(targetId, damage, isMine, isBottomPlayer);
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
        
        public void IceballBomb(int baseStatId)
        {
            ((Iceball)listMagic.Find(magic => magic.id == baseStatId))?.Bomb();
        }

        public void RocketBomb(int baseStatId)
        {
            ((Rocket)listMagic.Find(magic => magic.id == baseStatId))?.Bomb();
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
            Transform ts = listMinion.Find(minion => minion.id == baseStatId).transform;
            PoolManager.instance.ActivateObject("Effect_Ninja", ts.position);
            ts.position = new Vector3(x, 0, z);
            PoolManager.instance.ActivateObject("Effect_Ninja", ts.position);
        }

        //[PunRPC]
        public void FiremanFire(int baseStatId)
        {
            ((Minion_Fireman)listMinion.Find(minion => minion.id == baseStatId))?.Fire();
        }

        //[PunRPC]
        public void SpawnSkeleton(Vector3 pos)
        {
            //CreateMinion(InGameManager.Get().data_AllDice.listDice[1], pos, 1, 0, 0, -1);
            CreateMinion(InGameManager.Get().data_DiceInfo.GetData(1), pos, 1, 0, 0, -1);
            
            
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
                    listMinion[i].SetNetworkValue((Vector3)stream.ReceiveNext(), (Quaternion)stream.ReceiveNext(),
                        (Vector3)stream.ReceiveNext(), (float)stream.ReceiveNext(), info.SentServerTime);
                }
            }
        }

        #endregion
        
        #region photon send recv
        public void SendPlayer(RpcTarget target , E_PTDefine ptID , params object[] param)
        {
            if (PhotonNetwork.IsConnected)
            {
                photonView.RPC("RecvPlayer", target, ptID, param);
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
                case E_PTDefine.PT_SETDECK:
                    string deck = param[0] as string;
                    SetDeck(deck);
                    break;
                case E_PTDefine.PT_CHANGELAYER:
                    ChangeLayer((bool)param[0]);
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
                case E_PTDefine.PT_HITMINIONANDMAGIC:
                    int baseIDhit = (int)param[0];
                    float damage = (float)param[1];
                    float delay = (float)param[2];
                    //targetPlayer.HitDamageMinion(baseIDhit, damage, delay);
                    HitDamageMinionAndMagic(baseIDhit, damage, delay);
                    break;
                case E_PTDefine.PT_HITDAMAGE:
                    float damageH = (float)param[0];
                    float delayH = (float)param[1];
                    HitDamage(damageH, delayH);
                    break;
                case E_PTDefine.PT_DESTROYMINION:
                    int baseIdD = (int)param[0];
                    DestroyMinion(baseIdD);
                    break;
                case E_PTDefine.PT_DESTROYMAGIC:
                    baseIdD = (int) param[0];
                    DestroyMagic(baseIdD);
                    break;
                case E_PTDefine.PT_HEALMINION:
                    int baseId = (int)param[0];
                    float heal = (float)param[1];
                    HealMinion(baseId, heal);
                    break;
                case E_PTDefine.PT_FIREBALLBOMB:
                    FireballBomb((int) param[0]);
                    break;
                case E_PTDefine.PT_ICEBALLBOMB:
                    IceballBomb((int) param[0]);
                    break;
                case E_PTDefine.PT_ROCKETBOMB:
                    RocketBomb((int) param[0]);
                    break;
                case E_PTDefine.PT_MINIONATTACKSPEEDFACTOR:
                    listMinion.Find(minion => minion.id == (int) param[0])?.SetAttackSpeedFactor((float)param[1]);
                    break;
                case E_PTDefine.PT_STURNMINION:
                    SturnMinion((int) param[0], (float) param[1]);
                    break;
                case E_PTDefine.PT_SETMAGICTARGET:
                    if (param.Length > 2)
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
                    FireArrow((Vector3)param[0] , (int)param[1] , (float)param[2], (float)param[3]);
                    break;
                case E_PTDefine.PT_FIRESPEAR:
                    FireSpear((Vector3)param[0], (int) param[1], (float) param[2], (float)param[3]);
                    break;
                case E_PTDefine.PT_MINIONANITRIGGER:
                    SetMinionAnimationTrigger((int) param[0], (string) param[1]);
                    break;
                case E_PTDefine.PT_FIREMANFIRE:
                    FiremanFire((int) param[0]);
                    break;
                case E_PTDefine.PT_SPAWNSKELETON:
                    SpawnSkeleton((Vector3) param[0]);
                    break;
                case E_PTDefine.PT_TELEPORTMINION:
                    TeleportMinion((int) param[0],(float)param[1] , (float)param[2]);
                    break;
                case E_PTDefine.PT_SPAWN:
                    Spawn();
                    break;
                case E_PTDefine.PT_LAYZERTARGET:
                    ((Minion_Layzer)listMinion.Find(minion => minion.id == (int)param[0]))?.SetTargetList((int[])param[1]);
                    break;
                case E_PTDefine.PT_MINIONINVINCIBILITY:
                    listMinion.Find(m => m.id == (int)param[0])?.Invincibility((float)param[1]);
                    break;
                case E_PTDefine.PT_SCARECROW:
                    listMinion.Find(m => m.id == (int)param[0])?.Scarecrow((float)param[1]);
                    break;
                case E_PTDefine.PT_ACTIVATEPOOLOBJECT:
                    var ts = PoolManager.instance.ActivateObject((string)param[0], (Vector3)param[1]);
                    ts.rotation = (Quaternion)param[2];
                    ts.localScale = (Vector3)param[3];
                    break;
                case E_PTDefine.PT_MINIONCLOACKING:
                    listMinion.Find(m => m.id == (int)param[0])?.Cloacking((bool)param[1]);
                    break;
                case E_PTDefine.PT_MINIONFOGOFWAR:
                    listMinion.Find(m => m.id == (int)param[0])?.SetFlagOfWar((bool)param[1], (float)param[2]);
                    break;
                case E_PTDefine.PT_SENDMESSAGEVOID:
                    listMinion.Find(m => m.id == (int)param[0])?.SendMessage((string)param[1]);
                    listMagic.Find(m => m.id == (int)param[0])?.SendMessage((string)param[1]);
                    break;
                case E_PTDefine.PT_SENDMESSAGEPARAM1:
                    listMinion.Find(m => m.id == (int)param[0])?.SendMessage((string)param[1], param[2]);
                    listMagic.Find(m => m.id == (int)param[0])?.SendMessage((string)param[1], param[2]);
                    break;
                case E_PTDefine.PT_NECROMANCERBULLET:
                    FireNecromancerBullet((Vector3)param[0] , (int)param[1] , (float)param[2], (float)param[3]);
                    break;
                case E_PTDefine.PT_SETMINIONTARGET:
                    listMinion.Find(minion => minion.id == (int) param[0]).target =
                        targetPlayer.GetBaseStatFromId((int) param[1]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ptID), ptID, null);
            }
        }
        #endregion
        
        
    }
}
