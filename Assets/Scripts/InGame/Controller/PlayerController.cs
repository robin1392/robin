#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RWGameProtocol;
using RWGameProtocol.Msg;
using UnityEngine;
using UnityEngine.iOS;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;


#region photon
using Photon.Pun;
#endregion

namespace ED
{
    public class PlayerController : BaseStat //, IPunObservable
    {

        #region singleton
        
        private static PlayerController _instance = null;
        public int instanceID;

        public static PlayerController Get()
        {
            /*if (_instance == null)
            {
                PlayerController pc = GameObject.FindObjectOfType(typeof(PlayerController)) as PlayerController;
                //if (pc != null && pc.photonView.IsMine)
                if (pc != null && pc.isMine)
                {
                    _instance = pc;
                    _instance.Init();
                    return _instance;
                }
                else
                {
                    return null;
                }
            }*/
        
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
        
        [SerializeField]
        public int _spawnCount = 1;
        
        [SerializeField]
        protected int _sp = 0;
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
        private readonly string recvMessage = "RecvPlayer";

        #endregion

        #region unity base
        protected void Awake()
        {
            /*
            if (PhotonNetwork.IsConnected)
            {
                if (photonView.IsMine)
                {
                    isMine = true;
                    Init();
                }
            }
            else
            {
                if (_instance == null)
                {
                    Init();
                }
            }
            */
            

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
            
            if (InGameManager.Get().playType != PLAY_TYPE.CO_OP)
            {
                base.Start();
            }
            
            StartPlayerControll();
        }

        private void Update()
        {
            RefreshHealthBar();
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
            if(InGameManager.IsNetwork == false )
                sp = 200;
            
            currentHealth = maxHealth;
            
            for (var i = 0; i < arrDice.Length; i++)
            {
                arrDice[i] = new Dice {diceFieldNum = i};
            }
            uiDiceField = FindObjectOfType<UI_DiceField>();            
            uiDiceField.SetField(arrDice);

            
            // 
            image_HealthBar = WorldUIManager.Get().GetHealthBar(isBottomPlayer);
            text_Health = WorldUIManager.Get().GetHealthText(isBottomPlayer);
            text_Health.text = $"{Mathf.CeilToInt(currentHealth)}";
            
            InGameManager.Get().AddPlayerUnit(isBottomPlayer, this);
            
            SetColor(isBottomPlayer ? E_MaterialType.BOTTOM : E_MaterialType.TOP);

            //
            StartCoroutine(SyncMinionStatus());

            // not use
            /*
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
            */
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
                    case (int)DICE_CAST_TYPE.INSTALLATION:
                        CastMagic(arrDice[i].diceData, arrDice[i].level + 1, upgradeLevel, magicCastDelay, i);
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

        public void AddSpByWave(int addSp)
        {
            sp += addSp * (50 + 10 * spUpgradeLevel);
        }

        public void SetSp(int sp)
        {
            this.sp = sp;
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
        
        private void RefreshHealthBar()
        {
            image_HealthBar.fillAmount = currentHealth / maxHealth;
            text_Health.text = $"{Mathf.CeilToInt(currentHealth)}";
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
            if (InGameManager.IsNetwork && InGameManager.Get().playType != PLAY_TYPE.CO_OP && !isMine)
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
                Debug.LogFormat("{0} Pool Added 1", data.prefabName);
                m = PoolManager.instance.ActivateObject<Minion>(data.prefabName, spawnPos, InGameManager.Get().transform);
            }
            
            if (m != null)
            {
                m.castType = (DICE_CAST_TYPE)data.castType;
                m.id = _spawnCount++;
                m.controller = this;
                //m.isMine = PhotonNetwork.IsConnected ? photonView.IsMine : isMine;
                m.isMine = isMine;
                m.targetMoveType = (DICE_MOVE_TYPE)data.targetMoveType;
                m.ChangeLayer(isBottomPlayer);

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
                
                if (!listMinion.Contains(m)) 
                    listMinion.Add(m);
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
            if (InGameManager.IsNetwork && InGameManager.Get().playType != PLAY_TYPE.CO_OP && !isMine)
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
            // 서버에서 오는 레벨이 1부터 시작하기때문에 (클라는 0 부터 시작)1을 빼줘야된다
            int serverLevel = level - 1;
            if (serverLevel < 0)
                serverLevel = 0;
            arrDice[levelupFieldNum].Set(data, serverLevel);
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

            if (InGameManager.IsNetwork == true && this.isBottomPlayer == false && NetworkManager.Get().playType == PLAY_TYPE.BATTLE)
            {
                transform.rotation = Quaternion.Euler(0, 180f, 0);
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
       
        public void SetDeck(string deck)
        {
            var splitDeck = deck.Split('/');
            //if (arrDeck == null) arrDeck = new Data_Dice[5];
            
            //
            if(arrDiceDeck == null)_arrDiceDeck = new DiceInfoData[5];

            for (var i = 0; i < splitDeck.Length; i++)
            {
                var num = int.Parse(splitDeck[i]);
                
                arrDiceDeck[i] = InGameManager.Get().data_DiceInfo.GetData(num);
                
                // add pool
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
        
        public override void HitDamage(float damage, float delay = 0)
        {
            if (currentHealth > 0)
            {
                currentHealth -= damage;

                if (currentHealth <= 0)
                {
                    UI_InGamePopup.Get().ViewLowHP(false);
                    currentHealth = 0;
                    
                    Death();    // 
                }
                //else if (((PhotonNetwork.IsConnected && photonView.IsMine) || (!PhotonNetwork.IsConnected && isMine)) && currentHealth < 1000 && !UI_InGamePopup.Get().GetLowHP())
                else if( ( (InGameManager.IsNetwork && isMine) || (InGameManager.IsNetwork == false && isMine) ) 
                         && currentHealth < 1000 && !UI_InGamePopup.Get().GetLowHP())
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
                // 연결은 안되었으나 == 싱글모드 일때 && 내 타워라면
                if (InGameManager.IsNetwork == false && isMine)
                {
                    InGameManager.Get().EndGame(false);
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
        
        public void HitDamageMinionAndMagic(int baseStatId, float damage, float delay)
        {
            // baseStatId == 0 => Player tower
            if (baseStatId == 0)
            {
                //if (PhotonNetwork.IsConnected)
                if( InGameManager.IsNetwork == true )
                {
                    //SendPlayer(RpcTarget.All , E_PTDefine.PT_HITDAMAGE , damage, delay);
                    NetSendPlayer(GameProtocol.HIT_DAMAGE_REQ, damage);
                    //HitDamage(damage, delay);
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

        
        public void AttackEnemyMinion(int baseStatId, float damage, float delay)
        {
            //if (PhotonNetwork.IsConnected && PhotonNetwork.CurrentRoom.PlayerCount > 1)

            HitMinionDamage(true, baseStatId, damage, delay);
            
            /*targetPlayer.HitDamageMinionAndMagic(baseStatId, damage, delay);
            if(InGameManager.IsNetwork)
            {
                //targetPlayer.SendPlayer(RpcTarget.All , E_PTDefine.PT_HITMINIONANDMAGIC , baseStatId, damage, delay);
                NetSendPlayer(GameProtocol.HIT_DAMAGE_MINION_RELAY , NetworkManager.Get().OtherUID , baseStatId , damage, delay);
            }*/
        }

        public void HitMinionDamage(bool other , int minionId , float damage , float delay )
        {
            if (other == true)
            {
                if (InGameManager.IsNetwork )
                {
                    NetSendPlayer(GameProtocol.HIT_DAMAGE_MINION_RELAY , NetworkManager.Get().OtherUID , minionId , damage, delay);
                }
                targetPlayer.HitDamageMinionAndMagic(minionId, damage, delay);
            }
            else
            {
                if (InGameManager.IsNetwork)
                {
                    NetSendPlayer(GameProtocol.HIT_DAMAGE_MINION_RELAY , NetworkManager.Get().UserUID , minionId , damage, delay);
                }
                HitDamageMinionAndMagic(minionId, damage, delay);
            }    
        }

        
        private void MinionDestroyCallback(Minion minion)
        {
            if (InGameManager.IsNetwork && isMine)
            {
                NetSendPlayer(GameProtocol.REMOVE_MINION_RELAY , NetworkManager.Get().UserUID , minion.id);
            }
            
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
            if (InGameManager.IsNetwork && isMine)
            {
                NetSendPlayer(GameProtocol.DESTROY_MINION_RELAY , NetworkManager.Get().UserUID , baseStatId);
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
            if (InGameManager.IsNetwork)
            {
                NetSendPlayer(GameProtocol.REMOVE_MAGIC_RELAY , NetworkManager.Get().UserUID , magic.id );
            }
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
            if (InGameManager.IsNetwork)
            {
                NetSendPlayer(GameProtocol.DESTROY_MAGIC_RELAY , NetworkManager.Get().UserUID , baseStatId );
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


        public void HealerMinion(int baseStatId, float heal)
        {
            if (InGameManager.IsNetwork && isMine)
            {
                NetSendPlayer(GameProtocol.HEAL_MINION_RELAY , NetworkManager.Get().UserUID , baseStatId , heal);
            }
            
            HealMinion(baseStatId, heal);
        }

        public void MinionAniTrigger(int baseStatId , string aniName)
        {
            if (InGameManager.IsNetwork && isMine)
            {
                NetSendPlayer(GameProtocol.SET_MINION_ANIMATION_TRIGGER_RELAY , NetworkManager.Get().UserUID , baseStatId , aniName);
            }
            
            SetMinionAnimationTrigger(baseStatId, aniName);
        }

        public void ActionFireBallBomb(int bastStatId)
        {
            if (InGameManager.IsNetwork && isMine)
            {
                NetSendPlayer(GameProtocol.FIRE_BALL_BOMB_RELAY , NetworkManager.Get().UserUID , bastStatId );
            }
            FireballBomb(bastStatId);
        }

        public void ActionMineBomb(int baseStatId)
        {
            if (InGameManager.IsNetwork && isMine)
            {
                NetSendPlayer(GameProtocol.MINE_BOMB_RELAY , NetworkManager.Get().UserUID , baseStatId );
            }
            
            MineBomb(baseStatId);
        }

        public void ActionSetMagicTarget(int baseStatId , params object[] param)
        {
            if (param.Length > 1)
            {
                if (InGameManager.IsNetwork && isMine)
                {
                    int chX = (int) ((float) param[0] * Global.g_networkBaseValue);
                    int chZ = (int) ((float) param[1] * Global.g_networkBaseValue);
                    NetSendPlayer(GameProtocol.SET_MAGIC_TARGET_POS_RELAY , NetworkManager.Get().UserUID , baseStatId , chX , chZ );
                }
                SetMagicTarget(baseStatId, (float) param[0], (float) param[1]);
            }
            else // id 만 들어잇을테니..
            {
                if (InGameManager.IsNetwork && isMine)
                {
                    NetSendPlayer(GameProtocol.SET_MAGIC_TARGET_ID_RELAY , NetworkManager.Get().UserUID , baseStatId ,(int) param[0]);    
                }
                SetMagicTarget(baseStatId, (int) param[0]);
            }
        }
        
        public void ActionFireArrow(Vector3 startPos , int targetId , float damage , float moveSpeed)
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

        public void ActionSturn(bool other , int baseStatId, float duration)
        {
            int chDur = (int) (duration * Global.g_networkBaseValue);
            if (other == true)
            {
                targetPlayer.SturnMinion(baseStatId , duration);
                if (InGameManager.IsNetwork )
                {
                    NetSendPlayer(GameProtocol.STURN_MINION_RELAY, NetworkManager.Get().OtherUID, baseStatId, chDur);
                }
            }
            else
            {
                SturnMinion(baseStatId , duration);
                if (InGameManager.IsNetwork )
                {
                    NetSendPlayer(GameProtocol.STURN_MINION_RELAY, NetworkManager.Get().UserUID, baseStatId, chDur);
                }
            }
        }

        public void ActionRocketBomb(int baseStatId)
        {
            if (InGameManager.IsNetwork && isMine)
            {
                NetSendPlayer(GameProtocol.ROCKET_BOMB_RELAY, NetworkManager.Get().UserUID, baseStatId);
            }
            RocketBomb(baseStatId);
        }

        public void ActionIceBallBomb(int baseStatId)
        {
            if (InGameManager.IsNetwork && isMine)
            {
                NetSendPlayer(GameProtocol.ICE_BOMB_RELAY, NetworkManager.Get().UserUID, baseStatId);
            }
            IceballBomb(baseStatId);
        }

        public void ActionFireCannonBall(Vector3 shootPos , Vector3 targetPos , float damage , float range )
        {
            if (InGameManager.IsNetwork && isMine)
            {
                int chDamage = (int)(damage * Global.g_networkBaseValue);
                int chRange = (int)(range * Global.g_networkBaseValue);
                
                NetSendPlayer(GameProtocol.FIRE_CANNON_BALL_RELAY, NetworkManager.Get().UserUID, shootPos , targetPos , chDamage , chRange);
            }
            FireCannonBall(shootPos, targetPos, damage, range);
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

        }

        public void ActionFireManFire(int baseStatId)
        {
            if (InGameManager.IsNetwork && isMine)
            {
                NetSendPlayer(GameProtocol.FIRE_MAN_FIRE_RELAY, NetworkManager.Get().UserUID, baseStatId);
            }
            FiremanFire(baseStatId);
        }

        public void ActionActivePoolObject(string objName , Vector3 startPos , Quaternion rotate , Vector3 scale)
        {
            if (InGameManager.IsNetwork && isMine)
            {
                NetSendPlayer(GameProtocol.ACTIVATE_POOL_OBJECT_RELAY, NetworkManager.Get().UserUID, objName , startPos , scale , rotate);
            }
            ActivationPoolObject(objName , startPos , rotate , scale);
        }

        public void ActionCloacking(int bastStatId, bool isCloacking)
        {
            if (InGameManager.IsNetwork && isMine)
            {
                NetSendPlayer(GameProtocol.MINION_CLOACKING_RELAY, NetworkManager.Get().UserUID, bastStatId , isCloacking);
            }
            Cloacking(bastStatId, isCloacking);
        }

        public void ActionFlagOfWar(int bastStatId, bool isIn, float factor)
        {
            if (InGameManager.IsNetwork && isMine)
            {
                int convFactor = (int) (factor * Global.g_networkBaseValue);
                NetSendPlayer(GameProtocol.MINION_FOG_OF_WAR_RELAY, NetworkManager.Get().UserUID, bastStatId ,convFactor , isIn );
            }
            FlagOfWar(bastStatId , isIn , factor);
        }

        public void ActionSendMsg(int bastStatId, string msgFunc, int targetId = 1)
        {
            if (InGameManager.IsNetwork && isMine)
            {
                if (targetId == -1)
                {
                    NetSendPlayer(GameProtocol.SEND_MESSAGE_VOID_RELAY, NetworkManager.Get().UserUID, bastStatId ,msgFunc );
                }
                else
                {
                    NetSendPlayer(GameProtocol.SEND_MESSAGE_PARAM1_RELAY, NetworkManager.Get().UserUID, bastStatId ,msgFunc , targetId );
                }
            }
            MinionSendMessage(bastStatId, msgFunc, targetId);
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

        public void ActionMinionTarget(int baseStatId, int targetId)
        {
            if (InGameManager.IsNetwork && isMine)
            {
                NetSendPlayer(GameProtocol.SET_MINION_TARGET_RELAY, NetworkManager.Get().UserUID, baseStatId , targetId );
            }
            SetMinionTarget(baseStatId , targetId);
        }

        public void ActionMinionScareCrow(bool other , int targetId, float eyeLevel)
        {
            int chEyeLv = (int)(eyeLevel * Global.g_networkBaseValue);
            if (other == true)
            {
                targetPlayer.ScareCrow(targetId , eyeLevel);

                if (InGameManager.IsNetwork )
                {
                    NetSendPlayer(GameProtocol.STURN_MINION_RELAY, NetworkManager.Get().OtherUID, targetId, chEyeLv);
                }
            }
            else
            {
                ScareCrow(targetId , eyeLevel);
                if (InGameManager.IsNetwork )
                {
                    NetSendPlayer(GameProtocol.STURN_MINION_RELAY, NetworkManager.Get().UserUID, targetId, chEyeLv);
                }
            }
        }

        public void ActionLayzer(int baseStatId, int[] arrTarget)
        {
            if (InGameManager.IsNetwork && isMine)
            {
                NetSendPlayer(GameProtocol.SET_MINION_TARGET_RELAY, NetworkManager.Get().UserUID, baseStatId , arrTarget );
            }
            LayzerMinion(baseStatId, arrTarget);
        }
        #endregion
        
        
        #region remove & destroy
        //
        private void RemoveMinion(int baseStatId)
        {
            listMinion.Remove(listMinion.Find(minion => minion.id == baseStatId));
        }

        private void RemoveMagic(int baseStatId)
        {
            listMagic.Remove(listMagic.Find(magic => magic.id == baseStatId));
        }
        
        private void DestroyMinion(int baseStatId)
        {
            //UnityEngine.Debug.Log(listMinion.Find(minion => minion.id == baseStatId)?"TRUE" : "FALSE");
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
        
        public void SetMinionAnimationTrigger(int baseStatId, string trigger)
        {
            var m = listMinion.Find(minion => minion.id == baseStatId);
            if (m != null && m.animator != null)
            {
                m.animator.SetTrigger(trigger);
            }
        }
        
        public void FireballBomb(int baseStatId)
        {
            ((Fireball)listMagic.Find(magic => magic.id == baseStatId))?.Bomb();
        }
        
        public void MineBomb(int baseStatId)
        {
            ((Mine)listMagic.Find(magic => magic.id == baseStatId))?.Bomb();
        }
        
        public void SetMagicTarget(int baseStatId, int targetId)
        {
            StartCoroutine(SetMagicTargetCoroutine(baseStatId, targetId));
        }

        private IEnumerator SetMagicTargetCoroutine(int baseStatId, int targetId)
        {
            while (listMagic.Find(magic => magic.id == baseStatId) == null) yield return null;
            listMagic.Find(magic => magic.id == baseStatId)?.SetTarget(targetId);
        }

        public void SetMagicTarget(int baseStatId, float x, float z)
        {
            StartCoroutine(SetMagicTargetCoroutine(baseStatId, x, z));
        }

        private IEnumerator SetMagicTargetCoroutine(int baseStatId, float x, float z)
        {
            while (listMagic.Find(magic => magic.id == baseStatId) == null) yield return null;
            listMagic.Find(magic => magic.id == baseStatId)?.SetTarget(x, z);
        }
        
        public void SturnMinion(int baseStatId, float duration)
        {
            listMinion.Find(minion => minion.id == baseStatId)?.Sturn(duration);
        }

        public void RocketBomb(int baseStatId)
        {
            ((Rocket)listMagic.Find(magic => magic.id == baseStatId))?.Bomb();
        }
        
        public void IceballBomb(int baseStatId)
        {
            ((Iceball)listMagic.Find(magic => magic.id == baseStatId))?.Bomb();
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

        public void SetMinionTarget(int bastStatId , int targetId )
        {
            listMinion.Find(minion => minion.id == bastStatId).target = targetPlayer.GetBaseStatFromId(targetId);
        }
        
        public void MinionSendMessage(int bastStatId, string msgFunc , int targetId = -1 )
        {
            if (targetId == -1)
            {
                listMinion.Find(m => m.id == bastStatId)?.SendMessage(msgFunc, SendMessageOptions.DontRequireReceiver);
                listMagic.Find(m => m.id == bastStatId)?.SendMessage(msgFunc, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                listMinion.Find(m => m.id == bastStatId)?.SendMessage(msgFunc, targetId, SendMessageOptions.DontRequireReceiver);
                listMagic.Find(m => m.id == bastStatId)?.SendMessage(msgFunc, targetId, SendMessageOptions.DontRequireReceiver);
            }
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
        
        public void FireCannonBall(Vector3 startPos, Vector3 targetPos, float damage, float splashRange)
        {
            var b = PoolManager.instance.ActivateObject<CannonBall>("CannonBall", startPos);
            if (b != null)
            {
                b.transform.rotation = Quaternion.identity;
                b.controller = this;
                b.Initialize(targetPos, damage, splashRange, isMine, isBottomPlayer);
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

        public void ActivationPoolObject(string objectName , Vector3 startPos , Quaternion rotate , Vector3 scale )
        {
            Transform ts = PoolManager.instance.ActivateObject(objectName, startPos);
            if (ts != null)
            {
                ts.rotation = rotate;
                ts.localScale = scale;
            }
        }
        #endregion
        
        
        #region sync minion --

        public IEnumerator SyncMinionStatus()
        {
            
            if (InGameManager.IsNetwork == false)
                yield break;
            
            while (true)
            {
                yield return new WaitForSeconds(0.2f);

                if (InGameManager.IsNetwork && isMine)
                {
                    if (listMinion.Count > 0)
                    {
                        byte minionCount = (byte) listMinion.Count;
                        MsgVector3[] msgMinPos = new MsgVector3[100];
                    
                        for (int i = 0; i < listMinion.Count; i++)
                        {
                            //UnityEngine.Debug.Log(listMinion[i].rb.position);
                            msgMinPos[i] = NetworkManager.Get().VectorToMsg(listMinion[i].rb.position);
                        }
                    
                        NetSendPlayer(GameProtocol.MINION_STATUS_RELAY, NetworkManager.Get().UserUID, minionCount , msgMinPos );
                    }
                }
            }
            
        }

        public void SyncMinion(byte minionCount , MsgVector3[] msgPoss)
        {
            
            for (var i = 0; i < minionCount && i < listMinion.Count; i++)
            {
                Vector3 chPos = NetworkManager.Get().MsgToVector(msgPoss[i]);
                listMinion[i].SetNetworkValue(chPos);
            }
        }
        #endregion


        #region net packet player

        public void NetSendPlayer(GameProtocol protocol, params object[] param)
        {
            if (InGameManager.IsNetwork == true)
            {
                NetworkManager.Get().Send(protocol, param);
            }
            
            // 네트워크는 이렇게 하면안된다...파라메터에 uid 부터 들어가서...변수가 틀려진다
            /*else
            {
                NetRecvPlayer(protocol, param);
            }*/
        }
        
        public void NetRecvPlayer(GameProtocol protocol, params object[] param)
        {
            switch (protocol)
            {
                case GameProtocol.HIT_DAMAGE_ACK:
                {
                    // 기본적으로 타워가 맞은것을 상대방이 맞앗다고 보내는거니까...
                    MsgHitDamageAck damageack = (MsgHitDamageAck) param[0];

                    //float calDamage = (float)damageack.Damage / Global.g_networkBaseValue;
                    //targetPlayer.HitDamage(calDamage);
                    
                    break;
                }
                case GameProtocol.HIT_DAMAGE_NOTIFY:
                {
                    MsgHitDamageNotify damagenoti = (MsgHitDamageNotify) param[0];

                    float calDamage = damagenoti.Damage /  Global.g_networkBaseValue;
                    if (NetworkManager.Get().UserUID == damagenoti.PlayerUId)
                    {
                        HitDamage(calDamage);
                    }
                    else if (NetworkManager.Get().OtherUID == damagenoti.PlayerUId )
                    {
                        targetPlayer.HitDamage(calDamage);
                    }
                    
                    break;
                }
                case GameProtocol.REMOVE_MINION_RELAY:
                {
                    MsgRemoveMinionRelay ralayremove = (MsgRemoveMinionRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == ralayremove.PlayerUId )
                        RemoveMinion(ralayremove.Id);
                    else if (NetworkManager.Get().OtherUID == ralayremove.PlayerUId )
                        targetPlayer.RemoveMinion(ralayremove.Id);
                    
                    break;
                }
                case GameProtocol.HIT_DAMAGE_MINION_RELAY:
                {
                    MsgHitDamageMinionRelay hitminion = (MsgHitDamageMinionRelay) param[0];
                    float damage = hitminion.Damage / Global.g_networkBaseValue;
                    float delay = hitminion.Delay / Global.g_networkBaseValue;
                    
                    if (NetworkManager.Get().UserUID == hitminion.PlayerUId)
                        HitDamageMinionAndMagic(hitminion.Id, damage, delay);
                    else if (NetworkManager.Get().OtherUID == hitminion.PlayerUId )
                        targetPlayer.HitDamageMinionAndMagic(hitminion.Id, damage, delay);
                    
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
                    
                    break;
                }
                case GameProtocol.HEAL_MINION_RELAY:
                {
                    MsgHealMinionRelay healrelay = (MsgHealMinionRelay) param[0];

                    float serverHealVal =  (float)healrelay.Heal / Global.g_networkBaseValue;
                    if (NetworkManager.Get().UserUID == healrelay.PlayerUId)
                        HealMinion(healrelay.Id, serverHealVal);
                    else if (NetworkManager.Get().OtherUID == healrelay.PlayerUId )
                        targetPlayer.HealMinion(healrelay.Id, serverHealVal);
                    
                    break;
                }
                case GameProtocol.SET_MINION_ANIMATION_TRIGGER_RELAY:
                {
                    MsgSetMinionAnimationTriggerRelay anirelay = (MsgSetMinionAnimationTriggerRelay) param[0];
                    
                    //UnityEngine.Debug.Log(anirelay.Trigger);
                    if (NetworkManager.Get().UserUID == anirelay.PlayerUId)
                        SetMinionAnimationTrigger(anirelay.Id, anirelay.Trigger);
                    else if (NetworkManager.Get().OtherUID == anirelay.PlayerUId )
                        targetPlayer.SetMinionAnimationTrigger(anirelay.Id, anirelay.Trigger);
                    break;
                }
                case GameProtocol.FIRE_ARROW_RELAY:
                {
                    MsgFireArrowRelay arrrelay = (MsgFireArrowRelay) param[0];
                    
                    //Dir Damage MoveSpeed
                    Vector3 sPos = NetworkManager.Get().MsgToVector(arrrelay.Dir);
                    
                    float calDamage = (float)arrrelay.Damage / Global.g_networkBaseValue;
                    float calSpeed = (float)arrrelay.MoveSpeed / Global.g_networkBaseValue;
                    
                    if (NetworkManager.Get().UserUID == arrrelay.PlayerUId)
                        FireArrow(sPos , arrrelay.Id, calDamage , calSpeed);
                    else if (NetworkManager.Get().OtherUID == arrrelay.PlayerUId )
                        targetPlayer.FireArrow(sPos , arrrelay.Id, calDamage , calSpeed);
                    break;
                }
                case GameProtocol.REMOVE_MAGIC_RELAY:
                {
                    MsgRemoveMagicRelay remrelay = (MsgRemoveMagicRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == remrelay.PlayerUId)
                        RemoveMagic(remrelay.Id);
                    else if (NetworkManager.Get().OtherUID == remrelay.PlayerUId )
                        targetPlayer.RemoveMagic(remrelay.Id);
                    
                    break;
                }
                case GameProtocol.FIRE_BALL_BOMB_RELAY:
                {
                    MsgFireballBombRelay fbrelay = (MsgFireballBombRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == fbrelay.PlayerUId)
                        FireballBomb(fbrelay.Id);
                    else if (NetworkManager.Get().OtherUID == fbrelay.PlayerUId )
                        targetPlayer.FireballBomb(fbrelay.Id);
                    
                    break;
                }
                case GameProtocol.MINE_BOMB_RELAY:
                {
                    MsgMineBombRelay mbrelay = (MsgMineBombRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == mbrelay.PlayerUId)
                        MineBomb(mbrelay.Id);
                    else if (NetworkManager.Get().OtherUID == mbrelay.PlayerUId )
                        targetPlayer.MineBomb(mbrelay.Id);
                    break;
                }
                case GameProtocol.SET_MAGIC_TARGET_ID_RELAY:
                {
                    MsgSetMagicTargetIdRelay smtidrelay = (MsgSetMagicTargetIdRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == smtidrelay.PlayerUId)
                        SetMagicTarget(smtidrelay.Id, smtidrelay.TargetId);
                    else if (NetworkManager.Get().OtherUID == smtidrelay.PlayerUId )
                        targetPlayer.SetMagicTarget(smtidrelay.Id, smtidrelay.TargetId);
                    
                    break;
                }
                case GameProtocol.SET_MAGIC_TARGET_POS_RELAY:
                {
                    MsgSetMagicTargetRelay smtrelay = (MsgSetMagicTargetRelay) param[0];
                    
                    float chX =  (float)smtrelay.X / Global.g_networkBaseValue;
                    float chZ =  (float)smtrelay.Z / Global.g_networkBaseValue;
                    
                    if (NetworkManager.Get().UserUID == smtrelay.PlayerUId)
                        SetMagicTarget(smtrelay.Id, chX , chZ);
                    else if (NetworkManager.Get().OtherUID == smtrelay.PlayerUId )
                        targetPlayer.SetMagicTarget(smtrelay.Id, chX , chZ);
                    
                    break;
                }

                case GameProtocol.STURN_MINION_RELAY:
                {
                    MsgSturnMinionRelay sturelay = (MsgSturnMinionRelay) param[0];
                    
                    float chDur = (float)sturelay.SturnTime / Global.g_networkBaseValue;
                    
                    if (NetworkManager.Get().UserUID == sturelay.PlayerUId)
                        SturnMinion(sturelay.Id, chDur);
                    else if (NetworkManager.Get().OtherUID == sturelay.PlayerUId )
                        targetPlayer.SturnMinion(sturelay.Id, chDur);
                    
                    break;
                }
                case GameProtocol.DESTROY_MAGIC_RELAY:
                {
                    MsgDestroyMagicRelay desmagic = (MsgDestroyMagicRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == desmagic.PlayerUId)
                        DestroyMagic(desmagic.BaseStatId);
                    else if (NetworkManager.Get().OtherUID == desmagic.PlayerUId )
                        targetPlayer.DestroyMagic(desmagic.BaseStatId);
                    
                    break;
                }
                case GameProtocol.ROCKET_BOMB_RELAY:
                {
                    MsgRocketBombRelay rockrelay = (MsgRocketBombRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == rockrelay.PlayerUId)
                        DestroyMagic(rockrelay.Id);
                    else if (NetworkManager.Get().OtherUID == rockrelay.PlayerUId )
                        targetPlayer.DestroyMagic(rockrelay.Id);
                    
                    break;
                }
                case GameProtocol.ICE_BOMB_RELAY:
                {
                    MsgIceBombRelay icerelay = (MsgIceBombRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == icerelay.PlayerUId)
                        IceballBomb(icerelay.Id);
                    else if (NetworkManager.Get().OtherUID == icerelay.PlayerUId )
                        targetPlayer.IceballBomb(icerelay.Id);
                    
                    break;
                }
                case GameProtocol.FIRE_CANNON_BALL_RELAY:
                {
                    MsgFireCannonBallRelay fcannonrelay = (MsgFireCannonBallRelay) param[0];

                    Vector3 startPos = NetworkManager.Get().MsgToVector(fcannonrelay.ShootPos);
                    Vector3 targetPos = NetworkManager.Get().MsgToVector(fcannonrelay.TargetPos);
                    float chDamage = (float)fcannonrelay.Power / Global.g_networkBaseValue;
                    float chRange = (float)fcannonrelay.Range / Global.g_networkBaseValue;
        
                    if (NetworkManager.Get().UserUID == fcannonrelay.PlayerUId)
                        FireCannonBall(startPos, targetPos, chDamage, chRange);
                    else if (NetworkManager.Get().OtherUID == fcannonrelay.PlayerUId )
                        targetPlayer.FireCannonBall(startPos, targetPos, chDamage, chRange);
                    
                    break;
                }
                case GameProtocol.FIRE_SPEAR_RELAY:
                {
                    MsgFireSpearRelay spearrelay = (MsgFireSpearRelay) param[0];

                    Vector3 startPos = NetworkManager.Get().MsgToVector(spearrelay.ShootPos);
                    float chDamage = (float)spearrelay.Power / Global.g_networkBaseValue;
                    float chSpeed = (float)spearrelay.MoveSpeed /  Global.g_networkBaseValue;
                    
                    if (NetworkManager.Get().UserUID == spearrelay.PlayerUId)
                        FireSpear(startPos, spearrelay.TargetId, chDamage, chSpeed);
                    else if (NetworkManager.Get().OtherUID == spearrelay.PlayerUId )
                        targetPlayer.FireSpear(startPos, spearrelay.TargetId, chDamage, chSpeed);
                    
                    break;
                }
                case GameProtocol.FIRE_MAN_FIRE_RELAY:
                {
                    MsgFireManFireRelay firerelay = (MsgFireManFireRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == firerelay.PlayerUId)
                        FiremanFire(firerelay.Id);
                    else if (NetworkManager.Get().OtherUID == firerelay.PlayerUId )
                        targetPlayer.FiremanFire(firerelay.Id);
                    
                    break;
                }
                case GameProtocol.ACTIVATE_POOL_OBJECT_RELAY:
                {
                    MsgActivatePoolObjectRelay actrelay = (MsgActivatePoolObjectRelay) param[0];
                    
                    Vector3 stPos = NetworkManager.Get().MsgToVector(actrelay.HitPos);
                    Vector3 localScale = NetworkManager.Get().MsgToVector(actrelay.LocalScale);
                    Quaternion rotate = NetworkManager.Get().MsgToQuaternion(actrelay.Rotation);

                    if (NetworkManager.Get().UserUID == actrelay.PlayerUId)
                        ActivationPoolObject(actrelay.PoolName, stPos, rotate, localScale);
                    else if (NetworkManager.Get().OtherUID == actrelay.PlayerUId )
                        targetPlayer.ActivationPoolObject(actrelay.PoolName, stPos, rotate, localScale);

                    break;
                }
                case GameProtocol.MINION_CLOACKING_RELAY:
                {
                    MsgMinionCloackingRelay cloackrelay = (MsgMinionCloackingRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == cloackrelay.PlayerUId)
                        Cloacking(cloackrelay.Id, cloackrelay.IsCloacking);
                    else if (NetworkManager.Get().OtherUID == cloackrelay.PlayerUId )
                        targetPlayer.Cloacking(cloackrelay.Id, cloackrelay.IsCloacking);
                    
                    
                    break;
                }
                case GameProtocol.MINION_FOG_OF_WAR_RELAY:
                {
                    MsgMinionFogOfWarRelay flagrelay = (MsgMinionFogOfWarRelay) param[0];
                    
                    float convFactor = (float)flagrelay.Effect / Global.g_networkBaseValue;
                    
                    if (NetworkManager.Get().UserUID == flagrelay.PlayerUId)
                        FlagOfWar(flagrelay.BaseStatId , flagrelay.IsFogOfWar , convFactor);
                    else if (NetworkManager.Get().OtherUID == flagrelay.PlayerUId )
                        targetPlayer.FlagOfWar(flagrelay.BaseStatId , flagrelay.IsFogOfWar , convFactor);
                    
                    break;
                }
                case GameProtocol.SEND_MESSAGE_VOID_RELAY:
                {
                    MsgSendMessageVoidRelay voidmsg = (MsgSendMessageVoidRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == voidmsg.PlayerUId)
                        MinionSendMessage(voidmsg.Id , voidmsg.Message);
                    else if (NetworkManager.Get().OtherUID == voidmsg.PlayerUId )
                        targetPlayer.MinionSendMessage(voidmsg.Id , voidmsg.Message);
                    
                    break;
                }
                case GameProtocol.SEND_MESSAGE_PARAM1_RELAY:
                {
                    MsgSendMessageParam1Relay paramrelay = (MsgSendMessageParam1Relay) param[0];
                    
                    if (NetworkManager.Get().UserUID == paramrelay.PlayerUId)
                        MinionSendMessage(paramrelay.Id , paramrelay.Message , paramrelay.TargetId);
                    else if (NetworkManager.Get().OtherUID == paramrelay.PlayerUId )
                        targetPlayer.MinionSendMessage(paramrelay.Id , paramrelay.Message , paramrelay.TargetId);
                    
                    break;
                }
                case GameProtocol.NECROMANCER_BULLET_RELAY:
                {
                    MsgNecromancerBulletRelay necrorelay = (MsgNecromancerBulletRelay) param[0];

                    Vector3 shootPos = NetworkManager.Get().MsgToVector(necrorelay.ShootPos);
                        
                    if (NetworkManager.Get().UserUID == necrorelay.PlayerUId)
                        FireNecromancerBullet(shootPos , necrorelay.TargetId , necrorelay.Power , necrorelay.BulletMoveSpeed );
                    else if (NetworkManager.Get().OtherUID == necrorelay.PlayerUId )
                        targetPlayer.FireNecromancerBullet(shootPos , necrorelay.TargetId , necrorelay.Power , necrorelay.BulletMoveSpeed );
                    
                    break;
                }
                case GameProtocol.SET_MINION_TARGET_RELAY:
                {
                    MsgSetMinionTargetRelay targetrelay = (MsgSetMinionTargetRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == targetrelay.PlayerUId)
                        SetMinionTarget(targetrelay.Id , targetrelay.TargetId);
                    else if (NetworkManager.Get().OtherUID == targetrelay.PlayerUId )
                        targetPlayer.SetMinionTarget(targetrelay.Id , targetrelay.TargetId);
                    
                    break;
                }
                case GameProtocol.SCARECROW_RELAY:
                {
                    MsgScarecrowRelay scarelay = (MsgScarecrowRelay) param[0];
                    
                    float chEyeLv = (float)scarelay.EyeLevel / Global.g_networkBaseValue;
                    if (NetworkManager.Get().UserUID == scarelay.PlayerUId)
                        ScareCrow(scarelay.BaseStatId , chEyeLv);
                    else if (NetworkManager.Get().OtherUID == scarelay.PlayerUId )
                        targetPlayer.ScareCrow(scarelay.BaseStatId , chEyeLv);
                    
                    break;
                }
                case GameProtocol.LAYZER_TARGET_RELAY:
                {
                    MsgLazerTargetRelay lazerrelay = (MsgLazerTargetRelay) param[0];
                    
                    if (NetworkManager.Get().UserUID == lazerrelay.PlayerUId)
                        LayzerMinion(lazerrelay.Id, lazerrelay.TargetIdArray);
                    else if (NetworkManager.Get().OtherUID == lazerrelay.PlayerUId )
                        targetPlayer.LayzerMinion(lazerrelay.Id, lazerrelay.TargetIdArray);
                    
                    
                    break;
                }
                case GameProtocol.MINION_STATUS_RELAY:
                {
                    MsgMinionStatusRelay statusrelay = (MsgMinionStatusRelay) param[0];

                    if (NetworkManager.Get().OtherUID == statusrelay.PlayerUId)
                        targetPlayer.SyncMinion(statusrelay.PosIndex, statusrelay.Pos);
                    
                    break;
                }
                
            }
        }

        #endregion
        
        
        
        
        
        
        
        
        #region dice rpc
        
        //////////////////////////////////////////////////////////////////////
        // Dice RPCs
        //////////////////////////////////////////////////////////////////////

        public void PushMinion(int baseStatId, Vector3 dir, float pushPower)
        {
            listMinion.Find(minion => minion.id == baseStatId)?.Push(dir, pushPower);
        }

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
                    float delay = (float) param[2];
                    HitDamageMinionAndMagic(baseIDhit, damage, delay);
                    break;
                case E_PTDefine.PT_HITDAMAGE:            //
                    float damageH = (float) param[0];
                    float delayH = (float) param[1];
                    HitDamage(damageH, delayH);
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
                case E_PTDefine.PT_FIRECANNONBALL:            //
                    FireCannonBall((Vector3) param[0], (Vector3) param[1], (float) param[2], (float) param[3]);
                    break;
                case E_PTDefine.PT_FIREARROW:            //
                    FireArrow((Vector3) param[0], (int) param[1], (float) param[2], (float) param[3]);
                    break;
                case E_PTDefine.PT_FIRESPEAR:            //
                    FireSpear((Vector3) param[0], (int) param[1], (float) param[2], (float) param[3]);
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
                case E_PTDefine.PT_NECROMANCERBULLET:            //
                    FireNecromancerBullet((Vector3)param[0] , (int)param[1] , (float)param[2], (float)param[3]);
                    break;
                case E_PTDefine.PT_SETMINIONTARGET:            //
                    listMinion.Find(minion => minion.id == (int) param[0]).target = targetPlayer.GetBaseStatFromId((int) param[1]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(ptID), ptID, null);
            }
        }
        #endregion
        */
    }
}
