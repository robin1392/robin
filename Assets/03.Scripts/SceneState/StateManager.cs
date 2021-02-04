using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 스테이트 머신 매니저
/// State , Event 는 Enum 으로 정의된 값이 들어오면 된
/// </summary>
/// <typeparam name="STATE"></typeparam>
/// <typeparam name="EVENT"></typeparam>
/// <typeparam name="T"></typeparam>
public class StateManager<STATE, EVENT , T>
{
    //
    public T parent;
 
    #region class variable

    public STATE current_State { get; private set; }
    protected bool _isEntering;

    //
    private Dictionary<STATE, Dictionary<EVENT, STATE>> _stateEventMap;
    private Dictionary<STATE, BaseState<T>> _stateMap;

    public bool isEnable { get {return enableFlag;} }
    public bool enableFlag = false;
    #endregion
    
    #region get state
    public BaseState<T> Current()
    {
        return _stateMap[current_State];
    }

    public void Enable(STATE state)
    {
        if (enableFlag)
        {
            ChangeState(state);
        }
        else
        {
            current_State = state;
            Enable(true);
        }
    }

    public void Enable(bool flag)
    {
        if (enableFlag == flag)
            return;
        else enableFlag = flag;

        if (enableFlag)
        {
            foreach (KeyValuePair<STATE, BaseState<T>> std in _stateMap)
                std.Value.enabled = false;
            
            if(_stateMap.ContainsKey(current_State))
                _stateMap[current_State].EnterState(null);
        }
        else _stateMap[current_State].ExitState(null);
        
    }

    #endregion
    
    
    #region state manager func

    public StateManager(T _parant)
    {
        parent = _parant;
        Initialize();
    }

    public STATE GetCurrentState()
    {
        return current_State;
    }

    public bool GetState(STATE _state, out BaseState<T> state)
    {
        if (_stateMap.ContainsKey(_state))
        {
            state = _stateMap[_state];
            return true;
        }

        state = null;
        return false;
    }

    public bool HasState(STATE _state)
    {
        return _stateMap.ContainsKey(_state);
    }
    
    #endregion
    
    #region init destroy
    public void Initialize()
    {
        _stateEventMap = new Dictionary<STATE, Dictionary<EVENT, STATE>>();
        _stateMap = new Dictionary<STATE, BaseState<T>>();

        _isEntering = false;
    }

    public void Release()
    {
        _stateMap[current_State].ExitState(Destroy);
    }

    public void Destroy()
    {
        foreach ( BaseState<T> ibs in _stateMap.Values)
        {
            ibs.OnRelease();
        }
        
        _stateMap.Clear();
        _stateMap = null;
        
        _stateEventMap.Clear();
        _stateEventMap = null;
    }
    #endregion
    
    #region state change add remove

    /// <summary>
    /// state 추가 
    /// </summary>
    /// <param name="_state"></param>
    /// <param name="_stateBase"></param>
    public void AddState(STATE _state, BaseState<T> _stateBase)
    {
        _stateMap.Add(_state , _stateBase);
        _stateBase.InitializeState(parent);
    }

    /// <summary>
    /// state 상태 변경
    /// </summary>
    /// <param name="_state"></param>
    public bool ChangeState(STATE _state)
    {
        if (_isEntering)
        {
            Debug.LogWarning($"Current : {current_State}");
            Debug.LogWarning("current state is already entering");
            return false;
        }
        
        // 현재 상태 동일 리턴
        if (current_State.Equals(_state))
            return false;
        
        // 해당 상태 없을 경우 리턴
        if (!_stateMap.ContainsKey(_state))
            return false;


        _isEntering = true;
        _stateMap[current_State].ExitState(delegate
        {
            current_State = _state;
            _stateMap[current_State].EnterState(delegate
            {
                _isEntering = false;
            });
        });

        return true;
    }

    public bool ChangeState(EVENT _event)
    {
        if (_isEntering)
        {
            Debug.LogWarning($"Current : {current_State}" + $" Target : {_event}");
            Debug.LogWarning("current state is already entering");
            return false;
        }

        if (_stateEventMap[current_State].ContainsKey(_event))
        {
            // 현재 상태 동일 리턴
            if (_stateMap[current_State].Equals(_stateEventMap[current_State][_event]))
                return false;

            _isEntering = true;
            _stateMap[current_State].ExitState(delegate
            {
                current_State = _stateEventMap[current_State][_event];
                _stateMap[current_State].EnterState(delegate
                {
                    _isEntering = false;
                });
            });
        }
        else
        {
            Debug.LogWarning($"Current : {current_State}" + $" Target : {_event} state Map Event Error" );
            return false;
        }
        
        return true;
    }
    #endregion
    
    
    #region state event

    /// <summary>
    /// state 로 event 잇는지?
    /// </summary>
    /// <param name="_baseState"></param>
    /// <param name="_targetState"></param>
    /// <returns></returns>
    public EVENT GetEvent(STATE _baseState, STATE _targetState)
    {
        if (!_stateEventMap.ContainsKey(_baseState))
            return default(EVENT);

        foreach (EVENT e in _stateEventMap[_baseState].Keys)
        {
            if (_stateEventMap[_baseState][e].Equals(_targetState))
                return e;
        }
        
        return default(EVENT);
    }

    /// <summary>
    /// 이벤트 등록
    /// </summary>
    /// <param name="_state"></param>
    /// <param name="_event"></param>
    /// <param name="_targetState"></param>
    public void RegistEvent(STATE _state, EVENT _event, STATE _targetState)
    {
        try
        {
            if(!_stateEventMap.ContainsKey(_state))
                _stateEventMap.Add(_state , new Dictionary<EVENT, STATE>());
            
            _stateEventMap[_state].Add(_event , _targetState);
        }
        catch (Exception e)
        {
            Debug.LogWarning($"state map event add error {_event}" + $" {e.ToString()}");
            throw;
        }
    }

    public bool CheckEvent(EVENT _event)
    {
        if (_stateEventMap.ContainsKey(current_State) && _stateEventMap[current_State].ContainsKey(_event))
        {
            if (_stateMap[current_State].Equals(_stateEventMap[current_State][_event]))
                return false;
            else
                return true;
        }
        return false;
    }

    #endregion
}
