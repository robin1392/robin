using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using ED;
using Photon;
using Template.Match.RandomwarsMatch.Common;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class UI_MatchPopup : UI_Popup
{
    public RectTransform rts_VerticalLayoutGroup;
    public Text text_MatchTitle;
    public Text text_InviteRemainTime;
    public Text text_InviteCode;
    public InputField input_InviteCode;

    [Header("GameObject")] public GameObject obj_TypeButtons;
    public GameObject obj_InviteButtons;
    public GameObject obj_CreateRoom;
    public GameObject obj_JoinRoom;

    private PLAY_TYPE _playType;
    private bool isSearching;
    private string ticketId;

    public void Initialize(PLAY_TYPE playType)
    {
        _playType = playType;

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
        isSearching = false;
        base.Close();
    }

    /// <summary>
    /// 랜덤 매칭
    /// </summary>
    public void Click_RandomMatch()
    {
        UI_Main.Get().ShowMainUI(false);

        FirebaseManager.Get().LogEvent(_playType == PLAY_TYPE.BATTLE ? "PlayBattle" : "PlayCoop");

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
            Connect().Forget();
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
        CreateRoomAsync().Forget();
    }

    private bool _isRequesting = false;

    async UniTask CreateRoomAsync()
    {
        if (_isRequesting)
        {
            return;
        }

        _isRequesting = true;

        ticketId = await PhotonNetwork.Instance.CreateBattleRoom();
        _isRequesting = false;

        // 취소버튼 보이기
        obj_InviteButtons.SetActive(false);
        obj_CreateRoom.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rts_VerticalLayoutGroup);
        // 남은시간 표시하기
        text_InviteCode.text = $"CODE : {ticketId}";
        
        int t = 60;
        while (t >= 0)
        {
            if (text_InviteRemainTime == null)
            {
                break;
            }
            
            text_InviteRemainTime.text = $"{t}";
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            t--;
        }

        if (PhotonNetwork.Instance.State == PhotonNetwork.StateType.SceneChanged)
        {
            return;
        }
        else
        {
            PhotonNetwork.Instance.LocalBalancingClient.Disconnect();    
        }
        
        ticketId = string.Empty;
        Close();
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
        PhotonNetwork.Instance.LocalBalancingClient.Disconnect();
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
        PhotonNetwork.Instance.LocalBalancingClient.Disconnect();
        obj_CreateRoom.SetActive(false);
        obj_InviteButtons.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(rts_VerticalLayoutGroup);
        ticketId = string.Empty;
        isSearching = false;
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
        JoinRoomAsync().Forget();
    }

    async UniTask JoinRoomAsync()
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);

        try
        {
            await PhotonNetwork.Instance.JoinBattleModeByCode(ticketId.ToUpper());
        }
        catch (Exception e)
        {
            Debug.LogError($"입장 실패 {e.Message}");
            UI_ErrorMessage.Get().ShowMessage("잠시 후 다시 시도해주세요.");
            UI_Main.Get().obj_IndicatorPopup.SetActive(false);
        }
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

    async UniTask Connect()
    {
        if (_playType == PLAY_TYPE.BATTLE)
        {
            //TODO: 타임아웃
            try
            {
                await PhotonNetwork.Instance.JoinBattleModeByMatching();
            }
            catch (Exception e)
            {
                Debug.LogError($"입장 실패 {e.Message}");
                UI_ErrorMessage.Get().ShowMessage("잠시 후 다시 시도해주세요.");
                UI_Main.Get().obj_IndicatorPopup.SetActive(false);
            }
        }
    }
}