using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using ED;
using Mirage;
using MirageTest.Scripts.GameMode;
using MirageTest.Scripts.Messages;
using MirageTest.Scripts.SyncAction;
using Pathfinding;
using RandomWarsProtocol;
using RandomWarsResource.Data;
using Sirenix.OdinInspector;
using UnityEngine;
using Channel = Mirage.Channel;
using Debug = ED.Debug;
using Object = UnityEngine.Object;
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
        [SyncVar] public float moveSpeed;
        [SyncVar] public byte diceScale;
        [SyncVar] public byte ingameUpgradeLevel;
        [SyncVar] public byte outgameUpgradeLevel;
        [SyncVar] public float spawnTime;

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

        public readonly Buffs BuffList = new Buffs();
        public BuffState buffState = BuffState.None;
        public bool isClocking => ((buffState & BuffState.Clocking) != 0);
        public bool isHalfDamage => ((buffState & BuffState.HalfDamage) != 0);
        public bool isCantAI => ((buffState & BuffState.CantAI) != 0);

        [System.Serializable]
        public struct Buff
        {
            public byte id;
            public float endTime;

            public override string ToString()
            {
                return $"id: {id}, endTime: {endTime}";
            }
        }

        [System.Serializable]
        public class Buffs : SyncList<Buff>
        {
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
            NetIdentity.OnStopServer.AddListener(StopServer);

            _seeker = GetComponent<Seeker>();
            _aiPath = GetComponent<AIPath>();
        }

        private void StopClient()
        {
            var client = Client as RWNetworkClient;
            client.RemoveActorProxy(this);
            currentHealth = 0;

            if (baseStat != null)
            {
                baseStat.OnBaseStatDestroyed();
                baseStat.ActorProxy = null;
                baseStat = null;
            }

            stopped = true;
        }

        public void SetTeam(byte oldValue, byte newValue)
        {
            // renderer.material.color = newValue == 1? Color.blue : Color.red;
        }

        private void StartServer()
        {
            var server = Server as RWNetworkServer;
            server.AddActorProxy(this);
        }
        
        private void StopServer()
        {
            var server = Server as RWNetworkServer;
            server.RemoveActorProxy(this);
        }

        private void StartClient()
        {
            var client = Client as RWNetworkClient;
            if (client.enableActor)
            {
                BuffList.OnChange += OnBuffListChangedOnClientOnly;
                SpawnActor();
                OnBuffListChangedOnClientOnly();
            }

            client.AddActorProxy(this);
        }

        private void OnBuffListChangedOnClientOnly()
        {
            var state = BuffState.None;
            foreach (var buff in BuffList)
            {
                state |= BuffInfos.Data[buff.id];
            }

            if (this.buffState == state)
            {
                return;
            }

            if (baseStat is Minion minion)
            {
                var isClockingBefore = (buffState & BuffState.Clocking) != 0;
                var isClockkingNew = (state & BuffState.Clocking) != 0;
                if (isClockingBefore != isClockkingNew)
                {
                    minion.ApplyCloacking(isClockkingNew);
                }
            }

            this.buffState = state;

            EnableStunEffect((buffState & BuffState.Sturn) != 0);
            EnableFreezeEffect((buffState & BuffState.Freeze) != 0);

            if (isCantAI)
            {
                baseStat.animator.SetTrigger(AnimationHash.Idle);
                baseStat.StopAllAction();
            }
            else
            {
                baseStat.animator.SetTrigger(AnimationHash.Idle);
                if (isPlayingAI)
                {
                    baseStat.StartAI();
                }
            }
        }

        private void EnableStunEffect(bool b)
        {
            if (baseStat is Minion minion)
            {
                if (b)
                {
                    var ad = PoolManager.instance.ActivateObject<PoolObjectAutoDeactivate>("Effect_Sturn",
                        baseStat.ts_HitPos.position + Vector3.up * 0.65f);
                    minion._dicEffectPool.Add(MAZ.STURN, ad);
                }
                else
                {
                    if (minion._dicEffectPool.TryGetValue(MAZ.STURN, out var ad))
                    {
                        minion._dicEffectPool.Remove(MAZ.STURN);
                        ad.Deactive();
                    }
                }
            }
        }

        private void EnableFreezeEffect(bool b)
        {
            if (baseStat is Minion minion)
            {
                if (b)
                {
                    minion.animator.speed = 0f;
                }
                else
                {
                    minion.animator.speed = 1f;
                }
            }
        }

        void SpawnActor()
        {
            var client = Client as RWNetworkClient;
            if (actorType == ActorType.Tower)
            {
                SpawnTower();
            }
            else if (actorType == ActorType.Actor)
            {
                if (client.enableUI && IsLocalPlayerActor)
                {
                    var setting = UI_DiceField.Get().arrSlot[spawnSlot].ps.main;
                    setting.startColor = FileHelper.GetColor(diceInfo.color);
                    UI_DiceField.Get().arrSlot[spawnSlot].ps.Play();
                }

                if (diceInfo.castType == (int) DICE_CAST_TYPE.MINION || diceInfo.castType == (int) DICE_CAST_TYPE.HERO)
                {
                    SpawnMinionOrHero();
                }
                else if (diceInfo.castType == (int) DICE_CAST_TYPE.MAGIC ||
                         diceInfo.castType == (int) DICE_CAST_TYPE.INSTALLATION)
                {
                    SpawnMagicAndInstallation();
                }
            }
            else if (actorType == ActorType.SummonByMinion)
            {
                SpawnSummonActor();
            }

            EnableAI(client.IsPlayingAI);
            RefreshHpUI();
        }

        public bool IsBottomCamp()
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

            if (baseStat.image_HealthBar == null)
            {
                return;
            }

            baseStat.image_HealthBar.fillAmount = currentHealth / maxHealth;

            if (baseStat.text_Health != null)
            {
                baseStat.text_Health.text = $"{Mathf.CeilToInt(currentHealth)}";
            }
        }

        public void EnableAI(bool b)
        {
            isPlayingAI = b;

            var actor = baseStat;
            if (actor == null)
            {
                return;
            }

            if (b)
            {
                actor.StartAI();
            }
            else
            {
                actor.StopAI();
            }

            var collider = actor.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = b;
            }
        }

        public void AddBuff(byte id, float duration)
        {
            AddBuffOnServer(id, duration);
        }

        [ServerRpc(requireAuthority = false)]
        public void AddBuffOnServer(byte id, float duration)
        {
            BuffList.Add(new Buff()
            {
                id = id,
                endTime = (float) NetworkTime.Time + duration,
            });
        }

        public void HitDamage(float damage)
        {
            if (baseStat.OnBeforeHitDamage(damage))
            {
                return;
            }

            HitDamageOnServer(damage);
        }

        [ServerRpc(requireAuthority = false)]
        public void HitDamageOnServer(float damage)
        {
            ApplyDamage(damage);
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

            SoundManager.instance.PlayRandom(Global.E_SOUND.SFX_MINION_HIT);
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
        [NonSerialized] public MsgVector2 lastRecieved;
        private bool hasRecieved = false;

        private void Update()
        {
            if (IsServer)
            {
                //TODO: 부하가 많으면 클라이언트에서 하도록 수정
                if (BuffList.Count > 0)
                {
                    var now = NetworkTime.Time;
                    for (int i = BuffList.Count - 1; i >= 0; --i)
                    {
                        var buff = BuffList[i];
                        if (buff.endTime <= now)
                        {
                            BuffList.RemoveAt(i);
                        }
                    }
                }

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

            if (isPlayingAI && baseStat.NeedMoveSyncSend)
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
            else if (baseStat.SyncAction != null && baseStat.SyncAction.NeedMoveSync == false)
            {
            }
            else if (lastRecieved != null)
            {
                var position = ConvertNetMsg.MsgToVector3(lastRecieved);
                if (hasRecieved == false)
                {
                    hasRecieved = true;
                    transform.position = position;
                }
                else
                {
                    transform.position = Vector3.Lerp(transform.position, position, 0.5f);
                    transform.LookAt(position);

                    if (baseStat.animator != null)
                    {
                        float distance = Vector3.Magnitude(position - transform.position);
                        baseStat.animator.SetFloat(AnimationHash.MoveSpeed, distance * 5);
                    }
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

        //KZSee : TODO: 보스도 보게 할 것
        public BaseStat GetEnemyTower()
        {
            var rwClient = Client as RWNetworkClient;
            var enemyTower = rwClient.Towers.Find(t => t.team != team);
            return enemyTower.baseStat;
        }
        
        public BaseStat[] GetEnemies()
        {
            var rwClient = Client as RWNetworkClient;
            //TODO: 팀별로 액터를 분리해놓는다.
            var enemies = rwClient.ActorProxies.Where(actor =>
            {
                if (actor.team == team)
                {
                    return false;
                }

                if (actor.actorType == ActorType.Tower)
                {
                    return false;
                }

                if (actor.baseStat is Minion minion && (minion.collider == null || minion.isCanBeTarget == false))
                {
                    return false;
                }
                
                if (actor.baseStat is Magic magic && (magic.collider == null || magic.isCanBeTarget == false))
                {
                    return false; 
                }

                return true;
            }).Select(actor => actor.baseStat);

            return enemies.ToArray();
        }

        public BaseStat GetRandomEnemyCanBeAttacked()
        {
            var rwClient = Client as RWNetworkClient;
            //TODO: 팀별로 액터를 분리해놓는다.
            var enemies = rwClient.ActorProxies.Where(actor =>
            {
                if (actor.team == team)
                {
                    return false;
                }

                if (actor.actorType == ActorType.Tower)
                {
                    return true;
                }

                if (actor.baseStat is Minion minion && (minion.collider == null || minion.isCanBeTarget == false))
                {
                    return false;
                }
                
                if (actor.baseStat is Magic magic && (magic.collider == null || magic.isCanBeTarget == false))
                {
                    return false; 
                }

                return true;
            });

            var actorProxies = enemies as ActorProxy[] ?? enemies.ToArray();
            var selected = actorProxies.ElementAt(UnityEngine.Random.Range(0, actorProxies.Count()));
            return selected.baseStat;
        }

        public void Destroy()
        {
            DestroyOnServer();
        }

        [ServerRpc(requireAuthority = false)]
        public void DestroyOnServer()
        {
            DestroyInternal();
        }

        public void Destroy(float delay)
        {
            DestroyOnServerDelayed(delay);
        }

        [ServerRpc(requireAuthority = false)]
        public void DestroyOnServerDelayed(float delay)
        {
            DestroyInternalDelayed(delay).Forget();
        }

        async UniTask DestroyInternalDelayed(float delay)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            DestroyInternal();
        }

        void DestroyInternal()
        {
            currentHealth = 0;
            ServerObjectManager.Destroy(gameObject);
        }

        public void SummonActor(byte summonActorId, Vector2 position)
        {
            SummonActorOnServer(summonActorId, position);
        }

        [ServerRpc(requireAuthority = false)]
        public void SummonActorOnServer(byte summonActorId, Vector3 position)
        {
            var server = Server as RWNetworkServer;
            server.SummonActor(this, summonActorId, position);
        }

        public void DestroyAfterSummonActor(byte summonActorId, Vector3 position)
        {
            DestroyAfterSummonActorOnServer(summonActorId, position);
        }

        [ServerRpc(requireAuthority = false)]
        public void DestroyAfterSummonActorOnServer(byte summonActorId, Vector3 position)
        {
            var server = Server as RWNetworkServer;
            server.SummonActor(this, summonActorId, position);
            DestroyInternal();
        }
        
        public void CreateActorBy(int diceId, byte ingameLevel, byte outgameLevel, Vector3[] positions)
        {
            CreateActorByOnServer(diceId, ingameLevel, outgameLevel, positions);
        }


        [ServerRpc(requireAuthority = false)]
        public void CreateActorByOnServer(int diceId, byte inGameLevel, byte outGameLevel, Vector3[] positions)
        {
            if (TableManager.Get().DiceInfo.GetData(diceId, out var diceInfo) == false)
            {
                ED.Debug.LogError(
                    $"다이스정보 {diceId}가 없습니다.");
            }

            GameModeBase.Stat stat = new GameModeBase.Stat();
            if ((DICE_CAST_TYPE) diceInfo.castType == DICE_CAST_TYPE.MINION)
            {
                stat = GameModeBase.CalcMinionStat(diceInfo, inGameLevel, outGameLevel);
            }
            else if ((DICE_CAST_TYPE) diceInfo.castType == DICE_CAST_TYPE.HERO)
            {
                stat = GameModeBase.CalcHeroStat(diceInfo, inGameLevel, outGameLevel, 1);
            }
            else if ((DICE_CAST_TYPE) diceInfo.castType == DICE_CAST_TYPE.INSTALLATION ||
                     (DICE_CAST_TYPE) diceInfo.castType == DICE_CAST_TYPE.MAGIC)
            {
                stat = GameModeBase.CalcMagicOrInstallationStat(diceInfo, inGameLevel, outGameLevel, 1);
            }

            var isBottomCamp = team == GameConstants.BottomCamp;
            
            var server = Server as RWNetworkServer;
            var actorProxyPrefab = server.serverGameLogic.actorProxyPrefab;

            for (byte index = 0; index < positions.Length; ++index)
            {
                var position = positions[index];
                var spawnPosition = position;

                var actorProxy = Instantiate(actorProxyPrefab, spawnPosition, GameModeBase.GetRotation(isBottomCamp));
                actorProxy.SetDiceInfo(diceInfo);
                actorProxy.ownerTag = ownerTag;
                actorProxy.actorType = ActorType.Actor;
                actorProxy.team = team;
                actorProxy.spawnSlot = 0;
                actorProxy.power = stat.power;
                actorProxy.maxHealth = stat.maxHealth;
                actorProxy.currentHealth = stat.maxHealth;
                actorProxy.effect = stat.effect;
                actorProxy.attackSpeed = diceInfo.attackSpeed;
                actorProxy.moveSpeed = diceInfo.moveSpeed;
                actorProxy.diceScale = 1;
                actorProxy.ingameUpgradeLevel = inGameLevel;
                actorProxy.outgameUpgradeLevel = outGameLevel;
                actorProxy.spawnTime = (float) ServerObjectManager.Server.Time.Time;
                ServerObjectManager.Spawn(actorProxy.NetIdentity);
            }
        }
    }
}