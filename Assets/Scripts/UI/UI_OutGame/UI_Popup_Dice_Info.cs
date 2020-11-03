using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG;
using DG.Tweening;
using RandomWarsProtocol;

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
        public Text text_Grade;

        [Header("Button")]
        public Button btn_Upgrade;
        public Text text_Upgrade;
        public Text text_UpgradeGold;
        public Button btn_Use;

        //private Data_Dice data;
        private DiceInfoData data;

        public Transform infosTranform;
        //
        private List<InfoUI> listInfoUI = new List<InfoUI>();
        
        #region Base Region
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            image_Character.color = Color.clear;
            image_Character.DOColor(Color.white, 0.2f).SetDelay(0.1f);
            var anchPos = image_Character.rectTransform.anchoredPosition;
            //anchPos.y -= 1000f;
            //image_Character.rectTransform.anchoredPosition = anchPos;
            image_Character.rectTransform.DOAnchorPosY(0, 0.2f).SetEase(Ease.OutBack).SetDelay(0.1f);

            ContentUIInfo();
        }

        //public void Initialize(Data_Dice pData)
        public void Initialize(DiceInfoData pData)
        {
            data = pData;
            int diceLevel = 0;
            int diceCount = 0;

            image_Character.sprite = FileHelper.GetIllust(JsonDataManager.Get().dataDiceInfo.dicData[pData.id].illustName);
                
            if (UserInfoManager.Get().GetUserInfo().dicGettedDice.ContainsKey(data.id))
            {
                diceLevel = UserInfoManager.Get().GetUserInfo().dicGettedDice[data.id][0];
                diceCount = UserInfoManager.Get().GetUserInfo().dicGettedDice[data.id][1];
            }

            int goldCost = 0;//JsonDataManager.Get().dataDiceLevelUpInfo.dicData[diceLevel].;
            ui_getted_dice.Initialize(data, diceLevel, diceCount);
            
            text_Name.text = LocalizationManager.GetLangDesc((int)LANG_ENUM.DICE_NAME + data.id);
            text_Discription.text = LocalizationManager.GetLangDesc( (int)LANG_ENUM.DICE_DESC + data.id);

            btn_Use.interactable = diceLevel > 0;
            btn_Upgrade.interactable = (UserInfoManager.Get().GetUserInfo().gold >= goldCost) &&
                                       (diceCount >= Global.g_needDiceCount[diceLevel]);
            var images = btn_Upgrade.GetComponentsInChildren<Image>();
            foreach (var image in images)
            {
                image.color = btn_Upgrade.interactable ? Color.white : Color.gray;
            }

            var interactable = btn_Upgrade.interactable;
            text_Upgrade.color = interactable ? Color.white : Color.gray;
            text_UpgradeGold.color = interactable ? Color.white : Color.gray;

            SetUnitGrade();
            SetInfoDesc();
        }

        public override void Close()
        {
            CloseContectInfo();
            
            image_Character.rectTransform.DOAnchorPosY(image_Character.rectTransform.anchoredPosition.y - 1000f, 0.2f).SetEase(Ease.InBack);
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

        public void ContentUIInfo()
        {
            if(listInfoUI == null)
                listInfoUI = new List<InfoUI>();
            
            listInfoUI.Clear();


            if (infosTranform == null)
            {
                infosTranform = this.transform.Find("Frame/Image_Inner_Frame/Infos");
            }

            for (int i = 0; i < INFOCOUNT; i++)
            {
                InfoUI info = new InfoUI();
                info.textType = infosTranform.transform.Find("UI_Dice_Info_0" + i.ToString() + "/Text_Type").GetComponent<Text>();
                info.textValue = infosTranform.transform.Find("UI_Dice_Info_0" + i.ToString() + "/Text_Value").GetComponent<Text>();
                listInfoUI.Add(info);
            }
        }

        public void CloseContectInfo()
        {
            listInfoUI.Clear();
        }

        public void SetUnitGrade()
        {
            //text_Grade
            int gradeindex = (int) LANG_ENUM.UI_GRADE_NORMAL;
            switch (data.grade)
            {
                case (int)DICE_GRADE.NORMAL:
                    gradeindex = (int) LANG_ENUM.UI_GRADE_NORMAL;
                    break;
                case (int)DICE_GRADE.MAGIC:
                    gradeindex = (int) LANG_ENUM.UI_GRADE_MAGIC;
                    break;
                case (int)DICE_GRADE.EPIC:
                    gradeindex = (int) LANG_ENUM.UI_GRADE_EPIC;
                    break;
                case (int)DICE_GRADE.LEGEND:
                    gradeindex = (int) LANG_ENUM.UI_GRADE_LEGEND;
                    break;
            }
            
            text_Grade.text = LocalizationManager.GetLangDesc( gradeindex);
            
            if(data.grade == (int)DICE_GRADE.NORMAL)
                text_Grade.color = UnityUtil.HexToColor("FFFFFF");
            else
                text_Grade.color = UnityUtil.HexToColor(Global.g_gradeColor[data.grade]);
        }
        
        public void SetInfoDesc()
        {
            for (int i = 0; i < listInfoUI.Count; i++)
            {
                listInfoUI[i].textType.text = LocalizationManager.GetLangDesc( (int)LANG_ENUM.UI_DESC + i);
            }

            int castLangIndex = (int) LANG_ENUM.UI_TYPE_MINION;
            switch (data.castType)
            {
                case (int)DICE_CAST_TYPE.MINION:
                    castLangIndex = (int) LANG_ENUM.UI_TYPE_MINION;
                    break;
                case (int)DICE_CAST_TYPE.MAGIC:
                    castLangIndex = (int) LANG_ENUM.UI_TYPE_MAGIC;
                    break;
                case (int)DICE_CAST_TYPE.INSTALLATION:
                    castLangIndex = (int) LANG_ENUM.UI_TYPE_INSTALL;
                    break;
                case (int)DICE_CAST_TYPE.HERO:
                    castLangIndex = (int) LANG_ENUM.UI_TYPE_HERO;
                    break;
            }
            
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Type].textValue.text = LocalizationManager.GetLangDesc( castLangIndex);
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Hp].textValue.text = $"{data.maxHealth}";
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_AtkPower].textValue.text = $"{data.power:f1}";
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_AtkSpeed].textValue.text = $"{data.attackSpeed:f1}";
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_MoveSpeed].textValue.text = $"{data.moveSpeed:f1}";
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_SearchRange].textValue.text = $"{data.searchRange:f1}";
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_etc].textValue.text = LocalizationManager.GetLangDesc( (int)LANG_ENUM.UI_NONEVALUE1);
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Sp].textValue.text = LocalizationManager.GetLangDesc( (int)LANG_ENUM.UI_NONEVALUE1);

        }
        #endregion
        
        
    }
}
