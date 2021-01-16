using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup_Rank_Reward : UI_Popup
{
    [Header("Prefab")]
    public GameObject pref_RankRewardSlot;

    [Header("Link")]
    public Transform ts_Content;
    public List<UI_Rank_Reward_Slot> listSlot = new List<UI_Rank_Reward_Slot>();

    private int pageNum;

    public void Initialize()
    {
        gameObject.SetActive(true);
        SetSlots();
    }

    private void SetSlots()
    {
        // switch (pageNum)
        // {
        //     case 0:
        //     {
        //         int[] arrID = {1, 2, 3, 4, 5};
        //         for (int i = 0; i < arrID.Length; i++)
        //         {
        //             arrSlot[i].Initialize(arrID[i]);
        //         }
        //
        //         arrSlot[arrSlot.Length - 1].gameObject.SetActive(false);
        //     }
        //         break;
        //     case 1:
        //     {
        //         int[] arrID = {6, 7, 8, 9, 10};
        //         for (int i = 0; i < arrID.Length; i++)
        //         {
        //             arrSlot[i].Initialize(arrID[i]);
        //         }
        //
        //         arrSlot[arrSlot.Length - 1].gameObject.SetActive(false);
        //     }
        //         break;
        //     case 2:
        //     {
        //         int[] arrID = {11, 12, 13, 14, 15, 16};
        //         for (int i = 0; i < arrID.Length; i++)
        //         {
        //             arrSlot[i].Initialize(arrID[i]);
        //         }
        //
        //         arrSlot[arrSlot.Length - 1].gameObject.SetActive(true);
        //     }
        //         break;
        //     case 3:
        //     {
        //         int[] arrID = {17, 18, 19, 20, 21, 22};
        //         for (int i = 0; i < arrID.Length; i++)
        //         {
        //             arrSlot[i].Initialize(arrID[i]);
        //         }
        //
        //         arrSlot[arrSlot.Length - 1].gameObject.SetActive(true);
        //     }
        //         break;
        // }

        if (listSlot.Count == 0)
        {
            int count = TableManager.Get().RankingReward.Keys.Count;
            for (int i = 0; i < count; i++)
            {
                var slot = Instantiate(pref_RankRewardSlot, ts_Content).GetComponent<UI_Rank_Reward_Slot>();
                slot.Initialize(i + 1);
                listSlot.Add(slot);
            }
        }
    }

    public void Click_Page(int num)
    {
        pageNum = num;
        SetSlots();
    }
}
