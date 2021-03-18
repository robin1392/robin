using System.Collections;
using System.Collections.Generic;
using MirageTest.Scripts;
using UnityEngine;

namespace ED
{
    public class Invincibility : Magic
    {
        protected override IEnumerator Cast()
        {
            if (ActorProxy.isPlayingAI)
            {
                for (int i = 0; i < ActorProxy.diceScale; i++)
                {
                    var m = ActorProxy.GetRandomFirendlyMinion();
                    if (m != null)
                    {
                        m.ActorProxy.AddBuff(BuffInfos.Invincibility, effect);
                    }
                }
            }

            ActorProxy.Destroy();
            yield break;
        }
    }
}
