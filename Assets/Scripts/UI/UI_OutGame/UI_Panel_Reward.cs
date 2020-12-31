using System.Collections;
using System.Collections.Generic;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_Panel_Reward : MonoBehaviour
{
    public GameObject pref_RewardSlot;

    [Space]
    public GameObject obj_SeasonPass;
    public GameObject obj_Trophy;
    
    [Space]
    public Transform ts_SeasonPassContent;
    public Transform ts_TrophyContent;

    private int tab;

    private void Start()
    {
        InitializeSeasonPass();
    }

    public void InitializeSeasonPass()
    {
        int totalSlotCount = TableManager.Get().SeasonpassReward.Keys.Count / 2;
        int seasonPassTrophy = UserInfoManager.Get().GetUserInfo().seasonTrophy;
        var firstData = new TDataSeasonpassReward();
        TableManager.Get().SeasonpassReward.GetData(1, out firstData);
        int height = firstData.trophyPoint;
        for (int i = 0; i < totalSlotCount + 1; i++)
        {
            var obj = Instantiate(pref_RewardSlot, Vector3.zero, Quaternion.identity, ts_SeasonPassContent);
            var slot = obj.GetComponent<UI_RewardSlot>();
            slot.Initialize(i, seasonPassTrophy);
        }
    }
    
    public void InitializeTrophy()
    {
        
    }
}
