using System.Collections;
using System.Collections.Generic;
using ED;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_TrophyRewardSlot : MonoBehaviour
{
    [Space]
    public Text text_Trophy;

    [Space]
    public Button[] arrButton;
    public Image[] arrImage_Icon;
    public Text[] arrText_Value;
    public GameObject[] arrObj_Lock;
    public GameObject[] arrObj_Check;

    [Header("Split Line")]
    public GameObject obj_SplitLineTop;
    public GameObject obj_SplitLineMiddle;
    public GameObject obj_SplitLineBottom;
    public GameObject obj_BottomLeft;
    public GameObject obj_BottomRight;

    private bool isGetPremium;
    private int row;
    private int getVipRow;
    private int getNormalRow;
    private TDataClassReward rewardData;

    public void Initialize(int row, int myTrophy, int getVipRow, int getNormalRow)
    {
        this.row = row;
        this.getVipRow = getVipRow;
        this.getNormalRow = getNormalRow;
        
        rewardData = new TDataClassReward();
        TableManager.Get().ClassReward.GetData(row, out rewardData);

        text_Trophy.text = rewardData.rankPoint.ToString();
        TDataItemList item;
        if (TableManager.Get().ItemList.GetData(rewardData.rewardItem02, out item))
        {
            arrImage_Icon[0].sprite = FileHelper.GetIcon(item.itemIcon);
            arrImage_Icon[0].SetNativeSize();
        }
        if (TableManager.Get().ItemList.GetData(rewardData.rewardItem01, out item))
        {
            arrImage_Icon[1].sprite = FileHelper.GetIcon(item.itemIcon);
            arrImage_Icon[1].SetNativeSize();
        }
        arrText_Value[0].text = $"x{rewardData.rewardItemValue02}";
        arrText_Value[1].text = $"x{rewardData.rewardItemValue01}";
        
        // set buttons
        SetButton();
    }

    public void SetButton()
    {
        if (UserInfoManager.Get().GetUserInfo().trophy < rewardData.rankPoint)    // 트로피 부족
        {
            arrObj_Lock[0].SetActive(true);
            arrObj_Lock[1].SetActive(true);
            arrObj_Check[0].SetActive(false);
            arrObj_Check[1].SetActive(false);
            arrButton[0].interactable = false;
            arrButton[1].interactable = false;
        }
        else
        {
            bool getNormal = getNormalRow >= row;
            bool getPass = getVipRow >= row;
                
            arrObj_Lock[0].SetActive(false);
            arrObj_Lock[1].SetActive(false);
            arrObj_Check[0].SetActive(getPass);
            arrObj_Check[1].SetActive(getNormal);
            arrButton[0].interactable = !getPass && UserInfoManager.Get().GetUserInfo().buyVIP;
            arrButton[1].interactable = !getNormal;
        }
    }
    
    public void Click_PremiumGet()
    {
        isGetPremium = true;
        NetworkManager.Get().GetClassRewardReq(UserInfoManager.Get().GetUserInfo().userID, row + 1000, GetCallback);
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
    }

    public void Click_NormalGet()
    {
        isGetPremium = false;
        NetworkManager.Get().GetClassRewardReq(UserInfoManager.Get().GetUserInfo().userID, row, GetCallback);
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
    }
    
    public void GetCallback(MsgGetClassRewardAck msg)
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
