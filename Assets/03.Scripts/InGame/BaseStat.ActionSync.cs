using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using UnityEngine;

namespace ED
{
    public partial class BaseStat
    {
        private Coroutine _action;
        
        public void SyncActionWithTarget(int hash, ActorProxy actor, ActorProxy target)
        {
            if (_action != null)
            {
                StopCoroutine(_action);
            }
            
            _action = StartCoroutine(ActionLookup.GetActionWithTarget(hash).Action(actor, target));
        }
    }
}