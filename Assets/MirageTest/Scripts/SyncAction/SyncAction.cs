using System.Collections;

namespace MirageTest.Scripts.SyncAction
{
    public abstract class SyncActionWithTarget
    {
        public IEnumerator ActionWithSync(ActorProxy actor, ActorProxy target)
        {
            actor.SyncActionWithTarget(actor.Client.Connection.Identity.NetId, GetType().GetHashCode(), target.NetId);
            yield return Action(actor, target);
        }

        public abstract IEnumerator Action(ActorProxy actor, ActorProxy target);
    }
}