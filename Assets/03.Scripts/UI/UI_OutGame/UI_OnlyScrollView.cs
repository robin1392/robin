#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_OnlyScrollView : EventTrigger
{
    public UI_ScrollViewEvent ui_ScrollViewEvent;

    private void Start()
    {
        if (ui_ScrollViewEvent == null)
        {
            ui_ScrollViewEvent = GetComponentInParent<UI_ScrollViewEvent>();
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (ui_ScrollViewEvent != null)
        {
            ui_ScrollViewEvent.pointDownPos = eventData.currentInputModule.input.mousePosition;
        }
    }
    
    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (ui_ScrollViewEvent != null)
        {
            ui_ScrollViewEvent.OnBeginDrag(eventData);
        }
    }
    
    public override void OnDrag(PointerEventData eventData)
    {
        if (ui_ScrollViewEvent != null)
        {
            ui_ScrollViewEvent.OnDrag(eventData);
        }
    }
    
    public override void OnEndDrag(PointerEventData eventData)
    {
        if (ui_ScrollViewEvent != null)
        {
            ui_ScrollViewEvent.OnEndDrag(eventData);
        }
    }
}
