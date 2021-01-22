using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using Debug = ED.Debug;

public class UI_SearchingPopup : UI_Popup
{
    public Text text_Searching;
    public Button btn_Cancel;
    public Text text_Time;

    private DateTime dateSearchStart;

    #region  unity base

    private void Update()
    {
        var span = DateTime.Now.Subtract(dateSearchStart);
        text_Time.text = $"{span.Minutes:D2}:{span.Seconds:D2}";
    }

    #endregion
    
    #region override
    protected override void OnEnable()
    {
        btn_Cancel.interactable = true;
        dateSearchStart = DateTime.Now;
        
        text_Searching.rectTransform.DOAnchorPosY(-300f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.2f);
        ((RectTransform) btn_Cancel.transform).DOAnchorPosY(300f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.2f);
        text_Time.rectTransform.DOAnchorPosY(500f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.2f);
    }

    public override void Close()
    {
        base.Close();
        
        text_Searching.rectTransform.DOAnchorPosY(300f, 0.4f).SetEase(Ease.OutBack);
        ((RectTransform) btn_Cancel.transform).DOAnchorPosY(-300f, 0.4f).SetEase(Ease.OutBack);
        text_Time.rectTransform.DOAnchorPosY(-111f, 0.4f).SetEase(Ease.OutBack).SetDelay(0.2f);
    }
    
    #endregion
    
    
    #region click event

    public void ClickSearchingCancel()
    {
        if (NetworkManager.Get() != null)
        {
            switch (NetworkManager.Get().NetMatchStep)
            {
                case Global.E_MATCHSTEP.MATCH_NONE:
                    ClickSearchingCancelResult();
                    break;
                case Global.E_MATCHSTEP.MATCH_START:
                    btn_Cancel.interactable = false;
                    // 매칭 요청중이면 중단을 요청한다.
                    NetworkManager.Get().StopMatchReq(UserInfoManager.Get().GetUserInfo().ticketId);
                    break;
                case Global.E_MATCHSTEP.MATCH_CONNECT:
                    btn_Cancel.interactable = false;
                    return;
                case Global.E_MATCHSTEP.MATCH_CANCEL:
                    btn_Cancel.interactable = false;
                    break;
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
