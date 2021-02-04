#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ED
{
    public class UI_ScrollViewEvent : EventTrigger
    {
        public enum SCROLL
        {
            NONE,
            HORIZONTAL,
            VERTICAL,
        }
        
        public UnityEngine.UI.ScrollRect scrollView;

        public Vector2 pointDownPos;

        public SCROLL scrollType;

        public UI_Main _main;
        // Start is called before the first frame update
        private void Start()
        {
            _main = GetComponentInParent<UI_Main>();
            if (scrollView == null) scrollView = GetComponent<UnityEngine.UI.ScrollRect>();
            //var trigger = GetComponent<EventTrigger>();
            EventTrigger.Entry entryBegin = new EventTrigger.Entry(),
                entryDrag = new EventTrigger.Entry(),
                entryEnd = new EventTrigger.Entry(),
                entrypotential = new EventTrigger.Entry(),
                entryScroll = new EventTrigger.Entry();
     
            entryBegin.eventID = EventTriggerType.BeginDrag;
            //entryBegin.callback.AddListener(data => { scrollView.OnBeginDrag((PointerEventData)data); });
            //entryBegin.callback.AddListener(data => { _main.OnBeginDrag(data);});
            this.triggers.Add(entryBegin);
     
            entryDrag.eventID = EventTriggerType.Drag;
            //entryDrag.callback.AddListener(data => { scrollView.OnDrag((PointerEventData)data); });
            //entryDrag.callback.AddListener(data => { _main.OnDrag(data); });
            this.triggers.Add(entryDrag);
     
            entryEnd.eventID = EventTriggerType.EndDrag;
            entryEnd.callback.AddListener(data => { scrollView.OnEndDrag((PointerEventData)data); });
            entryEnd.callback.AddListener(data => { _main.OnEndDrag(data); });
            this.triggers.Add(entryEnd);
     
            entrypotential.eventID = EventTriggerType.InitializePotentialDrag;
            entrypotential.callback.AddListener(data => { scrollView.OnInitializePotentialDrag((PointerEventData)data); });
            this.triggers.Add(entrypotential);
     
            entryScroll.eventID = EventTriggerType.Scroll;
            entryScroll.callback.AddListener(data => { scrollView.OnScroll((PointerEventData)data); });
            this.triggers.Add(entryScroll);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            //Debug.Log("PointerDown: " + eventData.currentInputModule.input.mousePosition);
            pointDownPos = eventData.currentInputModule.input.mousePosition;
            BroadcastMessage("DeactivateSelectedObject", SendMessageOptions.DontRequireReceiver);
        }
        
        public override void OnBeginDrag(PointerEventData eventData)
        {
            // Debug.LogFormat("{0}-{1}\n{2}-{3}", eventData.currentInputModule.input.mousePosition.x,
            //     _pointDownPos.x, eventData.currentInputModule.input.mousePosition.y,
            //     _pointDownPos.y);
            
            if (Mathf.Abs(eventData.currentInputModule.input.mousePosition.x - pointDownPos.x) >
                Mathf.Abs(eventData.currentInputModule.input.mousePosition.y - pointDownPos.y))
            {
                _main.OnBeginDrag(eventData);
                scrollView.enabled = false;
                scrollType = SCROLL.HORIZONTAL;
            }
            else
            {
                scrollType = SCROLL.VERTICAL;
                scrollView.OnBeginDrag(eventData);

                // var buttons = GetComponentsInChildren<UnityEngine.UI.Button>();
                // foreach (var btn in buttons)
                // {
                //     btn.enabled = false;
                // }
            }
        }

        public override void OnDrag(PointerEventData eventData)
        {
            switch (scrollType)
            {
                case SCROLL.HORIZONTAL:
                    _main.OnDrag(eventData);
                    break;
                case SCROLL.VERTICAL:
                    scrollView.OnDrag(eventData);
                    break;
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            switch (scrollType)
            {
                case SCROLL.HORIZONTAL:
                    _main.OnEndDrag(eventData);
                    break;
                case SCROLL.VERTICAL:
                    scrollView.OnEndDrag(eventData);
                    // var buttons = GetComponentsInChildren<UnityEngine.UI.Button>();
                    // foreach (var btn in buttons)
                    // {
                    //     btn.enabled = true;
                    // }
                    break;
            }

            scrollView.enabled = true;
            scrollType = SCROLL.NONE;
        }
    }
}
