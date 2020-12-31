﻿using System.Collections;
using System.Collections.Generic;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.UI;

public class UI_Panel_Reward : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject pref_RewardSlot;
    public GameObject pref_TrophyRewardSlot;

    [Space]
    public GameObject obj_SeasonPass;
    public GameObject obj_Trophy;
    
    [Space]
    public Transform ts_SeasonPassContent;
    public Transform ts_TrophyContent;

    private int tab;

    private void Start()
    {
        obj_SeasonPass.SetActive(true);
        obj_Trophy.SetActive(false);
        
        InitializeSeasonPass();
        InitializeTrophy();
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
        int totalSlotCount = TableManager.Get().ClassReward.Keys.Count / 2;
        int myTrophy = UserInfoManager.Get().GetUserInfo().trophy;
        int vip = UserInfoManager.Get().GetUserInfo().trophyRewardIds.Count <= 1 ? 0 : UserInfoManager.Get().GetUserInfo().trophyRewardIds[0];
        int normal = UserInfoManager.Get().GetUserInfo().trophyRewardIds.Count <= 1 ? 0 : UserInfoManager.Get().GetUserInfo().trophyRewardIds[1];
        for (int i = 0; i < totalSlotCount + 1; i++)
        {
            var obj = Instantiate(pref_TrophyRewardSlot, Vector3.zero, Quaternion.identity, ts_TrophyContent);
            var slot = obj.GetComponent<UI_TrophyRewardSlot>();
            slot.Initialize(i, myTrophy, vip, normal);
        }
    }
}