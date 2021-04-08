using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using ED;
using Mirage;
using Mirage.Collections;
using Mirage.Logging;
using MirageTest.Scripts.Messages;
using Pathfinding;
using Pathfinding.RVO;
using RandomWarsProtocol;
using RandomWarsResource.Data;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MirageTest.Scripts
{
    public partial class ActorProxy : NetworkBehaviour
    {
        protected static readonly ILogger _logger = LogFactory.GetLogger(typeof(ActorProxy));

        [SyncVar] public byte ownerTag;
        [SyncVar(hook = nameof(SetTeam))] public byte team;
        [SyncVar] public byte spawnSlot; //0 ~ 14 필드 슬롯 
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
        [SyncVar] public bool isSkillInitialized;

        public bool isPlayingAI;
        public bool isMovable = true;

        public bool IsLocalPlayerActor => (Client as RWNetworkClient).IsLocalPlayerTag(ownerTag);

        public BaseStat baseStat;

        private bool stopped;

        public Seeker _seeker;
        public AIPath _aiPath;

        public AIDestinationSetter _aiDestinationSetter;
        public RVOController _rvoController;

        public readonly Buffs BuffList = new Buffs();
        public BuffState buffState = BuffState.None;
        public bool isClocking => ((buffState & BuffState.Clocking) != 0);
        public bool isHalfDamage => ((buffState & BuffState.HalfDamage) != 0);
        public bool isCantAI => ((buffState & BuffState.CantAI) != 0);
        public bool isTaunted => ((buffState & BuffState.Taunted) != 0);

        public bool isInAllyCamp
        {
            get
            {
                if (team == GameConstants.BottomCamp)
                {
                    return transform.position.z < 0;
                }
                else if(team == GameConstants.TopCamp)
                {
                    return transform.position.z > 0;
                }
                
                return false;
            }
        }

        public bool isInEnemyCamp => !isInAllyCamp;

        [Serializable]
        public struct Buff
        {
            public byte id;
            public uint target;
            public float endTime;

            public override string ToString()
            {
                return $"id: {id}, endTime: {endTime}";
            }
        }

        [Serializable]
        public class Buffs : SyncList<Buff>
        {
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

        public void EnablePathfinding(bool b)
        {
            EnableSeeker(b);
            if (RWNetworkClient.EnableRVO)
            {
                _rvoController.enabled = b;
                _aiDestinationSetter.enabled = b;
            }
        }
        
        public void EnableSeeker(bool b)
        {
            _seeker.enabled = b;
            _aiPath.enabled = b;
            _aiPath.isStopped = !b;
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

            if (Server.LocalClientActive)
            {
                StartClient();
            }
        }

        private void StopServer()
        {
            var server = Server as RWNetworkServer;
            server.RemoveActorProxy(this);

            if (Server.LocalClientActive)
            {
                StopClient();
            }
        }

        private void StartClient()
        {
            if (RWNetworkClient.EnableRVO)
            {
                _aiDestinationSetter = gameObject.AddComponent<AIDestinationSetter>();
                _rvoController = gameObject.AddComponent<RVOController>();
                var simpleSmoothModifier = gameObject.AddComponent<SimpleSmoothModifier>();
                _rvoController.maxNeighbours = 20;
                _rvoController.lockWhenNotMoving = !UI_Main.isPushMode;
                EnablePathfinding(false);
            }
            
            var client = Client as RWNetworkClient;
            if (client.enableActor)
            {
                BuffList.OnChange += OnBuffListChangedOnClient;
                SpawnActor();
                OnBuffListChangedOnClient();
            }

            client.AddActorProxy(this);
        }

        private void OnBuffListChangedOnClient()
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
                var isClockingOld = (buffState & BuffState.Clocking) != 0;
                var isClockkingNew = (state & BuffState.Clocking) != 0;
                if (isClockingOld != isClockkingNew)
                {
                    minion.ApplyCloacking(isClockkingNew);
                }
                
                var isTauntedOld = (buffState & BuffState.Taunted) != 0;
                var isTauntedNew = (state & BuffState.Taunted) != 0;
                if (isTauntedOld != isTauntedNew)
                {
                    if (isTauntedNew)
                    {
                        RestartAI();   
                    }
                    else
                    {
                        minion.ResetAttackedTarget();
                    }
                }
            }
            
            this.buffState = state;

            EnableInvincibilityEffect((buffState & BuffState.Invincibility) != 0);
            EnableStunEffect((buffState & BuffState.Sturn) != 0);
            EnableFreezeEffect((buffState & BuffState.Freeze) != 0);
            EnableScarecrowEffect((buffState & BuffState.Scarecrow) != 0);
            EnableTauntedEffect((buffState & BuffState.Taunted) != 0);

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

        private void EnableScarecrowEffect(bool b)
        {
            if (baseStat is Minion minion)
            {
                if (b)
                {
                    if (minion._dicEffectPool.ContainsKey(MAZ.SCARECROW) == false)
                    {
                        var ad = PoolManager.instance.ActivateObject<PoolObjectAutoDeactivate>("Scarecrow",
                            transform.position);
                        ad.transform.SetParent(transform);
                        minion._dicEffectPool.Add(MAZ.SCARECROW, ad);
                        minion.animator.gameObject.SetActive(false);
                    }
                }
                else
                {
                    if (minion._dicEffectPool.TryGetValue(MAZ.SCARECROW, out var ad))
                    {
                        minion._dicEffectPool.Remove(MAZ.SCARECROW);
                        ad.Deactive();
                        minion.animator.gameObject.SetActive(true);
                    }
                }
            }
        }

        private void EnableInvincibilityEffect(bool b)
        {
            if (baseStat is Minion minion)
            {
                if (b)
                {
                    if (minion._dicEffectPool.ContainsKey(MAZ.INVINCIBILITY) == false)
                    {
                        var ad = PoolManager.instance.ActivateObject<PoolObjectAutoDeactivate>("Shield",
                            transform.position);
                        ad.transform.SetParent(transform);
                        minion._dicEffectPool.Add(MAZ.INVINCIBILITY, ad);
                    }
                }
                else
                {
                    if (minion._dicEffectPool.TryGetValue(MAZ.INVINCIBILITY, out var ad))
                    {
                        minion._dicEffectPool.Remove(MAZ.INVINCIBILITY);
                        ad.Deactive();
                    }
                }
            }
        }

        private void EnableStunEffect(bool b)
        {
            if (baseStat is Minion minion)
            {
                if (b)
                {
                    if (minion._dicEffectPool.ContainsKey(MAZ.STUN) == false)
                    {
                        var ad = PoolManager.instance.ActivateObject<PoolObjectAutoDeactivate>("Effect_Sturn",
                            GetEffectPosition(baseStat, EffectLocation.Top));
                        ad.transform.SetParent(transform);
                        
                        minion._dicEffectPool.Add(MAZ.STUN, ad);
                    }
                }
                else
                {
                    if (minion._dicEffectPool.TryGetValue(MAZ.STUN, out var ad))
                    {
                        minion._dicEffectPool.Remove(MAZ.STUN);
                        ad.Deactive();
                    }
                }
            }
        }

        Vector3 GetEffectPosition(BaseStat baseStat, EffectLocation effectLocation)
        {
            switch (effectLocation)
            {
                case EffectLocation.Top:
                    // return baseStat.ts_TopEffectPosition.transform.position;
                return baseStat.ts_HitPos.transform.position + new Vector3(0, 0.65f, 0);
                case EffectLocation.Mid:
                    return baseStat.ts_HitPos.transform.position;
                case EffectLocation.Bottom:
                    return baseStat.transform.position;
            }
            
            return baseStat.transform.position;
        }
        
        private void EnableTauntedEffect(bool b)
        {
            if (baseStat is Minion minion)
            {
                if (b)
                {
                    if (minion._dicEffectPool.ContainsKey(MAZ.TAUNTED) == false)
                    {
                        var ad = PoolManager.instance.ActivateObject<PoolObjectAutoDeactivate>("Effect_Sturn",
                            GetEffectPosition(baseStat, EffectLocation.Top));
                        ad.transform.SetParent(transform);
                        minion._dicEffectPool.Add(MAZ.TAUNTED, ad);
                    }
                }
                else
                {
                    if (minion._dicEffectPool.TryGetValue(MAZ.TAUNTED, out var ad))
                    {
                        minion._dicEffectPool.Remove(MAZ.TAUNTED);
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
            OnSpawnActor();
            
            _aiPath.maxSpeed = moveSpeed;

            EnableAI(client.IsPlayingAI);
            RefreshHpUI();
        }

        protected virtual void OnSpawnActor()
        {
        }


        public bool IsBottomCamp()
        {
            return team == GameConstants.BottomCamp;
        }

        public void SetHp(float oldValue, float newValue)
        {
            RefreshHpUI();
            OnSetHp();
        }

        protected virtual void OnSetHp()
        {
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
        
        public void RestartAI()
        {
            if (isPlayingAI == false)
            {
                return;
            }
            
            var actor = baseStat;
            if (actor == null)
            {
                return;
            }

            actor.StopAI();
            actor.StartAI();
        }

        private void LateUpdate()
        {
            var client = Client as RWNetworkClient;
            if (client == null || client.IsConnected == false)
            {
                return;
            }

            if (client.enableActor == false)
            {
                return;
            }

            var localPlayerState = client.GetLocalPlayerState();
            if (localPlayerState == null)
            {
                return;
            }
            
            var tilt =  baseStat.transform.worldToLocalMatrix * new Vector3(20, 0, 0);
            baseStat.transform.localPosition = new Vector3(0, 0.3f, 0);
            if (localPlayerState.team == GameConstants.BottomCamp)
            {
                baseStat.transform.localEulerAngles = tilt;
            }
            else
            {
                baseStat.transform.localEulerAngles = -tilt;
            }
        }

        public void AddBuff(byte id, float duration)
        {
            if (IsLocalClient)
            {
                AddBuffInternal(id, duration);
                return;
            }

            AddBuffOnServer(id, duration);
        }

        [ServerRpc(requireAuthority = false)]
        public void AddBuffOnServer(byte id, float duration)
        {
            AddBuffInternal(id, duration);
        }

        void AddBuffInternal(byte id, float duration)
        {
            var findIndex = BuffList.FindIndex(buff => buff.id == id);
            if (findIndex >= 0)
            {
                BuffList.RemoveAt(findIndex);
            }
            
            BuffList.Add(new Buff()
            {
                id = id,
                endTime = (float) NetworkTime.Time + duration,
            });
        }
        
        public void AddBuffWithNetId(byte id, uint targetNetId, float duration)
        {
            if (IsLocalClient)
            {
                AddBuffWithNetIdInternal(id, targetNetId, duration);
                return;
            }

            AddBuffWithNetIdOnServer(id, targetNetId, duration);
        }

        [ServerRpc(requireAuthority = false)]
        public void AddBuffWithNetIdOnServer(byte id, uint targetNetId, float duration)
        {
            AddBuffWithNetIdInternal(id, targetNetId, duration);
        }

        void AddBuffWithNetIdInternal(byte id, uint targetNetId, float duration)
        {
            var findIndex = BuffList.FindIndex(buff => buff.id == id);
            if (findIndex >= 0)
            {
                BuffList.RemoveAt(findIndex);
            }
            
            BuffList.Add(new Buff()
            {
                id = id,
                target = targetNetId,
                endTime = (float) NetworkTime.Time + duration,
            });
        }

        public void HitDamage(float damage)
        {
            if ((buffState & BuffState.Invincibility) != 0)
            {
                return;
            }

            if (baseStat.OnBeforeHitDamage(damage))
            {
                return;
            }

            if (IsLocalClient)
            {
                ApplyDamage(damage);
                DamageOnInternal(damage);
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
        private void ApplyDamage(float damage)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            OnApplyDamageOnServer();

            if (currentHealth <= 0)
            {
                ServerObjectManager.Destroy(gameObject);
                return;
            }

            DamagedOnClient(damage);
        }

        protected virtual void OnApplyDamageOnServer()
        {
        }

        [ClientRpc]
        public void DamagedOnClient(float damage)
        {
            DamageOnInternal(damage);
        }

        void DamageOnInternal(float damage)
        {
            if (baseStat == null)
            {
                return;
            }

            baseStat.OnHitDamageOnClient(damage);
            var obj = PoolManager.Get().ActivateObject("Effect_ArrowHit", baseStat.ts_HitPos.position);
            obj.rotation = Quaternion.identity;
            obj.localScale = Vector3.one * 0.6f;

            SoundManager.instance.PlayRandom(Global.E_SOUND.SFX_MINION_HIT);
        }

        public void Heal(float amount)
        {
            if (IsLocalClient)
            {
                ApplyHealInternal(amount);
                HealOnInternal(amount);
                return;
            }

            AppyHealOnServer(amount);
        }

        [ServerRpc(requireAuthority = false)]
        public void AppyHealOnServer(float amount)
        {
            ApplyHealInternal(amount);
            HealedOnClient(amount);
        }

        void ApplyHealInternal(float amount)
        {
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        }

        [ClientRpc]
        public void HealedOnClient(float amount)
        {
            HealOnInternal(amount);
        }

        void HealOnInternal(float amount)
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
                SyncPosition(false);
            }
            else if (baseStat.SyncAction != null && baseStat.SyncAction.NeedMoveSync == false)
            {
                baseStat.animator.SetFloat(AnimationHash.MoveSpeed, 0);
            }
            else if (lastRecieved != null)
            {
                var position = MsgToVector3(lastRecieved);
                if (hasRecieved == false)
                {
                    hasRecieved = true;
                    transform.position = position;
                    EnableSeeker(true);
                }
                else
                {
                    var distance = (transform.position - position).magnitude;
                    var moveSpeedCalculated = moveSpeed;

                    if (distance > 2.0f)
                    {
                        transform.position = position;
                        return;
                    }
                    else if (distance > 0.5f)
                    {
                        moveSpeedCalculated *= distance * 2;
                    }
                    else if (distance > 0.1f)
                    {
                        moveSpeedCalculated *= 1.5f;
                    }
                    
                    _aiPath.maxSpeed = moveSpeedCalculated;
                    
                    _seeker.StartPath(transform.position, position);
                    baseStat.animator.SetFloat(AnimationHash.MoveSpeed, _aiPath.velocity.magnitude);
                }
            }
        }
        
        public static MsgVector2 Vector3ToMsg(Vector2 val)
        {
            MsgVector2 chVec = new MsgVector2();

            chVec.X = MsgFloatToShort(val.x);
            chVec.Y = MsgFloatToShort(val.y);

            return chVec;
        }
    
        public static Vector3 MsgToVector3(MsgVector2 msgVec)
        {
            Vector3 vecVal = new Vector3();

            vecVal.x = MsgShortToFloat(msgVec.X);
            vecVal.y = 0;
            vecVal.z = MsgShortToFloat(msgVec.Y);

            return vecVal;
        }
        
        public static short MsgFloatToShort(float value)
        {
            return Convert.ToInt16(value * 100);
        }

        public static float MsgShortToFloat(short value)
        {
            return Convert.ToInt32(value) * 0.01f;
        }

        public void SyncPosition(bool force)
        {
            var position = transform.position;
            var converted = Vector3ToMsg(new Vector2(position.x, position.z));
            if (lastSend == null || !Equal(lastSend, converted) || force)
            {
                lastSend = converted;
                Client.Send(new PositionRelayMessage()
                {
                    netId = NetId,
                    positionX = converted.X,
                    positionY = converted.Y,
                }, Channel.Unreliable);
            }
        }

        bool Equal(MsgVector2 a, MsgVector2 b)
        {
            return a.X == b.X &&
                   a.Y == b.Y;
        }

        public bool IsLocalPlayerAlly => (Client as RWNetworkClient).IsLocalPlayerAlly(team);

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
        
        public BaseStat GetEnemyTowerOrBossEgg()
        {
            var rwClient = Client as RWNetworkClient;
            var enemyTower = rwClient.Towers.Find(t => t.team != team);
            if (enemyTower != null)
            {
                return enemyTower.baseStat;
            }
            
            var enemyBoss = rwClient.Bosses.Find(t => t.team != team && t.isHatched == false);
            if (enemyBoss != null)
            {
                return enemyBoss.baseStat;
            }

            return null;
        }
        
        public BaseStat GetAllyTower()
        {
            var rwClient = Client as RWNetworkClient;
            var allyTower = rwClient.Towers.Find(t => t.team == team);
            if (allyTower != null)
            {
                return allyTower.baseStat;
            }
            
            return null;
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

                if (actor is TowerActorProxy)
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

        public BaseStat GetRandomFirendlyMinion()
        {
            var rwClient = Client as RWNetworkClient;
            var friends = rwClient.ActorProxies.Where(actor =>
            {
                if (actor.team != team) return false;
                if ((actor is DiceActorProxy) == false) return false;
                if (actor.baseStat is Minion minion && (minion.collider == null || minion.isCanBeTarget == false))
                    return false;
                if (actor.baseStat is Magic) return false;

                return true;
            });

            var actorProxies = friends as ActorProxy[] ?? friends.ToArray();
            if (actorProxies == null || actorProxies.Length == 0) return null;
            var selected = actorProxies.ElementAt(Random.Range(0, actorProxies.Count()));
            return selected.baseStat;
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

                if (actor is TowerActorProxy)
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
            if (actorProxies == null || actorProxies.Length == 0) return null;
            var selected = actorProxies.ElementAt(Random.Range(0, actorProxies.Count()));
            return selected.baseStat;
        }

        public void Destroy()
        {
            if (IsLocalClient)
            {
                DestroyInternal();
                return;
            }

            DestroyOnServer();
        }

        [ServerRpc(requireAuthority = false)]
        public void DestroyOnServer()
        {
            DestroyInternal();
        }

        public void Destroy(float delay)
        {
            if (IsLocalClient)
            {
                DestroyInternalDelayed(delay);
                return;
            }

            DestroyOnServerDelayed(delay);
        }

        [ServerRpc(requireAuthority = false)]
        public void DestroyOnServerDelayed(float delay)
        {
            DestroyInternalDelayed(delay);
        }

        public void DestroyInternalDelayed(float delay)
        {
            StartCoroutine(DestroyInternalDelayedCoroutine(delay));
        }

        IEnumerator DestroyInternalDelayedCoroutine(float delay)
        {
            yield return new WaitForSeconds(delay);

            DestroyInternal();
        }

        void DestroyInternal()
        {
            ServerObjectManager.Destroy(gameObject);
        }

        public void CreateActorBy(int diceId, byte inGameLevel, byte outGameLevel, Vector3[] positions,
            float delay = 0f)
        {
            if (IsLocalClient)
            {
                var server = Server as RWNetworkServer;
                server.CreateActorWithDiceId(diceId, ownerTag, team, inGameLevel, outGameLevel, positions, delay);
                return;
            }

            CreateActorByOnServer(diceId, inGameLevel, outGameLevel, positions, delay);
        }

        [ServerRpc(requireAuthority = false)]
        public void CreateActorByOnServer(int diceId, byte inGameLevel, byte outGameLevel, Vector3[] positions,
            float delay)
        {
            var server = Server as RWNetworkServer;
            server.CreateActorWithDiceId(diceId, ownerTag, team, inGameLevel, outGameLevel, positions, delay);
        }

        public void Fusion()
        {
            if (IsLocalClient)
            {
                FusionInternal();
                return;
            }

            FusionOnClient();
        }

        [ClientRpc]
        public void FusionOnClient()
        {
            FusionInternal();
        }

        private void FusionInternal()
        {
            var rwClient = Client as RWNetworkClient;
            var tower = rwClient.GetTower(ownerTag);
            Vector3 fusionPosition = transform.position;

            if (tower != null)
            {
                fusionPosition = tower.transform.position;
                fusionPosition.z += fusionPosition.z > 0 ? -2f : 2f;
            }

            transform.DOMove(fusionPosition, 0.5f).OnComplete(() => { Destroy(); });
        }

        public void UpdateSyncPositionToCurrentPosition()
        {
            var position = transform.position;
            var currentPosition = new Vector2(position.x, position.z);
            lastRecieved = Vector3ToMsg(currentPosition);
        }   
    }
}