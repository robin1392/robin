using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mirage;
using UnityEngine;
using UnityEngine.Serialization;

namespace MirageTest.Scripts.Entities
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class PlayerState : NetworkBehaviour
    {
        [SyncVar] public string userId;
        [SyncVar] public string nickName;
        [SyncVar] public int spGrade;
        [SyncVar] public int sp;
        
        public readonly Deck Deck = new Deck();
        public readonly Field Field = new Field();

        public DeckDice a;

        private Dictionary<int, DeckDice> _deckDiceMap = new Dictionary<int, DeckDice>();
        
        public const int FieldCount = 15;

        private bool _initalized;
        
        public void Init(string userId, string nickName, int sp, DeckDice[] deck)
        {
            if (_initalized)
            {
                ED.Debug.LogError("Init이 두번 호출됨.");
            }
            
            this.userId = userId;
            this.nickName = nickName;
            this.sp = sp;
            
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

        private void Update()
        {
            if (IsClient == false)
            {
                return;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                foreach (var d in Deck)
                {
                    Debug.Log(d.diceId);
                }
            }
        }

        [Server]
        public void SpawnMinions(ActorProxy actorPrefab, int team, byte spawnSlot)
        {
            var diceInfos = TableManager.Get().DiceInfo;

            for (var fieldIndex = 0; fieldIndex < Field.Count; ++fieldIndex)
            {
                var fieldSlot = Field[fieldIndex];
                if (fieldSlot.IsEmpty)
                {
                    continue;
                }
                
                if (diceInfos.GetData(fieldSlot.diceId, out var diceInfo) == false)
                {
                    ED.Debug.LogError($"다이스정보 {fieldSlot.diceId}가 없습니다. UserId : {userId} 필드 슬롯 : {fieldIndex}");
                    continue;
                }

                var actor = Instantiate(actorPrefab);
                actor.owner = userId;
                actor.team = team;
                actor.spawnSlot = spawnSlot;

                var diceId = diceInfo.id;
                var outGameLevel = _deckDiceMap[diceInfo.id].outGameLevel;
                
                
                ServerObjectManager.Spawn(actor.NetIdentity);
            }
        }

        public DeckDice GetDeckDice(int diceId)
        {
            if (_deckDiceMap.TryGetValue(diceId, out var deckDice))
            {
                return deckDice;
            }

            return DeckDice.Empty;
        }
    }
    
    [System.Serializable]
    public struct FieldDice
    {
        public int diceId;
        public int diceScale;

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
        public short inGameLevel;
        //TODO: entityDice.Count = short.Parse(response.Item["DiceInfo"].L[j].M["Count"].N); 원래 코드에 이런 구문이 있다. 사용처는 없음. 확인이 필요함.

        public override string ToString()
        {
            return $"id: {diceId}, outLv: {outGameLevel}, inLv: {inGameLevel}";
        }
        
        public static readonly DeckDice Empty = new DeckDice() { diceId = 0, outGameLevel = 0, inGameLevel = 0 };
    }

    [System.Serializable]
    public class Deck : SyncList<DeckDice>
    {
    }
}
