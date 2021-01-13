using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ED
{
    public class UI_Popup : MonoBehaviour
    {
        public bool isAutoOpen = true;
        public RectTransform rts_Frame;
        public Image image_BG;

        [Header("Sound")]
        public AudioClip clip_Open;
        public AudioClip clip_Close;

        protected Button btn_BG_Close;
        public static Stack<UI_Popup> stack = new Stack<UI_Popup>();

        private void Awake()
        {
            btn_BG_Close = image_BG.GetComponent<Button>();
        }

        protected virtual void OnEnable()
        {
            rts_Frame.localScale = Vector3.zero;
            
            if (isAutoOpen)
            {
                Open();
            }
        }

        protected void Open()
        {
            if (btn_BG_Close != null) btn_BG_Close.interactable = false;
            rts_Frame.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                if (btn_BG_Close != null) btn_BG_Close.interactable = true;
            });
            image_BG.DOFade(0.95f, 0.2f);

            stack.Push(this);
            SoundManager.instance.Play(clip_Open);
        }

        public virtual void Close()
        {
            if (btn_BG_Close != null) btn_BG_Close.interactable = false;
            rts_Frame.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
            image_BG.DOFade(0, 0.2f);

            if (stack.Contains(this) && stack.Peek() == this)
            {
                stack.Pop();
            }
            SoundManager.instance.Play(clip_Close);
        }
    }
}
