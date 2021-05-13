using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PoolableObject : MonoBehaviour
{
    public string AssetName;
    public bool PushBackReserved;
    public float PushBackDelay;

    public void ReservePushBack()
    {
        PushBackReserved = true;
        PushBackDelayed().Forget();
    }

    public async UniTask PushBackDelayed()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(PushBackDelay));

        if (this == null)
        {
            return;
        }
        
        if (string.IsNullOrWhiteSpace(AssetName))
        {
            Debug.LogError($"AssetName이 비어있습니다. {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        Pool.Push(AssetName, gameObject);
    }
}
