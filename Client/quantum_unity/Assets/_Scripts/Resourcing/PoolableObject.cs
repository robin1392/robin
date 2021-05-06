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
        
        OnPushBack();
        Pool.Push(AssetName, gameObject);
    }

    public void OnPushBack()
    {
        gameObject.SetActive(false);
    }
}
