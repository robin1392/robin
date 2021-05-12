using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                var t = FindObjectOfType<T>();
                if (t == null)
                {
                    var go = new GameObject(nameof(T));
                    t = go.AddComponent<T>();
                }

                _instance = t;
            }

            return _instance;
        }
    }

    public static T Get() => Instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        OnAwake();
    }

    protected virtual void OnAwake()
    {

    }
}
