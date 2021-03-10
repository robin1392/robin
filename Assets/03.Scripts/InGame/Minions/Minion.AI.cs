using System.Collections;
using UnityEngine;

namespace ED
{
    public partial class Minion
    {
        private Coroutine ai;
        public void StartAI()
        {
            ai = StartCoroutine(Root());
        }

        public void StopAI()
        {
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
                yield return ApproachToTarget();    
            }

            yield return Attack();
        }

        private IEnumerator ApproachToTarget()
        {
            while(target != null && target.isAlive)
            {
                Vector3 targetPos = target.transform.position + (target.transform.position - transform.position).normalized * range;
                _seeker.StartPath(transform.position, targetPos);
                yield return _waitForSeconds0_1;
            }
        }
        
        public virtual IEnumerator Attack()
        {
            _attackedTarget = target;
            ActorProxy.PlayAnimationWithRelay(_animatorHashAttack, target);
            yield return AttackCoroutine();
        }

        protected IEnumerator AttackCoroutine()
        {
            var pos = transform.position;
            pos.y = 0;
            transform.position = pos;
            pos = target.transform.position;
            pos.y = 0;
            transform.LookAt(pos);

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