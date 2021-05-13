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
                    var go = new GameObject(typeof(T).ToString());
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
        var already = FindObjectsOfType<T>();
        if (already.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        
        DontDestroyOnLoad(gameObject);
        OnAwake();
    }

    protected virtual void OnAwake()
    {

    }
}
