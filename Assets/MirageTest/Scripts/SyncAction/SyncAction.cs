using System.Collections;

namespace MirageTest.Scripts.SyncAction
{
    public abstract class SyncActionBase
    {
        public virtual void OnActionCancel(ActorProxy actorProxy)
        {
        }
    }
    
    public abstract class SyncActionWithTarget : SyncActionBase
    {
        public IEnumerator ActionWithSync(ActorProxy actor, ActorProxy target)
        {
            actor.SyncActionWithTarget(actor.Client.Connection.Identity.NetId, GetType().GetHashCode(), target.NetId);
            actor.baseStat.RunningAction = this;
            yield return Action(actor, target);
            actor.baseStat.RunningAction = null;
        }

        public abstract IEnumerator Action(ActorProxy actor, ActorProxy target);
    }
}