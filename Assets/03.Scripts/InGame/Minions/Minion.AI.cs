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

        public override void StartAI()
        {
            ActorProxy.lastRecieved = null;
            ActorProxy.EnablePathfinding(true);
            ActorProxy._aiPath.isStopped = true;
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
        protected WaitForSeconds _waitForSeconds0_3 = new WaitForSeconds(0.3f);
        
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
            
            if (target == null || target.CanBeTarget() == false)
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
            
            ActorProxy.EnablePathfinding(true);

            Vector3 targetPos = target.ActorProxy.transform.position + (ActorProxy.transform.position - target.ActorProxy.transform.position).normalized * target.Radius;
            ActorProxy._aiPath.destination = targetPos;
        }

        protected void StopApproachToTarget()
        {
            ActorProxy._aiPath.isStopped = true;
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

                //TODO: ????????????: ???????????? ????????? ????????? ????????? ?????? ?????? ????????? ???????????? ????????? ???????????? ????????? ??????. ???????????? ????????? ?????? ?????????, ?????? ?????? ???????????? ???. by Kwazii 
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