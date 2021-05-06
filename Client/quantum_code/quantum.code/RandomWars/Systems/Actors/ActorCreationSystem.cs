namespace Quantum.Actors
{
    public unsafe struct ActorCreationFilter
    {
        public EntityRef EntityRef;
        public ActorCreation* ActorCreation;
    }

    public unsafe class ActorCreationSystem : SystemMainThreadFilter<ActorCreationFilter>,
        ISignalOnComponentAdded<ActorCreation>, ISignalOnComponentRemoved<ActorCreation>
    {
        private static readonly string DICE_ACTOR_PROTOTYPE = "Resources/DB/EntityPrototypes/DiceActor|EntityPrototype"; 
        // private static readonly string TOWER_ACTOR_PROTOTYPE = "Resources/DB/EntityPrototypes/TowerActor|EntityPrototype";
        
        public override void Update(Frame f, ref ActorCreationFilter filter)
        {
            if (f.IsVerified == false)
            {
                return;
            }
            
            var creationCountPerFrame = 1;
            var list = f.ResolveList(filter.ActorCreation->creationList);
            var count = 0;
            for (var i = list.Count - 1; i >= 0; --i)
            {
                count++;
                if (count > creationCountPerFrame)
                {
                    break;
                }
                
                CreateActor(f, list[i]);
                list.RemoveAt(i);
            }

            if (list.Count < 1)
            {
                f.Destroy(filter.EntityRef);
            }
        }

        private void CreateActor(Frame f, ActorCreationSpec actorCreationSpec)
        {
            if (actorCreationSpec.ActorType == ActorType.Dice)
            {
                var actorPrototype = f.FindAsset<EntityPrototype>(DICE_ACTOR_PROTOTYPE);
                ActorFactory.CreateDiceActor(f, actorCreationSpec, actorPrototype);
            }
        }

        public void OnAdded(Frame f, EntityRef entity, ActorCreation* component)
        {
            component->creationList = f.AllocateList<ActorCreationSpec>();
        }

        public void OnRemoved(Frame f, EntityRef entity, ActorCreation* component)
        {
            f.FreeList(component->creationList);
            component->creationList = default;
        }
    }
}