using System.Collections;
using System.Collections.Generic;
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
    #endregion



    #region unity base

    public override void Awake()
    {
        base.Awake();

        InitUIElement();
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
