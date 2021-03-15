using System.Collections;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using UnityEngine;

namespace ED
{
    public partial class BaseStat
    {
        public SyncActionBase RunningAction;
        protected Coroutine _localActionCoroutine;
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
            _localActionCoroutine = StartCoroutine(RunLocalActionInternal(action, aiStop));
        }

        IEnumerator RunLocalActionInternal(IEnumerator action, bool aiStop)
        {
            if (aiStop)
            {
                StopAI();
            }

            yield return action;
            _localActionCoroutine = null;
            RunningAction = null;
            
            if (aiStop)
            {
                StartAI();
            }
        }

        public void SyncActionWithTarget(int hash, ActorProxy actorProxy, ActorProxy targetActorProxy)
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
        
        public void SyncActionWithoutTarget(int hash, ActorProxy actorProxy)
        {
            if (_syncActionCoroutine != null)
            {
                SyncAction.OnActionCancel(actorProxy);
                StopCoroutine(_syncActionCoroutine);
            }

            var action = ActionLookup.GetActionWithoutTarget(hash);
            SyncAction = action;
            _syncActionCoroutine = StartCoroutine( RunSyncAction(action.Action(actorProxy)));
        }
    }
}