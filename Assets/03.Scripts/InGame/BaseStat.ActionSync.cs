using System.Collections;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using UnityEngine;

namespace ED
{
    public partial class BaseStat
    {
        public SyncActionBase RunningAction;
        public SyncActionBase SyncAction;
        protected Coroutine _syncActionCoroutine;

        public void SyncActionWithTarget(int hash, ActorProxy actorProxy, ActorProxy targetActorProxy)
        {
            if (_syncActionCoroutine != null)
            {
                SyncAction.OnActionCancel(actorProxy);
                StopCoroutine(_syncActionCoroutine);
            }

            var action = ActionLookup.GetActionWithTarget(hash);
            SyncAction = action;
            _syncActionCoroutine = StartCoroutine( RunAction(action.Action(actorProxy, targetActorProxy)));
        }

        IEnumerator RunAction(IEnumerator action)
        {
            yield return action;
            SyncAction = null;
        }
    }
}