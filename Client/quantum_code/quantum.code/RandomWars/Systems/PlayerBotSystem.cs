namespace Quantum
{
    public unsafe struct PlayerBotFilter
    {
        public EntityRef EntityRef;
        public PlayerBot* PlayerBot;
        public BTAgent* BtAgent;
        public AIBlackboardComponent* BlackboardComponent;
    }
    
    public unsafe class PlayerBotSystem : SystemMainThreadFilter<PlayerBotFilter>, ISignalOnComponentAdded<PlayerBot>,
        ISignalOnComponentRemoved<AIBlackboardComponent>
    {
        public void OnAdded(Frame f, EntityRef entity, PlayerBot* component)
        {
            BTHelper.SetupBT(f, entity, "PlayerBot");
        }
        
        public void OnRemoved(Frame frame, EntityRef entity, AIBlackboardComponent* component)
        {
            component->FreeBlackboardComponent(frame);
        }

        public override void Update(Frame f, ref PlayerBotFilter filter)
        {
            if (f.Global->State != StateType.Play && f.Global->State != StateType.Countdown)
            {
                return;
            }
            
            filter.BtAgent->Update(f, filter.BtAgent, filter.BlackboardComponent, filter.EntityRef);
        }
    }
}