using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Global Variable 선언부
/// </summary>

public partial class Global
{
    // 로딩 씬 이름 - 글로벌로 그냥 지정
    public static string g_sceneLoadingName = "Loading";
    public static string g_sceneStartName = "StartScene";
    public static string g_sceneMainName = "Main";
    public static string g_sceneInGameBattle = "InGame_Battle";
    public static string g_sceneInGameCoop = "InGame_Coop";
    
    
    /// <summary>
    /// 게임 시스템에 필요한 변수 지정 -- 해킹에 영향받지 않는 변수
    /// </summary>
    #region system value
    // 덱의 개수
    public static int g_countDeck = 3;
    
    #endregion
    
    
    
    
    
    /// <summary>
    /// ui 에 들어갈 string 을 임시로 정의 해놓는다
    /// </summary>
    #region ui string

    public static string g_startStatusConnect = "Server Connecting..";
    public static string g_startStatusVersionCheck = "Game Version Check..";
    public static string g_startStatusDataDown = "Game Data Download..";
    public static string g_startStatusUserData = "User Data Setting..";


    public static string g_inGameWin = "승리";
    public static string g_inGameLose = "패배";

    public static string[] g_gradeColor = {"000000", "43C0FF", "851CEA", "E19F38"};

    #endregion
}
