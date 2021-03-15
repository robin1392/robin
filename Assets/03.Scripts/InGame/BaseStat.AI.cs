using Mono.Cecil.Cil;

namespace ED
{
    public partial class BaseStat
    {
        public virtual void StartAI()
        {
        }
        
        public virtual void StopAI()
        {
            if (RunningAction != null)
            {
                RunningAction.OnActionCancel(ActorProxy);
                RunningAction = null;
            }
            
            if (_runningActionCoroutine != null)
            {
                StopCoroutine(_runningActionCoroutine);
                _runningActionCoroutine = null;
            }
        }
        
        public void StopAllAction()
        {
            StopAI();

            if (SyncAction != null)
            {
                SyncAction.OnActionCancel(ActorProxy);
                SyncAction = null;
            }
            
            if (_syncActionCoroutine != null)
            {
                StopCoroutine(_syncActionCoroutine);
                _syncActionCoroutine = null;
            }
        }
    }
}