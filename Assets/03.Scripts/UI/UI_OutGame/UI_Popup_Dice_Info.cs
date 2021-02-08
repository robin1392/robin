using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG;
using DG.Tweening;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
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
            effectDuration = pData.effectDuration;
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
        public Image image_GradeBG;
        public Text text_TowerHP;
        public Text text_Type;
        public Text text_TypeInfo;
        public Image image_Type;
        public Sprite[] arrSprite_Type;
        public Sprite[] arrSprite_InfoBG;

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
            ps_LegendCharacterEffect.gameObject.SetActive(data.grade == DICE_GRADE.LEGEND);
            ps_NormalCharacterEffect.gameObject.SetActive(!ps_LegendCharacterEffect.gameObject.activeSelf);

            if (ps_NormalCharacterEffect.gameObject.activeSelf)
            {
                var particles = ps_NormalCharacterEffect.GetComponentsInChildren<ParticleSystem>();
                for (int i = 0; i < particles.Length; i++)
                {
                    var module = particles[i].main;
                    module.startColor = UnityUtil.HexToColor(Global.g_gradeColor[Mathf.Clamp((int)data.grade, 0, 3)]);
                }
            }
                
            if (UserInfoManager.Get().GetUserInfo().dicGettedDice.ContainsKey(data.id))
            {
                diceLevel = UserInfoManager.Get().GetUserInfo().dicGettedDice[data.id][0];
                diceCount = UserInfoManager.Get().GetUserInfo().dicGettedDice[data.id][1];
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
            }

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

            if (data.isGuardian == false ) text_TowerHP.text = bonusTowerHp.ToString();

            SetUnitGrade();
            SetInfoDesc();
            
            btn_Upgrade.gameObject.SetActive(!data.isGuardian);
            btn_ShowUpgrade.gameObject.SetActive(!data.isGuardian);
            btn_ShowLevelUp.gameObject.SetActive(!data.isGuardian);
            text_TowerHP.transform.parent.gameObject.SetActive(!data.isGuardian);

            SoundManager.instance.Play(clip_Open);
        }

        public override void Close()
        {
            CloseContectInfo();
            
            if (ps_LegendCharacterEffect.gameObject.activeSelf) ps_LegendCharacterEffect.Stop();
            if (ps_NormalCharacterEffect.gameObject.activeSelf) ps_NormalCharacterEffect.Stop();
            image_Character.rectTransform.DOAnchorPosY(image_Character.rectTransform.anchoredPosition.y - 1000f, 0.2f).SetEase(Ease.InBack);
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
                
                // Quest
                UI_Popup_Quest.QuestUpdate(msg.QuestData);
            }
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
                InfoUI info = new InfoUI
                {
                    imageBG = infosTranform.transform.Find("UI_Dice_Info_0" + i.ToString()).GetComponent<Image>(),
                    textType = infosTranform.transform.Find("UI_Dice_Info_0" + i.ToString() + "/Text_Type")
                        .GetComponent<Text>(),
                    textValue = infosTranform.transform.Find("UI_Dice_Info_0" + i.ToString() + "/Text_Value")
                        .GetComponent<Text>(),
                    textPlus = infosTranform.transform.Find("UI_Dice_Info_0" + i.ToString() + "/Text_Plus")
                        .GetComponent<Text>()
                };
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
            
            text_Grade.text = LocalizationManager.GetLangDesc( gradeindex );
            //
            // if(data.grade == (int)DICE_GRADE.NORMAL)
            //     text_Grade.color = UnityUtil.HexToColor("FFFFFF");
            // else
            //     text_Grade.color = UnityUtil.HexToColor(Global.g_gradeColor[data.grade]);
            image_GradeBG.color = UnityUtil.HexToColor(Global.g_gradeColor[Mathf.Clamp((int)data.grade, 0, 3)]);
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
            listInfoUI[7].textType.text = LocalizationManager.GetLangDesc( "Minioninfo_searchRange");

            string castLangIndex = string.Empty;
            switch (data.castType)
            {
                case DICE_CAST_TYPE.MINION:
                    castLangIndex = "Gameui_Unit";
                    break;
                case DICE_CAST_TYPE.MAGIC:
                    castLangIndex = "Gameui_Magic";
                    break;
                case DICE_CAST_TYPE.INSTALLATION:
                    castLangIndex = "Gameui_Install";
                    break;
                case DICE_CAST_TYPE.HERO:
                    castLangIndex = "Gameui_Hero";
                    break;
                default:
                    castLangIndex = "Itemname_Guardian_5001";
                    break;
            }
            
            text_Type.text = LocalizationManager.GetLangDesc( castLangIndex);
            if (data.isGuardian)
            {
                image_Type.sprite = arrSprite_Type[4];
            }
            else
            {
                image_Type.sprite = arrSprite_Type[(int) data.castType];
            }
            //listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Type].textValue.text = LocalizationManager.GetLangDesc( castLangIndex);
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Hp].textValue.text = $"{data.maxHealth + data.maxHpUpgrade * diceLevel}";
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_AtkPower].textValue.text = $"{(data.power + data.powerUpgrade * diceLevel):f1}";
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_AtkSpeed].textValue.text = $"{data.attackSpeed:f1}";
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_MoveSpeed].textValue.text = $"{data.moveSpeed:f1}";
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Skill].textValue.text = $"{(data.effect + data.effectUpgrade * diceLevel):f1}";
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Cooltime].textValue.text = $"{data.effectCooltime:f1}";
            listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Range].textValue.text = $"{data.range:f1}";
            switch ((DICE_MOVE_TYPE)data.targetMoveType)
            {
                case DICE_MOVE_TYPE.GROUND:
                    listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Target].textValue.text = LocalizationManager.GetLangDesc("Minioninfo_targetmovetype0");
                    break;
                case DICE_MOVE_TYPE.FLYING:
                    listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Target].textValue.text = LocalizationManager.GetLangDesc("Minioninfo_targetmovetype1");
                    break;
                case DICE_MOVE_TYPE.ALL:
                    listInfoUI[(int)Global.E_DICEINFOSLOT.Info_Target].textValue.text = LocalizationManager.GetLangDesc("Minioninfo_targetmovetype2");
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
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_Hp].imageBG.sprite = arrSprite_InfoBG[1];
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_Hp].textPlus.text = $"+{data.maxHpUpgrade}";
                }
                if (data.powerUpgrade > 0)
                {
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_AtkPower].imageBG.sprite = arrSprite_InfoBG[1];
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_AtkPower].textPlus.text = $"+{data.powerUpgrade:f1}";
                }
                if (data.effectUpgrade > 0)
                {
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_Skill].imageBG.sprite = arrSprite_InfoBG[1];
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_Skill].textPlus.text = $"+{data.effectUpgrade:f1}";
                }
            }
        }
        
        public void ClassUpUp(BaseEventData baseEventData)
        {
            for (int i = 0; i < listInfoUI.Count; i++)
            {
                listInfoUI[i].imageBG.sprite = arrSprite_InfoBG[0];
                listInfoUI[i].textPlus.text = String.Empty;
            }
        }
        
        public void PowerUpDown(BaseEventData baseEventData)
        {
            for (int i = 0; i < listInfoUI.Count; i++)
            {
                if (data.maxHpInGameUp > 0)
                {
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_Hp].imageBG.sprite = arrSprite_InfoBG[2];
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_Hp].textPlus.text = $"+{data.maxHpInGameUp}";
                }
                if (data.powerInGameUp > 0)
                {
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_AtkPower].imageBG.sprite = arrSprite_InfoBG[2];
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_AtkPower].textPlus.text = $"+{data.powerInGameUp:f1}";
                }
                if (data.effectInGameUp > 0)
                {
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_Skill].imageBG.sprite = arrSprite_InfoBG[2];
                    listInfoUI[(int) Global.E_DICEINFOSLOT.Info_Skill].textPlus.text = $"+{data.effectInGameUp:f1}";
                } 
            }
        }
        
        public void PowerUpUp(BaseEventData baseEventData)
        {
            for (int i = 0; i < listInfoUI.Count; i++)
            {
                listInfoUI[i].imageBG.sprite = arrSprite_InfoBG[0];
                listInfoUI[i].textPlus.text = String.Empty;
            }
        }
    }
}
