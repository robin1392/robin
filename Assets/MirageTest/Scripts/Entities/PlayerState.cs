using System;
using System.Collections.Generic;
using Mirage;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.Serialization;
using System.Linq;
using ED;
using Mirage.Collections;
using Mirage.Logging;
using Sirenix.OdinInspector;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace MirageTest.Scripts.Entities
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class PlayerState : NetworkBehaviour
    {
        static readonly ILogger logger = LogFactory.GetLogger(typeof(PlayerState));

        public bool EnableUI;
        [SyncVar] public string userId;

        [SyncVar] public string nickName;

        //TODO: 꼭 필요한지 고민이 필요함. connectionId는 재접 시 바뀌어서 사용못하고, 유저아이디는 스트링이어서 부담스러움.
        [SyncVar] public byte ownerTag;
        [SyncVar] public byte team; //상단 캠프, 하단 캠프 두가지로 나뉨. 팀의 개념
        [SyncVar(hook = nameof(SetSpGrade))] public int spGrade;
        [SyncVar(hook = nameof(SetSp))] public int sp;

        [SyncVar(hook = nameof(SetGetDiceCount))]
        public int getDiceCount;

        [SyncVar] public int guadianId;
        public readonly Deck Deck = new Deck();
        public readonly Field Field = new Field();

        private Dictionary<int, DeckDice> _deckDiceMap = new Dictionary<int, DeckDice>();

        public const int FieldCount = 15;

        private bool _initalized;

        public bool IsLocalPlayerState => (Client as RWNetworkClient).localPlayerId == userId;

        public void Init(string userId, string nickName, int sp, DeckDice[] deck, byte tag, int guadianId)
        {
            if (_initalized)
            {
                ED.Debug.LogError("Init이 두번 호출됨.");
            }

            this.userId = userId;
            this.nickName = nickName;
            this.sp = sp;
            this.ownerTag = tag;
            this.spGrade = 0;
            this.guadianId = guadianId;

            foreach (var deckDice in deck)
            {
                Deck.Add(deckDice);
                _deckDiceMap.Add(deckDice.diceId, deckDice);
            }

            for (int i = 0; i < FieldCount; ++i)
            {
                Field.Add(FieldDice.Empty);
            }

            _initalized = true;
        }

        private void Awake()
        {
            if (NetIdentity == null)
            {
                return;
            }

            NetIdentity.OnStartServer.AddListener(StartServer);
            NetIdentity.OnStopServer.AddListener(StopServer);

            NetIdentity.OnStartClient.AddListener(StartClient);
            NetIdentity.OnStopClient.AddListener(StopClient);
        }

        private void StopServer()
        {
            if (Server.LocalClientActive)
            {
                StopClient();
            }
        }

        private void StartServer()
        {
            if (Server.LocalClientActive)
            {
                StartClient();
            }
        }

        private void StartClient()
        {
            var client = Client as RWNetworkClient;
            client.AddPlayerState(this);

            Debug.Log($"PlayerState id:{userId} t:{team} o:{ownerTag}");

            EnableUI = client.enableUI;
            if (!EnableUI)
            {
                return;
            }

            if (client.localPlayerId == userId)
            {
                CameraController.Get().UpdateCameraRotation(team == GameConstants.BottomCamp);
            }

            Deck.OnChange += OnChangeDeckOnClientOnly;
            Field.OnSet += OnChangeFieldOnClientOnly;

            SetSp(sp, sp);
            SetSpGrade(spGrade, spGrade);
            OnChangeDeckOnClientOnly();
            SetGetDiceCount(getDiceCount, getDiceCount);
        }

        public DeckDice GetDeckDice(int diceId)
        {
            if (_deckDiceMap.TryGetValue(diceId, out var deckDice))
            {
                return deckDice;
            }

            return DeckDice.Empty;
        }

        public int GetDeckIndex(int diceId)
        {
            return Deck.IndexOf(GetDeckDice(diceId));
        }

        public void SetSp(int oldValue, int newValue)
        {
            if (!EnableUI)
            {
                return;
            }

            if (IsLocalPlayerState)
            {
                foreach (var upgradeButton in UI_InGame.Get().arrUpgradeButtons)
                {
                    upgradeButton.EditSpCallback(newValue);
                }

                UI_InGame.Get().btn_GetDice.EditSpCallback(newValue > GetDiceCost());
                UI_InGame.Get().button_SP_Upgrade.EditSpCallback(newValue > GetUpradeSpCost());
                UI_InGame.Get().SetSP(newValue);
            }
        }

        public void SetGetDiceCount(int oldValue, int newValue)
        {
            if (!EnableUI)
            {
                return;
            }

            if (IsLocalPlayerState)
            {
                UI_InGame.Get().SetDiceButtonText(GetDiceCost());
            }
        }

        public void SetSpGrade(int oldValue, int newValue)
        {
            if (!EnableUI)
            {
                return;
            }

            if (IsLocalPlayerState)
            {
                UI_InGame.Get().SetSPUpgrade(newValue + 1, GetUpradeSpCost());
                UI_InGame.Get().ShowSpUpgradeMessage();
            }
        }

        private void OnChangeFieldOnClientOnly(int index, FieldDice oldValue, FieldDice newValue)
        {
            if (!EnableUI)
            {
                return;
            }

            if (!IsLocalPlayerState)
            {
                return;
            }

            var uiDiceField = UI_DiceField.Get(); //싱글턴으로 대체
            if (oldValue.IsEmpty)
            {
                SoundManager.instance.Play(Global.E_SOUND.SFX_INGAME_UI_GET_DICE);
                uiDiceField.arrSlot[index].ani.SetTrigger("BBoing");
            }

            if (newValue.IsEmpty)
            {
                uiDiceField.arrSlot[index].SetDice(new Dice()
                {
                    diceFieldNum = index,
                    diceData = null,
                });
            }
            else
            {
                TableManager.Get().DiceInfo.GetData(newValue.diceId, out var diceInfo);
                uiDiceField.arrSlot[index].SetDice(new Dice()
                {
                    diceFieldNum = index,
                    diceData = diceInfo,
                    eyeLevel = newValue.diceScale
                });
            }

            uiDiceField.arrSlot[index].SetIcon();
        }

        private void OnChangeDeckOnClientOnly()
        {
            if (!EnableUI)
            {
                return;
            }

            var deckArr = Deck.Select(d =>
            {
                TableManager.Get().DiceInfo.GetData(d.diceId, out var diceInfo);
                return (diceInfo, d.inGameLevel);
            }).ToArray();

            if (IsLocalPlayerState)
            {
                UI_InGame.Get().SetArrayDeck(deckArr);
            }
            else
            {
                UI_InGame.Get().SetEnemyArrayDeck();
                UI_InGame.Get().SetEnemyUpgrade();
            }
        }

        private void StopClient()
        {
            var client = Client as RWNetworkClient;
            client.RemovePlayerState(this);
        }

        public int GetDiceCost()
        {
            var tableManager = TableManager.Get();
            var startCost = tableManager.Vsmode.KeyValues[(int) EVsmodeKey.GetStartDiceCost].value;
            int addDiceCost = tableManager.Vsmode.KeyValues[(int) EVsmodeKey.DiceCostUp].value;
            return startCost + (getDiceCount * addDiceCost);
        }

        public int GetUpradeSpCost()
        {
            return TableManager.Get().Vsmode.KeyValues[(int) EVsmodeKey.GetSPPlusLevelupCost01 + spGrade].value;
        }

        public int GetEmptySlotCount()
        {
            var emptySlotCount = Field.Count(f => f.IsEmpty);
            return emptySlotCount;
        }

        public int SelectEmptySlotRandom()
        {
            var emptySlotCount = Field.Count(f => f.IsEmpty);
            if (emptySlotCount < 1)
            {
                return -1;
            }

            var selectedIndexOnEmptySlots = Random.Range(0, emptySlotCount);
            var emptySlotIndex = 0;
            var selectedIndexOnField = -1;
            for (int indexOnField = 0; indexOnField < Field.Count; ++indexOnField)
            {
                var fieldDice = Field[indexOnField];
                if (!fieldDice.IsEmpty)
                {
                    continue;
                }

                if (selectedIndexOnEmptySlots == emptySlotIndex)
                {
                    selectedIndexOnField = indexOnField;
                }

                emptySlotIndex++;
            }

            return selectedIndexOnField;
        }

        public void AddSpByWave(int sp)
        {
            if (IsLocalClient)
            {
                AddSpByWaveInternal(sp);
                return;
            }

            AddSpByWaveOnClient(sp);
        }

        [ClientRpc]
        public void AddSpByWaveOnClient(int sp)
        {
            AddSpByWaveInternal(sp);
        }

        void AddSpByWaveInternal(int sp)
        {
            if (IsLocalPlayerState)
            {
                WorldUIManager.Get().AddSP(sp);
            }
        }


        public void GetDice()
        {
            var fieldIndex = SelectEmptySlotRandom();
            if (fieldIndex < 0)
            {
                logger.LogError($"비어있는 슬롯이 없습니다. playerId : {userId}");
                return;
            }

            var deckIndex = Random.Range(0, Deck.Count);
            GetDice((byte)deckIndex, (byte)fieldIndex);
        }

        public void GetDice(byte deckIndex, byte fieldIndex)
        {
            // SP를 차감한다.
            int needSp = GetDiceCost();
            if (sp < needSp)
            {
                logger.LogError($"주사위 추가를 위한 SP가 모자랍니다.: playerId:{userId} sp:{sp} 필요sp: {needSp}");
                return;
            }

            sp -= needSp;
            getDiceCount += 1;
            var selectedDeckDice = Deck[deckIndex];
            Field[fieldIndex] = new FieldDice()
            {
                diceId = selectedDeckDice.diceId,
                diceScale = 0,
                index = fieldIndex
            };
        }

        public void MergeDice(byte sourceDiceFieldIndex, byte targetDiceFieldIndex)
        {
            logger.Log(
                $"[MergeDice] sourceDiceFieldIndex:{sourceDiceFieldIndex} targetDiceFieldIndex{targetDiceFieldIndex}");

            var targetFieldDice = Field[targetDiceFieldIndex];
            if (targetFieldDice.IsEmpty)
            {
                logger.LogError(
                    $"필드에 주사위가 존재하지 않습니다.: playerId:{userId}, fieldIndex:{targetDiceFieldIndex}");
                return;
            }

            // 인게임 주사위의 최대 등급 여부를 체크한다.
            short maxInGameUp = 5;
            if (targetFieldDice.diceScale >= maxInGameUp)
            {
                logger.LogError($"주사위 눈금이 최대치입니다.: playerId:{userId}, fieldIndex:{targetDiceFieldIndex}");
                return;
            }

            var sourceFieldDice = Field[sourceDiceFieldIndex];
            if (sourceFieldDice.IsEmpty)
            {
                logger.LogError(
                    $"필드에 주사위가 존재하지 않습니다.: playerId:{userId}, fieldIndex:{sourceDiceFieldIndex}");
                return;
            }

            // 주사위 아이디 체크
            if (sourceFieldDice.diceId != targetFieldDice.diceId)
            {
                logger.LogError(
                    $"병합하려는 주사위의 아이디가 다릅니다.: playerId:{userId}, source:{sourceFieldDice.diceId} target:{targetFieldDice.diceId}");
                return;
            }

            if (sourceFieldDice.diceScale != targetFieldDice.diceScale)
            {
                logger.LogError(
                    $"필드에 주사위가 존재하지 않습니다.: playerId:{userId}, fieldIndex:{sourceDiceFieldIndex}");
                return;
            }

            // Deck에서 랜덤으로 주사위를 선택한다
            int randDeckIndex = Random.Range(0, Deck.Count);
            var selectedDeck = Deck[randDeckIndex];
            if (selectedDeck.IsEmpty)
            {
                logger.LogError(
                    $"덱에 주사위가 존재하지 않습니다.: playerId:{userId}, selectedDeckIndex:{randDeckIndex}");
                return;
            }

            Field[targetDiceFieldIndex] = new FieldDice()
            {
                diceId = selectedDeck.diceId,
                diceScale = ++sourceFieldDice.diceScale,
                index = targetDiceFieldIndex
            };

            // 선택 주사위는 제거한다.
            Field[sourceDiceFieldIndex] = FieldDice.Empty;
        }

        public void UpgradeIngameLevel(int diceId)
        {
            logger.Log($"[UpgradeIngameLevel] diceId:{diceId}");
        
            var deckDice = GetDeckDice(diceId);
            if (deckDice.IsEmpty)
            {
                logger.LogError($"덱에 주사위가 존재하지 않습니다.: playerId:{userId}, diceId:{diceId}");
                return;
            }

            byte MaxInGameUp = 6;
            if (deckDice.inGameLevel >= MaxInGameUp)
            {
                logger.LogError($"덱 주사위 레벨이 최대치입니다.: playerId:{userId}, diceId:{diceId}");
                return;
            }

            // 필요한 SP를 구한다.
            var needSp = GetUpgradeIngameCost(deckDice.inGameLevel);
            // 플레이어 SP를 업데이트 한다.
            if (sp < needSp)
            {
                logger.LogError(
                    $"덱 주사위 업그레이드를 위한 SP가 모자랍니다.: playerId:{userId}, diceId:{diceId}, sp:{sp} 팔요sp:{needSp}");
                return;
            }

            sp -= needSp;

            var deckIndex = GetDeckIndex(diceId);
            Deck[deckIndex] = new DeckDice()
            {
                diceId = deckDice.diceId,
                inGameLevel = ++deckDice.inGameLevel,
                outGameLevel = deckDice.outGameLevel,
            };
        }

        public int GetUpgradeIngameCost(int ingameLevel)
        {
            return TableManager.Get().Vsmode
                .KeyValues[(int) EVsmodeKey.DicePowerUpCost01 + ingameLevel].value;
        }

        public void UpgradSp()
        {
            logger.Log($"[UpgradeSp]");
            // sp 등급 체크
            int MaxSpGrade = 6;
            if (spGrade >= MaxSpGrade)
            {
                logger.LogError($"Sp 등급이 최대치입니다.: playerId:{userId}");
                return;
            }
            
            // SP를 차감한다.
            int needSp = GetUpradeSpCost();
            if (sp < needSp)
            {
                logger.LogError($"Sp 업그레이드를 위한 SP가 모자랍니다.: playerId:{userId} sp:{sp} 필요sp: {needSp}");
                return;
            }

            sp -= needSp;
            spGrade += 1;
        }
    }

    [System.Serializable]
    public struct FieldDice
    {
        public int diceId;
        public byte diceScale;
        public byte index;

        public static readonly FieldDice Empty = new FieldDice() {diceId = 0, diceScale = 0, index = 0};

        public bool IsEmpty => Equals(Empty);

        public override string ToString()
        {
            return $"Id: {diceId}, scale: {diceScale}";
        }
    }

    [System.Serializable]
    public class Field : SyncList<FieldDice>
    {
    }

    [System.Serializable]
    public struct DeckDice
    {
        public int diceId;
        public byte outGameLevel;

        public byte inGameLevel;
        //TODO: entityDice.Count = short.Parse(response.Item["DiceInfo"].L[j].M["Count"].N); 원래 코드에 이런 구문이 있다. 사용처는 없음. 확인이 필요함.

        public override string ToString()
        {
            return $"id: {diceId}, outLv: {outGameLevel}, inLv: {inGameLevel}";
        }

        public static readonly DeckDice Empty = new DeckDice() {diceId = 0, outGameLevel = 0, inGameLevel = 0};
        public bool IsEmpty => Equals(Empty); //TODO: equatable 구현
    }

    [System.Serializable]
    public class Deck : SyncList<DeckDice>
    {
    }
}