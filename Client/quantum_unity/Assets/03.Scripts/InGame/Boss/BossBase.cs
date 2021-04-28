using System.Collections;
using MirageTest.Scripts;

namespace ED.Boss
{
    public class BossBase : Minion
    {
        protected override IEnumerator Root()
        {
            var bossActorProxy = ActorProxy as BossActorProxy;
            while (isAlive)
            {
                while (bossActorProxy == null || bossActorProxy.isHatched == false)
                {
                    yield return _waitForSeconds0_1;
                }

                target = SetTarget();
                if (target != null)
                {
                    yield return Combat();
                }

                yield return _waitForSeconds0_1;
            }
        }
    }
}