using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BaseState<T> : MonoBehaviour
{
    
    public T entity { get; private set; }

    public virtual void Awake()
    {
        enabled = false;
    }
    
    // Update is called once per frame
    public virtual void Update()
    {
        UpdateState(Time.deltaTime);
    }

    public virtual void InitializeState(T _entity)
    {
        entity = _entity;
    }

    public virtual void EnterState(Action callback = null, object param = null)
    {
        enabled = true;

        if (callback != null)
            callback();
    }

    public virtual void UpdateState(float deltaTime)
    {
        
    }

    public virtual void ExitState(Action callback = null)
    {
        if (callback != null)
            callback();
        
        enabled = false;
    }


    public virtual void OnRelease()
    {
        enabled = false;
        Destroy(this);
    }
}
