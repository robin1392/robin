using System;
using Photon.Deterministic;
using Quantum;

public unsafe class PlayerInitSystem : SystemSignalsOnly, ISignalOnPlayerDataSet
{
    private const string PLAYER_PROTOTYPE = "Resources/DB/EntityPrototypes/RWPlayer|EntityPrototype";
    
    public void OnPlayerDataSet(Frame f, PlayerRef playerRef)
    {
        if (DoesPlayerExist(f, playerRef)) return;

        Log.Debug($"{f.Context.VsMode.DiceCostUp}");
            
        var playerPrototype = f.FindAsset<EntityPrototype>(PLAYER_PROTOTYPE);
        var entity = f.Create(playerPrototype);

        var playerData =  f.GetPlayerData(playerRef);
        var deck = f.Unsafe.GetPointer<Deck>(entity);
        deck->GuardianId = playerData.GuardianId;
        for (var i =0; i < playerData.DeckDiceIds.Length; ++i)
        {
            var diceId = playerData.DeckDiceIds[i];
            var outGameLevel = playerData.DeckDiceOutGameLevels[i];
            deck->Dices.GetPointer(i)->DiceId = diceId;
            deck->Dices.GetPointer(i)->outGameLevel = outGameLevel;
        }
        
        f.Global->Players.GetPointer(playerRef)->PlayerRef = playerRef;
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