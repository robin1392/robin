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
    public GameObject obj_Trophy;

    [Space]
    public Image image_Guage;
    public Image image_Guage_BG;
    public Text text_Trophy;

    [Space]
    public Button[] arrButton;
    public Image[] arrImage_Icon;
    public Text[] arrText_Value;
    public GameObject[] arrObj_Lock;
    public GameObject[] arrObj_Check;
    
    private bool isGetPremium;
    private int row;
    private TDataSeasonpassReward dataPremium;
    private TDataSeasonpassReward dataNormal;

    public void Initialize(int row, int myTrophy)
    {
        this.row = row;
        float minTrophy = 0;
        float maxTrophy = 0;
        
        if (row == 0)
        {
            text_Trophy.text = $"0";
            var v = image_Guage.rectTransform.sizeDelta;
            v.x *= 0.5f;
            image_Guage.rectTransform.sizeDelta = v;
            image_Guage_BG.rectTransform.sizeDelta = v;
            v = image_Guage.rectTransform.anchoredPosition;
            v.y = -200;
            image_Guage.rectTransform.anchoredPosition = v;
            image_Guage_BG.rectTransform.anchoredPosition = v;
            
            arrButton[0].gameObject.SetActive(false);
            arrButton[1].gameObject.SetActive(false);
            
            var dataNormal = new TDataSeasonpassReward();
            if (TableManager.Get().SeasonpassReward.GetData(row + 1, out dataNormal))
            {
                maxTrophy = Mathf.Lerp(0f, dataNormal.trophyPoint, 0.5f);
            }
        }
        else
        {
            dataPremium = new TDataSeasonpassReward();
            dataNormal = new TDataSeasonpassReward();
            TableManager.Get().SeasonpassReward.GetData(row, out dataNormal);
            TableManager.Get().SeasonpassReward.GetData(row + 1000, out dataPremium);

            text_Trophy.text = dataPremium.trophyPoint.ToString();
            arrText_Value[0].text = $"{dataPremium.rewardItem}\nx{dataPremium.rewardItemValue}";
            arrText_Value[1].text = $"{dataNormal.rewardItem}\nx{dataNormal.rewardItemValue}";
            
            // set min, max
            if (TableManager.Get().SeasonpassReward.GetData(row - 1, out dataNormal))
            {
                minTrophy = Mathf.Lerp(dataNormal.trophyPoint, dataPremium.trophyPoint, 0.5f);
            }
            else if (row == 1)
            {
                minTrophy = dataPremium.trophyPoint / 2;
            }
            
            if (TableManager.Get().SeasonpassReward.GetData(row + 1, out dataNormal) == false)
            {
                var v = image_Guage.rectTransform.sizeDelta;
                v.x *= 0.5f;
                image_Guage.rectTransform.sizeDelta = v;
                image_Guage_BG.rectTransform.sizeDelta = v;

                maxTrophy = dataPremium.trophyPoint;
            }
            else
            {
                maxTrophy = Mathf.Lerp(dataPremium.trophyPoint, dataNormal.trophyPoint, 0.5f);
            }
            
            // set buttons
            SetButton();
        }

        image_Guage.fillAmount = (myTrophy - minTrophy) / (float)(maxTrophy - minTrophy);
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
