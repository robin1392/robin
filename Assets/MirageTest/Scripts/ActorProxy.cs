using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ED;
using Mirage;
using MirageTest.Scripts.Messages;
using Pathfinding;
using RandomWarsProtocol;
using RandomWarsResource.Data;
using UnityEngine;
using Channel = Mirage.Channel;
using Debug = ED.Debug;
using Random = UnityEngine.Random;

namespace MirageTest.Scripts
{
    public partial class ActorProxy : NetworkBehaviour
    {
        [SyncVar] public byte ownerTag;
        [SyncVar(hook = nameof(SetTeam))] public byte team;
        [SyncVar] public byte spawnSlot; //0 ~ 14 필드 슬롯 
        [SyncVar] public ActorType actorType;
        [SyncVar] public int dataId;

        [SyncVar(hook = nameof(SetHp))] public float currentHealth;
        [SyncVar] public float maxHealth;
        [SyncVar] public float power;
        [SyncVar] public float effect;
        [SyncVar] public float attackSpeed;
        [SyncVar] public byte diceScale;
        [SyncVar] public byte ingameUpgradeLevel;
        
        [SyncVar] public int ccState;

        public bool isPlayingAI;
        public bool isMovable = true;
        
        public bool IsLocalPlayerActor => (Client as RWNetworkClient).IsLocalPlayerTag(ownerTag);

        public BaseStat baseStat;

        private TDataDiceInfo _diceInfo;
        private bool stopped;
        
        public Seeker _seeker;
        public AIPath _aiPath;

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

        public void SetDiceInfo(TDataDiceInfo info)
        {
            _diceInfo = info;
            dataId = info.id;
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
            
            _seeker = GetComponent<Seeker>();
            _aiPath = GetComponent<AIPath>();
        }

        private void StopClient()
        {
            var client = Client as RWNetworkClient;
            client.RemoveActorProxy(this);
            if (baseStat is Minion minion)
            {
                currentHealth = 0;
                minion.OnDeath();
            }

            baseStat = null;
            stopped = true;
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
                playerController.isMine = IsLocalPlayerActor;
                playerController.isBottomPlayer = IsBottomCamp();
                playerController.ChangeLayer(IsBottomCamp());
                // SetColor(isBottomPlayer ? E_MaterialType.BOTTOM : E_MaterialType.TOP);
                isMovable = false;
            }
            else if (actorType == ActorType.MinionFromDice)
            {
                //magicCastDelay 0.05f 로 시작해서 미니언 갯수 별로 0.6666666f 만큼 증가. 단일 프레임에 생성하지 않기 위한 트릭인듯한다. 처리 필요

                if (diceInfo.castType == (int)DICE_CAST_TYPE.MINION || diceInfo.castType == (int)DICE_CAST_TYPE.HERO)
                {
                    PoolManager.instance.ActivateObject("particle_necromancer", transform.position);
                }
                    
                var m = PoolManager.instance.ActivateObject<Minion>(diceInfo.prefabName, Vector3.zero, transform);
                if (m == null)
                {
                    PoolManager.instance.AddPool(
                        FileHelper.LoadPrefab(diceInfo.prefabName, Global.E_LOADTYPE.LOAD_MINION, transform), 1);
                    //Debug.LogFormat("{0} Pool Added 1", data.prefabName);
                    m = PoolManager.instance.ActivateObject<Minion>(diceInfo.prefabName, Vector3.zero, transform);
                }

                if (m != null)
                {
                    m.transform.localPosition = Vector3.zero;
                    m.transform.localRotation = Quaternion.identity;
                    baseStat = m;
                    m.ActorProxy = this;
                    m.SetPathFinding(_seeker, _aiPath);
                    m.Initialize(null);
                    m.controller = (Client as RWNetworkClient).GetTower(ownerTag);
                    m.castType = (DICE_CAST_TYPE)diceInfo.castType;
                    m.id = NetId;
                    m.isMine = IsLocalPlayerActor;
                    m.targetMoveType = (DICE_MOVE_TYPE)diceInfo.targetMoveType;
                    m.isBottomPlayer = IsBottomCamp();
                    m.ChangeLayer(IsBottomCamp());
                }

                var setting = UI_DiceField.Get().arrSlot[spawnSlot].ps.main;
                setting.startColor = FileHelper.GetColor(diceInfo.color);
                
                var dicePos = UI_DiceField.Get().arrSlot[spawnSlot].transform.position;
                if (!IsLocalPlayerActor)
                {
                    dicePos.x *= -1f;
                    dicePos.z *= -1f;
                }
                else
                {
                    UI_DiceField.Get().arrSlot[spawnSlot].ps.Play();    
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

            baseStat.image_HealthBar.fillAmount = currentHealth / maxHealth;
            if (baseStat.text_Health != null)
            {
                baseStat.text_Health.text = $"{Mathf.CeilToInt(currentHealth)}";   
            }
        }

        public void EnableClientCombatLogic(bool b)
        {
            isPlayingAI = b;
            
            var minion = baseStat as Minion;
            if (minion == null)
            {
                return;
            }

            if (b)
            {
                minion.StartAI();
            }
            else
            {
                minion.StopAI();
            }

            minion.SetControllEnable(b);

            minion.GetComponent<Collider>().enabled = b;
        }
        
        public void HitDamage(float damage)
        {
            HitDamageOnServer(damage);
        }

        [ServerRpc(requireAuthority = false)]
        public void HitDamageOnServer(float damage)
        {
            ApplyDamage(damage);
        }

        public void DamageTo(BaseStat target)
        {
            DamageToOnServer(target.id);
        }
        
        [ServerRpc(requireAuthority = false)]
        public void DamageToOnServer(uint targetNetId)
        {
            var target = ServerObjectManager[targetNetId];
            if (target == null)
            {
                return;
            }
            
            target.GetComponent<ActorProxy>().ApplyDamage(power);
        }

        [Server]
        public void ApplyDamage(float damage)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            
            if (currentHealth <= 0)
            {
                ServerObjectManager.Destroy(gameObject);
                return;
            }

            DamagedOnClient(damage);
        }

        [ClientRpc]
        public void DamagedOnClient(float damage)
        {
            if (baseStat == null)
            {
                return;
            }
            
            baseStat.HitDamage(damage);
            var obj = PoolManager.Get().ActivateObject("Effect_ArrowHit", baseStat.ts_HitPos.position);
            obj.rotation = Quaternion.identity;
            obj.localScale = Vector3.one * 0.6f;
            
            SoundManager.instance?.PlayRandom(Global.E_SOUND.SFX_MINION_HIT);
        }
        
        public void HealTo(BaseStat target)
        {
            HealToOnServer(target.id);
        }
        
        [ServerRpc(requireAuthority = false)]
        public void HealToOnServer(uint targetNetId)
        {
            var target = ServerObjectManager[targetNetId];
            if (target == null)
            {
                return;
            }
            
            target.GetComponent<ActorProxy>().AppyHeal(effect);
        }

        [Server]
        public void AppyHeal(float amount)
        {
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            HealedOnClient(amount);
        }

        [ClientRpc]
        public void HealedOnClient(float amount)
        {
            if (baseStat == null)
            {
                return;
            }
            
            PoolManager.instance.ActivateObject("Effect_Heal", transform.position);
        }

        private MsgVector2 lastSend;
        public MsgVector2 lastRecieved;
        private bool hasRecieved = false;
        
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
                var position = transform.position;
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
                if (hasRecieved == false)
                {
                    hasRecieved = true;
                    transform.position = position;
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, position, diceInfo.moveSpeed * Time.deltaTime);   
                }
            }
        }
        
        bool Equal(MsgVector2 a, MsgVector2 b)
        {
            return a.X == b.X &&
                   a.Y == b.Y;
        }

        public bool IsLocalPlayerAlly()
        {
            return (Client as RWNetworkClient).IsLocalPlayerAlly(team);
        }

        public BaseStat GetHighestHealthEnemy()
        {
            var rwNetworkClient = Client as RWNetworkClient;
            return rwNetworkClient.GetHighestHealthEnemy(team);
        }

        public BaseStat GetBaseStatWithNetId(uint netId)
        {
            var actor = ClientObjectManager[netId];
            if (actor == null)
            {
                return null;
            }
            
            return actor.GetComponent<ActorProxy>().baseStat;
        }
    }
}