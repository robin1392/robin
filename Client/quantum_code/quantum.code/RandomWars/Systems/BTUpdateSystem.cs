namespace Quantum
{
    public unsafe struct BTFilter
    {
        public EntityRef e;
        public BTAgent* bt;
        public AIBlackboardComponent* bb;
    }

    public unsafe class BTUpdateSystem : SystemMainThreadFilter<BTFilter>
    {
        public override void Update(Frame f, ref BTFilter filter)
        {
            filter.bt->Update(f, filter.bt, filter.bb, filter.e);
        }
    }
}