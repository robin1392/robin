using System.Collections;
using System.Collections.Generic;
using ED;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
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
            NetworkManager.Get().GetSeasonPassRewardReq(UserInfoManager.Get().GetUserInfo().userID,
                (int) REWARD_TARGET_TYPE.SEASON_PASS_BUY, row, GetCallback);
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
            NetworkManager.Get().GetSeasonPassRewardReq(UserInfoManager.Get().GetUserInfo().userID,
                (int) REWARD_TARGET_TYPE.ALL, row, GetCallback);
            UI_Main.Get().obj_IndicatorPopup.SetActive(true);
        }
        else
        {
            UI_ErrorMessage.Get().ShowMessage("이전 보상을 먼저 획득하세요.");
        }
    }

    public void GetCallback(MsgGetSeasonPassRewardAck msg)
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
        if (msg.ErrorCode == GameErrorCode.SUCCESS)
        {
            if (isGetPremium)
            {
                getVipRow++;
            }
            else
            {
                getNormalRow++;
            }
            
            UI_Main.Get().AddReward(msg.RewardInfo, arrButton[isGetPremium ? 0 : 1].transform.position);
            UI_Popup_Quest.QuestUpdate(msg.QuestData);
        }
        
        SetButton();
    }

    public void Click_BuyReward()
    {
        UI_Main.Get().seasonPassUnlockPopup.Initialize(data.rewardBuyPrice, SendBuyReward);
    }

    public void SendBuyReward()
    {
        NetworkManager.Get().SeasonPassRewardStepReq(UserInfoManager.Get().GetUserInfo().userID, row, BuyRewardCallback);
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
    }

    public void BuyRewardCallback(MsgSeasonPassRewardStepAck msg)
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);

        UI_Popup.AllClose();
        
        if (msg.ErrorCode == GameErrorCode.SUCCESS)
        {
            UserInfoManager.Get().GetUserInfo().seasonPassRewardStep = msg.OpenRewardId;

            switch (msg.UseItemInfo.ItemId)
            {
                case 1:
                    UserInfoManager.Get().GetUserInfo().gold += msg.UseItemInfo.Value;
                    break;
                case 2:
                    UserInfoManager.Get().GetUserInfo().diamond += msg.UseItemInfo.Value;
                    break;
            }
            UI_Main.Get().RefreshUserInfoUI();

            transform.parent.BroadcastMessage("SetButton");
        }
        else
        {
            UI_ErrorMessage.Get().ShowMessage("재화가 부족합니다.");
        }
    }
}
