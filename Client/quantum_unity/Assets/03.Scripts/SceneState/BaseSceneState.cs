using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class BaseSceneState : BaseState<GameStateManager>
{

    /// <summary>
    /// Scene Name - 
    /// </summary>
    public string sceneName; 
    
    #region override

    public override void EnterState(Action callback = null, object param = null)
    {
        base.EnterState(callback, param);

        SceneManager.sceneLoaded += OnSceneLoadFinish;
    }

    public override void UpdateState(float deltaTime)
    {
        base.UpdateState(deltaTime);
    }

    public override void ExitState(Action callback = null)
    {
        SceneManager.sceneLoaded -= OnSceneLoadFinish;
        
        base.ExitState(callback);
    }

    #endregion


    #region scene move

    public void LoadScene(string sceneName)
    {
        GameStateManager.Get().StartCoroutine(LoadLevelScene(sceneName));
    }

    private IEnumerator LoadLevelScene(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;        // 로딩 80% 이상될때까진 비활성화

        while (!op.isDone)
        {
            // 현재 씬이 로딩 씬 이라면...다음씬으로 넘어가는 프로그래스를 보여준다..
            if (SceneManager.GetActiveScene().name == Global.g_sceneLoadingName)
            {
                if (UI_Loading.Get() != null)
                {
                    UI_Loading.Get().SetViewProgress(op.progress);
                }

                if (op.progress > 0.8f)
                {
                    if (UI_Loading.Get() != null)
                    {
                        UI_Loading.Get().SetViewProgress(1.11f);
                        yield return new WaitForSeconds(1.5f);
                    }    
                }
            }
            
            op.allowSceneActivation = op.progress > 0.8f;
            if (op.allowSceneActivation)
            {
                // 현재 씬이 로딩 씬이라면 비활성 시킨다
                if (SceneManager.GetActiveScene().name == Global.g_sceneLoadingName)
                {
                    SceneManager.UnloadSceneAsync(Global.g_sceneLoadingName);
                }
            }
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    public virtual void OnSceneLoadFinish(Scene scene, LoadSceneMode mode) { }
    
    #endregion
    
    
    
    
    
}
