using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG;
using DG.Tweening;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;

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
        public ParticleSystem ps_NormalCharacterEffect;
        public ParticleSystem ps_LegendCharacterEffect;

        [Header("Info")] 
        public Text text_Name;
        public Text text_Discription;
        public Text text_Grade;

        [Header("Button")]
        public Button btn_Upgrade;
        public Text text_Upgrade;
        public Text text_UpgradeGold;
        public Button btn_Use;
        public Text text_Use;

        [Header("Dice LevelUp Result")]
        public GameObject obj_Result;
        public Image image_ResultBG;
        public RawImage image_ResubtBGPattern;
        public Image image_ResultDiceIcon;
        public Text text_ResultDiceName;
        public Text text_ResultDiceLevel;
        public RectTransform rts_ResultStatParent;
        public GameObject pref_ResultStatSlot;
        public ParticleSystem ps_ResultIconBackground;

        //private Data_Dice data;
        private DiceInfoData data;

        [Space]
        public Transform infosTranform;
        //
        private List<InfoUI> listInfoUI = new List<InfoUI>();
        private int diceLevel;
        private int needGold;
        private int needDiceCount;
        
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
            diceLevel = 0;
            int diceCount = 0;

            image_Character.sprite = FileHelper.GetIllust(JsonDataManager.Get().dataDiceInfo.dicData[pData.id].illustName);
            ps_LegendCharacterEffect.gameObject.SetActive(pData.grade == (int)DICE_GRADE.LEGEND);
            ps_NormalCharacterEffect.gameObject.SetActive(!ps_LegendCharacterEffect.gameObject.activeSelf);

            if (ps_NormalCharacterEffect.gameObject.activeSelf)
            {
                var particles = ps_NormalCharacterEffect.GetComponentsInChildren<ParticleSystem>();
                for (int i = 0; i < particles.Length; i++)
                {
                    var module = particles[i].main;
                    module.startColor = UnityUtil.HexToColor(Global.g_gradeColor[pData.grade]);
                }
            }
                
            if (UserInfoManager.Get().GetUserInfo().dicGettedDice.ContainsKey(data.id))
            {
                diceLevel = UserInfoManager.Get().GetUserInfo().dicGettedDice[data.id][0];
                diceCount = UserInfoManager.Get().GetUserInfo().dicGettedDice[data.id][1];
            }

            needGold = JsonDataManager.Get().dataDiceLevelUpInfo.dicData[diceLevel + 1].levelUpNeedInfo[pData.grade].needGold;
            needDiceCount = JsonDataManager.Get().dataDiceLevelUpInfo.dicData[diceLevel + 1].levelUpNeedInfo[pData.grade].needDiceCount;
            ui_getted_dice.Initialize(data, diceLevel, diceCount);
            
            text_Name.text = LocalizationManager.GetLangDesc((int)LANG_ENUM.DICE_NAME + data.id);
            text_Discription.text = LocalizationManager.GetLangDesc( (int)LANG_ENUM.DICE_DESC + data.id);
            text_UpgradeGold.text = needGold.ToString();

            btn_Use.interactable = diceLevel > 0;
            btn_Upgrade.interactable = (diceLevel > 0) &&
                                        (UserInfoManager.Get().GetUserInfo().gold >= needGold) &&
                                       (diceCount >= needDiceCount);
            var images = btn_Upgrade.GetComponentsInChildren<Image>();
            for (int i = 1; i < images.Length; ++i)
            {
                images[i].color = btn_Upgrade.interactable ? Color.white : Color.gray;
            }

            var interactable = btn_Upgrade.interactable;
            text_Upgrade.color = interactable ? Color.white : Color.gray;
            text_UpgradeGold.color = interactable ? Color.white : Color.gray;
            
            images = btn_Use.GetComponentsInChildren<Image>();
            for (int i = 1; i < images.Length; ++i)
            {
                images[i].color = btn_Use.interactable ? Color.white : Color.gray;
            }
            text_Use.color = btn_Use.interactable ? Color.white : Color.gray;

            SetUnitGrade();
            SetInfoDesc();
        }

        public override void Close()
        {
            CloseContectInfo();
            
            if (ps_LegendCharacterEffect.gameObject.activeSelf) ps_LegendCharacterEffect.Stop();
            if (ps_NormalCharacterEffect.gameObject.activeSelf) ps_NormalCharacterEffect.Stop();
            image_Character.rectTransform.DOAnchorPosY(image_Character.rectTransform.anchoredPosition.y - 1000f, 0.2f).SetEase(Ease.InBack);
            image_Character.DOFade(0, 0.2f);
            image_BG.DOFade(0, 0.2f).SetDelay(0.1f);
            rts_Frame.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).SetDelay(0.1f).OnComplete(()=>
            {
                gameObject.SetActive(false);
            });

        }

        public void Click_Upgrade()
        {
            NetworkManager.Get().LevelUpDiceReq(UserInfoManager.Get().GetUserInfo().userID, data.id, DiceUpgradeCallback);
            
            UI_Main.Get().obj_IndicatorPopup.SetActive(true);
        }

        public void DiceUpgradeCallback(MsgLevelUpDiceAck msg)
        {
            UI_Main.Get().obj_IndicatorPopup.SetActive(false);

            if (msg.ErrorCode == 0)
            {
                var info = UserInfoManager.Get().GetUserInfo();
                if (info.dicGettedDice.ContainsKey(data.id))
                {
                    info.gold -= needGold;
                    diceLevel++;
                    info.dicGettedDice[data.id][0]++;
                    info.dicGettedDice[data.id][1] -= needDiceCount;
                    
                    obj_Result.SetActive(true);
                    StartCoroutine(SetDiceLevelUpResultCoroutine());
                }
            }
        }

        private bool isDiceLevelUpCompleted;
        private IEnumerator SetDiceLevelUpResultCoroutine()
        {
            isDiceLevelUpCompleted = false;
            
            image_ResultDiceIcon.transform.localScale = Vector3.zero;
            text_ResultDiceName.transform.localScale = Vector3.zero;
            text_ResultDiceLevel.transform.localScale = Vector3.zero;
            //rts_ResultStatParent.localScale = Vector3.zero;
            
            // Data set
            image_ResultDiceIcon.sprite = FileHelper.GetIcon(data.iconName);
            text_ResultDiceName.text = LocalizationManager.GetLangDesc((int)LANG_ENUM.DICE_NAME + data.id);
            text_ResultDiceLevel.text = $"LEVEL {diceLevel - 1}";
            text_ResultDiceLevel.color = Color.white;
            
            image_ResultBG.DOFade(0f, 0f);
            image_ResubtBGPattern.DOFade(0f, 0f);
            image_ResultBG.DOFade(1f, 0.2f);
            image_ResubtBGPattern.DOFade(0.007843f, 0.2f);
            
            for (int i = rts_ResultStatParent.childCount - 1; i >= 0; --i)
            {
                DestroyImmediate(rts_ResultStatParent.GetChild(i).gameObject);
            }

            yield return new WaitForSeconds(0.2f);

            image_Character.rectTransform.DOScale(1.2f, 0.1f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                image_Character.rectTransform.DOScale(1f, 0.1f);
            });

            image_ResultDiceIcon.rectTransform.DOScale(1.4f, 0.2f).SetEase(Ease.OutBack).SetDelay(0.2f);
            text_ResultDiceName.rectTransform.DOScale(1f, 0.2f).SetEase(Ease.OutBack).SetDelay(0.3f);
            text_ResultDiceLevel.rectTransform.DOScale(1f, 0.2f).SetEase(Ease.OutBack).SetDelay(0.4f);
            //rts_ResultStatParent.DOScale(1f, 0.2f).SetEase(Ease.OutBack).SetDelay(0.8f);

            for (int i = 0; i < 2; i++)
            {
                var obj = Instantiate(pref_ResultStatSlot, rts_ResultStatParent);
                var ui = obj.GetComponent<UI_DiceLevelUpResultSlot>();
                float current = 0f;
                float add = 0f;

                switch (i)
                {
                    case 0:
                        current = data.maxHealth + data.maxHpUpgrade * diceLevel;
                        add = data.maxHpUpgrade;
                        break;
                    case 1:
                        current = data.power + data.powerUpgrade * diceLevel;
                        add = data.powerUpgrade;
                        break;
                }
                
                ui.Initialize(i == 0 ? Global.E_DICEINFOSLOT.Info_Hp : Global.E_DICEINFOSLOT.Info_AtkPower,
                    current, add, 1.2f + 0.1f * i);
                
                obj.transform.localScale = Vector3.zero;
                obj.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack).SetDelay(0.5f + 0.1f * i);
            }
            
            image_ResultDiceIcon.transform.DOScale(1.6f, 0.15f).SetDelay(1.7f).OnComplete(() =>
            {
                text_ResultDiceLevel.text = $"LEVEL {diceLevel}";
                text_ResultDiceLevel.color = UnityUtil.HexToColor("71FA4A");
                image_ResultDiceIcon.transform.DOScale(1.4f, 0.15f);
                ps_ResultIconBackground.Play();
            });
            text_ResultDiceLevel.transform.DOScale(1.2f, 0.15f).SetDelay(1.7f).OnComplete(() =>
            {
                text_ResultDiceLevel.transform.DOScale(1f, 0.15f);
            });
            
            yield return new WaitForSeconds(1f);
            
            UI_Main.Get().panel_Dice.RefreshGettedDice();
            UI_Main.Get().RefreshUserInfoUI();
            Initialize(data);

            yield return new WaitForSeconds(1f);
            isDiceLevelUpCompleted = true;
        }

        public void Click_DiceLevelUpResult()
        {
            if (isDiceLevelUpCompleted)
            {
                obj_Result.SetActive(false);
            }
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
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Hp].textValue.text = $"{data.maxHealth + data.maxHpUpgrade * diceLevel}";
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_AtkPower].textValue.text = $"{(data.power + data.powerUpgrade * diceLevel):f1}";
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_AtkSpeed].textValue.text = $"{data.attackSpeed:f1}";
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_MoveSpeed].textValue.text = $"{data.moveSpeed:f1}";
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_SearchRange].textValue.text = $"{data.range:f1}";
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_etc].textValue.text = LocalizationManager.GetLangDesc( (int)LANG_ENUM.UI_NONEVALUE1);
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Sp].textValue.text = LocalizationManager.GetLangDesc( (int)LANG_ENUM.UI_NONEVALUE1);

        }
        #endregion
        
        
    }
}
