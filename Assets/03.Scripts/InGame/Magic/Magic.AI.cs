using System.Collections;
using UnityEngine;

namespace ED
{
    public partial class Magic
    {
        private Coroutine cast;

        public override void StartAI()
        {
            if (_syncActionCoroutine != null)
            {
                //매직은 한번만 수행된다. 동기화 받은 액션이 있다면 이미 수행되고 있는 것이므로 리턴
                return;
            }
            
            cast = StartCoroutine(Cast());
        }

        protected virtual IEnumerator Cast()
        {
            yield break;
        }
        
        public override void StopAI()
        {
            base.StopAI();
            if (cast != null)
            {
                StopCoroutine(cast);
                cast = null;
            }
        }
    }
}