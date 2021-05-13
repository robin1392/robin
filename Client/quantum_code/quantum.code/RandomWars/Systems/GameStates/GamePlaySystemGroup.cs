using Quantum.Task;

namespace Quantum
{
    public unsafe class GamePlaySystemGroup : SystemMainThreadGroup
    {
        public GamePlaySystemGroup(string update, params SystemMainThread[] children) : base(update, children)
        {
        }

        protected override TaskHandle Schedule(Frame f, TaskHandle taskHandle)
        {
            if (f.Global->State == StateType.GameOver)
            {
                return taskHandle;
            }
            
            return base.Schedule(f, taskHandle);
        }
    }
}