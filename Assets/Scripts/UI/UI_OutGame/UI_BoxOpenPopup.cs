#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using ED;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using UnityEngine;
using UnityEngine.UI;

public class UI_BoxOpenPopup : UI_Popup
{
    public enum COST_TYPE
    {
        KEY,
        GOLD,
        DIAMOND
    }

    [Header("Link")]
    public Button btn_Open;
    public Image image_CostIcon;
    public Text text_Cost;
    
    [Header("Gold")]
    public GameObject obj_Gold;
    public Text text_Gold;

    [Header("Dice")]
    public GameObject obj_Dice;
    public Text text_Dice;

    [Header("Diamond")]
    public GameObject obj_Diamond;
    public Text text_Diamond;

    [Header("Magic Dice")]
    public GameObject obj_MagicDice;
    public Text text_MagicDice;

    [Header("EpicDice")]
    public GameObject obj_EpicDice;
    public Text text_EpicDice;

    [Header("LegendDice")]
    public GameObject obj_LegendDice;
    public Text text_LegendDice;

    [Header("Sprite")]
    public Sprite[] arrSprite_CostType;

    private int boxID;
    
    public void Initialize(int id, COST_TYPE costType, int cost)
    {
        boxID = id;
        
        var data = JsonDataManager.Get().dataBoxInfo.GetData(id);
        var classData = new List<RewardData>(data.classRewards[UserInfoManager.Get().GetUserInfo().nClass]);
        
        // Gold
        var goldData = classData.Find(d => d.rewardType == ERewardType.Gold);
        obj_Gold.SetActive(goldData != null);
        if (goldData != null)
        {
            text_Gold.text = goldData.value.ToString();
        }
        
        int totalDiceCount = 0;
        // Normal Dice
        var normalDiceData = classData.Find(d => d.rewardType == ERewardType.DiceNormal);
        if (normalDiceData != null) totalDiceCount += normalDiceData.value;
        
        // Magic Dice
        var magicDiceData = classData.Find(d => d.rewardType == ERewardType.DiceMagic);
        if (magicDiceData != null) totalDiceCount += magicDiceData.value;

        // Epic Dice
        var epicDiceData = classData.Find(d => d.rewardType == ERewardType.DiceEpic);
        if (epicDiceData != null) totalDiceCount += epicDiceData.value;

        // Legend Dice
        var legendDiceData = classData.Find(d => d.rewardType == ERewardType.DiceLegend);
        if (legendDiceData != null) totalDiceCount += legendDiceData.value;
        
        // Total Dice
        obj_Dice.SetActive(totalDiceCount > 0);
        if (totalDiceCount > 0) text_Dice.text = $"x{totalDiceCount}";
        
        // Diamond
        var diamondData = classData.Find(d => d.rewardType == ERewardType.Diamond);
        obj_Diamond.SetActive(diamondData != null);
        if (diamondData != null)
        {
            text_Diamond.text = diamondData.value.ToString();
        }
        
        // Include Magic Dice
        obj_MagicDice.SetActive(magicDiceData.value > 0);
        if (magicDiceData.value > 0) text_MagicDice.text = $"x{magicDiceData.value}";
        
        // Include Epic Dice
        obj_EpicDice.SetActive(epicDiceData.value > 0);
        if (epicDiceData.value > 0) text_EpicDice.text = $"x{epicDiceData.value}";
        
        // Include Legend Dice
        obj_LegendDice.SetActive(legendDiceData.value > 0);
        if (legendDiceData.value > 0) text_LegendDice.text = $"x{legendDiceData.value}";

        image_CostIcon.sprite = arrSprite_CostType[(int)costType];
        text_Cost.text = cost.ToString();
    }

    public void Click_Open()
    {
        NetworkManager.Get().OpenBoxReq(UserInfoManager.Get().GetUserInfo().userID, boxID, Callback_BoxOpen);
    }

    public void Callback_BoxOpen(MsgOpenBoxAck msg)
    {
        for (int i = 0; i < msg.BoxReward.Length; i++)
        {
            Debug.Log($"Reward   ID:{msg.BoxReward[i].Id} , Type:{msg.BoxReward[i].RewardType.ToString()}, Count:{msg.BoxReward[i].Value}");
        }
    }
}
