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
        //덱 유아이의 대상인지를 나타내는 값: 아군 덱을 보는 기능에서 사용중
        public bool IsDeckUITarget;
        [SyncVar] public string userId;

        [SyncVar(hook = nameof(SetNickName))] public string nickName;

        //TODO: 꼭 필요한지 고민이 필요함. connectionId는 재접 시 바뀌어서 사용못하고, 유저아이디는 스트링이어서 부담스러움.
        [SyncVar] public byte ownerTag;
        [SyncVar] public byte team; //상단 캠프, 하단 캠프 두가지로 나뉨. 팀의 개념
        [SyncVar(hook = nameof(SetSpGrade))] public int spGrade;
        [SyncVar(hook = nameof(SetSp))] public int sp;

        [SyncVar(hook = nameof(SetGetDiceCount))]
        public int getDiceCount;

        [SyncVar] public int guardianId;
        public readonly Deck Deck = new Deck();
        public readonly Field Field = new Field();

        public const int FieldCount = 15;

        private bool _initalized;
        [SyncVar(hook = nameof(SetCommingSp))]public int commingSp;

        public bool IsLocalPlayerState => (Client as RWNetworkClient).LocalUserId == userId;

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
            this.guardianId = guadianId;

            foreach (var deckDice in deck)
            {
                Deck.Add(deckDice);
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

            EnableUI = client.enableUI;
            if (!EnableUI)
            {
                return;
            }

            if (client.LocalUserId == userId)
            {
                CameraController.Get().UpdateCameraRotation(team == GameConstants.BottomCamp);
            }

            Deck.OnChange += OnChangeDeckOnClientOnly;
            Field.OnSet += OnChangeFieldOnClientOnly;

            SetSp(sp, sp);
            SetSpGrade(spGrade, spGrade);
            OnChangeDeckOnClientOnly();
            SetGetDiceCount(getDiceCount, getDiceCount);
            SetNickName(nickName, nickName);
        }

        public DeckDice GetDeckDice(int diceId)
        {
            foreach (var deck in Deck)
            {
                if (deck.diceId == diceId)
                {
                    return deck;
                }
            }

            return DeckDice.Empty;
        }

        public void SetNickName(string oldValue, string newValue)
        {
            if (!EnableUI)
            {
                return;
            }

            if (IsLocalPlayerState)
            {
                UI_InGame.Get().SetMyNickName(newValue);
            }
            else
            {
                UI_InGame.Get().SetEnemyNickName(newValue);
            }
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

                UI_InGame.Get().btn_GetDice.EditSpCallback(newValue >= GetDiceCost());
                UI_InGame.Get().button_SP_Upgrade.EditSpCallback(newValue >= GetUpradeSpCost() && spGrade < GameConstants.MaxSpUpgradeLevel);
                UI_InGame.Get().SetSP(newValue);
            }
        }
        
        public void SetCommingSp(int oldValue, int newValue)
        {
            if (!EnableUI)
            {
                return;
            }

            if (IsLocalPlayerState)
            {
                WorldUIManager.Get().SetAddSpText(newValue);
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
                UI_InGame.Get().button_SP_Upgrade.EditSpCallback(sp >= GetUpradeSpCost() && newValue < GameConstants.MaxSpUpgradeLevel);
                UI_InGame.Get().SetSPUpgrade(newValue + 1, GetUpradeSpCost());
                UI_InGame.Get().ShowSpUpgradeMessage();
            }
        }

        private void OnChangeFieldOnClientOnly(int index, FieldDice oldValue, FieldDice newValue)
        {
            if (EnableUI == false)
            {
                return;
            }

            if (IsDeckUITarget == false)
            {
                return;
            }

            var uiDiceField = UI_DiceField.Get();
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
            if (EnableUI == false)
            {
                return;
            }
            
            if(IsLocalPlayerState == false)
            {
                UI_InGame.Get().SetEnemyArrayDeck();
                UI_InGame.Get().SetEnemyUpgrade();
            }
            
            if(IsDeckUITarget == false)
            {
                return;
            }

            var deckArr = Deck.Select(d =>
            {
                TableManager.Get().DiceInfo.GetData(d.diceId, out var diceInfo);
                return (diceInfo, d.inGameLevel);
            }).ToArray();
            
            UI_InGame.Get().SetArrayDeck(deckArr);
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
            if (EnableUI == false)
            {
                return;
            }
            
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

        public void UpgradeIngameLevel(int deckIndex)
        {
            logger.Log($"[UpgradeIngameLevel] deckIndex:{deckIndex}");
        
            var deckDice = Deck[deckIndex];
            if (deckDice.IsEmpty)
            {
                logger.LogError($"덱에 주사위가 존재하지 않습니다.: playerId:{userId}, index:{deckIndex}");
                return;
            }

            if (deckDice.inGameLevel >= GameConstants.MaxIngameUpgradeLevel)
            {
                logger.LogError($"덱 주사위 레벨이 최대치입니다.: playerId:{userId}, diceId:{deckDice.diceId}");
                return;
            }

            // 필요한 SP를 구한다.
            var needSp = GetUpgradeIngameCost(deckDice.inGameLevel);
            // 플레이어 SP를 업데이트 한다.
            if (sp < needSp)
            {
                logger.LogError(
                    $"덱 주사위 업그레이드를 위한 SP가 모자랍니다.: playerId:{userId}, :{deckDice.diceId}, sp:{sp} 팔요sp:{needSp}");
                return;
            }

            sp -= needSp;
            
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

        public void UpgradeSp()
        {
            logger.Log($"[UpgradeSp]");
            // sp 등급 체크
            if (spGrade >= GameConstants.MaxSpUpgradeLevel)
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

            var server = Server as RWNetworkServer;
            commingSp = server.serverGameLogic._gameMode.CalculateSp(this);
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