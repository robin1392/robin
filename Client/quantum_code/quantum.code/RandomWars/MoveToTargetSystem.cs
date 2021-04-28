namespace Quantum
{
    public unsafe struct MoveToTargets
    {
        public EntityRef e;
        public Transform2D* t;
        public MoveToTarget* mt;
    }
    
    public unsafe class MoveToTargetSystem : SystemMainThreadFilter<MoveToTargets>, ISignalOnComponentAdded<MoveToTarget>
    {
        public override void Update(Frame f, ref MoveToTargets filter)
        {
        }

        public void OnAdded(Frame f, EntityRef entity, MoveToTarget* component)
        {
            // Create the BT Agent and pick the AIConfig, if there is any
            var btAgent = new BTAgent();
            f.Set(entity, btAgent);
        }
    }
}