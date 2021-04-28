#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class PoolObjectAutoDeactivate : MonoBehaviour {

        #region VARIABLE
        [HideInInspector]
        public string poolName;
        public float lifeTime = 1f;
        #endregion

        #region UNITY_METHOD
        private void OnEnable()
        {
            if(lifeTime > 0)
            {
                StartCoroutine(AutoDeactivate(lifeTime));
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            var rb = GetComponent<Rigidbody>();
            if(rb != null)
            {
                rb.velocity = Vector3.zero;
            }
        }
        #endregion

        #region METHOD
        public void AutoDeactive(float time)
        {
            StopAllCoroutines();
            lifeTime = time;
            StartCoroutine(AutoDeactivate(lifeTime));
        }

        public void Deactive(float delay = 0)
        {
            StopAllCoroutines();
            if (delay <= float.Epsilon)
            {
                PoolManager.instance.DeactivateObject(poolName, gameObject.name);
            }
            else
            {
                StartCoroutine(AutoDeactivate(delay));
            }
        }
        #endregion

        #region COROUTINE
        /// <summary>
        /// 자동 비활성화 코루틴(활성화되면 시작되고 비활성화 되면 멈춘다)
        /// </summary>
        /// <returns></returns>
        private IEnumerator AutoDeactivate(float delay)
        {
            yield return new WaitForSeconds(delay);

            PoolManager.instance.DeactivateObject(poolName, gameObject.name);
        }
        #endregion
    }
}