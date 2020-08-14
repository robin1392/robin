using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateMain : BaseSceneState
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
        sceneName = "Main";
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
        else
        {
            // 개발 버전이라..중간에서 실행햇을시에..
            //DataPatchManager.Get().JsonDownLoad();
        }
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
