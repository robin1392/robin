using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class ResourceManager
{
    public static async UniTask<TMonobehavour> LoadMonobehaviourAsync<TMonobehavour>(string assetName, Vector3 position,
        Quaternion rotation) where TMonobehavour : MonoBehaviour
    {
        var go = await LoadGameObjectAsync(assetName, position, rotation);
        return go.GetComponent<TMonobehavour>();
    }
    
    public static async UniTask<TMonobehavour> LoadPoolableAsync<TMonobehavour>(string assetName, Vector3 position,
        Quaternion rotation) where TMonobehavour : PoolableObject
    {
        var mb = await LoadMonobehaviourAsync<TMonobehavour>(assetName, position, rotation);
        mb.AssetName = assetName;
        return mb;
    }
    
    public static async UniTask<GameObject> LoadGameObjectAsync(string assetName, Vector3 position, Quaternion rotation)
    {
        var go = Pool.Pop(assetName);
        if (go != null)
        {
            go.transform.position = position;
            go.transform.rotation = rotation;
            return go;
        }

        return await Addressables.InstantiateAsync(assetName, position, rotation);
    }
    
    public static async UniTask LoadGameObjectAsyncAndReseveDeacivate(string assetName, Vector3 position, Quaternion rotation)
    {
         var go = await LoadGameObjectAsync(assetName, position, rotation);
         var poolableObject = go.GetComponent<PoolableObject>();
         poolableObject.AssetName = assetName;
         poolableObject.ReservePushBack();
    }
}

public static class Pool
{
    private static Dictionary<string, Stack<GameObject>> _pools = new Dictionary<string, Stack<GameObject>>();

    public static void Push(string assetName, GameObject go)
    {
        if (_pools.TryGetValue(assetName, out var stack) == false)
        {
            stack = new Stack<GameObject>();
            _pools.Add(assetName, stack);
        }
        
        go.transform.SetParent(null);
        go.gameObject.SetActive(false);
        stack.Push(go);
    }
    
    public static void Push(PoolableObject poolableObject)
    {
        Push(poolableObject.AssetName, poolableObject.gameObject);
    }

    public static GameObject Pop(string assetName)
    {
        if (_pools.TryGetValue(assetName, out var stack) == false)
        {
            return null;
        }

        if (stack.Count < 1)
        {
            return null;
        }
        
        var go = stack.Pop();
        go.SetActive(true);
        return go;
    }
}
