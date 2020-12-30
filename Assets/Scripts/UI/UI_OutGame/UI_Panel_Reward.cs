using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Panel_Reward : MonoBehaviour
{
    public GameObject pref_RewardSlot;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        //NetworkManager.Get().GetSeasonInfoReq(UserInfoManager.Get().GetUserInfo().userID, GetSeasonInfoCallback);
        //NetworkManager.Get().GetSeasonPassRewardReq(UserInfoManager.Get().GetUserInfo().userID, );

        // var keyvalue = TableManager.Get().SeasonpassReward.KeyValues;
        // foreach (var kvp in keyvalue)
        // {
        //     //Debug.Log($"{kvp.Value.id}");
        // }
    }
}
