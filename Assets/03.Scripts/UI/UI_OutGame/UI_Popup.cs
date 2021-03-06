using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ED
{
    interface IPopup
    {
        void Close();
    }
    public class UI_Popup : MonoBehaviour
    {
        [Header("Setting")]
        public bool isAutoOpen = true;
        public bool isPopAnimation = true;
        public bool isDestroyWithClose = false;
        public float alphaBG = 0.95f;
        [Space]
        public RectTransform rts_Frame;
        public Image image_BG;

        [Header("Sound")]
        public AudioClip clip_Open;
        public AudioClip clip_Close;

        protected Button btn_BG_Close;
        protected bool isBgButtonEnable = true;
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

        protected virtual void Open()
        {
            if (btn_BG_Close != null) btn_BG_Close.interactable = false;
            StartCoroutine(BGButtonEnable());
            
            if (isPopAnimation)
            {
                rts_Frame.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack).OnComplete(() =>
                {
                    if (btn_BG_Close != null) btn_BG_Close.interactable = isBgButtonEnable;
                });
                image_BG.DOFade(alphaBG, 0.2f);
            }
            else
            {
                if (btn_BG_Close != null) btn_BG_Close.interactable = isBgButtonEnable;
                rts_Frame.DOScale(Vector3.one, 0f);
                image_BG.DOFade(alphaBG, 0f);
            }

            stack.Push(this);
            SoundManager.instance.Play(clip_Open);
        }

        IEnumerator BGButtonEnable()
        {
            yield return new WaitForSeconds(1f);
            btn_BG_Close.interactable = isBgButtonEnable;
        }

        public virtual void Close()
        {
            if (isPopAnimation)
            {
                if (btn_BG_Close != null) btn_BG_Close.interactable = false;
                rts_Frame.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    if (isDestroyWithClose)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                });
                image_BG.DOFade(0, 0.2f);
            }
            else
            {
                if (btn_BG_Close != null) btn_BG_Close.interactable = false;
                rts_Frame.DOScale(Vector3.zero, 0f);
                image_BG.DOFade(0, 0f);
                if (isDestroyWithClose)
                {
                    Destroy(gameObject);
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }

            if (stack.Contains(this) && stack.Peek() == this)
            {
                stack.Pop();
            }
            SoundManager.instance.Play(clip_Close);
        }

        public static void AllClose()
        {
            while (stack.Count > 0)
            {
                stack.Peek().Close();
            }
        }

        public static void ClosePop()
        {
            if (stack.Count > 0)
            {
                stack.Peek().Close();
            }
        }
    }
}
