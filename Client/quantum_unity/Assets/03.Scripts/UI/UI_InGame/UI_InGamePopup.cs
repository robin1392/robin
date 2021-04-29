using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Scripts;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using ED;
using MirageTest.Scripts;
using RandomWarsProtocol;
using Service.Core;
using MatchPlayer = _Scripts.MatchPlayer;

public class UI_InGamePopup : SingletonDestroy<UI_InGamePopup>
{
    #region ui element variable

    public GameObject popup_Waiting;
    public UI_InGamePopup_Result popup_Result;

    public GameObject obj_Low_HP_Effect;
    //[Header("DEV UI")] 

    public GameObject obj_Indicator;

    [Header("Start popup")] public GameObject obj_Start;
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

    public void InitUIElement(MatchPlayer local, MatchPlayer remote)
    {
        winlose_My.Initialize(local.Deck.GuardianId, local.Deck.DiceInfos.Select(d => d.DiceId).ToArray(),
            local.NickName, local.Trophy);
        winlose_Other.Initialize(remote.Deck.GuardianId, remote.Deck.DiceInfos.Select(d => d.DiceId).ToArray(),
            remote.NickName, remote.Trophy);

        obj_Start.SetActive(true);
    }

    public void DisableStartPopup()
    {
        ((RectTransform) winlose_Other.transform).DOAnchorPosY(2000f, 0.5f).SetUpdate(true);
        ((RectTransform) winlose_My.transform).DOAnchorPosY(-2000f, 0.5f).SetUpdate(true);
        obj_Start.transform.GetChild(1).DOScale(Vector3.zero, 0.5f).SetUpdate(true).OnComplete(() =>
        {
            rts_Fight.localScale = Vector3.zero;
            rts_Fight.DOScale(1f, 0.2f).SetEase(Ease.OutBack).SetUpdate(true).OnComplete(() =>
            {
                rts_Fight.DOScale(0, 0.2f).SetEase(Ease.OutBack).SetUpdate(true).SetDelay(0.4f).OnComplete(() =>
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

    public void SetPopupResult(
        Global.PLAY_TYPE playType, MatchPlayer localPlayer, MatchPlayer otherPlayer,
        bool view, bool winLose, int winningStreak, bool perfect,
        List<ItemBaseInfo> normalReward, List<ItemBaseInfo> streakReward, List<ItemBaseInfo> perfectReward, AdRewardInfo loseReward)
    {
        popup_Result.gameObject.SetActive(view);
        if (view)
            popup_Result.Initialize(playType, localPlayer, otherPlayer, 
                winLose, winningStreak, perfect,
                normalReward, streakReward, perfectReward, loseReward);
        
        ViewGameIndicator(false);
        ShowLowHPEffect(false);
        SetViewWaiting(false);
    }

    public void ShowLowHPEffect(bool view)
    {
        obj_Low_HP_Effect.SetActive(view);
    }

    public bool IsLowHpEffectActiveSelf()
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