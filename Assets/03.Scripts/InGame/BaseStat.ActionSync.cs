using System.Collections;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using UnityEngine;

namespace ED
{
    public partial class BaseStat
    {
        //마스터가 사용하는 액션으로 AI 루틴 수행중에 동기화 액션을 대입하거나
        //실더의 방패막기 같은 AI루틴 이외의 액션을 대입함.
        //_runningActionCoroutine 의 경우 AI루틴 이외의 액션을 실행하는 코루틴만 대입됨.  
        public SyncActionBase RunningAction;
        protected Coroutine _runningActionCoroutine;
        
        //동기화 받는 플레이어가 동기화 받은 액션
        public SyncActionBase SyncAction;
        protected Coroutine _syncActionCoroutine;

        public bool NeedMoveSyncSend
        {
            get
            {
                if (RunningAction != null)
                {
                    return RunningAction.NeedMoveSync;
                }

                return true;
            }
        }

        public void RunLocalAction(IEnumerator action, bool aiStop)
        {
            _runningActionCoroutine = StartCoroutine(RunLocalActionInternal(action, aiStop));
        }

        IEnumerator RunLocalActionInternal(IEnumerator action, bool aiStop)
        {
            if (aiStop)
            {
                StopAI();
            }

            yield return action;
            _runningActionCoroutine = null;
            RunningAction = null;
            
            if (aiStop)
            {
                StartAI();
            }
        }

        public void SyncActionWithTarget(string hash, ActorProxy actorProxy, ActorProxy targetActorProxy)
        {
            if (_syncActionCoroutine != null)
            {
                SyncAction.OnActionCancel(actorProxy);
                StopCoroutine(_syncActionCoroutine);
            }
            
            var action = ActionLookup.GetActionWithTarget(hash);
            SyncAction = action;
            _syncActionCoroutine = StartCoroutine( RunSyncAction(action.Action(actorProxy, targetActorProxy)));
        }

        public IEnumerator RunSyncAction(IEnumerator action)
        {
            yield return action;
            SyncAction = null;
            _syncActionCoroutine = null;
        }
        
        public void SyncActionWithoutTarget(string hash)
        {
            //푸시를 만들면 이 부분에 수행되어야 한다.
            //스턴 중에도 푸시 액션을 허용해야 함.
            
            if (ActorProxy.isCantAI)
            {
                return;
            }
            
            if (_syncActionCoroutine != null)
            {
                SyncAction.OnActionCancel(ActorProxy);
                StopCoroutine(_syncActionCoroutine);
            }

            var action = ActionLookup.GetActionWithoutTarget(hash);
            SyncAction = action;
            _syncActionCoroutine = StartCoroutine( RunSyncAction(action.Action(ActorProxy)));
        }
    }
}