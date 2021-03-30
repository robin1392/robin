using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using Template.Match.RandomwarsMatch.Common;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

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

    public void Initialize()
    {
        _playType = NetworkManager.Get().playType;

        text_MatchTitle.text = $"{_playType.ToString()}";
    }
    
    public void Click_RandomMatch()
    {
        UI_Main.Get().ShowMainUI(false);
        
        FirebaseManager.Get().LogEvent(_playType == PLAY_TYPE.BATTLE ? "PlayBattle":"PlayCoop");

        CameraGyroController.Get().FocusIn();
        
        if (UI_Main.Get().isAIMode || TutorialManager.isTutorial)
        {
            UI_Main.Get().btn_PlayBattle.interactable = false;
            UI_Main.Get().btn_PlayCoop.interactable = false;
            UI_Main.Get().searchingPopup.gameObject.SetActive(true);
            
            StartCoroutine(AIMode(_playType));
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

    public void Click_Invite()
    {
        obj_TypeButtons.SetActive(false);
        obj_InviteButtons.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rts_VerticalLayoutGroup);
    }

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

    public void Click_JoinRoom()
    {
        obj_InviteButtons.SetActive(false);
        obj_JoinRoom.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rts_VerticalLayoutGroup);
    }

    public void Click_CancelTogether()
    {
        obj_TypeButtons.SetActive(true);
        obj_InviteButtons.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rts_VerticalLayoutGroup);
    }

    public void Click_CopyCode()
    {
        ticketId.CopyToClipboard();
        
        UI_ErrorMessage.Get().ShowMessage(LocalizationManager.GetLangDesc("Option_Pidcopy"));
    }

    public void Click_CancelCreateRoom()
    {
        NetworkManager.session.MatchTemplate.MatchCancelReq(
            NetworkManager.session.HttpClient,
            ticketId,
            CancelCreateRoomCallback
        );
    }

    public void Click_CancelJoinRoom()
    {
        obj_JoinRoom.SetActive(false);
        obj_InviteButtons.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rts_VerticalLayoutGroup);
    }

    public void Click_JoinRoomWithCode()
    {
        if (input_InviteCode.text.IsNullOrEmpty())
        {
            return;
        }
        
        var eGameMode = _playType == PLAY_TYPE.BATTLE ? EGameMode.DeathMatch : EGameMode.Coop;
        NetworkManager.session.MatchTemplate.MatchJoinReq(
            NetworkManager.session.HttpClient,
            input_InviteCode.text,
            (int) eGameMode,
            UserInfoManager.Get().GetUserInfo().activateDeckIndex,
            JoinRoomCallback
        );
    }

    private bool JoinRoomCallback(ERandomwarsMatchErrorCode errorCode)
    {
        if (errorCode == ERandomwarsMatchErrorCode.Success)
        {
            return true;
        }

        return false;
    }

    private bool CancelCreateRoomCallback(ERandomwarsMatchErrorCode errorCode)
    {
        StopAllCoroutines();
        
        if (errorCode == ERandomwarsMatchErrorCode.Success)
        {
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

            NetworkManager.Get().OnStartMatchAck(ERandomwarsMatchErrorCode.Success, ticketId);

            StartCoroutine(CreateRoomRemainTime());
            return true;
        }

        this.ticketId = string.Empty;
        return false;
    }

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
        
        Click_CancelCreateRoom();
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
