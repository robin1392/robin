using System.Collections;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using Pathfinding;
using UnityEngine;

namespace ED
{
    public partial class Minion
    {
        private Coroutine _ai;
        protected Seeker Seeker;
        protected AIPath AiPath;

        public void SetPathFinding(Seeker seeker, AIPath aiPath)
        {
            Seeker = seeker;
            AiPath = aiPath;
        }
        
        public override void StartAI()
        {
            Seeker.enabled = true;
            AiPath.enabled = true;
            AiPath.isStopped = true;
            if (_ai != null)
            {
                return;
            }
            
            _ai = StartCoroutine(Root());
        }

        public override void StopAI()
        {
            ActorProxy.EnablePathfinding(false);
            
            base.StopAI();
            
            if (_ai == null)
            {
                return;
            }

            StopCoroutine(_ai);
            _ai = null;
        }

        protected WaitForSeconds _waitForSeconds0_1 = new WaitForSeconds(0.1f);
        
        protected virtual IEnumerator Root()
        {
            while (isAlive)
            {
                target = SetTarget();
                if (target != null)
                {
                    yield return Combat();
                }

                yield return _waitForSeconds0_1;
            }
        }

        protected virtual IEnumerator Combat()
        {
            if (ActorProxy == null)
            {
                yield break;
            }
            
            while (!IsTargetInnerRange())
            {
                ApproachToTarget();
                
                yield return null;

                target = SetTarget();
            }

            StopApproachToTarget();
            
            if (target == null || target.ActorProxy == null)
            {
                yield break;
            }

            yield return Attack();
        }

        protected void ApproachToTarget()
        {
            if (target == null)
            {
                return;
            }
            
            if (target.ActorProxy == null)
            {
                return;
            }

            if (ActorProxy == null)
            {
                return;
            }
            
            AiPath.isStopped = false;
            Vector3 targetPos = target.ActorProxy.transform.position + (target.ActorProxy.transform.position - ActorProxy.transform.position).normalized * range;
            Seeker.StartPath(ActorProxy.transform.position, targetPos);
        }

        protected void StopApproachToTarget()
        {
            AiPath.isStopped = true;
        }
        
        public virtual IEnumerator Attack()
        {
            yield return AttackCoroutine(attackSpeed);
        }

        protected IEnumerator AttackCoroutine(float attackSpeed)
        {
            _attackedTarget = target;
            ActorProxy.PlayAnimationWithRelay(AnimationHash.Attack, target);
            
            var tr = ActorProxy.transform;
            var pos = tr.position;
            pos.y = 0;
            tr.position = pos;
            pos = target.transform.position;
            pos.y = 0;
            tr.LookAt(pos);

            float t = 0;
            while (t < attackSpeed)
            {
                t += Time.deltaTime;
                yield return null;

                //TODO: 히스토리: 공격모션 시작과 동시에 대상이 죽을 경우 허공에 칼질하는 문제를 피하려고 삽입된 코드. 동기화가 애매해 주석 처리함, 복구 방법 고려해볼 것. by Kwazii 
                // if (target == null || target.isAlive == false)
                // {
                //     animator.SetTrigger(AnimationHash.Idle);
                //     yield break;
                // }
            }

            if (_attackedTarget != null && _attackedTarget.isAlive == false) _attackedTarget = null;
        }
    }
}