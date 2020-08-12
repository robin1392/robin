#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class PoolManager : MonoBehaviour {

        #region SINGLETONE
        public static PoolManager instance;
        #endregion

        #region VARIABLE
        public PoolData data;

        private Dictionary<string, Dictionary<string, Transform>> dic;
        #endregion

        #region UNITY_METHOD
        private void Awake()
        {
            if (instance == null)
                instance = this;

            dic = new Dictionary<string, Dictionary<string, Transform>>();
            MakePool();
        }

        private void OnDestroy()
        {
            instance = null;
        }
        #endregion

        #region METHOD
        /// <summary>
        /// 등록된 개수만큼 풀 생성
        /// </summary>
        private void MakePool()
        {
            if (data == null) return;
            foreach (var poolData in data.listPool)
            {
                dic[poolData.name] = new Dictionary<string, Transform>();

                for(var j = 0; j < poolData.count; ++j)
                {
                    var obj = Instantiate(poolData.obj, transform);
                    obj.gameObject.SetActive(false);
                    obj.gameObject.name = $"{poolData.name}[{j}]";
                    dic[poolData.name].Add(obj.gameObject.name, obj.transform);

                    // InteractionObject과 PoolObjectAutoDeactivate에 오브젝트에 풀 이름 설정
                    var pad = obj.GetComponent<PoolObjectAutoDeactivate>();
                    if(pad != null)
                    {
                        pad.poolName = poolData.name;
                    }
                }
            }
        }

        public void AddPool(GameObject prefab, int count)
        {
            if (prefab == null) return;

            StartCoroutine(AddPoolCoroutine(prefab, count));
        }

        IEnumerator AddPoolCoroutine(GameObject prefab, int count)
        {
            if (dic.ContainsKey(prefab.name) == false)
            {
                dic[prefab.name] = new Dictionary<string, Transform>();
            }

            var originCount = dic[prefab.name].Count;
            for(var i = originCount; i < originCount + count; ++i)
            {
                var obj = Instantiate(prefab, transform);
                obj.gameObject.SetActive(false);
                obj.gameObject.name = $"{prefab.name}[{dic[prefab.name].Count}]";
                dic[prefab.name].Add(obj.gameObject.name, obj.transform);

                // InteractionObject과 PoolObjectAutoDeactivate에 오브젝트에 풀 이름 설정
                var pad = obj.GetComponent<PoolObjectAutoDeactivate>();
                if(pad != null)
                {
                    pad.poolName = prefab.name;
                }
                
                yield return new WaitForSeconds(0.2f);
            }
        }

        /// <summary>
        /// 오브젝트 활성화
        /// </summary>
        /// <param name="poolName">풀 이름</param>
        /// <param name="parent">활성화 후 부모 트랜스폼</param>
        /// <returns></returns>
        public T ActivateObject<T>(string poolName, Transform parent = null)
        {
            Transform t = null;

            foreach(var kvp in dic[poolName])
            {
                if(!dic[poolName][kvp.Key].gameObject.activeInHierarchy)
                {
                    t = dic[poolName][kvp.Key];
                    if(parent != null)
                    {
                        t.parent = parent;
                        t.gameObject.SetActive(true);
                    }
                    else
                    {
                        t.gameObject.SetActive(true);
                    }

                    return t.GetComponent<T>();
                }
            }

            Debug.LogError(poolName + " pool is empty.");
            return default;
        }

        public Transform ActivateObject(string poolName, Transform parent = null)
        {
            Transform t = null;

            foreach(var kvp in dic[poolName])
            {
                if(!dic[poolName][kvp.Key].gameObject.activeInHierarchy)
                {
                    t = dic[poolName][kvp.Key];
                    if(parent != null)
                    {
                        t.parent = parent;
                        t.gameObject.SetActive(true);
                    }
                    else
                    {
                        t.gameObject.SetActive(true);
                    }

                    return t;
                }
            }

            Debug.LogError(poolName + " pool is empty.");
            return t;
        }

        /// <summary>
        /// 오브젝트 활성화
        /// </summary>
        /// <param name="poolName">풀 이름</param>
        /// <param name="position">활성화 후 위치</param>
        /// <param name="parent">활성화 후 부모 트랜스폼</param>
        /// <returns></returns>
        public T ActivateObject<T>(string poolName, Vector3 position, Transform parent = null)
        {
            Transform t = null;

            if(dic.ContainsKey(poolName))
            {
                foreach(var kvp in dic[poolName])
                {
                    if(!dic[poolName][kvp.Key].gameObject.activeInHierarchy)
                    {
                        t = dic[poolName][kvp.Key];
                        if (parent != null)
                        {
                            t.parent = parent;
                            t.gameObject.SetActive(true);
                            t.position = position;
                        }
                        else
                        {
                            t.gameObject.SetActive(true);
                            t.position = position;
                        }

                        return t.GetComponent<T>();
                    }
                }
            }
            else
            {
                Debug.LogError(poolName + " pool is not created.");
                return default;
            }

            Debug.LogError(poolName + " pool is empty. And add pool.");
            //AddPool(data.listPool.Find(data => data.name == poolName).obj, 1);
            //return ActivateObject<T>(poolName, position, parent);
            return default;
        }

        public Transform ActivateObject(string poolName, Vector3 position, Transform parent = null)
        {
            Transform t = null;

            if(dic.ContainsKey(poolName))
            {
                foreach(var kvp in dic[poolName])
                {
                    if(!dic[poolName][kvp.Key].gameObject.activeInHierarchy)
                    {
                        t = dic[poolName][kvp.Key];
                        if (parent != null)
                        {
                            t.parent = parent;
                            t.gameObject.SetActive(true);
                            t.position = position;
                        }
                        else
                        {
                            t.gameObject.SetActive(true);
                            t.position = position;
                        }

                        return t;
                    }
                }
            }
            else
            {
                Debug.LogError(poolName + " pool is not created.");
                return t;
            }

            Debug.LogError(poolName + " pool is empty. And add pool.");
            AddPool(data.listPool.Find(data => data.name == poolName).obj, 1);
            return ActivateObject(poolName, position, parent);
        }

        /// <summary>
        /// 오브젝트 비활성화
        /// </summary>
        /// <param name="poolName">풀 이름</param>
        /// <param name="objectName">오브젝트 이름</param>
        public void DeactivateObject(string poolName, string objectName, float delayTime = 0)
        {
            StartCoroutine(DeactivateCoroutine(poolName, objectName, delayTime));
        }

        private IEnumerator DeactivateCoroutine(string poolName, string objectName, float delayTime = 0)
        {
            if(delayTime > 0)
                yield return new WaitForSeconds(delayTime);

            if(dic.ContainsKey(poolName) && dic[poolName].ContainsKey(objectName))
            {
                dic[poolName][objectName].parent = transform;
                dic[poolName][objectName].gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 풀 안의 모든 오브젝트 비활성화
        /// </summary>
        /// <param name="poolName">풀 이름</param>
        public void DeactivateAllObjects(string poolName)
        {
            foreach(var kvp in dic[poolName])
            {
                kvp.Value.parent = transform;
                kvp.Value.gameObject.SetActive(false);
            }
        }
        #endregion
    }
}
