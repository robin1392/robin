using System.Collections;
using System.Collections.Generic;
using ED;
using Service.Core;
//using RandomWarsProtocol;
//using RandomWarsProtocol.Msg;
using Template.Season.RandomwarsSeason.Common;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_RewardSlot : MonoBehaviour
{
    public Text text_Trophy;

    [Space]
    public Button[] arrButton;
    public Image[] arrImage_Icon;
    public Text[] arrText_Value;
    public GameObject[] arrObj_Lock;
    public GameObject[] arrObj_Check;
    
    [Header("Unlock")]
    public Button btn_Unlock;
    public Text text_UnlockCost;

    [Header("Split Line")]
    public GameObject obj_SplitLineTop;
    public GameObject obj_SplitLineMiddle;
    public GameObject obj_SplitLineBottom;
    public GameObject obj_BottomLeft;
    public GameObject obj_BottomRight;

    public static bool isUnlockEnable;
    
    private bool isGetPremium;
    private int row;
    private TDataSeasonpassReward data;
    public static int getVipRow;
    public static int getNormalRow;

    public void Initialize(int row)//, int myTrophy, int getVipRow, int getNormalRow)
    {
        this.row = row;
        // this.getVipRow = getVipRow;
        // this.getNormalRow = getNormalRow;
        
        data = new TDataSeasonpassReward();
        //dataNormal = new TDataSeasonpassReward();
        TableManager.Get().SeasonpassReward.GetData(row, out data);
        //TableManager.Get().SeasonpassReward.GetData(row + 1000, out dataPremium);

        text_Trophy.text = (row - 1).ToString();
        TDataItemList itemData;
        if (TableManager.Get().ItemList.GetData(data.rewardItem02, out itemData))
        {
            arrImage_Icon[0].sprite = FileHelper.GetIcon(itemData.itemIcon);
            arrImage_Icon[0].SetNativeSize();
            arrText_Value[0].text = $"x{data.rewardItemValue02}";
        }

        if (TableManager.Get().ItemList.GetData(data.rewardItem01, out itemData))
        {
            arrImage_Icon[1].sprite = FileHelper.GetIcon(itemData.itemIcon);
            arrImage_Icon[1].SetNativeSize();
            arrText_Value[1].text = $"x{data.rewardItemValue01}";
        }
        
        // set buttons
        SetButton();
    }

    public void SetButton()
    {
        if (UserInfoManager.Get().GetUserInfo().seasonPassRewardStep < row)    // 트로피 부족
        {
            arrObj_Lock[0].SetActive(true);
            arrObj_Lock[1].SetActive(true);
            arrObj_Check[0].SetActive(false);
            arrObj_Check[1].SetActive(false);
            arrButton[0].interactable = false;
            arrButton[1].interactable = false;

            btn_Unlock.gameObject.SetActive(row == UserInfoManager.Get().GetUserInfo().seasonPassRewardStep + 1);
            text_UnlockCost.text = data.rewardBuyPrice.ToString();
        }
        else
        {
            bool getNormal = getNormalRow >= row;
            bool getPass = getVipRow >= row;
                
            arrObj_Lock[0].SetActive(false);
            arrObj_Lock[1].SetActive(false);
            arrObj_Check[0].SetActive(getPass);
            arrObj_Check[1].SetActive(getNormal);
            arrButton[0].interactable = !getPass && UserInfoManager.Get().GetUserInfo().buySeasonPass;
            arrButton[1].interactable = !getNormal;
            
            btn_Unlock.gameObject.SetActive(false);
        }
    }

    public void Click_PremiumGet()
    {
        if (getVipRow + 1 == row)
        {
            isGetPremium = true;
            //NetworkManager.Get().GetSeasonPassRewardReq(UserInfoManager.Get().GetUserInfo().userID,
            //    (int) REWARD_TARGET_TYPE.SEASON_PASS_BUY, row, GetCallback);
            NetworkManager.session.SeasonTemplate.SeasonPassRewardReq(NetworkManager.session.HttpClient, 
                row, (int)REWARD_TARGET_TYPE.SEASON_PASS_BUY, OnReceiveSeasonPassRewardAck);
            UI_Main.Get().obj_IndicatorPopup.SetActive(true);
        }
        else
        {
            UI_ErrorMessage.Get().ShowMessage("이전 보상을 먼저 획득하세요.");
        }
    }

    public void Click_NormalGet()
    {
        if (getNormalRow + 1 == row)
        {
            isGetPremium = false;
            //NetworkManager.Get().GetSeasonPassRewardReq(UserInfoManager.Get().GetUserInfo().userID,
            //    (int) REWARD_TARGET_TYPE.ALL, row, GetCallback);
            NetworkManager.session.SeasonTemplate.SeasonPassRewardReq(NetworkManager.session.HttpClient,
                row, (int)REWARD_TARGET_TYPE.ALL, OnReceiveSeasonPassRewardAck);

            UI_Main.Get().obj_IndicatorPopup.SetActive(true);
        }
        else
        {
            UI_ErrorMessage.Get().ShowMessage("이전 보상을 먼저 획득하세요.");
        }
    }

    public bool OnReceiveSeasonPassRewardAck(ERandomwarsSeasonErrorCode errorCode, int[] arrayRewardId, MsgReward[] arrayRewardInfo, MsgQuestData[] arrayQuestData)
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
        if (errorCode == ERandomwarsSeasonErrorCode.Success)
        {
            if (isGetPremium)
            {
                getVipRow++;
            }
            else
            {
                getNormalRow++;
            }
            
            UI_Main.Get().AddReward(arrayRewardInfo, arrButton[isGetPremium ? 0 : 1].transform.position);
            UI_Popup_Quest.QuestUpdate(arrayQuestData);
        }
        
        SetButton();
        return true;
    }

    public void Click_BuyReward()
    {
        UI_Main.Get().seasonPassUnlockPopup.Initialize(data.rewardBuyPrice, SendBuyReward);
    }

    public void SendBuyReward()
    {
        //NetworkManager.Get().SeasonPassRewardStepReq(UserInfoManager.Get().GetUserInfo().userID, row, BuyRewardCallback);
        NetworkManager.session.SeasonTemplate.SeasonPassStepReq(NetworkManager.session.HttpClient, row, OnReceiveSeasonPassStepAck);
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
    }

    public bool OnReceiveSeasonPassStepAck(ERandomwarsSeasonErrorCode errorCode, int rewardId, MsgReward useItemInfo, MsgReward rewardInfo, MsgQuestData[] arrayQuestData)
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);

        UI_Popup.AllClose();
        
        if (errorCode == ERandomwarsSeasonErrorCode.Success)
        {
            switch (useItemInfo.ItemId)
            {
                case 1:
                    UserInfoManager.Get().GetUserInfo().gold += useItemInfo.Value;
                    break;
                case 2:
                    UserInfoManager.Get().GetUserInfo().diamond += useItemInfo.Value;
                    break;
            }

            UserInfoManager.Get().GetUserInfo().seasonPassRewardStep = rewardId;
            UserInfoManager.Get().GetUserInfo().seasonTrophy += rewardInfo.Value;
            
            UI_Main.Get().RefreshUserInfoUI();
            UI_Popup_Quest.QuestUpdate(arrayQuestData);
            SendMessageUpwards("RefreshSeasonInfo", SendMessageOptions.DontRequireReceiver);

            transform.parent.BroadcastMessage("SetButton");
        }
        else
        {
            UI_ErrorMessage.Get().ShowMessage("재화가 부족합니다.");
        }

        return true;
    }

    public void SetSplitLine(bool top, bool middle, bool bottom)
    {
        obj_SplitLineTop.SetActive(top);
        obj_SplitLineMiddle.SetActive(middle);
        obj_SplitLineBottom.SetActive(bottom);

        if (bottom == true)
        {
            obj_BottomLeft.SetActive(false);
            obj_BottomRight.SetActive(false);
        }
    }
}
