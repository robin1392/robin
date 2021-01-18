using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_Panel_Reward : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject pref_RewardSlot;
    public GameObject pref_TrophyRewardSlot;

    [Space]
    public Image image_MenuSelected;
    public Button btn_SeasonPass;
    public Button btn_Trophy;
    public GameObject obj_SeasonPass;
    public GameObject obj_Trophy;
    
    [Space]
    public Transform ts_SeasonPassContent;
    public Transform ts_TrophyContent;

    [Header("Season Info")]
    public Text text_SeasonID;
    public Text text_SeasonName;
    public Text text_SeasonRemainTime;

    private bool isSeasonPassInitialized;
    private float refreshTime;

    private void Start()
    {
        obj_SeasonPass.SetActive(true);
        obj_Trophy.SetActive(false);
        
        InitializeSeasonPass();
        InitializeTrophy();
    }

    private void Update()
    {
        refreshTime -= Time.deltaTime;
        if (refreshTime <= 0)
        {
            refreshTime = 0.1f;

            var span = UserInfoManager.Get().GetUserInfo().seasonEndTime.Subtract(DateTime.Now);

            if (span.TotalSeconds >= 0)
            {
                if (span.Days > 0) text_SeasonRemainTime.text = $"{span.Days}일{span.Hours}시간{span.Minutes}분";
                else if (span.Hours > 0) text_SeasonRemainTime.text = string.Format("{0:D2}시간{1:D2}분", span.Hours, span.Minutes);
                else text_SeasonRemainTime.text = string.Format("{0:D2}분", span.Minutes);
            }
            else
            {
                text_SeasonRemainTime.text = string.Empty;

                if (isSeasonPassInitialized == true && UI_Popup.stack.Count == 0)
                {
                    isSeasonPassInitialized = false;
                    UI_Main.Get().ShowMessageBox("시즌 종료", "시즌이 종료되었습니다.", UI_Main.Get().seasonEndPopup.Initialize);
                }
            }
        }
    }

    public void InitializeSeasonPass()
    {
        UI_RewardSlot.isUnlockEnable = false;
        
        text_SeasonID.text = $"Season {UserInfoManager.Get().GetUserInfo().seasonPassId}";
        
        int totalSlotCount = TableManager.Get().SeasonpassReward.Keys.Count / 2;
        int seasonPassTrophy = UserInfoManager.Get().GetUserInfo().seasonTrophy;
        var firstData = new TDataSeasonpassReward();
        TableManager.Get().SeasonpassReward.GetData(1/*UserInfoManager.Get().GetUserInfo().seasonPassId*/, out firstData);
        int height = firstData.trophyPoint;
        for (int i = 1; i <= totalSlotCount; i++)
        {
            var obj = Instantiate(pref_RewardSlot, Vector3.zero, Quaternion.identity, ts_SeasonPassContent);
            var slot = obj.GetComponent<UI_RewardSlot>();
            slot.Initialize(i, seasonPassTrophy);
        }

        isSeasonPassInitialized = true;
    }
    
    public void InitializeTrophy()
    {
        int totalSlotCount = TableManager.Get().ClassReward.Keys.Count / 2;
        int myTrophy = UserInfoManager.Get().GetUserInfo().trophy;
        int normal = UserInfoManager.Get().GetUserInfo().trophyRewardIds.Count <= 1 ? 0 : UserInfoManager.Get().GetUserInfo().trophyRewardIds[0];
        int vip = UserInfoManager.Get().GetUserInfo().trophyRewardIds.Count <= 1 ? 0 : UserInfoManager.Get().GetUserInfo().trophyRewardIds[1];
        for (int i = 0; i < totalSlotCount + 1; i++)
        {
            var obj = Instantiate(pref_TrophyRewardSlot, Vector3.zero, Quaternion.identity, ts_TrophyContent);
            var slot = obj.GetComponent<UI_TrophyRewardSlot>();
            slot.Initialize(i, myTrophy, vip, normal);
        }
    }

    public void Click_TopButton(int index)
    {
        switch (index)
        {
            case 0:
                image_MenuSelected.rectTransform.DOAnchorPos(
                    ((RectTransform) btn_SeasonPass.transform).anchoredPosition, 0.2f).SetEase(Ease.OutBack);
                break;
            case 1:
                image_MenuSelected.rectTransform.DOAnchorPos(
                    ((RectTransform) btn_Trophy.transform).anchoredPosition, 0.2f).SetEase(Ease.OutBack);
                break;
        }
    }
}
