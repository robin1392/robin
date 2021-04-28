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

    public bool applicateStart = false;
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
        sceneName = "StartScene";
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

        if (applicateStart)
            LoadScene(sceneName);
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
      
        if(applicateStart == false)
            applicateStart = true;
        
        // 씬로딩이 끝나고 난후
        StartStateAction();
        
    }

    #endregion


    #region start state

    IEnumerator waitStarSceneName()
    {
        while (true)
        {
            if (SceneManager.GetActiveScene().name == Global.g_sceneStartName)
            {
                StartStateAction();
                break;
            }
            
            yield return null;
        }
    }

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
