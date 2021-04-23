using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using Template.Match.RandomwarsMatch.Common;
using UnityEngine;
using UnityEngine.UI;

public class UI_MatchPopup : UI_Popup
{
    public RectTransform rts_VerticalLayoutGroup;
    public Text text_MatchTitle;
    public Text text_InviteRemainTime;
    public Text text_InviteCode;
    public InputField input_InviteCode;

    [Header("GameObject")] 
    public GameObject obj_TypeButtons;
    public GameObject obj_InviteButtons;
    public GameObject obj_CreateRoom;
    public GameObject obj_JoinRoom;

    private PLAY_TYPE _playType;
    private string ticketId;
    private bool isSearching;

    public void Initialize()
    {
        _playType = NetworkManager.Get().playType;

        switch (_playType)
        {
            case PLAY_TYPE.BATTLE:
                text_MatchTitle.text = LocalizationManager.GetLangDesc("Gameinvite_Titlepvp");
                break;
            case PLAY_TYPE.CO_OP:
                text_MatchTitle.text = LocalizationManager.GetLangDesc("Gameinvite_Titlecoop");
                break;
        }
    }

    public override void Close()
    {
        if (isSearching && string.IsNullOrEmpty(ticketId) == false)
        {
            NetworkManager.Get().StopMatchReq(ticketId);
        }
        
        isSearching = false;
        
        base.Close();
    }

    /// <summary>
    /// 랜덤 매칭
    /// </summary>
    public void Click_RandomMatch()
    {
        UI_Main.Get().ShowMainUI(false);
        
        FirebaseManager.Get().LogEvent(_playType == PLAY_TYPE.BATTLE ? "PlayBattle":"PlayCoop");

        // CameraGyroController.Get().FocusIn();
        
        if (UI_Main.Get().isAIMode || TutorialManager.isTutorial)
        {
            UI_Main.Get().btn_PlayBattle.interactable = false;
            UI_Main.Get().btn_PlayCoop.interactable = false;
            UI_Main.Get().searchingPopup.gameObject.SetActive(true);
            UI_Main.Get().searchingPopup.StartCoroutine(AIMode(_playType));
        }
        else
        {
            UI_Main.Get().btn_PlayBattle.interactable = false;
            UI_Main.Get().btn_PlayCoop.interactable = false;
            UI_Main.Get().searchingPopup.gameObject.SetActive(true);
            Connect();
        }
        
        Close();
    }

    /// <summary>
    /// 친구와 함께하기
    /// </summary>
    public void Click_Invite()
    {
        obj_TypeButtons.SetActive(false);
        obj_InviteButtons.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rts_VerticalLayoutGroup);
    }

    /// <summary>
    /// 초대코드 만들기 요청
    /// </summary>
    public void Click_CreateRoom()
    {
        var eGameMode = _playType == PLAY_TYPE.BATTLE ? EGameMode.DeathMatch : EGameMode.Coop;
        NetworkManager.session.MatchTemplate.MatchInviteReq(
            NetworkManager.session.HttpClient,
            (int) eGameMode,
            UserInfoManager.Get().GetUserInfo().activateDeckIndex,
            InviteCallback
        );
    }

    /// <summary>
    /// 초대코드 입력창 열기
    /// </summary>
    public void Click_JoinRoom()
    {
        obj_InviteButtons.SetActive(false);
        obj_JoinRoom.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rts_VerticalLayoutGroup);
    }

    /// <summary>
    /// 첫화면으로 돌아가기
    /// </summary>
    public void Click_CancelTogether()
    {
        obj_TypeButtons.SetActive(true);
        obj_InviteButtons.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rts_VerticalLayoutGroup);
    }
    
    /// <summary>
    /// 초대코드 복사하기 버튼
    /// </summary>
    public void Click_CopyCode()
    {
        ticketId.CopyToClipboard();
        
        UI_ErrorMessage.Get().ShowMessage(LocalizationManager.GetLangDesc("Gameinvite_Codecopyfin"));
    }

    /// <summary>
    /// 초대코드 생성후에 취소요청
    /// </summary>
    public void Click_CancelCreateRoom()
    {
        NetworkManager.session.MatchTemplate.MatchCancelReq(
            NetworkManager.session.HttpClient,
            ticketId,
            CancelCreateRoomCallback
        );
    }

    /// <summary>
    /// 초대코드 입력창 닫고 뒤로 돌아가기
    /// </summary>
    public void Click_CancelJoinRoom()
    {
        obj_JoinRoom.SetActive(false);
        obj_InviteButtons.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rts_VerticalLayoutGroup);
    }

    /// <summary>
    /// 입력된 코드로 입장 요청하기
    /// </summary>
    public void Click_JoinRoomWithCode()
    {
        if (string.IsNullOrEmpty(input_InviteCode.text))
        {
            return;
        }

        ticketId = input_InviteCode.text;
        var eGameMode = _playType == PLAY_TYPE.BATTLE ? EGameMode.DeathMatch : EGameMode.Coop;
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
        NetworkManager.session.MatchTemplate.MatchJoinReq(
            NetworkManager.session.HttpClient,
            ticketId,
            (int) eGameMode,
            UserInfoManager.Get().GetUserInfo().activateDeckIndex,
            JoinRoomCallback
        );
    }

    /// <summary>
    /// 입장 요청 콜백 (성공시 MatchStatusReq 요청)
    /// </summary>
    /// <param name="errorCode"></param>
    /// <returns></returns>
    private bool JoinRoomCallback(ERandomwarsMatchErrorCode errorCode)
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
        if (errorCode == ERandomwarsMatchErrorCode.Success)
        {
            isSearching = true;
            NetworkManager.Get().OnStartMatchAck(ERandomwarsMatchErrorCode.Success, ticketId);
            return true;
        }
        
        UI_ErrorMessage.Get().ShowMessage("존재하지 않는 방입니다");

        return false;
    }

    /// <summary>
    /// 방 생성 취소요청 콜백
    /// </summary>
    /// <param name="errorCode"></param>
    /// <returns></returns>
    private bool CancelCreateRoomCallback(ERandomwarsMatchErrorCode errorCode)
    {
        StopAllCoroutines();
        
        if (errorCode == ERandomwarsMatchErrorCode.Success)
        {
            isSearching = false;
            NetworkManager.Get().StopMatchReq(ticketId);
            ticketId = string.Empty;
            
            // 취소버튼 감추기
            obj_CreateRoom.SetActive(false);
            obj_InviteButtons.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rts_VerticalLayoutGroup);
            return true;
        }

        return false;
    }

    /// <summary>
    /// 방생성 콜백 (성공시 MatchStatusReq 요청)
    /// </summary>
    /// <param name="errorCode"></param>
    /// <param name="ticketId"></param>
    /// <returns></returns>
    private bool InviteCallback(ERandomwarsMatchErrorCode errorCode, string ticketId)
    {
        if (errorCode == ERandomwarsMatchErrorCode.Success)
        {
            this.ticketId = ticketId;
            
            // 취소버튼 보이기
            obj_InviteButtons.SetActive(false);
            obj_CreateRoom.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(rts_VerticalLayoutGroup);
            // 남은시간 표시하기
            text_InviteCode.text = $"CODE : {ticketId}";

            isSearching = true;
            NetworkManager.Get().OnStartMatchAck(ERandomwarsMatchErrorCode.Success, ticketId);

            StartCoroutine(CreateRoomRemainTime());
            return true;
        }

        this.ticketId = string.Empty;
        return false;
    }

    /// <summary>
    /// 방생성 남은시간 표시 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator CreateRoomRemainTime()
    {
        int t = 60;
        var wait = new WaitForSeconds(1f);

        while (t >= 0)
        {
            text_InviteRemainTime.text = $"{t}";
            yield return wait;
            t--;
        }
        
        Close();
    }
    
    private IEnumerator AIMode(PLAY_TYPE playType)
    {
        yield return new WaitForSeconds(1f);

        if (playType == PLAY_TYPE.BATTLE)
        {
            GameStateManager.Get().MoveInGameBattle();
        }
        else
        {
            GameStateManager.Get().MoveInGameCoop();
        }
    }
    
    private void Connect()
    {
        if (NetworkManager.Get().UseLocalServer == true)
        {
            //TODO: 기능 복구
            NetworkManager.Get().ConnectServer(_playType, NetworkManager.Get().LocalServerAddr, NetworkManager.Get().LocalServerPort, UserInfoManager.Get().GetUserInfo().userID);
            return;
        }

        var eGameMode = _playType == PLAY_TYPE.BATTLE ? EGameMode.DeathMatch : EGameMode.Coop;
        NetworkManager.Get().StartMatchReq(eGameMode, UserInfoManager.Get().GetActiveDeckIndex());
    }
}
