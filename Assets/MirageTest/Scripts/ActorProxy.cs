using System;
using ED;
using Mirage;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace MirageTest.Scripts
{
    public class ActorProxy : NetworkBehaviour
    {
        [SyncVar] public byte ownerTag;
        [SyncVar(hook = nameof(SetTeam))] public byte team;
        [SyncVar] public byte spawnSlot; //0 ~ 14 필드 슬롯 
        [SyncVar] public ActorType actorType;
        [SyncVar] public int dataId;

        [SyncVar(hook = nameof(SetHp))] public float health;
        [SyncVar] public float maxHealth;
        [SyncVar] public float power;
        [SyncVar] public float range;
        [SyncVar] public float effect;
        [SyncVar] public float attackSpeed;
        [SyncVar] public byte diceScale;
        [SyncVar] public byte ingameUpgradeLevel;

        public bool isPlayingAI;
        
        public bool IsLocalPlayerActor => (Client as RWNetworkClient).IsLocalPlayerTag(ownerTag);

        public BaseStat baseStat;

        private TDataDiceInfo _diceInfo;

        public TDataDiceInfo diceInfo
        {
            get
            {
                if (_diceInfo == null)
                {
                    TableManager.Get().DiceInfo.GetData(dataId, out _diceInfo);
                }

                return _diceInfo;
            }
        }

        private void Awake()
        {
            if (NetIdentity == null)
            {
                return;
            }
            
            NetIdentity.OnStartClient.AddListener(StartClient);
            NetIdentity.OnStopClient.AddListener(StopClient);
            NetIdentity.OnStartServer.AddListener(StartServer);
        }

        private void StopClient()
        {
            var client = Client as RWNetworkClient;
            client.RemoveActorProxy(this);
        }

        public void SetTeam(byte oldValue, byte newValue)
        {
            // renderer.material.color = newValue == 1? Color.blue : Color.red;
        }

        private void StartServer()
        {
        }

        private void StartClient()
        {
            var client = Client as RWNetworkClient;
            if (client.enableActor)
            {
                SpawnActor();
            }
            
            client.AddActorProxy(this);
        }

        void SpawnActor()
        {
            if (actorType == ActorType.Tower)
            {
                var towerPrefab = Resources.Load<PlayerController>("Tower/Player");
                
                var playerController = Instantiate(towerPrefab, transform);
                baseStat = playerController; 
                baseStat.id = NetId;
            }
            else if (actorType == ActorType.MinionFromDice)
            {
                //magicCastDelay 0.05f 로 시작해서 미니언 갯수 별로 0.6666666f 만큼 증가. 단일 프레임에 생성하지 않기 위한 트릭인듯한다. 처리 필요
                var spawnTransform = IsBottomCamp()
                    ? FieldManager.Get().GetBottomListTs(spawnSlot)
                    : FieldManager.Get().GetTopListTs(spawnSlot);
                var spawnPosition = spawnTransform.position;
                spawnPosition.x += Random.Range(-0.2f, 0.2f);
                spawnPosition.z += Random.Range(-0.2f, 0.2f);

                var m = PoolManager.instance.ActivateObject<Minion>(diceInfo.prefabName, spawnPosition, transform);
                if (m == null)
                {
                    //PoolManager.instance.AddPool(data.prefab, 1);
                    PoolManager.instance.AddPool(
                        FileHelper.LoadPrefab(diceInfo.prefabName, Global.E_LOADTYPE.LOAD_MINION, transform), 1);
                    //Debug.LogFormat("{0} Pool Added 1", data.prefabName);
                    m = PoolManager.instance.ActivateObject<Minion>(diceInfo.prefabName, spawnPosition, transform);
                }

                if (m != null)
                {
                    m.controller = RWNetworkClient.instance.GetTower(ownerTag);
                    m.castType = (DICE_CAST_TYPE)diceInfo.castType;
                    m.id = NetId;
                    m.isMine = IsLocalPlayerActor;
                    m.targetMoveType = (DICE_MOVE_TYPE)diceInfo.targetMoveType;
                    m.ChangeLayer(IsBottomCamp());
                    m.power = power;
                    m.maxHealth = maxHealth;
                    m.currentHealth = health;
                    m.effect = effect;
                    m.effectUpByUpgrade = diceInfo.effectUpgrade;
                    m.effectUpByInGameUp = diceInfo.effectInGameUp;
                    m.effectDuration = diceInfo.effectDuration;
                    m.attackSpeed = attackSpeed;
                    m.range = diceInfo.range;
                    m.searchRange = diceInfo.searchRange;
                    m.eyeLevel = diceScale;
                    m.ingameUpgradeLevel = ingameUpgradeLevel;
                    m.actorProxy = this;
                }

                var setting = UI_DiceField.Get().arrSlot[spawnSlot].ps.main;
                setting.startColor = FileHelper.GetColor(diceInfo.color);
                UI_DiceField.Get().arrSlot[spawnSlot].ps.Play();
                var dicePos = UI_DiceField.Get().arrSlot[spawnSlot].transform.position;
                if (!IsLocalPlayerActor)
                {
                    dicePos.x *= -1f;
                    dicePos.z *= -1f;
                }
                
                var lr = PoolManager.instance.ActivateObject<LineRenderer>("Effect_SpawnLine", Vector3.zero);
                if (lr == null)
                {
                    lr = PoolManager.instance.ActivateObject<LineRenderer>("Effect_SpawnLine", Vector3.zero);
                }
                
                if (lr != null)
                {
                    lr.SetPositions(new Vector3[2] {dicePos, m.ts_HitPos.position});
                    lr.startColor = FileHelper.GetColor(diceInfo.color);
                    lr.endColor = FileHelper.GetColor(diceInfo.color);
                }
            }
            
            

            // actorPathfinding.EnableAIPathfinding(false);
            // actorAi.graphOwner.StopBehaviour();
            // actorAi.graphOwner.enabled = false;
            // GetComponentInChildren<Collider>().enabled = false;
            // gameObject.SetActive(false);

            RefreshHpUI();
            SoundManager.instance?.Play(Global.E_SOUND.SFX_MINION_GENERATE);
        }

        bool IsBottomCamp()
        {
            return team == GameConstants.BottomCamp;
        }

        public void SetHp(float oldValue, float newValue)
        {
            RefreshHpUI();
        }

        void RefreshHpUI()
        {
            if (baseStat == null)
            {
                return;
            }

            baseStat.image_HealthBar.fillAmount = health / maxHealth;
            baseStat.text_Health.text = $"{Mathf.CeilToInt(health)}";
        }

        private void Update()
        {
            if (ServerObjectManager == null)
            {
                return;
            }

            // if (actor.IsAlive == false)
            // {
            //     ServerObjectManager.Destroy(gameObject);
            // }
        }

        public void EnableClientCombatLogic(bool b)
        {
            var minion = baseStat as Minion;
            if (minion == null)
            {
                return;
            }

            if (b)
            {
                minion.behaviourTreeOwner.behaviour.Resume();
            }
            else
            {
                minion.behaviourTreeOwner.behaviour.Pause();
            }

            isPlayingAI = b;
        }
    }
}