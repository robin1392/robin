using Cysharp.Threading.Tasks;
using ED;
using Mirage;
using MirageTest.Scripts.Messages;
using RandomWarsProtocol;
using RandomWarsResource.Data;
using UnityEngine;
using Channel = Mirage.Channel;

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
        public bool isMovable = true;
        
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
            if (baseStat is Minion minion)
            {
                minion.currentHealth = 0;
                minion._poolObjectAutoDeactivate.Deactive();    
            }
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
                baseStat.currentHealth = health;
                baseStat.maxHealth = maxHealth;
                playerController.isMine = IsLocalPlayerActor;
                var isBottom = team == GameConstants.BottomCamp;
                playerController.isBottomPlayer = isBottom;
                playerController.ChangeLayer(isBottom);
                isMovable = false;
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
                    m.Initialize(null);
                    baseStat = m;
                    m.actorProxy = this;
                    m.controller = (Client as RWNetworkClient).GetTower(ownerTag);
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
                    var pool = PoolManager.instance.data.listPool.Find(data => data.obj.name == "Effect_SpawnLine");
                    PoolManager.instance.AddPool(pool.obj, 1);    
                    lr = PoolManager.instance.ActivateObject<LineRenderer>("Effect_SpawnLine", Vector3.zero);
                }
                
                if (lr != null)
                {
                    lr.SetPositions(new Vector3[2] {dicePos, m.ts_HitPos.position});
                    lr.startColor = FileHelper.GetColor(diceInfo.color);
                    lr.endColor = FileHelper.GetColor(diceInfo.color);
                }
            }

            baseStat.ActorProxy = this;

            var client = Client as RWNetworkClient;
            EnableClientCombatLogic(client.IsPlayingAI);

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
            if (baseStat.text_Health != null)
            {
                baseStat.text_Health.text = $"{Mathf.CeilToInt(health)}";   
            }
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
            minion.SetControllEnable(b);

            minion.GetComponent<Collider>().enabled = b;
            minion.rb.detectCollisions = b;
                
            isPlayingAI = b;
        }

        public void HitDamage(float mPower)
        {
            HitDamageOnServer(power);
        }
        
        [ServerRpc(requireAuthority = false)]
        public void HitDamageOnServer(float mPower)
        {
            health -= mPower;
            if (health < 0)
            {
                ServerObjectManager.Destroy(gameObject);
                return;
            }

            HitDamageOnClient();
        }

        [ClientRpc]
        public void HitDamageOnClient()
        {
            if (baseStat == null)
            {
                return;
            }
            
            var obj = PoolManager.Get().ActivateObject("Effect_ArrowHit", baseStat.ts_HitPos.position);
            obj.rotation = Quaternion.identity;
            obj.localScale = Vector3.one * 0.6f;
            
            SoundManager.instance?.PlayRandom(Global.E_SOUND.SFX_MINION_HIT);
        }

        public void PlayAnimation(uint baseStatId, int aniEnum, uint targetId)
        {
            Client.SendAsync(new PlayAnimationRelayMessage()
            {
                actorNetId = baseStatId,
                aniId = aniEnum,
                targetNetId = targetId,
            }).Forget();
        }

        private MsgVector2 lastSend;
        public MsgVector2 lastRecieved;
        private void Update()
        {
            if (IsServer)
            {
                return;
            }

            if (!isMovable)
            {
                return;
            }
            
            if (baseStat == null)
            {
                return;
            }
            
            if (isPlayingAI)
            {
                var position = baseStat.transform.position;
                var converted = ConvertNetMsg.Vector3ToMsg(new Vector2(position.x, position.z));
                if (lastSend == null || !Equal(lastSend, converted))
                {
                    lastSend = converted;
                    Client.SendAsync(new PositionRelayMessage()
                    {
                        netId = NetId,
                        positionX = converted.X,
                        positionY = converted.Y,
                    }, Channel.Unreliable).Forget();
                }
            }
            else if(lastRecieved != null)
            {
                var position = ConvertNetMsg.MsgToVector3(lastRecieved);
                baseStat.transform.position = Vector3.Lerp(baseStat.transform.position, position, baseStat.moveSpeed * Time.deltaTime);
            }
        }
        
        bool Equal(MsgVector2 a, MsgVector2 b)
        {
            return a.X == b.X &&
                   a.Y == b.Y;
        }
    }
}