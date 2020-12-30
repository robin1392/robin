using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = ED.Debug;

public class UI_RankingReward : MonoBehaviour
{
    public void Initialize(int ranking)
    {
        Debug.Log($"Ranking:{ranking}");

        int dicCount = TableManager.Get().RankingReward.Keys.Count;
        //TDataRankingReward data = new TDataRankingReward();

        // for (int i = 1; i <= dicCount; i++)
        // {
        //     TableManager.Get().RankingReward.GetData(i, out data);
        //     Debug.Log($"data min:{data.rankMin}, max:{data.rankMax}");
        // }
    }
}
