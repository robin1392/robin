using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using ED;
using RandomWarsProtocol;

public class UI_InGamePopup : SingletonDestroy<UI_InGamePopup>
{
    
    
    
    #region ui element variable
    public GameObject popup_Waiting;
    public UI_InGamePopup_Result popup_Result;
    public GameObject obj_Low_HP_Effect;
    //[Header("DEV UI")] 

    public GameObject obj_Indicator;

    [Header("Start popup")] 
    public GameObject obj_Start;
    public UI_WinLose winlose_My;
    public UI_WinLose winlose_Other;
    #endregion



    #region unity base

    public override void Awake()
    {
        base.Awake();

    }

    public void Start()
    {
        Invoke("InitUIElement", 0.1f);
    }

    public override void OnDestroy()
    {
        DestroyElement();
        
        base.OnDestroy();
    }

    #endregion
    
    #region init destroy

    public void InitUIElement()
    {
        // start popup
        if (TutorialManager.isTutorial)
        {
            winlose_My.Initialize(UserInfoManager.Get().GetActiveDeck(),
                UserInfoManager.Get().GetUserInfo().userNickName, UserInfoManager.Get().GetUserInfo().trophy);
            winlose_Other.Initialize(UserInfoManager.Get().GetActiveDeck(), "AI",
                UserInfoManager.Get().GetUserInfo().trophy);
        }
        else
        {
            winlose_My.Initialize(NetworkManager.Get().GetNetInfo().playerInfo.DiceIdArray,
                NetworkManager.Get().GetNetInfo().playerInfo.Name, NetworkManager.Get().GetNetInfo().playerInfo.Trophy);
            winlose_Other.Initialize(NetworkManager.Get().GetNetInfo().otherInfo.DiceIdArray,
                NetworkManager.Get().GetNetInfo().otherInfo.Name, NetworkManager.Get().GetNetInfo().otherInfo.Trophy);
        }

        obj_Start.SetActive(true);
        
        Invoke("DisableStartPopup", 2f);
    }

    private void DisableStartPopup()
    {
        ((RectTransform) winlose_Other.transform).DOAnchorPosY(2000f, 0.5f);
        ((RectTransform) winlose_My.transform).DOAnchorPosY(-2000f, 0.5f);
        obj_Start.transform.GetChild(1).DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            obj_Start.SetActive(false);
        });
    }

    public void DestroyElement()
    {
        
    }
    #endregion

    
    
    public void SetViewWaiting(bool view)
    {
        popup_Waiting.SetActive(view);
    }

    public void SetPopupResult(bool view, bool winLose, int winningStreak, MsgReward[] normalReward, MsgReward[] streakReward, MsgReward[] perfectReward)
    {
        popup_Result.gameObject.SetActive(view);
        if (view) popup_Result.Initialize(winLose, winningStreak, normalReward, streakReward, perfectReward);
        ViewGameIndicator(false);
        ViewLowHP(false);
        SetViewWaiting(false);
    }

    public void ViewLowHP(bool view)
    {
        obj_Low_HP_Effect.SetActive(view);
    }

    public bool GetLowHP()
    {
        return obj_Low_HP_Effect.activeSelf;
    }

    public void ViewGameIndicator(bool view)
    {
        obj_Indicator.SetActive(view);
    }

    public bool IsIndicatorActive()
    {
        return obj_Indicator.activeSelf;
    }
}
