#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Minion_Ninja : Minion
    {
        protected override IEnumerator Root()
        {
            //BuffState 셋 대기
            yield return null;
            
            var transform = ActorProxy.transform;
            while (isAlive)
            {
                animator.SetFloat("MoveSpeed", 1f);
                while (ActorProxy.isClocking)
                {
                    transform.position += (isBottomCamp ? Vector3.forward : Vector3.back) * moveSpeed * Time.deltaTime;
                    yield return null;
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
