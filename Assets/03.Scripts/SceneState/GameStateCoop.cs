using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateCoop : BaseSceneState
{
    
    #region override

    public override void Awake()
    {
        base.Awake();
    }

    public override void InitializeState(GameStateManager _entity)
    {
        base.InitializeState(_entity);
        
        //
        sceneName = "InGame_Coop_Mirage";
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
        if( !GameStateManager.Get().isDevMode )
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
        
        // 씬로딩이 끝나고 난후
    }

    #endregion
}
