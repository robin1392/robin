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
    
    #region network

    public static float g_networkBaseValue = 100.0f;
    #endregion
    
    /// <summary>
    /// 게임 시스템에 필요한 변수 지정 -- 해킹에 영향받지 않는 변수
    /// </summary>
    #region system value
    // 덱의 개수
    public static int g_countDeck = 3;


    // 재접속 시간 체크를 위해 - 2wave 시간 정도..
    public static float g_reconnectGameTimeCheck = 40.0f;

    //public static readonly int[] g_needDiceCount = {1, 2, 4, 10, 20, 50, 100, 200, 400, 800, 2000, 5000, 10000, 20000, 50000};
    
    #endregion
    
    
    
    
    
    /// <summary>
    /// ui 에 들어갈 string 을 임시로 정의 해놓는다
    /// </summary>
    #region ui string

    public static readonly string g_startStatusConnect = "Server Connecting..";
    public static readonly string g_startStatusVersionCheck = "Game Version Check..";
    public static readonly string g_startStatusDataDown = "Game Data Download..";
    public static readonly string g_startStatusUserData = "User Data Setting..";


    public static readonly string g_inGameWin = "승리";
    public static readonly string g_inGameLose = "패배";
    public static readonly string g_level = "레벨";
    public static readonly string[] g_grade = {"일반", "매직", "에픽", "전설"};

    public static string[] g_gradeColor = {"FFFFFF", "5A96E8", "805DE6", "FFA318"};

    #endregion
}
