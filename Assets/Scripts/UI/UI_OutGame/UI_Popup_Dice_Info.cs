using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG;
using DG.Tweening;

namespace ED
{
    public class UI_Popup_Dice_Info : UI_Popup
    {
        public UI_Panel_Dice ui_Panel_Dice;
        public UI_Getted_Dice ui_getted_dice;
        public Image image_Character;

        [Header("Info")] 
        public Text text_Name;
        public Text text_Discription;

        //private Data_Dice data;
        private DiceInfoData data;
        
        
        #region Base Region
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            image_Character.color = Color.clear;
            image_Character.DOColor(Color.white, 0.2f).SetDelay(0.1f);
            var anchPos = image_Character.rectTransform.anchoredPosition;
            anchPos.y = -1000f;
            image_Character.rectTransform.anchoredPosition = anchPos;
            image_Character.rectTransform.DOAnchorPosY(anchPos.y + 400f, 0.2f).SetEase(Ease.OutBack).SetDelay(0.1f);
        }

        //public void Initialize(Data_Dice pData)
        public void Initialize(DiceInfoData pData)
        {
            data = pData;
            ui_getted_dice.Initialize(pData);
            text_Name.text = pData.name;
            text_Discription.text = pData.name;
        }

        public override void Close()
        {
            image_Character.rectTransform.DOAnchorPosY(image_Character.rectTransform.anchoredPosition.y - 400f, 0.2f).SetEase(Ease.InBack);
            image_Character.DOFade(0, 0.2f);
            image_BG.DOFade(0, 0.2f).SetDelay(0.1f);
            rts_Frame.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).SetDelay(0.1f).OnComplete(()=>
            {
                gameObject.SetActive(false);
            });
        }

        public void Click_Use()
        {
            Close();
            ui_Panel_Dice.Click_Dice_Use(data.id);
        }
        
        #endregion
        
        
    }
}
