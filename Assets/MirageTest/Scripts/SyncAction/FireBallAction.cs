using System.Collections;
using Cysharp.Threading.Tasks;
using ED;
using UnityEngine;

namespace MirageTest.Scripts.SyncAction
{
    //TODO : 서버에서 타겟을 내려주도록 변경하는 것을 고려한다.
    //       생성 시간을 서버에서 적어두고 이동치를 생성시간을 기준으로 계산한다.
    //       이렇게 변경하면 중간 지점에 재접속해도 자연스러운 연출이 가능하다.
    //       현재는 재진입 시에는 파이어볼이 표시 되지 않는다.
    public class FireBallAction : SyncActionWithTarget
    {
        public override IEnumerator Action(ActorProxy actorProxy, ActorProxy targetActorProxy)
        {
            var fireBall = actorProxy.baseStat as Fireball;
            var actorTransform = actorProxy.transform;
            var target = targetActorProxy.baseStat;

            SoundManager.instance.Play(Global.E_SOUND.SFX_FIREBALL_FIRE);
            
            fireBall.light.enabled = true;
            
            var startPos = actorTransform.position;

            //?
            while (target == null)
            {
                yield return null;
            }
            
            var endPos = target.ts_HitPos.position;
            var distance = Vector3.Distance(startPos, endPos);
            var moveTime = distance / fireBall.moveSpeed;

            float t = 0;
            
            while (t < moveTime)
            {
                if (target != null && target.isAlive)
                {
                    endPos = target.ts_HitPos.position;
                }
                actorTransform.position = Vector3.Lerp(startPos, endPos, t / moveTime);

                t += Time.deltaTime;
                yield return null;
            }

           fireBall.Bomb();
        }
    }
}