using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomWarsResource.Data;

namespace Quantum {
  unsafe partial class Frame {
      public void GetFieldDiceInfo(PlayerRef playerRef, int fieldIndex, out int diceId, out int diceScale)
      {
          var player = Global->Players[playerRef];
          var field = Get<Field>(player.EntityRef);
          var fieldDice = field.Dices[fieldIndex];
          
          var deck = Get<Deck>(player.EntityRef);
          var deckDice = deck.Dices[fieldDice.DeckIndex];

          diceId = deckDice.DiceId;
          diceScale = fieldDice.DiceScale;
      }
      
      public bool CanCreateFieldDice(PlayerRef playerRef)
      {
          if (IsFieldFull(playerRef))
          {
              return false;
          }
          
          var player = Global->Players[playerRef];
          var sp = Get<Sp>(player.EntityRef);
          var currentSp = sp.CurrentSp;

          return GetDiceCost(playerRef) > currentSp;
      }

      public int GetDiceCost(PlayerRef playerRef)
      {
          var player = Global->Players[playerRef];
          var diceCreation = Get<DiceCreation>(player.EntityRef);
          
          var tableData = Context.TableData;
          
          if(_runtimeConfig.Mode == 1) //Coop
          {
              var startCost = tableData.CoopMode.KeyValues[(int) EVsmodeKey.GetStartDiceCost].value;
              int addDiceCost = tableData.CoopMode.KeyValues[(int) EVsmodeKey.DiceCostUp].value;
              return startCost + (diceCreation.Count * addDiceCost);    
          }
          else
          {
              var startCost = tableData.VsMode.KeyValues[(int) EVsmodeKey.GetStartDiceCost].value;
              int addDiceCost = tableData.VsMode.KeyValues[(int) EVsmodeKey.DiceCostUp].value;
              return startCost + (diceCreation.Count * addDiceCost);   
          }
      }

      public bool IsFieldFull(PlayerRef playerRef)
      {
          var player = Global->Players[playerRef];
          var field = Get<Field>(player.EntityRef);
          for (var i = 0; i < field.Dices.Length; ++i)
          {
              var fieldDice = field.Dices[i];
              if (fieldDice.IsEmpty)
              {
                  return false;
              }
          }

          return true;
      }
  }
}
