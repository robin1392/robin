using Photon.Deterministic;
using Quantum.Actors;
using RandomWarsResource.Data;

namespace Quantum
{
    public unsafe class BattleModeSystem : SystemSignalsOnly, ISignalOnWave, ISignalOnPlayerDataSet, ISignalOnTowerDestroyed
    {
        public override void OnInit(Frame f)
        {
            f.Context.SetNavmesh(f.FindAsset<NavMesh>("Resources/DB/BattleMap_Navmesh"));
        }

        public void OnWave(Frame f, int wave)
        {
            var endWave = f.Context.TableData.VsMode.KeyValues[(int) EVsmodeKey.EndWave].value;
            if(wave > endWave)
            {
                GameOver(f);
                return;
            }
            
            for (var i = 0; i < f.Global->Players.Length; ++i)
            {
                var rwPlayer = f.Global->Players[i];
                CreateActorByPlayerFieldDice(f, rwPlayer);
            }
        }

        void CreateActorByPlayerFieldDice(Frame f, RWPlayer player)
        {
            var playerEntity = player.EntityRef;
            var diceInfos = f.Context.TableData.DiceInfo;
            var field = f.Get<Field>(playerEntity);
            var deck = f.Get<Deck>(playerEntity);

            for (byte fieldIndex = 0; fieldIndex < field.Dices.Length; ++fieldIndex)
            {
                var fieldDice = field.Dices[fieldIndex];
                if (fieldDice.IsEmpty)
                {
                    continue;
                }
                
                var deckDice = deck.Dices[fieldDice.DeckIndex];
                
                var creationEntity = f.Create();
                f.Add<ActorCreationSpec>(creationEntity);
                f.Add<ActorCreation>(creationEntity);
                var creationSpec = f.Unsafe.GetPointer<ActorCreationSpec>(creationEntity);
                creationSpec->Owner = player.PlayerRef;
                creationSpec->ActorType = ActorType.Dice;
                creationSpec->DataId = deckDice.DiceId;
                creationSpec->IngameLevel = deckDice.InGameLevel;
                creationSpec->OutgameLevel = deckDice.OutGameLevel;
                creationSpec->DiceScale = fieldDice.DiceScale;
                creationSpec->Position = f.Context.FieldPositions.GetPosition(player.Team, fieldIndex);
                creationSpec->FieldIndex = fieldIndex;
                creationSpec->Team = player.Team;
                
                var creation = f.Unsafe.GetPointer<ActorCreation>(creationEntity);
                creation->Delay = fieldIndex;
            }
        }

        public void OnPlayerDataSet(Frame f, PlayerRef playerRef)
        {
            var rwPlayer = f.Global->Players.GetPointer(playerRef);
            if (playerRef == 0)
            {
                rwPlayer->Team = GameConstants.BottomCamp;
                CreateTower(f, rwPlayer, f.Context.FieldPositions.GetBottomPlayerPosition());
            }
            else
            {
                rwPlayer->Team = GameConstants.TopCamp;
                CreateTower(f, rwPlayer, f.Context.FieldPositions.GetTopPlayerPosition());
            }
        }

        void CreateTower(Frame f, RWPlayer* rwPlayer, FPVector2 position)
        {
            var entityRef = f.Create();
            f.Add<ActorCreation>(entityRef);
            f.Add<ActorCreationSpec>(entityRef);
            var actorCreation = f.Unsafe.GetPointer<ActorCreationSpec>(entityRef);
            actorCreation->Owner = rwPlayer->PlayerRef;
            actorCreation->ActorType = ActorType.Tower;
            actorCreation->Position = position;
            actorCreation->Team = rwPlayer->Team;
        }

        public void OnTowerDestroyed(Frame f, EntityRef entity)
        {
            GameOver(f);
        }

        public void GameOver(Frame f)
        {
            f.Global->State = StateType.GameOver;
            f.Events.GameOver();
        }
    }
}