using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateStart : BaseSceneState
{
    
    #region variable
    // start scene 에서 행동할 상황들
    public Global.E_STARTSTEP startState = Global.E_STARTSTEP.START_NONE; 
    #endregion
    
    #region override

    public override void Awake()
    {
        base.Awake();

        
    }

    public override void InitializeState(GameStateManager _entity)
    {
        base.InitializeState(_entity);
        
        //
        sceneName = "Start Scene";
    }

    public override void OnRelease()
    {
        base.OnRelease();
    }

    #endregion
    
    #region override fsm

    public override void EnterState(Action callback = null, object param = null)
    {
        base.EnterState(callback, param);
        
        //  state 에 들어오면 할일
        startState = Global.E_STARTSTEP.START_CONNECT;
        StartStateAction();
    }

    public override void UpdateState(float deltaTime)
    {
        base.UpdateState(deltaTime);
    }

    public override void ExitState(Action callback = null)
    {
        // state 가 빠져 나가기 전에 할일
        
        
        base.ExitState(callback);
    }

    public override void OnSceneLoadFinish(Scene scene, LoadSceneMode mode)
    {
        base.OnSceneLoadFinish(scene, mode);
        
        // 씬로딩이 끝나고 난후
    }

    #endregion


    #region start state

    public void SetStartState(Global.E_STARTSTEP state)
    {
        startState = state;

        StartStateAction();
    }
    
    private void StartStateAction()
    {
        GameStateManager.Get().StartSceneToDo(startState);
    }

    #endregion
}
