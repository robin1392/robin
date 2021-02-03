#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using DG.Tweening;
using ED;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = ED.Debug;

public class UI_BoxOpenPopup : UI_Popup
{
    public enum COST_TYPE
    {
        KEY,
        GOLD,
        DIAMOND
    }

    [Header("Link")]
    public Text text_BoxName;
    public Button btn_Open;
    public Image image_CostIcon;
    public Text text_Cost;

    [Header("Box")]
    public Animator ani_Box;
    public RuntimeAnimatorController[] arrAniController_Box;
    public GameObject obj_BoxOpenParticle;

    [Header("Items")]
    public Image image_Blind;
    public Button btn_Blind;
    public RawImage image_Pattern;
    public Animator ani_Item;
    public Image image_ItemIcon;
    public Text text_ItemName;
    public Text text_ItemCount;
    public Text text_ItemGuageCount;
    public GameObject obj_Guage;
    public Image image_ItemGuage;
    public Image image_UpgradeIcon;
    public ParticleSystem[] arrPs_ItemNormal;

    [Header("Gold")]
    public GameObject obj_Gold;
    public Text text_Gold;

    [Header("Dice")]
    public GameObject obj_Dice;
    public Text text_Dice;

    [Header("Diamond")]
    public GameObject obj_Diamond;
    public Text text_Diamond;

    [Header("Magic Dice")]
    public GameObject obj_MagicDice;
    public Text text_MagicDice;

    [Header("EpicDice")]
    public GameObject obj_EpicDice;
    public Text text_EpicDice;

    [Header("LegendDice")]
    public GameObject obj_LegendDice;
    public Text text_LegendDice;

    [Header("Sprite")]
    public Sprite[] arrSprite_CostType;
    public Sprite sprite_Gold;
    public Sprite sprite_Diamond;
    public Sprite[] arrSprite_UnknownDiceIcon;

    [Header("Result")]
    public GameObject obj_Result;
    public Image image_ResultTitle;
    public Text text_ResultGold;
    public Text text_ResultDiamond;
    public RectTransform rts_ResultDiceParent;
    public GameObject pref_ResultDice;

    private int boxID;
    private COST_TYPE costType;
    private int cost;
    private int openCount;
    private MsgOpenBoxAck msg;
    private AudioSource _currentAudio;
    
    public void Initialize(int id, COST_TYPE costType, int cost)
    {
        Debug.Log($"BOX ID:{id}");
        this.boxID = id;
        this.costType = costType;
        this.cost = cost;

        RandomWarsResource.Data.TDataItemList dataBoxList;
        if (TableManager.Get().ItemList.GetData(id, out dataBoxList) == false)
        {
            return;
        }


        int totalGold = 0;
        int totalDiamond = 0;
        int totalDiceCount = 0;
        foreach (var productId in dataBoxList.productId)
        {
            RandomWarsResource.Data.TDataBoxOpenInfo dataBoxProductInfo;
            if (TableManager.Get().BoxProductInfo.GetData(productId, out dataBoxProductInfo) == false)
            {
                return;
            }


            // TODO : [개선] 우선 최소값만 합산함.
            totalGold += dataBoxProductInfo.itemValue01[0];
            totalDiamond += dataBoxProductInfo.itemValue02[0] == -1 ? 0 : dataBoxProductInfo.itemValue02[0];
            if (dataBoxProductInfo.rewardCardGradeType1 != 0)
            {
                totalDiceCount += dataBoxProductInfo.rewardCardValue1[0];
            }
            if (dataBoxProductInfo.rewardCardGradeType2 != 0)
            {
                totalDiceCount += dataBoxProductInfo.rewardCardValue2[0];
            }
            if (dataBoxProductInfo.rewardCardGradeType3 != 0)
            {
                totalDiceCount += dataBoxProductInfo.rewardCardValue3[0];
            }
            if (dataBoxProductInfo.rewardCardGradeType4 != 0)
            {
                totalDiceCount += dataBoxProductInfo.rewardCardValue4[0];
            }
            if (dataBoxProductInfo.rewardCardGradeType5 != 0)
            {
                totalDiceCount += dataBoxProductInfo.rewardCardValue5[0];
            }
        }


        text_BoxName.text = LocalizationManager.GetLangDesc(dataBoxList.itemName_langId);

        // Gold
        obj_Gold.SetActive(totalGold > 0);
        if (obj_Gold.activeSelf)
        {
            text_Gold.text = totalGold.ToString();
        }


        // Total Dice
        obj_Dice.SetActive(totalDiceCount > 0);
        if (totalDiceCount > 0) text_Dice.text = $"x{totalDiceCount}";

        // Diamond
        obj_Diamond.SetActive(totalDiamond > 0);
        if (obj_Diamond.activeSelf)
        {
            text_Diamond.text = totalDiamond.ToString();
        }


        image_CostIcon.sprite = arrSprite_CostType[(int)costType];
        text_Cost.text = cost.ToString();
        obj_BoxOpenParticle.SetActive(false);
        openCount = 0;
        
        RectTransform rts = ((RectTransform) ani_Box.transform); 
        rts.DOAnchorPosY(400f, 0);
        rts.DOScale(1f, 0);

        image_Blind.gameObject.SetActive(false);
        image_Blind.raycastTarget = false;
        image_Blind.DOFade(0, 0);
        image_Pattern.DOFade(0, 0);
        ani_Item.gameObject.SetActive(false);
        switch ((RandomWarsResource.Data.EItemListKey)id)
        {
            case RandomWarsResource.Data.EItemListKey.boss01box:
            case RandomWarsResource.Data.EItemListKey.boss02box:
            case RandomWarsResource.Data.EItemListKey.boss03box:
            case RandomWarsResource.Data.EItemListKey.boss04box:
            case RandomWarsResource.Data.EItemListKey.boss05box:
                ani_Box.runtimeAnimatorController = arrAniController_Box[1];
                SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_COOP_APPEAR);
                break;
            default:
                ani_Box.runtimeAnimatorController = arrAniController_Box[0];
                SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_NORMAL_APPEAR);
                break;
        }
        //ani_Box.runtimeAnimatorController = arrAniController_Box[id - (int)RandomWarsResource.Data.EItemListKey.boss01box];
        ani_Box.gameObject.SetActive(true);
        obj_Result.SetActive(false);
    }


    public void Click_Open()
    {
        NetworkManager.Get().OpenBoxReq(UserInfoManager.Get().GetUserInfo().userID, boxID, Callback_BoxOpen);
        //SetShowItems();
        
        UI_Main.Get().obj_IndicatorPopup.SetActive(true);
    }

    public void Callback_BoxOpen(MsgOpenBoxAck msg)
    {
        this.msg = msg;
        UI_Main.Get().obj_IndicatorPopup.SetActive(false);
        SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_COMMON_FALLDOWN);
        
        // 재화 감소
        switch (costType)
        {
            case COST_TYPE.KEY:
                UserInfoManager.Get().GetUserInfo().key -= cost;
                break;
            case COST_TYPE.GOLD:
                UserInfoManager.Get().GetUserInfo().gold -= cost;
                break;
            case COST_TYPE.DIAMOND:
                UserInfoManager.Get().GetUserInfo().diamond -= cost;
                break;
        }

        UserInfoManager.Get().GetUserInfo().dicBox[boxID]--;
        if (UserInfoManager.Get().GetUserInfo().dicBox[boxID] == 0)
        {
            UserInfoManager.Get().GetUserInfo().dicBox.Remove(boxID);
        }
        
        /////////////////////////////////////////////////////////////////
        ///
        ///
        ///
        ///
        /// 

        UI_Main.Get().gerResult.Initialize(msg.BoxReward, true, true);

        UI_Popup_Quest.QuestUpdate(msg.QuestData);
        
        return;
        
        // for (int i = 0; i < msg.BoxReward.Length; i++)
        // {
        //     Debug.Log($"Reward   ID:{msg.BoxReward[i].ItemId} , Count:{msg.BoxReward[i].Value}");
        //
        //     var level = 0;
        //     var count = 0;
        //     if (UserInfoManager.Get().GetUserInfo().dicGettedDice.ContainsKey(msg.BoxReward[i].ItemId))
        //     {
        //         level = UserInfoManager.Get().GetUserInfo().dicGettedDice[msg.BoxReward[i].ItemId][0];
        //         count = UserInfoManager.Get().GetUserInfo().dicGettedDice[msg.BoxReward[i].ItemId][1];
        //     }
        //
        //     RandomWarsResource.Data.TDataItemList tDataItemList;
        //     if (TableManager.Get().ItemList.GetData(msg.BoxReward[i].ItemId, out tDataItemList) == false)
        //     {
        //         Debug.LogErrorFormat($"Failed to get table data. ID:{msg.BoxReward[i].ItemId}");
        //         return;
        //     }
        //
        //
        //     switch ((ITEM_TYPE)tDataItemList.itemType)
        //     {
        //         case ITEM_TYPE.TROPHY:
        //             UserInfoManager.Get().GetUserInfo().trophy += msg.BoxReward[i].Value;
        //             break;
        //         case ITEM_TYPE.GOLD:
        //             UserInfoManager.Get().GetUserInfo().gold += msg.BoxReward[i].Value;
        //             break;
        //         case ITEM_TYPE.DIAMOND:
        //             UserInfoManager.Get().GetUserInfo().diamond += msg.BoxReward[i].Value;
        //             break;
        //         case ITEM_TYPE.KEY:
        //             UserInfoManager.Get().GetUserInfo().key += msg.BoxReward[i].Value;
        //             break;
        //         case ITEM_TYPE.BOX:
        //             if (UserInfoManager.Get().GetUserInfo().dicBox.ContainsKey(msg.BoxReward[i].ItemId))
        //             {
        //                 UserInfoManager.Get().GetUserInfo().dicBox[msg.BoxReward[i].ItemId] += msg.BoxReward[i].Value;
        //             }
        //             else
        //             {
        //                 UserInfoManager.Get().GetUserInfo().dicBox.Add(msg.BoxReward[i].ItemId, msg.BoxReward[i].Value);
        //             }
        //             break;
        //         case ITEM_TYPE.DICE:
        //         {
        //             if (level == 0)
        //             {
        //                 RandomWarsResource.Data.TDataDiceInfo tDataDiceInfo;
        //                 if (TableManager.Get().DiceInfo.GetData(tDataItemList.id, out tDataDiceInfo) == false)
        //                 {
        //                     Debug.LogErrorFormat($"Failed to get table data from DiceInfo. ID:{tDataItemList.id}");
        //                     return;
        //                 }
        //
        //                 RandomWarsResource.Data.TDataDiceLevelInfo dataDiceLevelInfo;
        //                 if (TableManager.Get().DiceLevelInfo.GetData(tDataDiceInfo.grade, out dataDiceLevelInfo) == false)
        //                 {
        //                     return;
        //                 }
        //
        //                 count = msg.BoxReward[i].Value;
        //                 UserInfoManager.Get().GetUserInfo().dicGettedDice.Add(msg.BoxReward[i].ItemId, new int[] { dataDiceLevelInfo.baseLevel, count });
        //             }
        //             else
        //             {
        //                 count += msg.BoxReward[i].Value;
        //                 UserInfoManager.Get().GetUserInfo().dicGettedDice[msg.BoxReward[i].ItemId][1] = count;
        //             }
        //         }
        //         break;
        //     }
        // }
        //
        // UI_Main.Get().boxPopup.RefreshBox();
        // UI_Main.Get().RefreshUserInfoUI();
        // UI_Main.Get().panel_Dice.RefreshGettedDice();
        //
        // SetShowItems();
    }

    // private void SetShowItems()
    // {
    //     RectTransform rts = ((RectTransform) ani_Box.transform); 
    //     rts.DOAnchorPosY(-500f, 0.5f).SetEase(Ease.OutQuint);
    //     rts.DOScale(1.4f, 0.5f);
    //
    //     image_Blind.gameObject.SetActive(true);
    //     image_Blind.raycastTarget = true;
    //     image_Blind.DOFade(1f, 0.5f);
    //     image_Pattern.DOFade(0.007843f, 0.5f);
    // }
    //
    // public void Click_NextButton()
    // {
    //     if (_currentAudio != null)
    //     {
    //         SoundManager.instance.Stop(_currentAudio);
    //         _currentAudio = null;
    //     }
    //     
    //     if (msg != null && openCount < msg.BoxReward.Length)
    //     {
    //         ani_Box.SetTrigger("Open");
    //
    //         btn_Blind.interactable = false;
    //         if (openCount == 0)
    //         {
    //             obj_BoxOpenParticle.SetActive(true);
    //
    //             Invoke("ItemAnimation", 0.6666f);
    //             SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_COMMON_OPEN);
    //         }
    //         else
    //         {
    //             ItemAnimation();
    //             SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_COMMON_OPEN_REPEAT);
    //         }
    //     }
    //     else if (openCount == msg.BoxReward.Length)
    //     {
    //         btn_Blind.interactable = false;
    //         ani_Item.gameObject.SetActive(false);
    //         StartCoroutine(ShowResultCoroutine());
    //     }
    //     else
    //     {
    //         gameObject.SetActive(false);
    //         image_Blind.gameObject.SetActive(false);
    //     }
    // }
    //
    // private void ItemAnimation()
    // {
    //     if (crt_IconChange != null) StopCoroutine(crt_IconChange);
    //     if (crt_TextCount != null) StopCoroutine(crt_TextCount);
    //     ani_Item.gameObject.SetActive(true);
    //
    //     MsgReward reward = msg.BoxReward[openCount];
    //
    //     RandomWarsResource.Data.TDataItemList tDataItemList;
    //     if (TableManager.Get().ItemList.GetData(reward.ItemId, out tDataItemList) == false)
    //     {
    //         Debug.LogErrorFormat($"Failed to get table data from ItemList. ID:{reward.ItemId}");
    //         return;
    //     }
    //
    //
    //     // 보상내용 세팅
    //     switch ((ITEM_TYPE)tDataItemList.itemType)
    //     {
    //         case ITEM_TYPE.GOLD:
    //             image_ItemIcon.sprite = sprite_Gold;
    //             image_ItemIcon.SetNativeSize();
    //             crt_IconChange = StartCoroutine(IconChangeCoroutine(sprite_Gold, 0.6f));
    //             ani_Item.SetTrigger("Get");
    //             text_ItemName.text = LocalizationManager.GetLangDesc(tDataItemList.itemName_langId);
    //             text_ItemCount.text = $"x{reward.Value}";
    //             obj_Guage.SetActive(false);
    //             SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_COMMON_GET_GOLD, 0.5f);
    //             break;
    //         case ITEM_TYPE.DIAMOND:
    //             image_ItemIcon.sprite = sprite_Diamond;
    //             image_ItemIcon.SetNativeSize();
    //             crt_IconChange = StartCoroutine(IconChangeCoroutine(sprite_Diamond, 0.6f));
    //             ani_Item.SetTrigger("Get");
    //             text_ItemName.text = LocalizationManager.GetLangDesc(tDataItemList.itemName_langId);
    //             text_ItemCount.text = $"x{reward.Value}";
    //             obj_Guage.SetActive(false);
    //             SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_COMMON_GET_DIAMOND, 0.5f);
    //             break;
    //         case ITEM_TYPE.DICE:
    //         {
    //             obj_Guage.SetActive(true);
    //             for (int i = 0; i < arrPs_ItemNormal.Length; i++)
    //             {
    //                 var module = arrPs_ItemNormal[i].main;
    //                 module.startColor = UnityUtil.HexToColor(Global.g_gradeColor[0]);
    //             }
    //
    //             image_ItemIcon.sprite = arrSprite_UnknownDiceIcon[0];
    //
    //             RandomWarsResource.Data.TDataDiceInfo dataDiceInfo;
    //             if (TableManager.Get().DiceInfo.GetData(reward.ItemId, out dataDiceInfo) == false)
    //             {
    //                 return;
    //             }
    //             
    //             crt_IconChange = StartCoroutine(IconChangeCoroutine(
    //                 FileHelper.GetDiceIcon(dataDiceInfo.iconName), 0.6f));
    //
    //             if ((DICE_GRADE) dataDiceInfo.grade == DICE_GRADE.LEGEND)
    //             {
    //                 _currentAudio = SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_COMMON_GET_DICE_LEGEND);
    //                 ani_Item.SetTrigger("GetLegend");
    //             }
    //             else
    //             {
    //                 SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_COMMON_GET_DICE, 0.5f);
    //                 ani_Item.SetTrigger("Get");
    //             }
    //
    //             image_ItemIcon.SetNativeSize();
    //             text_ItemName.text = LocalizationManager.GetLangDesc(tDataItemList.itemName_langId);
    //             text_ItemCount.text = $"x{reward.Value}";
    //             int level = 0;
    //             if (UserInfoManager.Get().GetUserInfo().dicGettedDice.ContainsKey(reward.ItemId))
    //             {
    //                 level = UserInfoManager.Get().GetUserInfo().dicGettedDice[reward.ItemId][0];
    //             }
    //
    //             RandomWarsResource.Data.TDataDiceUpgrade dataDiceUpgrade;
    //             if (TableManager.Get().DiceUpgrade.GetData(x => x.diceLv == level + 1 && x.diceGrade == dataDiceInfo.grade, out dataDiceUpgrade) == false)
    //             {
    //                 return;
    //             }
    //
    //             int needDiceCount = dataDiceUpgrade.needCard;
    //             crt_TextCount = StartCoroutine(TextCountCoroutine(
    //                 UserInfoManager.Get().GetUserInfo().dicGettedDice[reward.ItemId][1],
    //                 reward.Value, needDiceCount, 1.2f));
    //             
    //         }
    //             break;
    //     }
    //     
    //     // 애니메이션
    //     RectTransform rts = (RectTransform) ani_Item.transform;
    //     rts.DOKill();
    //     rts.anchoredPosition = new Vector2(0, -250);
    //     rts.DOAnchorPosY(350f, 0.5f);
    //     rts.localScale = Vector3.zero;
    //     rts.DOScale(1.4f, 0.5f);
    //
    //     SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_COMMON_ITEM_APPEAR);
    //
    //     openCount++;
    // }
    //
    // private Coroutine crt_IconChange;
    // IEnumerator IconChangeCoroutine(Sprite icon, float delay)
    // {
    //     yield return new WaitForSeconds(delay);
    //
    //     btn_Blind.interactable = true;
    //     image_ItemIcon.sprite = icon;
    //     image_ItemIcon.SetNativeSize();
    // }
    //
    // private Coroutine crt_TextCount;
    // IEnumerator TextCountCoroutine(int current, int add, int max, float delay)
    // {
    //     int before = current - add;
    //     text_ItemGuageCount.text = $"{before}/{max}";
    //
    //     image_ItemGuage.fillAmount = before / (float)max;
    //     image_ItemGuage.color = before >= max ? Color.green : UnityUtil.HexToColor("6AD3E5");
    //     image_UpgradeIcon.gameObject.SetActive(before >= max);
    //     
    //     yield return new WaitForSeconds(delay);
    //
    //     float t = 0;
    //     float timeMax = Mathf.Clamp(0.1f * add, 0, 0.5f);
    //     while (t < timeMax)
    //     {
    //         float v = Mathf.Lerp(before, current, t / timeMax);
    //         text_ItemGuageCount.text = $"{(int)v}/{max}";
    //         image_ItemGuage.fillAmount = v / (float)max;
    //         
    //         image_ItemGuage.color = (int)v >= max ? Color.green : UnityUtil.HexToColor("6AD3E5");
    //         if (image_UpgradeIcon.gameObject.activeSelf == false && (int) v >= max)
    //         {
    //             image_UpgradeIcon.gameObject.SetActive(true);
    //             image_UpgradeIcon.rectTransform.localScale = Vector3.zero;
    //             image_UpgradeIcon.rectTransform.DOScale(1f, 0.25f);
    //         }
    //
    //         t += Time.deltaTime;
    //         yield return null;
    //         
    //     }
    //     
    //     btn_Blind.interactable = true;
    //     
    //     text_ItemGuageCount.text = $"{current}/{max}";
    //     image_ItemGuage.fillAmount = current / (float)max;
    //     
    //     image_ItemGuage.color = current >= max ? Color.green : UnityUtil.HexToColor("6AD3E5");
    //     if (image_UpgradeIcon.gameObject.activeSelf == false && current >= max)
    //     {
    //         image_UpgradeIcon.gameObject.SetActive(true);
    //         image_UpgradeIcon.rectTransform.localScale = Vector3.zero;
    //         image_UpgradeIcon.rectTransform.DOScale(1f, 0.25f);
    //     }
    // }
    //
    // IEnumerator ShowResultCoroutine()
    // {
    //     SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_COMMON_RESULT);
    //     obj_Result.SetActive(true);
    //     ani_Box.gameObject.SetActive(false);
    //     
    //     List<MsgReward> list = new List<MsgReward>(msg.BoxReward);
    //     
    //     var gold = list.Find(m => m.ItemId == (int)RandomWarsResource.Data.EItemListKey.gold);
    //     text_ResultGold.text = $"{gold?.Value ?? 0}";
    //     
    //     var diamond = list.Find(m => m.ItemId == (int)RandomWarsResource.Data.EItemListKey.dia);
    //     text_ResultDiamond.text = $"{diamond?.Value ?? 0}";
    //
    //     int childCount = rts_ResultDiceParent.childCount;
    //     for (int i = childCount - 1; i >= 0; i--)
    //     {
    //         DestroyImmediate(rts_ResultDiceParent.GetChild(i).gameObject);
    //     }
    //
    //     int loopCount = 0;
    //     foreach (var msgReward in list)
    //     {
    //         RandomWarsResource.Data.TDataItemList tDataItemList;
    //         if (TableManager.Get().ItemList.GetData(msgReward.ItemId, out tDataItemList) == true)
    //         {
    //             if (tDataItemList.itemType == (int)ITEM_TYPE.DICE)
    //             {
    //                 RandomWarsResource.Data.TDataDiceInfo dataDiceInfo;
    //                 if (TableManager.Get().DiceInfo.GetData(msgReward.ItemId, out dataDiceInfo) == false)
    //                 {
    //                     break;
    //                 }
    //
    //                 var dice = Instantiate(pref_ResultDice, rts_ResultDiceParent);
    //                 dice.GetComponent<Image>().sprite = FileHelper.GetDiceIcon(dataDiceInfo.iconName);
    //                 dice.transform.GetChild(0).GetComponent<Text>().text = LocalizationManager.GetLangDesc((int)LANG_ENUM.DICE_NAME + dataDiceInfo.id);
    //                 dice.transform.GetChild(1).GetComponent<Text>().text = $"x{msgReward.Value}";
    //                 dice.transform.localScale = Vector3.zero;
    //                 dice.transform.DOScale(Vector3.one, 0.2f).SetDelay(0.05f * loopCount++).SetEase(Ease.OutBack)
    //                     .OnComplete(() =>
    //                     {
    //                         SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_COMMON_RESULT_ITEM);
    //                     });
    //                 
    //             }
    //         }
    //
    //
    //         //switch (tDataDiceInfo.grade)
    //         //{
    //         //    case REWARD_TYPE.DICE_NORMAL:
    //         //    case REWARD_TYPE.DICE_MAGIC:
    //         //    case REWARD_TYPE.DICE_EPIC:
    //         //    case REWARD_TYPE.DICE_LEGEND:
    //         //    {
    //         //        RandomWarsResource.Data.TDataDiceInfo dataDiceInfo;
    //         //        if (TableManager.Get().DiceInfo.GetData(msgReward.Id, out dataDiceInfo) == false)
    //         //        {
    //         //                break;
    //         //        }
    //
    //         //        var dice = Instantiate(pref_ResultDice, rts_ResultDiceParent);
    //         //        dice.GetComponent<Image>().sprite = FileHelper.GetIcon(dataDiceInfo.iconName);
    //         //        dice.transform.GetChild(0).GetComponent<Text>().text = LocalizationManager.GetLangDesc((int)LANG_ENUM.DICE_NAME + dataDiceInfo.id);
    //         //        dice.transform.GetChild(1).GetComponent<Text>().text = $"x{msgReward.Value}";
    //         //        dice.transform.localScale = Vector3.zero;
    //         //        dice.transform.DOScale(Vector3.one, 0.2f).SetDelay(0.05f * loopCount++).SetEase(Ease.OutBack);
    //         //    }
    //         //        break;
    //         //}
    //     }
    //
    //     openCount++;
    //     yield return new WaitForSeconds(1.5f);
    //     btn_Blind.interactable = true;
    // }
    
}
