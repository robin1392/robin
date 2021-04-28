namespace Quantum
{
    public unsafe struct TestF
    {
        public EntityRef e;
        public TestTest* t;
    }
    
    public unsafe class TestTestSystem : SystemMainThreadFilter<TestF>
    {
        public override void Update(Frame f, ref TestF filter)
        {
            filter.t->A += 1;
        }
    }
}