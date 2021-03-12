using System.Collections;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using Pathfinding;
using UnityEngine;

namespace ED
{
    public partial class Minion
    {
        private Coroutine ai;
        protected Seeker Seeker;
        protected AIPath AiPath;

        public void SetPathFinding(Seeker seeker, AIPath aiPath)
        {
            Seeker = seeker;
            AiPath = aiPath;
        }
        
        public void StartAI()
        {
            // behaviourTreeOwner.behaviour.Stop();
            ai = StartCoroutine(Root());
            Seeker.enabled = true;
            AiPath.enabled = true;
            AiPath.isStopped = true;
        }

        public void StopAI()
        {
            Seeker.enabled = false;
            AiPath.enabled = false;
            
            if (ai == null)
            {
                return;
            }
            
            StopCoroutine(ai);
            ai = null;
        }

        private WaitForSeconds _waitForSeconds0_1 = new WaitForSeconds(0.1f);
        
        IEnumerator Root()
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

        private IEnumerator Combat()
        {
            while (!IsTargetInnerRange())
            {
                ApproachToTarget();  
                
                yield return null;

                target = SetTarget();
            }

            StopApproachToTarget();
            
            if (target == null)
            {
                yield break;
            }

            yield return Attack();
        }

        private void ApproachToTarget()
        {
            AiPath.isStopped = false;
            Vector3 targetPos = target.transform.position + (target.transform.position - transform.position).normalized * range;
            Seeker.StartPath(transform.position, targetPos);
        }

        private void StopApproachToTarget()
        {
            AiPath.isStopped = true;
        }
        
        public virtual IEnumerator Attack()
        {
            _attackedTarget = target;
            ActorProxy.PlayAnimationWithRelay(_animatorHashAttack, target);
            yield return AttackCoroutine();
        }

        protected IEnumerator AttackCoroutine()
        {
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
                //     animator.SetTrigger(_animatorHashIdle);
                //     yield break;
                // }
            }

            if (_attackedTarget != null && _attackedTarget.isAlive == false) _attackedTarget = null;
        }
    }
}