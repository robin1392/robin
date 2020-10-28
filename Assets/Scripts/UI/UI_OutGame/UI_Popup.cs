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
        public RectTransform rts_Frame;
        public Image image_BG;

        protected virtual void OnEnable()
        {
            rts_Frame.localScale = Vector3.zero;
            rts_Frame.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
            image_BG.DOFade(0.95f, 0.2f);
        }

        public virtual void Close()
        {
            rts_Frame.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                gameObject.SetActive(false);
            });
            image_BG.DOFade(0, 0.2f);
        }
    }
}
