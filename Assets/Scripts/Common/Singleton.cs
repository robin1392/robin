using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.PlayerLoop;

/// <summary>
/// 싱글톤 정의
/// Singleton 은 기본적으로 DontDestroyOnLoad 를 가진다
/// SingletonDestroy 는 싱글톤으로 쓰면서 씬 이동시 삭제되어야 될 클래스 적용
/// </summary>


#region Singlton
public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    private static T _instance = null;
    public int instanceID;

    public static T Get()
    {
        if (_instance == null)
        {
            _instance = GameObject.FindObjectOfType(typeof(T)) as T;
            if (_instance == null)
            {
                // 이런경우도 null 이라면..자동생성이 맞긴한데
                // 주로 manager 에서 쓰이기 때문에 고민좀 해봐야될듯하다
            }
            else
                _instance.Init();

        }
        
        return _instance;
    }

    public virtual void Awake()
    {
        Init();
    }


    protected virtual void Init()
    {
        if (_instance == null)
        {
            instanceID = GetInstanceID();
            _instance = this as T;
        }

        if (instanceID == 0)
            instanceID = GetInstanceID();
        
        DontDestroyOnLoad(base.gameObject);
    }

    public virtual void OnDestroy()
    {
        _instance = null;
    }
}
#endregion



#region Singloton Destroy

public class SingletonDestroy<T> : MonoBehaviour where T : SingletonDestroy<T>
{
    private static T _instance = null;
    public int instanceID;

    public static T Get()
    {
        if (_instance == null)
        {
            _instance = GameObject.FindObjectOfType(typeof(T)) as T;
            if (_instance == null)
            {
                // 이런경우도 null 이라면..자동생성이 맞긴한데
                // 주로 manager 에서 쓰이기 때문에 고민좀 해봐야될듯하다
            }
            else
                _instance.Init();

        }
        
        return _instance;
    }

    public virtual void Awake()
    {
        Init();
    }


    protected virtual void Init()
    {
        if (_instance == null)
        {
            instanceID = GetInstanceID();
            _instance = this as T;
        }

        if (instanceID == 0)
            instanceID = GetInstanceID();
    }

    public virtual void OnDestroy()
    {
        _instance = null;
    }
}


#endregion

#region singleton photon



public class SingletonPhoton<T> : MonoBehaviourPunCallbacks where T : SingletonPhoton<T>
{
    private static T _instance = null;
    public int instanceID;

    public static T Get()
    {
        if (_instance == null)
        {
            _instance = GameObject.FindObjectOfType(typeof(T)) as T;
            if (_instance == null)
            {
                // 이런경우도 null 이라면..자동생성이 맞긴한데
                // 주로 manager 에서 쓰이기 때문에 고민좀 해봐야될듯하다
            }
            else
                _instance.Init();

        }
        
        return _instance;
    }

    public virtual void Awake()
    {
        Init();
    }


    protected virtual void Init()
    {
        if (_instance == null)
        {
            instanceID = GetInstanceID();
            _instance = this as T;
        }

        if (instanceID == 0)
            instanceID = GetInstanceID();
    }

    public virtual void OnDestroy()
    {
        _instance = null;
    }
}

#endregion




