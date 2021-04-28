#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS
#define ENABLE_LOG
#endif

#define NETWORK_ACT

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
//using RandomWarsProtocol;
using CodeStage.AntiCheat.ObscuredTypes;

using Service.Core;
using Template.Account.GameBaseAccount.Common;
using Template.User.RandomwarsUser.Common;

public class GameStateManager : Singleton<GameStateManager>
{
    #region system variable
    public StateManager<Global.E_GAMESTATE, Global.E_STATEACTION, GameStateManager> gameState = null;

    private Global.E_STATEACTION _nextAction;

    private Action _callBackLoadingSuccess;
    #endregion
    
    
    #region game variable
    
    public bool isDevMode = false;

    #endregion


    #region unity base

    public override void Awake()
    {
        
        if ( GameStateManager.Get() != null && this != GameStateManager.Get() )
        {
            GameObject.Destroy(this.gameObject);
            return;
        }
        
        base.Awake();


        Application.logMessageReceived += OnLogHandle;

        InitializeGameStateManager();
    }
    
    public override void OnDestroy()
    {
        if(gameState != null)
            gameState.Destroy();
        
        Application.logMessageReceived -= OnLogHandle;
        
        base.OnDestroy();
    }

    #endregion


    #region init destroy

    public void InitializeGameStateManager()
    {
        // 화면 안꺼지게
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        
        
        gameState = new StateManager<Global.E_GAMESTATE, Global.E_STATEACTION, GameStateManager>(this);
        
        gameState.AddState(Global.E_GAMESTATE.STATE_START , this.gameObject.AddComponent<GameStateStart>());
        gameState.AddState(Global.E_GAMESTATE.STATE_LOADING , this.gameObject.AddComponent<GameStateLoading>());
        gameState.AddState(Global.E_GAMESTATE.STATE_MAIN , this.gameObject.AddComponent<GameStateMain>());
        gameState.AddState(Global.E_GAMESTATE.STATE_INGAME , this.gameObject.AddComponent<GameStateInGame>());
        gameState.AddState(Global.E_GAMESTATE.STATE_COOP , this.gameObject.AddComponent<GameStateCoop>());
        
        gameState.RegistEvent(Global.E_GAMESTATE.STATE_LOADING , Global.E_STATEACTION.ACTION_START , Global.E_GAMESTATE.STATE_START);
        gameState.RegistEvent(Global.E_GAMESTATE.STATE_LOADING , Global.E_STATEACTION.ACTION_MAIN , Global.E_GAMESTATE.STATE_MAIN);
        gameState.RegistEvent(Global.E_GAMESTATE.STATE_LOADING , Global.E_STATEACTION.ACTION_INGAME , Global.E_GAMESTATE.STATE_INGAME);
        gameState.RegistEvent(Global.E_GAMESTATE.STATE_LOADING , Global.E_STATEACTION.ACTION_COOP , Global.E_GAMESTATE.STATE_COOP);

        
        // 시작을 start scene 부터 
        // 시작을 main 부터..
        if (SceneManager.GetActiveScene().name == Global.g_sceneStartName)
        {
            // 처음 시작 state = state start
            // 정상적으로 시작했을경우니까...
            isDevMode = false;
            gameState.Enable(Global.E_GAMESTATE.STATE_START);
        }
        else if (SceneManager.GetActiveScene().name == Global.g_sceneMainName)
        {   
            isDevMode = true;
            gameState.Enable(Global.E_GAMESTATE.STATE_MAIN);
            
            // 개발 버전이라..중간에서 실행햇을시에..
            if (DataPatchManager.Get().isDataLoadComplete == false)
            {
                DataPatchManager.Get().JsonDownLoad();
            }
                
        }
        else if (SceneManager.GetActiveScene().name == Global.g_sceneInGameBattle)
        {
            isDevMode = true;
            gameState.Enable(Global.E_GAMESTATE.STATE_INGAME);
        }
        else if (SceneManager.GetActiveScene().name == Global.g_sceneInGameCoop)
        {
            isDevMode = true;
            gameState.Enable(Global.E_GAMESTATE.STATE_COOP);
        }
        
        
    }
    

    #endregion
    
    
    #region state system
    public void ChangeScene(Global.E_GAMESTATE nextState)
    {
        gameState.ChangeState(nextState);
    }

    public BaseSceneState GetCurrentState()
    {
        return (BaseSceneState) gameState.Current();
    }

    public Global.E_GAMESTATE GetCurrentName()
    {
        return gameState.GetCurrentState();
    }

    public T GetState<T>() where T : BaseSceneState
    {
        return this.gameObject.GetComponent<T>();
    }
    #endregion
    
    
    #region action state

    /// <summary>
    /// 씬이동시 이벤트로..이동
    /// </summary>
    /// <param name="newAction"></param>
    /// <param name="callBack"></param>
    public void ActionEvent(Global.E_STATEACTION newAction, Action callBack = null)
    {
        if(isDevMode == true)
            isDevMode = false;
        
        _nextAction = newAction;
        _callBackLoadingSuccess = callBack;
        ChangeScene(Global.E_GAMESTATE.STATE_LOADING);
    }
    
    /// <summary>
    /// 로딩 씬에서 다음씬으로 이동할시에
    /// </summary>
    public void LoadingAfterNextScene()
    {
        gameState.ChangeState(_nextAction);
    }
    #endregion
    
    
    #region start scene to do

    public void StartSceneToDo(Global.E_STARTSTEP startState)
    {
        switch (startState)
        {
            case Global.E_STARTSTEP.START_CONNECT:
                StartCoroutine(GameWebConnect());
                break;
            case Global.E_STARTSTEP.START_VERSION:
                StartCoroutine(GameVersionCheck());
                break;
            case Global.E_STARTSTEP.START_DATADOWN:
                StartCoroutine(GameDataDownLoad());
                break;
            case Global.E_STARTSTEP.START_USERDATA:
                StartCoroutine(GameUserData());
                break;
        }
    }

    private IEnumerator GameWebConnect()
    {
        // 상태 접속중으로 한다
        UI_Start.Get().SetTextStatus(Global.g_startStatusConnect);
        
        yield return new WaitForSeconds(0.3f);

#if UNITY_EDITOR
#else
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            UI_Start.Get().commonMessageBox.Initialize(LocalizationManager.GetLangDesc("Option_Internet"),
                LocalizationManager.GetLangDesc("Option_Internetconfirm"),
                LocalizationManager.GetLangDesc("Option_Quit"), null,
                () =>
                {
                    Application.Quit();
                }, false);

            yield break;
        }
#endif

        // 서버 접속이 끝난후 버전 체크를 한다
        GetState<GameStateStart>().SetStartState(Global.E_STARTSTEP.START_VERSION);
    }

    private IEnumerator GameVersionCheck()
    {
        // 상태 버전 체크중..
        UI_Start.Get().SetTextStatus(Global.g_startStatusVersionCheck);
        
        yield return new WaitForSeconds(0.3f);
        
        // 버전 체크후 데이터 다운 및 로딩
        GetState<GameStateStart>().SetStartState(Global.E_STARTSTEP.START_DATADOWN);
    }

    private IEnumerator GameDataDownLoad()
    {
        // 상태 데이터 다운중
        UI_Start.Get().SetTextStatus(Global.g_startStatusDataDown);
        
        yield return new WaitForSeconds(0.3f);

        // game data patch download
        DataPatchManager.Get().JsonDownLoad();

        // TODO : [개선] 신규 테이블 매니져 초기화
        TableManager.Get().Init(Application.persistentDataPath + "/Resources/");


        // 상태 데이터 로딩중
        yield return new WaitForSeconds(0.1f);
        
        // 데이터 다운 및 로딩 후 로그인 유저 정보 받아오기
        GetState<GameStateStart>().SetStartState(Global.E_STARTSTEP.START_USERDATA);
    }

    private IEnumerator GameUserData()
    {
        // 상태 유저 정보 받는중..혹은 로그인중
        UI_Start.Get().SetTextStatus(Global.g_startStatusUserData);
        
        yield return new WaitForSeconds(0.1f);

#if NETWORK_ACT

        if (NetworkManager.Get().IsConnect() == true)
        {
            if (NetworkManager.Get().CheckReconnection() == false)
            {
                MoveMainScene();
            }
        }
        else
        {
            string id = UserInfoManager.Get().GetUserInfo().platformID;
            if (string.IsNullOrEmpty(id))
            {
                UI_Start.Get().SetTextStatus(string.Empty);
                UI_Start.Get().btn_GuestAccount.gameObject.SetActive(true);
                // UI_Start.Get().btn_GuestAccount.onClick.AddListener(() =>
                // {
                //     UI_Start.Get().btn_GuestAccount.gameObject.SetActive(false);
                //     UI_Start.Get().btn_GooglePlay.gameObject.SetActive(false);
                //     UI_Start.Get().btn_GameCenter.gameObject.SetActive(false);
                //     UI_Start.Get().SetTextStatus(Global.g_startStatusUserData);
                //
                //     NetworkManager.session.AccountTemplate.AccountLoginReq(NetworkManager.session.HttpClient, string.Empty, (int)EPlatformType.Guest, string.Empty, string.Empty, string.Empty, string.Empty, OnReceiveAccountLoginAck);
                // });
#if UNITY_EDITOR           
#elif UNITY_ANDROID
                UI_Start.Get().btn_GooglePlay.gameObject.SetActive(true);
                UI_Start.Get().btn_GooglePlay.onClick.AddListener(() =>
                {
                    UI_Start.Get().btn_GuestAccount.gameObject.SetActive(false);
                    UI_Start.Get().btn_GooglePlay.gameObject.SetActive(false);
                    UI_Start.Get().btn_GameCenter.gameObject.SetActive(false);
                    UI_Start.Get().SetTextStatus(Global.g_startStatusUserData);

                    AuthManager.Get().Login();
                });
#elif UNITY_IOS
                UI_Start.Get().btn_GameCenter.gameObject.SetActive(true);
                UI_Start.Get().btn_GameCenter.onClick.AddListener(() =>
                {
                    UI_Start.Get().btn_GuestAccount.gameObject.SetActive(false);
                    UI_Start.Get().btn_GooglePlay.gameObject.SetActive(false);
                    UI_Start.Get().btn_GameCenter.gameObject.SetActive(false);
                    UI_Start.Get().SetTextStatus(Global.g_startStatusUserData);

                    AuthManager.Get().Login();
                });
#endif
            }
            else
            {
#if UNITY_EDITOR
                NetworkManager.session.AccountTemplate.AccountLoginReq(NetworkManager.session.HttpClient, UserInfoManager.Get().GetUserInfo().platformID, (int)EPlatformType.Guest, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, OnReceiveAccountLoginAck);
#elif UNITY_ANDROID
                NetworkManager.session.AccountTemplate.AccountLoginReq(NetworkManager.session.HttpClient, UserInfoManager.Get().GetUserInfo().platformID, ObscuredPrefs.GetInt("PlatformType", (int)EPlatformType.Guest), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, OnReceiveAccountLoginAck);
#elif UNITY_IOS
                NetworkManager.session.AccountTemplate.AccountLoginReq(NetworkManager.session.HttpClient, UserInfoManager.Get().GetUserInfo().platformID, ObscuredPrefs.GetInt("PlatformType", (int)EPlatformType.Guest), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, OnReceiveAccountLoginAck);
#endif
            }
        }

#else
        // 추후 필요에 의해 다른 스텝이 낄경우 스텝 추가  가능
        // 유저 정보 까지 받고 다 했으면 다음 씬으로 이동
        ChangeScene(Global.E_GAMESTATE.STATE_MAIN);
#endif

    }

    public void UserAuthOK()
    {
        if (NetworkManager.Get().CheckReconnection() == false)
        {
            // 추후 필요에 의해 다른 스텝이 낄경우 스텝 추가  가능
            // 유저 정보 까지 받고 다 했으면 다음 씬으로 이동
            ChangeScene(Global.E_GAMESTATE.STATE_MAIN);
        }
    }


    /// <summary>
    /// 계정 로그인 응답 처리
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="accountInfo"></param>
    /// <returns></returns>
    public bool OnReceiveAccountLoginAck(EGameBaseAccountErrorCode errorCode, AccountInfo accountInfo)
    {
        if (errorCode != EGameBaseAccountErrorCode.Success)
        {
            Debug.LogError("Error OnReceiveAccountLoginAck. errorCode: " + errorCode);
            return false;
        }

        // 계정 로그인 성공시 발급받은 토큰을 저장한다.
        NetworkManager.session.HttpClient.SetAccessToken(accountInfo.AccessToken);

        UserInfoManager.Get().GetUserInfo().SetPlatformID(accountInfo.PlatformId);
        ObscuredPrefs.SetInt("PlatformType", accountInfo.PlatformType);

       // TODD : [임시] 곧바로 유저 정보 요청. 차후에 로그인 flow에 따라 위치를 변경해야함.
        NetworkManager.session.UserTemplate.UserInfoReq(NetworkManager.session.HttpClient, OnReceiveUserInfoAck);
        return true;
    }


    /// <summary>
    /// 유저 정보 응답 처리
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="userInfo"></param>
    /// <param name="arrayUserDeck"></param>
    /// <param name="arrayUserDice"></param>
    /// <param name="arrayUserBox"></param>
    /// <param name="questInfo"></param>
    /// <param name="seasonInfo"></param>
    /// <returns></returns>
    public bool OnReceiveUserInfoAck(ERandomwarsUserErrorCode errorCode, MsgUserInfo userInfo, UserDeck[] arrayUserDeck, UserDice[] arrayUserDice, UserItemInfo userItemInfo, QuestInfo questInfo, UserSeasonInfo seasonInfo)
    {
        if (errorCode != ERandomwarsUserErrorCode.Success)
        {
            UserInfoManager.Get().ResetUserId();
            
            return true;
        }

        UserInfoManager.Get().SetUserInfo(userInfo, seasonInfo);
        UserInfoManager.Get().SetDeck(arrayUserDeck);
        UserInfoManager.Get().SetDice(arrayUserDice);
        UserInfoManager.Get().SetItem(userItemInfo);
        UI_Popup_Quest.QuestUpdate(questInfo);

        GameStateManager.Get().UserAuthOK();
        //UnityUtil.Print("RECV AUTH => msg", Newtonsoft.Json.JsonConvert.SerializeObject(msg), "green");

        return true;
    }
    #endregion
    
    
    #region main scene to do

    public void MoveInGameBattle()
    {
        //WebPacket.Get().netMatchStep = Global.E_MATCHSTEP.MATCH_NONE;
        NetworkManager.Get().NetMatchStep = Global.E_MATCHSTEP.MATCH_NONE;
        ActionEvent(Global.E_STATEACTION.ACTION_INGAME);
    }

    public void MoveInGameCoop()
    {
        NetworkManager.Get().NetMatchStep = Global.E_MATCHSTEP.MATCH_NONE;
        ActionEvent(Global.E_STATEACTION.ACTION_COOP);
    }
    
    #endregion
    
    #region in game to do

    public void MoveMainScene()
    {
        ActionEvent(Global.E_STATEACTION.ACTION_MAIN);
    }
    #endregion
    
    
    
    #region on application log

    public void OnLogHandle(string logString, string stackTrace, LogType type)
    {
        //string clipText = "3242";
        //clipText.CopyToClipboard();
        string copyText = "";
        
        switch (type)
        {
            case LogType.Assert:
                copyText = "ASSERT  ::: > " + logString + "    trace : " + stackTrace;
                copyText.CopyToClipboard();
                break;
            case LogType.Error:
                copyText = "ERROR  ::: > " + logString + "    trace : " + stackTrace;
                copyText.CopyToClipboard();
                break;
            case LogType.Exception:
                copyText = "EXCEPTION  ::: > " + logString + "    trace : " + stackTrace;
                copyText.CopyToClipboard();
                break;
        }
    }
    
    #endregion


}
