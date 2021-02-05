﻿using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Debug = ED.Debug;

public class UI_Popup_Rank : UI_Popup
{
    public RectTransform rts_Content;
    
    [Header("Prefab")] 
    public GameObject pref_RankSlot;

    [Space]
    public Text text_RankMessage;
    
    public Text text_Season;
    public Text text_SeasonRemainTime;
    public Text text_MyRanking;
    public Text text_MyTrophy;

    public bool isInitialized;
    private bool isRankCalling;
    private int pageNum = 2;
    private List<UI_Rank_Slot> listSlot = new List<UI_Rank_Slot>();
    //private System.DateTime time;
    private float refreshTime = 0.1f;

    private void Update()
    {
        if (isInitialized)
        {
            refreshTime -= Time.deltaTime;

            if (refreshTime <= 0)
            {
                refreshTime = 0.1f;

                var span = UserInfoManager.Get().GetUserInfo().seasonEndTime.Subtract(DateTime.Now);

                if (span.TotalSeconds >= 0)
                {
                    text_SeasonRemainTime.text = string.Format("{0}Days {1:D2}:{2:D2}:{3:D2}", span.Days, span.Hours,
                        span.Minutes, span.Seconds);
                }
                else
                {
                    text_SeasonRemainTime.text = string.Empty;
                    Close();
                }
            }
        }
    }

    public void Initialize()
    {
        gameObject.SetActive(true);
        if (isInitialized)
        {
            Open();
        }
        else
        {
            isRankCalling = true;
            pageNum = 2;
            NetworkManager.Get().GetSeasonInfoReq(UserInfoManager.Get().GetUserInfo().userID, GetSeasonInfoCallback);
            UI_Main.Get().obj_IndicatorPopup.SetActive(true);
            //StartCoroutine(WaitCoroutine());
        }
    }

    public void GetSeasonInfoCallback(MsgSeasonInfoAck msg)
    {
        Invoke("RankCallingFalse", 1f);
        StopAllCoroutines();
        
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);

        if (msg.NeedSeasonReset)
        {
            //UI_Main.Get().seasonEndPopup.Initialize();
            
            UI_Main.Get().ShowMessageBox("시즌 종료", "시즌이 종료되었습니다.", UI_Main.Get().seasonEndPopup.Initialize);
            
            Close();
        }

        Debug.Log($"MsgSeasonInfoAck {msg.SeasonIndex} state {(SEASON_STATE)msg.SeasonState}, remainTime {msg.SeasonRemainTime}, needReset {msg.NeedSeasonReset}\n" +
                  $"MyRank:{msg.myRanking}, MyTrophy:{msg.myTrophy}, Time:{msg.SeasonRemainTime}");

        ResetSlots();
            
        switch ((SEASON_STATE)msg.SeasonState)
        {
            case SEASON_STATE.NONE:
                text_RankMessage.gameObject.SetActive(true);
                break;
            //case SEASON_STATE.PRE:
            //    text_RankMessage.gameObject.SetActive(true);
            //    break;
            case SEASON_STATE.GOING:
                isInitialized = true;
                text_RankMessage.gameObject.SetActive(false);
                
                text_Season.text = $"SEASON {msg.SeasonIndex}";
                //text_SeasonRemainTime.text = msg.SeasonRemainTime.ToString();
                //StartCoroutine(TimerCoroutine(msg.SeasonRemainTime));
                //time = DateTime.Now.AddSeconds(msg.SeasonRemainTime);
                text_MyRanking.text = msg.myRanking > 0 ? msg.myRanking.ToString() : string.Empty;
                text_MyTrophy.text = msg.myTrophy.ToString();
            
                Debug.Log($"RankInfoCount: {msg.TopRankInfo.Length}");
                AddSlots(msg.TopRankInfo);
                // for (int i = 0; i < listSlot.Count && i < msg.TopRankInfo.Length; i++)
                // {
                //     listSlot[i].Initialize(
                //         msg.TopRankInfo[i].Ranking,
                //         msg.TopRankInfo[i].Trophy,
                //         msg.TopRankInfo[i].Name,
                //         msg.TopRankInfo[i].Class,
                //         msg.TopRankInfo[i].DeckInfo
                //     );
                // }
                break;
            case SEASON_STATE.BREAK:
                text_RankMessage.gameObject.SetActive(true);
                break;
            case SEASON_STATE.END:
                text_RankMessage.gameObject.SetActive(true);
                break;
        }

        Open();
    }

    public void GetRankCallback(MsgGetRankAck msg)
    {
        Invoke("RankCallingFalse", 1f);
        
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
        if (msg != null && msg.ErrorCode == GameErrorCode.SUCCESS && msg.RankInfo != null)
        {
            Debug.Log($"Msg error code: {msg.ErrorCode}");
            Debug.Log($"MsgGetRankAck errorCode:{msg.ErrorCode} count:{msg.RankInfo.Length}");
            AddSlots(msg.RankInfo);
        }
    }

    private void RankCallingFalse()
    {
        isRankCalling = false;
    }

    IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(10f);
        
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
    }

    private void AddSlots(MsgRankInfo[] array)
    {
        text_RankMessage.gameObject.SetActive(array.Length == 0);
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] != null)
            {
                var slot = Instantiate(pref_RankSlot, Vector3.zero, Quaternion.identity, rts_Content)
                    .GetComponent<UI_Rank_Slot>();
                slot.Initialize(
                    array[i].Ranking,
                    array[i].Trophy,
                    array[i].Name,
                    array[i].Class,
                    array[i].DeckInfo
                );

                listSlot.Add(slot);
            }
        }
    }

    private void ResetSlots()
    {
        for (int i = listSlot.Count - 1; i >= 0; i--)
        {
            var d = listSlot[i];
            listSlot.Remove(d);
            DestroyImmediate(d.gameObject);
        }
    }

    public override void Close()
    {
        base.Close();

        // while (listSlot.Count > 0)
        // {
        //     var slot = listSlot[listSlot.Count - 1];
        //     listSlot.Remove(slot);
        //     Destroy(slot.gameObject);
        // }
    }

    public void ScrollChange(Vector2 v)
    {
        if (isInitialized && isRankCalling == false && v.y < 0 && pageNum < 11)
        {
            isRankCalling = true;
            
            UI_Main.Get().obj_IndicatorPopup.SetActive(true);
            NetworkManager.Get().GetRankReq(UserInfoManager.Get().GetUserInfo().userID, pageNum++, GetRankCallback);
        }
    }
}