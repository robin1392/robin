using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using UnityEngine.UI;

public class UI_BoxPopup : UI_Popup
{
    [Header("Box")]
    public GameObject pref_BoxSlot;

    public Text text_Empty;
    public RectTransform rts_ScrollViewFrameBG;
    public RectTransform rts_ScrollView;
    public RectTransform rts_Content;
    public RectTransform rts_Grid;

    public void Initialize()
    {
        Clear();
        SetBoxs();
        ResizeFrame();
    }

    public void RefreshBox()
    {
        Clear();
        SetBoxs();
        ResizeFrame();
    }

    private void Clear()
    {
        int count = rts_Grid.childCount;
        for (int i = count - 1; i >= 0; i--)
        {
            DestroyImmediate(rts_Grid.GetChild(i).gameObject);
        }
    }

    private void SetBoxs()
    {
        int count = UserInfoManager.Get().GetUserInfo().dicBox.Count;

        foreach (var boxData in UserInfoManager.Get().GetUserInfo().dicBox)
        {
            var obj = Instantiate(pref_BoxSlot, rts_Grid);
            var box = obj.GetComponent<UI_Box_Slot>();
            var id = boxData.Key;
            var subCount = boxData.Value;

            RandomWarsResource.Data.TDataItemList dataBoxList;
            if (TableManager.Get().ItemList.GetData(id, out dataBoxList) == false)
            {
                return;
            }
            box.Initialize(id, subCount, dataBoxList.openKeyValue);
        }
    }

    private void ResizeFrame()
    {
        text_Empty.gameObject.SetActive(rts_Grid.childCount <= 0);
        
        Vector2 size = rts_Content.sizeDelta;
        size.y = Mathf.Clamp((rts_Grid.childCount + 2) / 3 * 600, 600, Int32.MaxValue);
        rts_Content.sizeDelta = size;
        Vector2 svFrameSize = rts_ScrollViewFrameBG.sizeDelta;
        svFrameSize.y = Mathf.Clamp(size.y, 600, 1200) + 60;
        rts_ScrollViewFrameBG.sizeDelta = svFrameSize;
        Vector2 svSize = rts_ScrollView.sizeDelta;
        svSize.y = Mathf.Clamp(size.y, 600, 1200) + 10;
        rts_ScrollView.sizeDelta = svSize;
        Vector2 frameSize = rts_Frame.sizeDelta;
        frameSize.y = Mathf.Clamp(size.y, 600, 1200) + 300;
        rts_Frame.sizeDelta = frameSize;
    }
}
