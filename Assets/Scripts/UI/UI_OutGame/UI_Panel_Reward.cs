using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using RandomWarsResource.Data;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Debug = ED.Debug;
using Image = UnityEngine.UI.Image;
using Slider = UnityEngine.UI.Slider;

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
    public ScrollView scrollView_SeasonPass;
    public ScrollView scrollView_Trophy;

    [Header("Season Info")]
    public Text text_SeasonID;
    public Text text_SeasonName;
    public Slider slider_Star;
    public Text text_StarCount;
    public Text text_StarLevel;
    public Text text_SeasonRemainTime;

    [Header("Trophy Info")]
    public Text text_MyTrophy;

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

        if (ts_SeasonPassContent.childCount > 0)
        {
            for (int i = ts_SeasonPassContent.childCount - 1; i >= 0; i--)
            {
                DestroyImmediate(ts_SeasonPassContent.GetChild(i).gameObject);
            }
        }
        
        RefreshSeasonInfo();
        
        int totalSlotCount = TableManager.Get().SeasonpassReward.Keys.Count;
        UI_RewardSlot.getNormalRow = UserInfoManager.Get().GetUserInfo().seasonPassRewardIds[0];
        UI_RewardSlot.getVipRow = UserInfoManager.Get().GetUserInfo().seasonPassRewardIds[1];
        
        for (int i = 1; i <= totalSlotCount; i++)
        {
            var obj = Instantiate(pref_RewardSlot, Vector3.zero, Quaternion.identity, ts_SeasonPassContent);
            var slot = obj.GetComponent<UI_RewardSlot>();
            slot.Initialize(i);//, seasonPassTrophy, vip, normal);
            
            if (i == 1) slot.SetSplitLine(true, false, false);
            else if (i == totalSlotCount) slot.SetSplitLine(false, false, true);
        }
        var empty = Instantiate(pref_RewardSlot, Vector3.zero, Quaternion.identity, ts_SeasonPassContent);
        empty.transform.GetChild(0).gameObject.SetActive(false);

        isSeasonPassInitialized = true;
    }

    public void RefreshSeasonInfo()
    {
        int myStar = UserInfoManager.Get().GetUserInfo().seasonTrophy;
        int starLevel = UserInfoManager.Get().GetUserInfo().seasonPassRewardStep;

        TDataSeasonpassReward currentRewardInfo;
        TableManager.Get().SeasonpassReward.GetData(starLevel, out currentRewardInfo);
        text_SeasonID.text = $"Season {UserInfoManager.Get().GetUserInfo().seasonPassId}";
        
        if (starLevel >= TableManager.Get().SeasonpassReward.Keys.Count)
        {
            //int needStar = NextRewardInfo.trophyPoint - currentRewardInfo.trophyPoint;
            text_StarCount.text = $"MAX";
            slider_Star.value = 1f;
            text_StarLevel.text = $"{starLevel}";
        }
        else
        {
            TDataSeasonpassReward NextRewardInfo;
            TableManager.Get().SeasonpassReward.GetData(starLevel + 1, out NextRewardInfo);
            int needStar = NextRewardInfo.trophyPoint - currentRewardInfo.trophyPoint;
            text_SeasonID.text = $"Season {UserInfoManager.Get().GetUserInfo().seasonPassId}";
            text_StarCount.text = $"{myStar - currentRewardInfo.trophyPoint}/{needStar}";
            slider_Star.value = (myStar - currentRewardInfo.trophyPoint) / (float)needStar;
            text_StarLevel.text = $"{starLevel}";
        }
    }
    
    public void InitializeTrophy()
    {
        int myTrophy = UserInfoManager.Get().GetUserInfo().trophy;
        text_MyTrophy.text = myTrophy.ToString();
        int totalSlotCount = TableManager.Get().ClassReward.Keys.Count;
        UI_TrophyRewardSlot.getNormalRow = UserInfoManager.Get().GetUserInfo().trophyRewardIds[0];
        UI_TrophyRewardSlot.getVipRow = UserInfoManager.Get().GetUserInfo().trophyRewardIds[1];
        for (int i = 1; i <= totalSlotCount; i++)
        {
            var obj = Instantiate(pref_TrophyRewardSlot, Vector3.zero, Quaternion.identity, ts_TrophyContent);
            var slot = obj.GetComponent<UI_TrophyRewardSlot>();
            slot.Initialize(i);
            
            if (i == 1) slot.SetSplitLine(true, false, false);
            else if (i == totalSlotCount) slot.SetSplitLine(false, false, true);
        }
        var empty = Instantiate(pref_TrophyRewardSlot, Vector3.zero, Quaternion.identity, ts_TrophyContent);
        empty.transform.GetChild(0).gameObject.SetActive(false);
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
