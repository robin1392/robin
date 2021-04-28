using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG;
using DG.Tweening;
//using RandomWarsProtocol;
//using RandomWarsProtocol.Msg;
using Service.Core;
using Template.Quest.RandomwarsQuest.Common;
using Template.Character.RandomwarsDice.Common;
using RandomWarsResource.Data;
using UnityEngine.EventSystems;

namespace ED
{
    [Serializable]
    public class InfoUI
    {
        public Image imageBG;
        public Text textType;
        public Text textValue;
        public Text textPlus;
    }

    public class InfoData
    {
        public bool isGuardian;
        public int id;
        public string iconName;
        public string illustName;
        public float maxHealth;
        public float maxHpUpgrade;
        public float maxHpInGameUp;
        public float power;
        public float powerUpgrade;
        public float powerInGameUp;
        public float effect;
        public float effectUpgrade;
        public float effectInGameUp;
        public float effectCooltime;
        public float effectDuration;
        public float attackSpeed;
        public float moveSpeed;
        public float range;
        public int[] color;
        public DICE_MOVE_TYPE targetMoveType;
        public DICE_GRADE grade;
        public DICE_CAST_TYPE castType;

        public InfoData(RandomWarsResource.Data.TDataDiceInfo pData)
        {
            isGuardian = false;
            id = pData.id;
            iconName = pData.iconName;
            illustName = pData.illustName;
            maxHealth = pData.maxHealth;
            maxHpUpgrade = pData.maxHpUpgrade;
            maxHpInGameUp = pData.maxHpInGameUp;
            power = pData.power;
            powerUpgrade = pData.powerUpgrade;
            powerInGameUp = pData.powerInGameUp;
            effect = pData.effect;
            effectUpgrade = pData.effectUpgrade;
            effectInGameUp = pData.effectInGameUp;
            effectCooltime = pData.effectCooltime;
            effectDuration = pData.effectDurationTime;
            attackSpeed = pData.attackSpeed;
            moveSpeed = pData.moveSpeed;
            range = pData.range;
            color = pData.color;
            targetMoveType = (DICE_MOVE_TYPE)pData.targetMoveType;
            grade = (DICE_GRADE)pData.grade;
            castType = (DICE_CAST_TYPE)pData.castType;
        }
        
        public InfoData(RandomWarsResource.Data.TDataGuardianInfo pData)
        {
            isGuardian = true;
            id = pData.id;
            iconName = pData.iconName;
            illustName = pData.illustName;
            maxHealth = pData.maxHealth;
            maxHpUpgrade = pData.maxHpUpgrade;
            maxHpInGameUp = pData.maxHpInGameUp;
            power = pData.power;
            powerUpgrade = pData.powerUpgrade;
            powerInGameUp = pData.powerInGameUp;
            effect = pData.effect;
            effectUpgrade = pData.effectUpgrade;
            effectInGameUp = pData.effectInGameUp;
            effectCooltime = pData.effectCooltime;
            effectDuration = pData.effectDuration;
            attackSpeed = pData.attackSpeed;
            moveSpeed = pData.moveSpeed;
            range = pData.range;
            color = pData.color;
            targetMoveType = (DICE_MOVE_TYPE)pData.targetMoveType;
            grade = (DICE_GRADE)pData.grade;
            castType = (DICE_CAST_TYPE)pData.castType;
        }
    }
    
    public class UI_Popup_Dice_Info : UI_Popup
    {
        //
        public readonly int INFOCOUNT = 8;
        
        public UI_Panel_Dice ui_Panel_Dice;
        public UI_Getted_Dice ui_getted_dice;
        public Image image_Character;

        [Header("Info")] 
        public Text text_Name;
        public Text text_Discription;
        public Text text_Grade;
        public Image image_GradeBG;
        public Text text_TowerHP;
        public Text text_TowerHPUpgrade;
        public Text text_Type;
        public Text text_TypeInfo;

        [Header("Button")]
        public Button btn_Upgrade;
        public Text text_Upgrade;
        public Text text_UpgradeGold;
        public Button btn_Use;
        public Text text_Use;
        public Button btn_ShowUpgrade;
        public Button btn_ShowLevelUp;

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

        [Header("Color")] public Color color_ClassUp;
        public Color color_PowerUp;

        //private Data_Dice data;
        //private RandomWarsResource.Data.TDataDiceInfo data;
        private InfoData data;

        [Space]
        public Transform infosTranform;
        //
        [SerializeField]
        private List<InfoUI> listInfoUI = new List<InfoUI>();
        private int diceLevel;
        private int needGold;
        private int needDiceCount;
        private int upgradeTowerHp;
        private readonly string none = "--";
        
        #region Base Region
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            image_Character.color = Color.clear;
            image_Character.DOColor(Color.white, 0.2f).SetDelay(0.1f);
            //var anchPos = image_Character.rectTransform.anchoredPosition;
            //anchPos.y -= 1000f;
            //image_Character.rectTransform.anchoredPosition = anchPos;
            //image_Character.rectTransform.DOAnchorPosY(0, 0.2f).SetEase(Ease.OutBack).SetDelay(0.1f);
        }

        //public void Initialize(Data_Dice pData)
        public void Initialize(TDataDiceInfo pData)
        {
            data = new InfoData(pData);
            Initialize();
        }
        
        public void Initialize(TDataGuardianInfo pData)
        {
            data = new InfoData(pData);
            Initialize();
        }

        private void Initialize()
        {
            gameObject.SetActive(true);
            
            diceLevel = 0;
            int diceCount = 0;

            // RandomWarsResource.Data.TDataDiceInfo dataDiceInfo;
            // if (TableManager.Get().DiceInfo.GetData(data.id, out dataDiceInfo) == false)
            // {
            //     return;
            // }

            image_Character.sprite = FileHelper.GetIcon(data.illustName);

            if (UserInfoManager.Get().GetUserInfo().dicGettedDice.ContainsKey(data.id))
            {
                diceLevel = UserInfoManager.Get().GetUserInfo().dicGettedDice[data.id][0];
                diceCount = UserInfoManager.Get().GetUserInfo().dicGettedDice[data.id][1];
                btn_Use.gameObject.SetActive(true);
                btn_Upgrade.gameObject.SetActive(true);
            }
            else
            {
                diceLevel = 1;
                diceCount = 0;
                btn_Use.gameObject.SetActive(false);
                btn_Upgrade.gameObject.SetActive(false);
            }

            int bonusTowerHp = 0;
            if (data.isGuardian == false)
            {
                RandomWarsResource.Data.TDataDiceUpgrade dataDiceCurrentUpgrade;
                RandomWarsResource.Data.TDataDiceUpgrade dataDiceUpgrade;
                if (TableManager.Get().DiceUpgrade.GetData(x => x.diceLv == diceLevel && x.diceGrade == (int)data.grade, out dataDiceCurrentUpgrade) == false)
                {
                    return;
                }
                if (TableManager.Get().DiceUpgrade.GetData(x => x.diceLv == diceLevel + 1 && x.diceGrade == (int)data.grade, out dataDiceUpgrade) == false)
                {
                    return;
                }
                needGold = dataDiceUpgrade.needGold;
                needDiceCount = dataDiceUpgrade.needCard;
                bonusTowerHp = dataDiceCurrentUpgrade.getTowerHp;
                upgradeTowerHp = dataDiceUpgrade.getTowerHp;
            }

            ui_getted_dice.Initialize(data, diceLevel, diceCount);
            
            text_Name.text = LocalizationManager.GetLangDesc((int)LANG_ENUM.DICE_NAME + data.id);
            text_Discription.text = LocalizationManager.GetLangDesc( (int)LANG_ENUM.DICE_DESC + data.id);
            text_UpgradeGold.text = needGold.ToString();

            btn_Use.interactable = diceLevel > 0;
            btn_Upgrade.gameObject.SetActive(diceLevel > 0 && diceCount >= needDiceCount);
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

            if (data.isGuardian == false )
            {
                text_TowerHP.text = $"타워 HP {bonusTowerHp}";
                text_TowerHPUpgrade.text = string.Empty;
            }

            SetUnitGrade();
            SetInfoDesc();
            
            btn_Upgrade.gameObject.SetActive(btn_Upgrade.gameObject.activeSelf && !data.isGuardian);
            btn_ShowUpgrade.gameObject.SetActive(!data.isGuardian);
            btn_ShowLevelUp.gameObject.SetActive(!data.isGuardian);
            text_TowerHP.gameObject.SetActive(!data.isGuardian);

            SoundManager.instance.Play(clip_Open);
        }

        public override void Close()
        {
            //image_Character.rectTransform.DOAnchorPosY(image_Character.rectTransform.anchoredPosition.y - 1000f, 0.2f).SetEase(Ease.InBack);
            image_Character.DOFade(0, 0.2f);
            rts_Frame.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).SetDelay(0.1f).OnComplete(()=>
            {
                gameObject.SetActive(false);
            });
            image_BG.DOFade(0, 0.2f).SetDelay(0.1f);
            
            if (stack.Contains(this) && stack.Peek() == this)
            {
                stack.Pop();
            }

            SoundManager.instance.Play(clip_Close);
        }

        public void Click_Upgrade()
        {
            if (UserInfoManager.Get().GetUserInfo().gold >= needGold)
            {
                NetworkManager.session.DiceTemplate.DiceUpgradeReq(NetworkManager.session.HttpClient, data.id,
                    OnReceiveDiceUpgradeAck);

                UI_Main.Get().obj_IndicatorPopup.SetActive(true);
            }
            else
            {
                FindObjectOfType<UI_Popup_MoveShop>().Initialize(UI_BoxOpenPopup.COST_TYPE.GOLD);
            }
        }


        public bool OnReceiveDiceUpgradeAck(ERandomwarsDiceErrorCode errorCode, MsgDiceInfo diceInfo, QuestData[] arrayQuestData, int updateGold)
        {
            UI_Main.Get().obj_IndicatorPopup.SetActive(false);

            if (errorCode == ERandomwarsDiceErrorCode.Success)
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

                // Quest
                UI_Popup_Quest.QuestUpdate(arrayQuestData);
            }

            return true;
        }


        private bool isDiceLevelUpCompleted;
        private IEnumerator SetDiceLevelUpResultCoroutine()
        {
            SoundManager.instance.Play(Global.E_SOUND.SFX_UI_DICE_LVUP_EFX);
            
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

            var arrAdd = new float[3] { data.maxHpUpgrade, data.powerUpgrade, data.effectUpgrade};
            for (int i = 0; i < 3; i++)
            {
                float current = 0f;
                
                if (arrAdd[i] == 0) continue;
                
                var obj = Instantiate(pref_ResultStatSlot, rts_ResultStatParent);
                var ui = obj.GetComponent<UI_DiceLevelUpResultSlot>();

                switch (i)
                {
                    case 0:
                        current = data.maxHealth + data.maxHpUpgrade * diceLevel;
                        ui.Initialize(Global.E_DICEINFOSLOT.Info_Hp, current, arrAdd[i], 1.2f + 0.1f * i);
                        break;
                    case 1:
                        current = data.power + data.powerUpgrade * diceLevel;
                        ui.Initialize(Global.E_DICEINFOSLOT.Info_AtkPower, current, arrAdd[i], 1.2f + 0.1f * i);
                        break;
                    case 2:
                        current = data.effect + data.effectUpgrade + diceLevel;
                        ui.Initialize(Global.E_DICEINFOSLOT.Info_Skill, current, arrAdd[i], 1.2f + 0.1f * i);
                        break;
                }
                
                obj.transform.localScale = Vector3.zero;
                obj.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack).SetDelay(0.5f + 0.1f * i);
            }
            
            image_ResultDiceIcon.transform.DOScale(1.6f, 0.15f).SetDelay(1.7f).OnComplete(() =>
            {
                text_ResultDiceLevel.text = $"CLASS {diceLevel}";
                text_ResultDiceLevel.color = UnityUtil.HexToColor("71FA4A");
                image_ResultDiceIcon.transform.DOScale(1.4f, 0.15f);
                ps_ResultIconBackground.Play();
                SoundManager.instance.Play(Global.E_SOUND.SFX_UI_LVUP_RESULT);
            });
            text_ResultDiceLevel.transform.DOScale(1.2f, 0.15f).SetDelay(1.7f).OnComplete(() =>
            {
                text_ResultDiceLevel.transform.DOScale(1f, 0.15f);
            });
            
            yield return new WaitForSeconds(1f);
            
            UI_Main.Get().panel_Dice.RefreshGettedDice();
            UI_Main.Get().RefreshUserInfoUI();
            Initialize();

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

        public void SetUnitGrade()
        {
            //text_Grade
            int gradeindex = (int) LANG_ENUM.UI_GRADE_NORMAL;
            switch (data.grade)
            {
                case DICE_GRADE.NORMAL:
                    gradeindex = (int) LANG_ENUM.UI_GRADE_NORMAL;
                    break;
                case DICE_GRADE.MAGIC:
                    gradeindex = (int) LANG_ENUM.UI_GRADE_MAGIC;
                    break;
                case DICE_GRADE.EPIC:
                    gradeindex = (int) LANG_ENUM.UI_GRADE_EPIC;
                    break;
                case DICE_GRADE.LEGEND:
                    gradeindex = (int) LANG_ENUM.UI_GRADE_LEGEND;
                    break;
            }
            
            image_GradeBG.color = UnityUtil.HexToColor(Global.g_gradeColor[Mathf.Clamp((int)data.grade, 0, 3)]);
            text_Grade.text = LocalizationManager.GetLangDesc( gradeindex );
            text_Grade.color = image_GradeBG.color;
        }
        
        public void SetInfoDesc()
        {
            listInfoUI[0].textType.text = LocalizationManager.GetLangDesc( "Minioninfo_Hp");
            listInfoUI[1].textType.text = LocalizationManager.GetLangDesc( "Minioninfo_Movespd");
            listInfoUI[2].textType.text = LocalizationManager.GetLangDesc( "Minioninfo_Atk");
            listInfoUI[3].textType.text = LocalizationManager.GetLangDesc( "Minioninfo_Atkspd");
            listInfoUI[4].textType.text = LocalizationManager.GetLangDesc( "Minioninfo_Effect");
            listInfoUI[5].textType.text = LocalizationManager.GetLangDesc( "Minioninfo_Effectcooltime");
            listInfoUI[6].textType.text = LocalizationManager.GetLangDesc( "Minioninfo_Atkrange");
            listInfoUI[7].textType.text = LocalizationManager.GetLangDesc( "Minioninfo_Searchrange");

            string castLangIndex = string.Empty;
            string castInfoLangIndex = string.Empty;
            switch (data.castType)
            {
                case DICE_CAST_TYPE.MINION:
                    castLangIndex = "Minioninfo_Casttype0";
                    castInfoLangIndex = "Minioninfo_Casttypeguide0";
                    break;
                case DICE_CAST_TYPE.MAGIC:
                    castLangIndex = "Minioninfo_Casttype2";
                    castInfoLangIndex = "Minioninfo_Casttypeguide2";
                    break;
                case DICE_CAST_TYPE.INSTALLATION:
                    castLangIndex = "Minioninfo_Casttype3";
                    castInfoLangIndex = "Minioninfo_Casttypeguide3";
                    break;
                case DICE_CAST_TYPE.HERO:
                    castLangIndex = "Minioninfo_Casttype1";
                    castInfoLangIndex = "Minioninfo_Casttypeguide1";
                    break;
                default:
                    castLangIndex = "Minioninfo_Casttype4";
                    castInfoLangIndex = "Minioninfo_Casttypeguide4";
                    break;
            }
            
            text_Type.text = LocalizationManager.GetLangDesc(castLangIndex);
            text_TypeInfo.text = LocalizationManager.GetLangDesc(castInfoLangIndex);

            //listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Type].textValue.text = LocalizationManager.GetLangDesc( castLangIndex);
            float value = data.maxHealth + data.maxHpUpgrade * diceLevel;
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Hp].textValue.text = value > 0 ? $"{value:f1}" : none;
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Hp].textValue.DOFade(value > 0 ? 1f : 0.5f, 0f);
            
            value = data.power + data.powerUpgrade * diceLevel;
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_AtkPower].textValue.text = value > 0 ? $"{value:f1}" : none;
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_AtkPower].textValue.DOFade(value > 0 ? 1f : 0.5f, 0f);
            
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_AtkSpeed].textValue.text = data.attackSpeed > 0 ? $"{data.attackSpeed:f1}" : none;
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_AtkSpeed].textValue.DOFade(data.attackSpeed > 0 ? 1f : 0.5f, 0f);
            
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_MoveSpeed].textValue.text = data.moveSpeed > 0 ? $"{data.moveSpeed:f1}" : none;
            listInfoUI[(int) Global.E_DICEINFOSLOT.Info_MoveSpeed].textValue.DOFade(data.moveSpeed > 0 ? 1f : 0.5f, 0f);

            value = data.effect + data.effectUpgrade * diceLevel;
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Skill].textValue.text = value > 0 ? $"{value:f1}" : none;
            listInfoUI[(int) Global.E_DICEINFOSLOT.Info_Skill].textValue.DOFade(value > 0 ? 1f : 0.5f, 0f);
            
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Cooltime].textValue.text = data.effectCooltime > 0 ? $"{data.effectCooltime:f1}" : none;
            listInfoUI[(int) Global.E_DICEINFOSLOT.Info_Cooltime].textValue
                .DOFade(data.effectCooltime > 0 ? 1f : 0.5f, 0f);

            listInfoUI[(int) Global.E_DICEINFOSLOT.Info_Range].textValue.text =
                data.range > 0 ? $"{data.range:f1}" : none;
            listInfoUI[(int) Global.E_DICEINFOSLOT.Info_Range].textValue.DOFade(data.range > 0 ? 1f : 0.5f, 0f);
            
            switch ((DICE_MOVE_TYPE)data.targetMoveType)
            {
                case DICE_MOVE_TYPE.GROUND:
                    listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Target].textValue.text = LocalizationManager.GetLangDesc("Minioninfo_Targetmovetype0");
                    break;
                case DICE_MOVE_TYPE.FLYING:
                    listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Target].textValue.text = LocalizationManager.GetLangDesc("Minioninfo_Targetmovetype1");
                    break;
                case DICE_MOVE_TYPE.ALL:
                    listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Target].textValue.text = LocalizationManager.GetLangDesc("Minioninfo_Targetmovetype2");
                    break;
            }

        }
        #endregion

        public void ClassUpDown(BaseEventData baseEventData)
        {
            for (int i = 0; i < listInfoUI.Count; i++)
            {
                if (data.maxHpUpgrade > 0)
                {
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_Hp].imageBG.color = color_ClassUp;
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_Hp].textPlus.text = $"+{data.maxHpUpgrade}";
                }
                if (data.powerUpgrade > 0)
                {
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_AtkPower].imageBG.color = color_ClassUp;
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_AtkPower].textPlus.text = $"+{data.powerUpgrade:f1}";
                }
                if (data.effectUpgrade > 0)
                {
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_Skill].imageBG.color = color_ClassUp;
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_Skill].textPlus.text = $"+{data.effectUpgrade:f1}";
                }

                text_TowerHPUpgrade.text = $"+{upgradeTowerHp}";
            }
        }
        
        public void ClassUpUp(BaseEventData baseEventData)
        {
            for (int i = 0; i < listInfoUI.Count; i++)
            {
                listInfoUI[i].imageBG.color = Color.white;
                listInfoUI[i].textPlus.text = String.Empty;
            }
            text_TowerHPUpgrade.text = string.Empty;
        }
        
        public void PowerUpDown(BaseEventData baseEventData)
        {
            for (int i = 0; i < listInfoUI.Count; i++)
            {
                if (data.maxHpInGameUp > 0)
                {
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_Hp].imageBG.color = color_PowerUp;
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_Hp].textPlus.text = $"+{data.maxHpInGameUp}";
                }
                if (data.powerInGameUp > 0)
                {
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_AtkPower].imageBG.color = color_PowerUp;
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_AtkPower].textPlus.text = $"+{data.powerInGameUp:f1}";
                }
                if (data.effectInGameUp > 0)
                {
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_Skill].imageBG.color = color_PowerUp;
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_Skill].textPlus.text = $"+{data.effectInGameUp:f1}";
                } 
            }
        }
        
        public void PowerUpUp(BaseEventData baseEventData)
        {
            for (int i = 0; i < listInfoUI.Count; i++)
            {
                listInfoUI[i].imageBG.color = Color.white;
                listInfoUI[i].textPlus.text = String.Empty;
            }
        }
    }
}
