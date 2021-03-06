using System.Collections;
using System.Collections.Generic;
using ED;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UI_Popup_DailyShopReset : UI_Popup
{
    [Header("ResetButton")]
    public Button btn_AD;
    public Button btn_Point;
    public Text text_ADCount;
    public Text text_PointCount;
    public Text text_PointPrice;
    
    private int cost;
    public delegate void ResetCallback(EBuyTypeKey type);
    private ResetCallback callback;

    public void Initialize(int maxAD, int remainAD, int maxPoint, int remainPoint, int cost, ResetCallback callback)
    {
        this.cost = cost;
        this.callback = callback;

        text_ADCount.text = $"{remainAD}/{maxAD}";
        text_PointCount.text = $"{remainPoint}/{maxPoint}";
        text_PointPrice.text = cost.ToString();

        btn_AD.interactable = remainAD > 0;
        btn_Point.interactable = remainPoint > 0;
        
        gameObject.SetActive(true);
    }

    public void Click_ResetButton(int caseNum)
    {
        switch (caseNum)
        {
            case 0:
#if UNITY_EDITOR
                ADCallback(true);
#else
                MopubCommunicator.Instance.showVideo(ADCallback);
#endif
                break;
            case 1:
            {
                if (UserInfoManager.Get().GetUserInfo().gold >= cost)
                {
                    PointReset();
                    Close();
                }
                else
                {
                    UI_Main.Get().moveShopPopup.Initialize(UI_BoxOpenPopup.COST_TYPE.GOLD);
                }
            }
                break;
        }
    }

    public void ADCallback(bool isComplete)
    {
        if (isComplete)
        {
            callback?.Invoke(EBuyTypeKey.buy_adfree);
            Close();
        }
    }
    
    public void PointReset()
    {
        callback?.Invoke(EBuyTypeKey.buy_gold);
    }
}
