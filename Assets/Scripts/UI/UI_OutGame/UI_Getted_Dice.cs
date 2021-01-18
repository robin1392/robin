#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ParadoxNotion.Design;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ED
{
    public class UI_Getted_Dice : MonoBehaviour
    {
        public Image image_Icon;
        public Image image_Eye;
        public Button button_LevelUp;
        public Text text_LevelUpCost;
        public Button button_Use;
        public Button button_Info;
        public GameObject obj_Selected;
        public Transform ts_Move;
        public int slotNum;

        [Header("Dice Info")]
        public Color[] arrColor;
        public Image image_DiceGuageBG;
        public Image image_DiceGuage;
        public Slider slider_DiceGuage;
        public GameObject obj_UpgradeIcon;
        public Text text_DiceCount;
        public Text text_DiceLevel;

        //private Data_Dice _data;
        private RandomWarsResource.Data.TDataDiceInfo _data;
        private RandomWarsResource.Data.TDataGuardianInfo _dataGuardian;
        
        private UI_Panel_Dice _panelDice;
        private Transform _grandParent;

        [Space]
        public Image[] arrImage_GradeBG;
        public Sprite[] arrSprite_GradeBG;
        public Material mtl_Grayscale;
        
        private bool isUngetted;
        private VerticalLayoutGroup verticalLayoutGroup;
        private ContentSizeFitter contentSizeFitter;

        private void Awake()
        {
            _grandParent = transform.parent.parent;
            _panelDice = FindObjectOfType<UI_Panel_Dice>();
            verticalLayoutGroup = GetComponentInParent<VerticalLayoutGroup>();
            contentSizeFitter = _grandParent.GetComponent<ContentSizeFitter>();
        }

        public void Initialize(RandomWarsResource.Data.TDataDiceInfo pData, int level, int count)
        {
            _data = pData;

            RandomWarsResource.Data.TDataDiceUpgrade dataDiceUpgrade;
            if (TableManager.Get().DiceUpgrade.GetData(x => x.diceLv == level + 1 && x.diceGrade == pData.grade, out dataDiceUpgrade) == false)
            {
                return;
            }

            int needDiceCount = dataDiceUpgrade.needCard;
            if (needDiceCount == 0) needDiceCount = 1;
            
            image_Icon.sprite = FileHelper.GetDiceIcon( pData.iconName );
            image_Eye.color = FileHelper.GetColor(pData.color);

            text_DiceLevel.text = $"{Global.g_class} {level}";
            text_DiceCount.text = $"{count}/{needDiceCount}";
            //image_DiceGuage.fillAmount = count / (float)needDiceCount;
            slider_DiceGuage.value = count / (float)needDiceCount;
            image_DiceGuage.color = arrColor[count >= needDiceCount ? 1 : 0];
            //count >= needDiceCount ? arrColor[1] : arrColor[0];//UnityUtil.HexToColor("6AD3E5");

            bool isUpgradeEnable = count >= needDiceCount;
            obj_UpgradeIcon.SetActive(isUpgradeEnable);
            button_LevelUp.gameObject.SetActive(isUpgradeEnable);
            //obj_UpgradeLight.SetActive(obj_UpgradeIcon.activeSelf);
            
            button_Use.onClick.AddListener(() => { _panelDice.Click_Dice_Use(pData.id); });
            button_Info.onClick.AddListener(() => { _panelDice.Click_Dice_Info(pData.id); });
            button_LevelUp.onClick.AddListener(() => { _panelDice.Click_Dice_Info(pData.id); });

            
            arrImage_GradeBG[0].sprite = arrSprite_GradeBG[pData.grade];
            arrImage_GradeBG[1].sprite = arrSprite_GradeBG[isUpgradeEnable ? 4 : pData.grade];
        }
        
        public void Initialize(RandomWarsResource.Data.TDataGuardianInfo pData, int level, int count)
        {
            _dataGuardian = pData;
            
            image_Icon.sprite = FileHelper.GetDiceIcon( pData.iconName );
            image_Eye.color = FileHelper.GetColor(pData.color);

            text_DiceLevel.text = $"{Global.g_class} {level}";
            slider_DiceGuage.gameObject.SetActive(false);
            button_Use.onClick.AddListener(() => { _panelDice.Click_Dice_Use(pData.id); });
            button_Info.onClick.AddListener(() => { _panelDice.Click_Dice_Info(pData.id); });
            button_LevelUp.onClick.AddListener(() => { _panelDice.Click_Dice_Info(pData.id); });

            for (int i = 0; i < arrImage_GradeBG.Length; ++i)
            {
                arrImage_GradeBG[i].sprite = arrSprite_GradeBG[0];
            }
        }

        public void Click_Dice()
        {
            _grandParent.BroadcastMessage("DeactivateSelectedObject", SendMessageOptions.DontRequireReceiver);

            if (isUngetted)
            {
                _panelDice.Click_Dice_Info(_data.id);
            }
            else
            {
                obj_Selected.SetActive(true);
                verticalLayoutGroup.enabled = false;
                contentSizeFitter.enabled = false;
                ts_Move.SetParent(_grandParent);

                int diceCount = UserInfoManager.Get().GetUserInfo().dicGettedDice[_data.id][1];
                int diceLevel = UserInfoManager.Get().GetUserInfo().dicGettedDice[_data.id][0];

                RandomWarsResource.Data.TDataDiceUpgrade dataDiceUpgrade;
                if (TableManager.Get().DiceUpgrade.GetData(x => x.diceLv == diceLevel + 1 && x.diceGrade == _data.grade, out dataDiceUpgrade) == false)
                {
                    return;
                }

                int needCount = dataDiceUpgrade.needCard;
                int needGold = dataDiceUpgrade.needGold;
                bool isEnableLevelUp = diceCount >= needCount;

                if (isEnableLevelUp)
                {
                    button_Info.gameObject.SetActive(false);
                    button_LevelUp.gameObject.SetActive(true);

                    text_LevelUpCost.text = needGold.ToString();
                    text_LevelUpCost.color =
                        UserInfoManager.Get().GetUserInfo().gold >= needGold ? Color.white : Color.red;
                }
                else
                {
                    button_Info.gameObject.SetActive(true);
                    button_LevelUp.gameObject.SetActive(false);
                }
            }
            
            var parent = transform.parent.parent;
            ((RectTransform)parent).DOAnchorPosY(
                Mathf.Clamp(Mathf.Abs((((RectTransform)transform.parent).anchoredPosition.y + 980) + ((RectTransform)transform).anchoredPosition.y - 200), 0,
                    ((RectTransform)parent).sizeDelta.y - ((RectTransform)parent.parent).rect.height), 0.3f);
        }

        public void DeactivateSelectedObject()
        {
            obj_Selected.SetActive(false);
            verticalLayoutGroup.enabled = true;
            contentSizeFitter.enabled = true;
            ts_Move.SetParent(transform);
        }

        public void SetGrayscale()
        {
            isUngetted = true;

            var images = GetComponentsInChildren<Image>();
            foreach(var item in images)
            {
                item.material = mtl_Grayscale;
            }
            
            text_DiceLevel.transform.parent.gameObject.SetActive(false);
            image_DiceGuageBG.gameObject.SetActive(false);
            
            if (_data != null)
                text_DiceCount.text = $"{Global.g_grade[Mathf.Clamp(_data.grade, 0, Global.g_grade.Length)]}";
            else
                text_DiceCount.text = $"{Global.g_grade[Mathf.Clamp(_dataGuardian.grade, 0, Global.g_grade.Length)]}";
        }
    }
}