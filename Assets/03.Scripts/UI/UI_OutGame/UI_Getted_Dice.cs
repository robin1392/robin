#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using DG.Tweening;
using RandomWarsResource.Data;
using Service.Core;
using UnityEngine;
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
        public Sprite[] arrSprite_DiceGuage;
        public Slider slider_DiceGuage;
        public GameObject obj_UpgradeIcon;
        public Text text_DiceCount;
        public Text text_DiceLevel;

        //private Data_Dice _data;
        // private RandomWarsResource.Data.TDataDiceInfo _data;
        // private RandomWarsResource.Data.TDataGuardianInfo _dataGuardian;
        private InfoData _data;
        
        private UI_Panel_Dice _panelDice;
        private Transform _grandParent;

        [Space]
        public Image image_GradeBG;
        public Image image_SelectGradeBG;
        public Sprite[] arrSprite_GradeBG;
        public Sprite[] arrSprite_SelectGradeBG;
        public Material mtl_Grayscale;

        [Header("Upgrade")] public GameObject obj_Upgradeable;
        [Header("Equiped")] public GameObject obj_Equiped;

        [Header("Ungetted")] 
        public Sprite sprite_UngettedBG;
        public Text text_UngettedGrade;
        
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

        public void Initialize(InfoData pData, int level, int count)
        {
            _data = pData;

            int needDiceCount = int.MaxValue;
            if (_data.isGuardian == false)
            {
                TDataDiceUpgrade dataDiceUpgrade;
                if (TableManager.Get().DiceUpgrade.GetData(x => x.diceLv == level + 1 && x.diceGrade == (int)_data.grade, out dataDiceUpgrade))
                {
                    needDiceCount = dataDiceUpgrade.needCard;
                }
            }
            
            if (needDiceCount == 0) needDiceCount = 1;
            
            image_Icon.sprite = FileHelper.GetIcon( _data.iconName );
            image_Eye.color = FileHelper.GetColor(_data.color);

            bool isUpgradePossible = count >= needDiceCount;
            text_DiceLevel.text = $"{Global.g_class} {level}";
            text_DiceCount.text = $"{count}/{needDiceCount}";
            //image_DiceGuage.fillAmount = count / (float)needDiceCount;
            slider_DiceGuage.gameObject.SetActive(!_data.isGuardian);
            slider_DiceGuage.value = count / (float)needDiceCount;
            //image_DiceGuage.color = arrColor[count >= needDiceCount ? 1 : 0];
            image_DiceGuage.sprite = arrSprite_DiceGuage[isUpgradePossible ? 1 : 0];
            //count >= needDiceCount ? arrColor[1] : arrColor[0];//UnityUtil.HexToColor("6AD3E5");

            bool isUpgradeEnable = count >= needDiceCount;
            obj_UpgradeIcon.SetActive(isUpgradeEnable);
            obj_Upgradeable.SetActive(isUpgradeEnable);
            button_LevelUp.gameObject.SetActive(isUpgradeEnable);
            //obj_UpgradeLight.SetActive(obj_UpgradeIcon.activeSelf);
            
            button_Use.onClick.AddListener(() => { _panelDice.Click_Dice_Use(_data.id); });
            button_Info.onClick.AddListener(() => { _panelDice.Click_Dice_Info(_data.id); });
            button_LevelUp.onClick.AddListener(() => { _panelDice.Click_Dice_Info(_data.id); });

            if (_data.isGuardian)
            {
                image_GradeBG.sprite = arrSprite_GradeBG[0];
                image_SelectGradeBG.sprite = arrSprite_SelectGradeBG[0];
            }
            else
            {
                image_GradeBG.sprite = arrSprite_GradeBG[(int)_data.grade];
                //image_SelectGradeBG.sprite = arrSprite_SelectGradeBG[isUpgradeEnable ? 4 : (int)_data.grade];
                image_SelectGradeBG.sprite = arrSprite_SelectGradeBG[(int)_data.grade];
            }
            
            // 장착중인지
            SetEquipedMark();
        }

        /// <summary>
        /// 현재 덱에 장착중이면 장착중마크 활성화
        /// </summary>
        public void SetEquipedMark()
        {
            int currentDeckIndex = UserInfoManager.Get().GetUserInfo().activateDeckIndex;
            var deck = UserInfoManager.Get().GetUserInfo().arrDeck;
            obj_Equiped.SetActive(false);
            for (int i = 0; i < deck[currentDeckIndex].Length; i++)
            {
                if (deck[currentDeckIndex][i] == _data.id)
                {
                    obj_Equiped.SetActive(true);
                    return;
                }
            }
        }
        
        // public void Initialize(RandomWarsResource.Data.TDataGuardianInfo pData, int level, int count)
        // {
        //     _dataGuardian = pData;
        //     
        //     image_Icon.sprite = FileHelper.GetDiceIcon( pData.iconName );
        //     image_Eye.color = FileHelper.GetColor(pData.color);
        //
        //     text_DiceLevel.text = $"{Global.g_class} {level}";
        //     slider_DiceGuage.gameObject.SetActive(false);
        //     button_Use.onClick.AddListener(() => { _panelDice.Click_Dice_Use(pData.id); });
        //     button_Info.onClick.AddListener(() => { _panelDice.Click_Dice_Info(pData.id); });
        //     button_LevelUp.onClick.AddListener(() => { _panelDice.Click_Dice_Info(pData.id); });
        //
        //     for (int i = 0; i < arrImage_GradeBG.Length; ++i)
        //     {
        //         arrImage_GradeBG[i].sprite = arrSprite_GradeBG[0];
        //     }
        // }

        public void Click_Dice()
        {
            _grandParent.BroadcastMessage("DeactivateSelectedObject", SendMessageOptions.DontRequireReceiver);

            if (isUngetted)
            {
                _panelDice.Click_Dice_Info(_data.id);
            }
            else
            {
                if (_data.isGuardian)
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

                    TDataDiceUpgrade dataDiceUpgrade;
                    if (TableManager.Get().DiceUpgrade
                            .GetData(x => x.diceLv == diceLevel + 1 && x.diceGrade == (int)_data.grade,
                                out dataDiceUpgrade) ==
                        false)
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
            }
            
            // var parent = transform.parent.parent;
            // ((RectTransform)parent).DOAnchorPosY(
            //     Mathf.Clamp(Mathf.Abs((((RectTransform)transform.parent).anchoredPosition.y + 980) + ((RectTransform)transform).anchoredPosition.y - 200), 0,
            //         ((RectTransform)parent).sizeDelta.y - ((RectTransform)parent.parent).rect.height), 0.3f);
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

            int gradeindex = (int) LANG_ENUM.UI_GRADE_NORMAL;
            switch (_data.grade)
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

            text_UngettedGrade.gameObject.SetActive(true);
            text_UngettedGrade.text = LocalizationManager.GetLangDesc(gradeindex);
            
            var images = GetComponentsInChildren<Image>();
            foreach(var item in images)
            {
                item.material = mtl_Grayscale;
            }

            image_GradeBG.sprite = sprite_UngettedBG;
            image_GradeBG.material = null;

            if (_data.isGuardian == false)
            {
                text_DiceLevel.transform.parent.gameObject.SetActive(false);
                image_DiceGuageBG.gameObject.SetActive(false);
                
                if (_data != null)
                    text_DiceCount.text = $"{Global.g_grade[Mathf.Clamp((int)_data.grade, 0, Global.g_grade.Length)]}";
            }
        }
    }
}