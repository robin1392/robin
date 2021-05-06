using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RandomWarsResource.Data;

namespace Quantum {
  unsafe partial class Frame {
      public bool TryGetFieldDiceInfo(PlayerRef playerRef, int fieldIndex, out int diceId, out int diceScale)
      {
          var player = Global->Players[playerRef];
          var field = Get<Field>(player.EntityRef);
          var fieldDice = field.Dices[fieldIndex];
          
          if(fieldDice.IsEmpty)
          {
              diceId = 0;
              diceScale = 0;
              return false;
          }
          
          var deck = Get<Deck>(player.EntityRef);
          var deckDice = deck.Dices[fieldDice.DeckIndex];

          diceId = deckDice.DiceId;
          diceScale = fieldDice.DiceScale;

          return true;
      }
      
      public bool HasEnouphSpToCreateFieldDice(PlayerRef playerRef)
      {
          var player = Global->Players[playerRef];
          var sp = Get<Sp>(player.EntityRef);
          var currentSp = sp.CurrentSp;

          return CreateFieldDiceCost(playerRef) <= currentSp;
      }

      public int CreateFieldDiceCost(PlayerRef playerRef)
      {
          var player = Global->Players[playerRef];
          var diceCreation = Get<DiceCreation>(player.EntityRef);
          
          var tableData = Context.TableData;
          
          if(_runtimeConfig.Mode == 1) //Coop
          {
              var startCost = tableData.CoopMode.KeyValues[(int) ECoopModeKey.GetStartDiceCost].value;
              int addDiceCost = tableData.CoopMode.KeyValues[(int) ECoopModeKey.DiceCostUp].value;
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
      
      public int GetPowerUpCost(int ingameLevel)
      {
          return Context.TableData.VsMode
              .KeyValues[(int) EVsmodeKey.DicePowerUpCost01 + ingameLevel].value;
      }

      public int SpUpgradeCost(int spUpgrade)
      {
          return Context.TableData.VsMode
              .KeyValues[(int) EVsmodeKey.GetSPPlusLevelupCost01 + spUpgrade - 1].value;
      }

      public int CalculateCommingSp(int spUpgrade)
      {
          var waveForCalculation = Global->Wave;
          if (waveForCalculation < 1)
          {
              waveForCalculation = 1;
          }
          
          var tableData = Context.TableData;
          if (RuntimeConfig.Mode == 1)
          {
              var defaultSp = tableData.CoopMode.KeyValues[(int) ECoopModeKey.DefaultSp].value;
              var upgradeSp = tableData.CoopMode.KeyValues[(int) ECoopModeKey.UpgradeSp].value;
              var waveSp = tableData.CoopMode.KeyValues[(int) ECoopModeKey.WaveSp].value;
              var spByUpgrade = (spUpgrade - 1) * upgradeSp;
              return defaultSp + waveForCalculation * (waveSp + spByUpgrade);   
          }
          else
          {
              var defaultSp = tableData.VsMode.KeyValues[(int) EVsmodeKey.DefaultSp].value;
              var upgradeSp = tableData.VsMode.KeyValues[(int) EVsmodeKey.UpgradeSp].value;
              var waveSp = tableData.VsMode.KeyValues[(int) EVsmodeKey.WaveSp].value;
              var spByUpgrade = (spUpgrade - 1) * upgradeSp;
              return defaultSp + waveForCalculation * (waveSp + spByUpgrade);   
          }
      }

      public bool AreEachOtherEnemy(Actor actor, PlayerRef localPlayer)
      {
          return Global->Players[localPlayer].Team != actor.Team;
      }
  }
}
