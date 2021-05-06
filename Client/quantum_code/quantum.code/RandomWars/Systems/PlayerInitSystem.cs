using System;
using Photon.Deterministic;
using Quantum;
using RandomWarsResource.Data;

public unsafe class PlayerInitSystem : SystemSignalsOnly, ISignalOnPlayerDataSet
{
    private static readonly string PLAYER_PROTOTYPE = "Resources/DB/EntityPrototypes/RWPlayer|EntityPrototype";
    
    public void OnPlayerDataSet(Frame f, PlayerRef playerRef)
    {
        if (DoesPlayerExist(f, playerRef)) return;

        var playerPrototype = f.FindAsset<EntityPrototype>(PLAYER_PROTOTYPE);
        var entity = f.Create(playerPrototype);

        var playerData =  f.GetPlayerData(playerRef);

        if (playerData.IsBot)
        {
            f.Add<PlayerBot>(entity);
        }
        
        var deck = f.Unsafe.GetPointer<Deck>(entity);
        deck->GuardianId = playerData.GuardianId;
        for (var i =0; i < playerData.DeckDiceIds.Length; ++i)
        {
            var diceId = playerData.DeckDiceIds[i];
            var outGameLevel = playerData.DeckDiceOutGameLevels[i];
            deck->Dices.GetPointer(i)->DiceId = diceId;
            deck->Dices.GetPointer(i)->outGameLevel = outGameLevel;
        }
        
        var field = f.Unsafe.GetPointer<Field>(entity);
        for (var i =0; i < field->Dices.Length; ++i)
        {
            field->Dices.GetPointer(i)->DeckIndex = -1;
        }

        var rwPlayer = f.Global->Players.GetPointer(playerRef); 
        rwPlayer->PlayerRef = playerRef;
        rwPlayer->EntityRef = entity;

        var sp = f.Unsafe.GetPointer<Sp>(entity);

        var tableData = f.Context.TableData;
        if(f.RuntimeConfig.Mode == 1) //Coop
        {
            sp->CurrentSp = tableData.CoopMode.KeyValues[(int) ECoopModeKey.GetStartSP].value;    
        }
        else
        {
            sp->CurrentSp = tableData.VsMode.KeyValues[(int) EVsmodeKey.GetStartSP].value;    
        }

        sp->CommingSpGrade = 1;
        sp->CommingSp = f.CalculateCommingSp(sp->CommingSpGrade);
        
        f.Events.PlayerInitialized(playerRef);
        f.Events.SpIncreased(playerRef);
        f.Events.CommingSpChanged(playerRef);
        f.Events.CommingSpGradeChanged(playerRef);
        f.Events.DiceCreationCountChanged(playerRef);
    }

    private bool DoesPlayerExist(Frame f, PlayerRef playerRef)
    {
        foreach (var player in f.GetComponentIterator<RWPlayer>())
        {
            if (player.Component.PlayerRef == playerRef)
            {
                return true;
            }
        }

        return false;
    }
}