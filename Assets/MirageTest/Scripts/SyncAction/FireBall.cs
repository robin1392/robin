using System.Collections;

namespace MirageTest.Scripts.SyncAction
{
    public class FireBallAction : SyncActionWithTarget
    {
        public override IEnumerator Action(ActorProxy actor, ActorProxy target)
        {
            yield return null;
        }
    }
}