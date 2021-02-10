using System.Collections;
using System.Collections.Generic;
using RandomWarsProtocol;
using RandomWarsResource.Data;
using Service.Core;
using UnityEngine;
using UnityEngine.UI;

public class UI_Rank_Reward_Slot : MonoBehaviour
{
    public Image image_Rank;
    public Text text_Rank;
    public UI_RewardItem_Slot[] arrRewardItemSlots;

    public void Initialize(int tableId)
    {
        Debug.Log($"Rank reward slot initialize: rank id[{tableId}]");
        
        // 보상 이미지와 개수 설정
        TDataRankingReward data = new TDataRankingReward();
        if (TableManager.Get().RankingReward.GetData(tableId, out data))
        {
            if (data.rankRewardType == 1)
            {
                if (data.rankMin <= 4) text_Rank.text = $"{data.rankMin}위";
                else text_Rank.text = $"{data.rankMin}~{data.rankMax}위";
            }
            else if (data.rankRewardType == 2)
            {
                text_Rank.text = $"상위 {data.rankMax}%";
            }
            
            // arrText_RewardSlot[0].text = $"{data.rankRewardItem01}\nx{data.rankRewardIValue01}";
            // arrText_RewardSlot[1].text = $"{data.rankRewardItem02}\nx{data.rankRewardIValue02}";
            // arrText_RewardSlot[2].text = $"{data.rankRewardItem03}\nx{data.rankRewardIValue03}";
            // arrText_RewardSlot[3].text = $"{data.rankRewardItem04}\nx{data.rankRewardIValue04}";
            // arrText_RewardSlot[4].text = $"{data.rankRewardItem05}\nx{data.rankRewardIValue05}";
            arrRewardItemSlots[0].Initialize(data.rankRewardItem01, data.rankRewardIValue01);
            arrRewardItemSlots[1].Initialize(data.rankRewardItem02, data.rankRewardIValue02);
            arrRewardItemSlots[2].Initialize(data.rankRewardItem03, data.rankRewardIValue03);
            arrRewardItemSlots[3].Initialize(data.rankRewardItem04, data.rankRewardIValue04);
            arrRewardItemSlots[4].Initialize(data.rankRewardItem05, data.rankRewardIValue05);
            
            //if (data.rankMin <= UserInfoManager.Get().GetUserInfo().)
        }
    }

    public void Initialize(MsgRewardMultiple[] rewards)
    {
        if (rewards != null)
        {
            for (int i = 0; i < arrRewardItemSlots.Length; i++)
            {
                // if (i < rewards.Length)
                // {
                //     arrText_RewardSlot[i].text = $"{rewards[i].ItemId}\nx{rewards[i].Value}";
                // }
                // else
                // {
                //     arrText_RewardSlot[i].text = string.Empty;
                // }
                if (i < rewards.Length)
                {
                    arrRewardItemSlots[i].Initialize(rewards[i].ItemId,
                        rewards[i].ItemId > 100 && rewards[i].ItemId < 1000 ? 1 : rewards[i].arrayReward[0].Value);
                }
                else
                {
                    arrRewardItemSlots[i].Initialize(0, 0);
                }
            }
        }
        else
        {
            for (int i = 0; i < arrRewardItemSlots.Length; i++)
            {
                arrRewardItemSlots[i].Initialize(0, 0);
            }
        }
    }
}
