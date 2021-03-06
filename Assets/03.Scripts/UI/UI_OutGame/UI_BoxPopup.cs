using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using UnityEngine.UI;
using Debug = ED.Debug;

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
        Debug.Log("Box Count: " + count);

        foreach (var boxData in UserInfoManager.Get().GetUserInfo().dicBox)
        {
            var obj = Instantiate(pref_BoxSlot, rts_Grid);
            var box = obj.GetComponent<UI_Box_Slot>();
            var id = boxData.Key;
            var subCount = boxData.Value;
            box.Initialize(id, subCount);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(rts_Grid);
    }

    private void ResizeFrame()
    {
        text_Empty.gameObject.SetActive(rts_Grid.childCount <= 0);

        var size = rts_Content.sizeDelta;
        size.y = rts_Grid.sizeDelta.y;
        rts_Content.sizeDelta = size;

        // Vector2 size = rts_Content.sizeDelta;
        // size.y = Mathf.Clamp((rts_Grid.childCount + 2) / 3 * 480 + 80, 520, Int32.MaxValue);
        // rts_Content.sizeDelta = size;
        // Vector2 svFrameSize = rts_ScrollViewFrameBG.sizeDelta;
        // svFrameSize.y = Mathf.Clamp(size.y, 600, 1200);
        // rts_ScrollViewFrameBG.sizeDelta = svFrameSize;
        // Vector2 svSize = rts_ScrollView.sizeDelta;
        // svSize.y = Mathf.Clamp(size.y, 560, 1200);
        // rts_ScrollView.sizeDelta = svSize;
        // Vector2 frameSize = rts_Frame.sizeDelta;
        // frameSize.y = Mathf.Clamp(size.y, 600, 1200) + 240;
        // rts_Frame.sizeDelta = frameSize;
    }
}
