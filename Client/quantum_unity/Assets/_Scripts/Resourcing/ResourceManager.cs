using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Quantum.Actors;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;

public static class ResourceManager
{
    public static async UniTask<AudioClip> LoadClip(string assetName)
    {
        return await Addressables.LoadAssetAsync<AudioClip>(assetName);
    }
    
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

public static class PreloadedResourceManager
{
    private static Dictionary<string, GameObject> _pool = new Dictionary<string, GameObject>();

    public static async UniTask Preload(IEnumerable<string> assetNames)
    {
        var root = new GameObject("Preloaded"); 
        foreach (var assetName in assetNames)
        {
            var go = await ResourceManager.LoadGameObjectAsync(assetName, Vector3.zero, quaternion.identity);
            _pool.Add(assetName, go);
            var actorModel = go.GetComponent<ActorModel>();
            if (actorModel != null)
            {
                actorModel.ResetHealthBar();
            }
            go.SetActive(false);
            go.transform.SetParent(root.transform);
        }
    }
    
    public static TMonobehavour LoadMonobehaviour<TMonobehavour>(string assetName, Vector3 position,
        Quaternion rotation) where TMonobehavour : MonoBehaviour
    {
        var go = LoadGameObject(assetName, position, rotation);
        return go.GetComponent<TMonobehavour>();
    }
    
    public static  TMonobehavour LoadPoolable<TMonobehavour>(string assetName, Vector3 position,
        Quaternion rotation) where TMonobehavour : PoolableObject
    {
        var mb = LoadMonobehaviour<TMonobehavour>(assetName, position, rotation);
        mb.AssetName = assetName;
        return mb;
    }
    
    public static GameObject LoadGameObject(string assetName, Vector3 position, Quaternion rotation)
    {
        var go = Pool.Pop(assetName);
        if (go != null)
        {
            go.transform.position = position;
            go.transform.rotation = rotation;
            return go;
        }

        if (_pool.TryGetValue(assetName, out var goOrigin) == false)
        {
            return null;
        }

        var instance = Object.Instantiate(goOrigin, position, rotation);
        instance.SetActive(true);
        return instance;
    }
    
    public static void LoadGameObjectAndReseveDeacivate(string assetName, Vector3 position, Quaternion rotation)
    {
        var go = LoadGameObject(assetName, position, rotation);
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
