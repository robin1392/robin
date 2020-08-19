using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG;
using DG.Tweening;

namespace ED
{

    public class InfoUI
    {
        public Text textType;
        public Text textValue;
    }
    
    public class UI_Popup_Dice_Info : UI_Popup
    {
        //
        public const int INFOCOUNT = 8;
        
        public UI_Panel_Dice ui_Panel_Dice;
        public UI_Getted_Dice ui_getted_dice;
        public Image image_Character;

        [Header("Info")] 
        public Text text_Name;
        public Text text_Discription;

        //private Data_Dice data;
        private DiceInfoData data;

        //
        private List<InfoUI> listInfoUI = new List<InfoUI>();
        
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

            ConnentUIInfo();
        }

        //public void Initialize(Data_Dice pData)
        public void Initialize(DiceInfoData pData)
        {
            data = pData;
            ui_getted_dice.Initialize(data);
            //text_Name.text = pData.name;
            text_Name.text = LocalizationManager.GetLangDesc(data.id);
            text_Discription.text = LocalizationManager.GetLangDesc( (int)LANG_ENUM.DICE_DESC + data.id);

            SetInfoDesc();
        }

        public override void Close()
        {
            CloseDisConnectInfo();
            
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
        
        #region ui info

        public void ConnentUIInfo()
        {
            if(listInfoUI == null)
                listInfoUI = new List<InfoUI>();
            
            listInfoUI.Clear();

            for (int i = 0; i < INFOCOUNT; i++)
            {
                InfoUI info = new InfoUI();
                info.textType = this.transform.Find("Frame/Image_Inner_Frame/Infos/UI_Dice_Info_0" + i.ToString() + "/Text_Type")
                    .GetComponent<Text>();
                
                info.textValue = this.transform.Find("Frame/Image_Inner_Frame/Infos/UI_Dice_Info_0" + i.ToString() + "/Text_Value")
                    .GetComponent<Text>();
                
                listInfoUI.Add(info);
            }
        }

        public void CloseDisConnectInfo()
        {
            listInfoUI.Clear();
        }

        public void SetInfoDesc()
        {
            for (int i = 0; i < listInfoUI.Count; i++)
            {
                listInfoUI[i].textType.text = LocalizationManager.GetLangDesc( (int)LANG_ENUM.UI_DESC + i);
            }
            
            //listInfoUI[0].textValue.text = 
        }
        #endregion
        
        
    }
}
