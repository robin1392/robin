using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UI_SearchingPopup : UI_Popup
{
    public Text text_Searching;
    public Button btn_Cancel;

    #region  unity base

    #endregion
    
    #region override
    protected override void OnEnable()
    {
        btn_Cancel.interactable = true;
        //if (btn_BG_Close != null) btn_BG_Close.interactable = false;
        // rts_Frame.localScale = Vector3.zero;
        // rts_Frame.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
        // {
        //     if (btn_BG_Close != null) btn_BG_Close.interactable = true;
        // });

        text_Searching.rectTransform.DOAnchorPosY(-300f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.2f);
        ((RectTransform) btn_Cancel.transform).DOAnchorPosY(300f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.2f);
    }

    public override void Close()
    {
        base.Close();
        
        text_Searching.rectTransform.DOAnchorPosY(300f, 0.4f).SetEase(Ease.OutBack);
        ((RectTransform) btn_Cancel.transform).DOAnchorPosY(-300f, 0.4f).SetEase(Ease.OutBack);
    }
    
    #endregion
    
    
    #region click event

    public void ClickSearchingCancel()
    {
        if (NetService.Get() != null)
        {
            if (NetService.Get().NetMatchStep == Global.E_MATCHSTEP.MATCH_START)
            {
                btn_Cancel.interactable = false;
                // 매칭 요청중이면 중단을 요청한다.
                //NetworkManager.Get().StopMatchReq(UserInfoManager.Get().GetUserInfo().ticketId);
                NetService.Get().GameSession.Send(Template.Stage.RandomWarsMatch.Common.ERandomWarsMatchProtocol.CANCEL_MATCH_REQ, UserInfoManager.Get().GetUserInfo().ticketId);
            }
            else if (NetService.Get().NetMatchStep == Global.E_MATCHSTEP.MATCH_CONNECT)
            {
                // 이미 상대 찾아서 커넥트 중이면 취소 못한다..
                return;
            }
        }
    }


    public void ClickSearchingCancelResult()
    {
        btn_Cancel.interactable = false;

        //
        UI_Main.Get().Click_DisconnectButton();
        Close();

        UI_Main.Get().ShowMainUI(true);
        CameraGyroController.Get().FocusOut();

    }
    #endregion

}
