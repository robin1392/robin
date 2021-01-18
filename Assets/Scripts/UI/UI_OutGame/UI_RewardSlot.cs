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
    public Button btn_Unlock;
    public Text text_Trophy;

    [Space]
    public Button[] arrButton;
    public Image[] arrImage_Icon;
    public Text[] arrText_Value;
    public GameObject[] arrObj_Lock;
    public GameObject[] arrObj_Check;

    public static bool isUnlockEnable;
    
    private bool isGetPremium;
    private int row;
    private TDataSeasonpassReward dataPremium;
    private TDataSeasonpassReward dataNormal;

    public void Initialize(int row, int myTrophy)
    {
        this.row = row;
        float minTrophy = 0;
        float maxTrophy = 0;
        
        dataPremium = new TDataSeasonpassReward();
        dataNormal = new TDataSeasonpassReward();
        TableManager.Get().SeasonpassReward.GetData(row, out dataNormal);
        TableManager.Get().SeasonpassReward.GetData(row + 1000, out dataPremium);

        text_Trophy.text = dataPremium.trophyPoint.ToString();
        TDataItemList itemData;
        if (TableManager.Get().ItemList.GetData(dataPremium.rewardItem, out itemData))
        {
            arrImage_Icon[0].sprite = FileHelper.GetIcon(itemData.itemIcon);
            arrImage_Icon[0].SetNativeSize();
            arrText_Value[0].text = $"x{dataPremium.rewardItemValue}";
        }

        if (TableManager.Get().ItemList.GetData(dataNormal.rewardItem, out itemData))
        {
            arrImage_Icon[1].sprite = FileHelper.GetIcon(itemData.itemIcon);
            arrImage_Icon[1].SetNativeSize();
            arrText_Value[1].text = $"x{dataNormal.rewardItemValue}";
        }
        
        // set buttons
        SetButton();
    }

    public void SetButton()
    {
        if (UserInfoManager.Get().GetUserInfo().seasonTrophy < dataPremium.trophyPoint)    // 트로피 부족
        {
            arrObj_Lock[0].SetActive(true);
            arrObj_Lock[1].SetActive(true);
            arrObj_Check[0].SetActive(false);
            arrObj_Check[1].SetActive(false);
            arrButton[0].interactable = false;
            arrButton[1].interactable = false;

            if (isUnlockEnable == false && row > 1)
            {
                btn_Unlock.gameObject.SetActive(true);
                isUnlockEnable = true;
            }
            else
            {
                btn_Unlock.gameObject.SetActive(false);
            }
        }
        else
        {
            bool getNormal = UserInfoManager.Get().GetUserInfo().seasonPassRewardIds.Contains(row);
            bool getPass = UserInfoManager.Get().GetUserInfo().seasonPassRewardIds.Contains(row + 1000);
                
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
        isGetPremium = true;
        NetworkManager.Get().GetSeasonPassRewardReq(UserInfoManager.Get().GetUserInfo().userID, row + 1000, GetCallback);
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
    }

    public void Click_NormalGet()
    {
        isGetPremium = false;
        NetworkManager.Get().GetSeasonPassRewardReq(UserInfoManager.Get().GetUserInfo().userID, row, GetCallback);
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
    }

    public void GetCallback(MsgGetSeasonPassRewardAck msg)
    {
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
        if (msg.ErrorCode == GameErrorCode.SUCCESS)
        {
            if (isGetPremium)
            {
                UserInfoManager.Get().GetUserInfo().seasonPassRewardIds.Add(row + 1000);
            }
            else
            {
                UserInfoManager.Get().GetUserInfo().seasonPassRewardIds.Add(row);
            }
            
            UI_Main.Get().AddReward(msg.RewardInfo, arrButton[isGetPremium ? 0 : 1].transform.position);
            UI_Popup_Quest.QuestUpdate(msg.QuestData);
        }
        
        SetButton();
    }
}
