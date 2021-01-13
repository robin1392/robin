using System.Collections;
using System.Collections.Generic;
using RandomWarsProtocol;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_Rank_Reward_Slot : MonoBehaviour
{
    public Image image_Rank;
    public Text text_Rank;
    public Image[] arrImage_RewardSlot;
    public Text[] arrText_RewardSlot;

    public Sprite[] arrSprite_Rank;
    
    public void Initialize(int tableId)
    {
        Debug.Log($"Rank reward slot initialize: rank id[{tableId}]");
        
        // 보상 이미지와 개수 설정
        TDataRankingReward data = new TDataRankingReward();
        if (TableManager.Get().RankingReward.GetData(tableId, out data))
        {
            if (data.rankRewardType == 1)
            {
                image_Rank.sprite = arrSprite_Rank[Mathf.Clamp(data.rankMin - 1, 0, 3)];
                if (data.rankMin < 4) text_Rank.text = string.Empty;
                else if (data.rankMin == 4) text_Rank.text = data.rankMin.ToString();
                else text_Rank.text = $"{data.rankMin}~{data.rankMax}";
            }
            else if (data.rankRewardType == 2)
            {
                image_Rank.sprite = arrSprite_Rank[3];
                text_Rank.text = $"상위\n{data.rankMax}%";
            }
            //text_Rank.text = data.rankMin > 3 ? data.rankMin.ToString() : string.Empty;
            
            arrText_RewardSlot[0].text = $"{data.rankRewardItem01}\nx{data.rankRewardIValue01}";
            arrText_RewardSlot[1].text = $"{data.rankRewardItem02}\nx{data.rankRewardIValue02}";
            arrText_RewardSlot[2].text = $"{data.rankRewardItem03}\nx{data.rankRewardIValue03}";
            arrText_RewardSlot[3].text = $"{data.rankRewardItem04}\nx{data.rankRewardIValue04}";
            arrText_RewardSlot[4].text = $"{data.rankRewardItem05}\nx{data.rankRewardIValue05}";
        }
    }

    public void Initialize(MsgReward[] rewards)
    {
        if (rewards != null)
        {
            for (int i = 0; i < arrText_RewardSlot.Length; i++)
            {
                if (i < rewards.Length)
                {
                    arrText_RewardSlot[i].text = $"{rewards[i].ItemId}\nx{rewards[i].Value}";
                }
                else
                {
                    arrText_RewardSlot[i].text = string.Empty;
                }
            }
        }
    }
}
