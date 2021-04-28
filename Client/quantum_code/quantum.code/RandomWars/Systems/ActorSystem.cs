namespace Quantum
{
    public unsafe class ActorSystem : SystemSignalsOnly, ISignalOnComponentAdded<Actor>, ISignalOnComponentRemoved<AIBlackboardComponent>
    {
        public void OnAdded(Frame f, EntityRef entity, Actor* component)
        {
            BTHelper.SetupBT(f, entity);
        }
        
        public void OnRemoved(Frame frame, EntityRef entity, AIBlackboardComponent* component)
        {
            component->FreeBlackboardComponent(frame);
        }
    }
}