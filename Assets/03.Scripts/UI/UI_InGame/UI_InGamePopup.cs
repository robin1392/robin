using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using ED;
using MirageTest.Scripts;
using RandomWarsProtocol;
using Service.Core;

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
    public RectTransform rts_Fight;
    #endregion



    #region unity base

    public override void Awake()
    {
        base.Awake();
    }

    public void Start()
    {
    }

    public override void OnDestroy()
    {
        DestroyElement();
        
        base.OnDestroy();
    }

    #endregion
    
    #region init destroy

    public void InitUIElement(MatchPlayer player1, MatchPlayer player2)
    {
        winlose_My.Initialize(player1.Deck.GuadialId, player1.Deck.DiceInfos.Select(d => d.DiceId).ToArray(),
            player1.UserNickName, player1.Trophy);
        winlose_My.Initialize(player2.Deck.GuadialId, player2.Deck.DiceInfos.Select(d => d.DiceId).ToArray(),
            player2.UserNickName, player2.Trophy);

        obj_Start.SetActive(true);
        
        Invoke("DisableStartPopup", 2f);
    }

    private void DisableStartPopup()
    {
        ((RectTransform) winlose_Other.transform).DOAnchorPosY(2000f, 0.5f);
        ((RectTransform) winlose_My.transform).DOAnchorPosY(-2000f, 0.5f);
        obj_Start.transform.GetChild(1).DOScale(Vector3.zero, 0.5f).OnComplete(() =>
        {
            rts_Fight.localScale = Vector3.zero;
            rts_Fight.DOScale(1f, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                rts_Fight.DOScale(0, 0.2f).SetEase(Ease.OutBack).SetDelay(0.4f).OnComplete(() =>
                {
                    obj_Start.SetActive(false);
                });
            });
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

    public void SetPopupResult(bool view, bool winLose, int winningStreak, ItemBaseInfo[] normalReward, ItemBaseInfo[] streakReward, ItemBaseInfo[] perfectReward)
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
