using System;
using System.Collections.Generic;
using Mirage;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.Serialization;
using System.Linq;
using ED;
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
        [SyncVar] public byte team;  //상단 캠프, 하단 캠프 두가지로 나뉨. 팀의 개념
        [SyncVar(hook = nameof(SetSpGrade))] public int spGrade;
        [SyncVar(hook = nameof(SetSp))] public int sp;
        [SyncVar(hook = nameof(SetGetDiceCount))] public int getDiceCount;

        public readonly Deck Deck = new Deck();
        public readonly Field Field = new Field();

        private Dictionary<int, DeckDice> _deckDiceMap = new Dictionary<int, DeckDice>();

        public const int FieldCount = 15;

        private bool _initalized;

        public bool IsLocalPlayerState => (Client as RWNetworkClient).localPlayerId == userId;
        
        public void Init(string userId, string nickName, int sp, DeckDice[] deck, byte tag)
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
            
            NetIdentity.OnStartClient.AddListener(StartClient);
            NetIdentity.OnStopClient.AddListener(StopClient);
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
                UI_InGame.Get().SetEnemyArrayDeck(deckArr.Select(d => d.diceInfo).ToArray());
                UI_InGame.Get().SetEnemyUpgrade(deckArr.Select(d => d.inGameLevel).ToArray());
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
            var startCost = tableManager.Vsmode.KeyValues[(int)EVsmodeKey.GetStartDiceCost].value;
            int addDiceCost = tableManager.Vsmode.KeyValues[(int)EVsmodeKey.DiceCostUp].value;
            return startCost + (getDiceCount * addDiceCost);
        }
        
        public int GetUpradeSpCost()
        {
            return TableManager.Get().Vsmode.KeyValues[(int)EVsmodeKey.GetSPPlusLevelupCost01 + spGrade].value;
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

        [ClientRpc]
        public void AddSpByWave(int sp)
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
        
            // SP를 차감한다.
            int needSp = GetDiceCost();
            // if (sp < needSp)
            // {
            //     logger.LogError($"주사위 추가를 위한 SP가 모자랍니다.: playerId:{userId} sp:{sp} 필요sp: {needSp}");
            //     return;
            // }

            sp -= needSp;
            getDiceCount += 1;
            var selectedDeckDice = Deck[Random.Range(0, Deck.Count)];
            Field[fieldIndex] = new FieldDice()
            {
                diceId = selectedDeckDice.diceId,
                diceScale = 0,
            };
        }
    }
    
    [System.Serializable]
    public struct FieldDice
    {
        public int diceId;
        public byte diceScale;

        public static readonly FieldDice Empty = new FieldDice() { diceId = 0, diceScale = 0 };

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
        public short outGameLevel;
        public byte inGameLevel;
        //TODO: entityDice.Count = short.Parse(response.Item["DiceInfo"].L[j].M["Count"].N); 원래 코드에 이런 구문이 있다. 사용처는 없음. 확인이 필요함.

        public override string ToString()
        {
            return $"id: {diceId}, outLv: {outGameLevel}, inLv: {inGameLevel}";
        }
        
        public static readonly DeckDice Empty = new DeckDice() { diceId = 0, outGameLevel = 0, inGameLevel = 0 };
        public bool IsEmpty => Equals(Empty); //TODO: equatable 구현
    }

    [System.Serializable]
    public class Deck : SyncList<DeckDice>
    {
    }
}
