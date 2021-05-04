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
        private static readonly string ACTOR_PROTOTYPE = "Resources/DB/EntityPrototypes/RWActor|EntityPrototype"; 
        
        public override void Update(Frame f, ref ActorCreationFilter filter)
        {
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
                // f.Remove<ActorCreation>(filter.EntityRef);
                f.Destroy(filter.EntityRef);
            }
        }

        private void CreateActor(Frame f, ActorCreationSpec actorCreationSpec)
        {
            var actorPrototype = f.FindAsset<EntityPrototype>(ACTOR_PROTOTYPE);
            ActorFactory.CreateDiceActor(f, actorCreationSpec, actorPrototype);
        }

        public void OnAdded(Frame f, EntityRef entity, ActorCreation* component)
        {
            component->creationList = f.AllocateList<ActorCreationSpec>();
        }

        public void OnRemoved(Frame f, EntityRef entity, ActorCreation* component)
        {
            f.FreeList(component->creationList);
            component->creationList = default;
            Log.Debug($"ActorCreation Removed");
        }
    }
}