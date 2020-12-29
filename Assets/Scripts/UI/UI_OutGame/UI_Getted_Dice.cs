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
        public Image image_DiceGuageBG;
        public Image image_DiceGuage;
        public GameObject obj_UpgradeIcon;
        public GameObject obj_UpgradeLight;
        public Text text_DiceCount;
        public Text text_DiceLevel;

        //private Data_Dice _data;
        private RandomWarsResource.Data.TDataDiceInfo _data;
        
        private UI_Panel_Dice _panelDice;
        private Transform _grandParent;

        [Space]
        public Image image_GradeBG;
        public Sprite[] arrSprite_GradeBG;
        public Material mtl_Grayscale;

        private bool isUngetted;

        private void Awake()
        {
            _panelDice = FindObjectOfType<UI_Panel_Dice>();
            _grandParent = transform.parent.parent;

            if (image_GradeBG == null)
            {
                image_GradeBG = this.transform.Find("Parent/Image").GetComponent<Image>();
            }
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
            
            image_Icon.sprite = FileHelper.GetIcon( pData.iconName );
            image_Eye.color = FileHelper.GetColor(pData.color);

            text_DiceLevel.text = $"{Global.g_level} {level}";
            text_DiceCount.text = $"{count}/{needDiceCount}";
            image_DiceGuage.fillAmount = count / (float)needDiceCount;
            image_DiceGuage.color =
                count >= needDiceCount ? Color.green : UnityUtil.HexToColor("6AD3E5");
            obj_UpgradeIcon.SetActive(count >= needDiceCount);
            obj_UpgradeLight.SetActive(obj_UpgradeIcon.activeSelf);
            
            button_Use.onClick.AddListener(() => { _panelDice.Click_Dice_Use(pData.id); });
            button_Info.onClick.AddListener(() => { _panelDice.Click_Dice_Info(pData.id); });
            button_LevelUp.onClick.AddListener(() => { _panelDice.Click_Dice_Info(pData.id); });

            image_GradeBG.sprite = arrSprite_GradeBG[pData.grade];
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
            text_DiceCount.text = $"{Global.g_grade[_data.grade]}";
        }
    }
}