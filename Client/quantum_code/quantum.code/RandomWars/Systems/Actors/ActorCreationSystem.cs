namespace Quantum.Actors
{
    public unsafe struct ActorCreationFilter
    {
        public EntityRef EntityRef;
        public ActorCreation* ActorCreation;
    }

    public unsafe class ActorCreationSystem : SystemMainThreadFilter<ActorCreationFilter>
    {
        private static readonly string DICE_ACTOR_PROTOTYPE = "Resources/DB/EntityPrototypes/DiceActor|EntityPrototype"; 
        private static readonly string DICE_MAGIC_ACTOR_PROTOTYPE = "Resources/DB/EntityPrototypes/DiceMagicActor|EntityPrototype";
        private static readonly string TOWER_ACTOR_PROTOTYPE = "Resources/DB/EntityPrototypes/TowerActor|EntityPrototype";
        
        public override void Update(Frame f, ref ActorCreationFilter filter)
        {
            if (f.IsVerified == false)
            {
                return;
            }

            if (filter.ActorCreation->Delay > 0)
            {
                filter.ActorCreation->Delay -= 1;
                return;
            }
            
            var actorCreation = f.Get<ActorCreationSpec>(filter.EntityRef);
            CreateActor(f, actorCreation);

            f.Destroy(filter.EntityRef);
        }

        private void CreateActor(Frame f, ActorCreationSpec actorCreation)
        {
            if (actorCreation.ActorType == ActorType.Dice)
            {
                f.Context.TableData.DiceInfo.GetData(actorCreation.DataId, out var data);
                if (data.castType == (int)DiceType.Magic)
                {
                    var actorPrototype = f.FindAsset<EntityPrototype>(DICE_MAGIC_ACTOR_PROTOTYPE);
                    ActorFactory.CreateDiceActor(f, actorCreation, actorPrototype);    
                }
                else
                {
                    var actorPrototype = f.FindAsset<EntityPrototype>(DICE_ACTOR_PROTOTYPE);
                    ActorFactory.CreateDiceActor(f, actorCreation, actorPrototype);
                }
                
            }
            else if (actorCreation.ActorType == ActorType.Tower)
            {
                var actorPrototype = f.FindAsset<EntityPrototype>(TOWER_ACTOR_PROTOTYPE);
                ActorFactory.CreateTowerActor(f, actorCreation, actorPrototype);
            }
        }
    }
}