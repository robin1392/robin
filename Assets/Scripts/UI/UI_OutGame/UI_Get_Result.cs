using System.Collections;
using System.Collections.Generic;
using RandomWarsProtocol;
using UnityEngine;
using UnityEngine.UI;
using ED;
using DG.Tweening;
using ParadoxNotion;
using Debug = UnityEngine.Debug;

public class UI_Get_Result : MonoBehaviour
{
    public Animator ani_Box;
    public Button btn_Blind;
    public Image image_Blind;
    public RawImage image_Pattern;
    public GameObject obj_BoxOpenParticle;
    
    [Header("Item")]
    public Animator ani_Item;
    public Image image_ItemIcon;
    public Text text_ItemName;
    public Text text_ItemCount;
    public GameObject obj_Guage;
    public Text text_ItemGuageCount;
    public Image image_ItemGuage;
    public Image image_UpgradeIcon;
    public ParticleSystem[] arrPs_ItemNormal;
    public Sprite[] arrSprite_UnknownDiceIcon;

    [Header("Result")]
    public GameObject obj_Result;
    public Text text_ResultGold;
    public Text text_ResultDiamond;
    public RectTransform rts_ResultDiceParent;
    public GameObject pref_ResultDice;

    [Header("Sprite")]
    public Sprite sprite_Gold;
    public Sprite sprite_Diamond;

    private int openCount;
    private MsgReward[] msg;
    private AudioSource _currentAudio;
    
    private bool isBoxOpen;
    private bool isShowEndResult;
    private float iconChangeDelay;
    
    public void Initialize(MsgReward[] msg, bool isBoxOpen, bool isShowEndResult)
    {
        gameObject.SetActive(true);
        this.msg = msg;
        this.isBoxOpen = isBoxOpen;
        this.isShowEndResult = isShowEndResult;
        this.openCount = 0;
        obj_Result.SetActive(false);
        
        for (int i = 0; i < msg.Length; i++)
        {
            Debug.Log($"Reward   ID:{msg[i].ItemId} , Count:{msg[i].Value}");

            var level = 0;
            var count = 0;
            if (UserInfoManager.Get().GetUserInfo().dicGettedDice.ContainsKey(msg[i].ItemId))
            {
                level = UserInfoManager.Get().GetUserInfo().dicGettedDice[msg[i].ItemId][0];
                count = UserInfoManager.Get().GetUserInfo().dicGettedDice[msg[i].ItemId][1];
            }

            RandomWarsResource.Data.TDataItemList tDataItemList;
            if (TableManager.Get().ItemList.GetData(msg[i].ItemId, out tDataItemList) == false)
            {
                Debug.LogErrorFormat($"Failed to get table data. ID:{msg[i].ItemId}");
                return;
            }

            switch ((ITEM_TYPE)tDataItemList.itemType)
            {
                case ITEM_TYPE.TROPHY:
                    UserInfoManager.Get().GetUserInfo().trophy += msg[i].Value;
                    break;
                case ITEM_TYPE.GOLD:
                    UserInfoManager.Get().GetUserInfo().gold += msg[i].Value;
                    break;
                case ITEM_TYPE.DIAMOND:
                    UserInfoManager.Get().GetUserInfo().diamond += msg[i].Value;
                    break;
                case ITEM_TYPE.KEY:
                    UserInfoManager.Get().GetUserInfo().key += msg[i].Value;
                    break;
                case ITEM_TYPE.BOX:
                    if (UserInfoManager.Get().GetUserInfo().dicBox.ContainsKey(msg[i].ItemId))
                    {
                        UserInfoManager.Get().GetUserInfo().dicBox[msg[i].ItemId] += msg[i].Value;
                    }
                    else
                    {
                        UserInfoManager.Get().GetUserInfo().dicBox.Add(msg[i].ItemId, msg[i].Value);
                    }
                    break;
                case ITEM_TYPE.DICE:
                {
                    if (level == 0)
                    {
                        RandomWarsResource.Data.TDataDiceInfo tDataDiceInfo;
                        if (TableManager.Get().DiceInfo.GetData(tDataItemList.id, out tDataDiceInfo) == false)
                        {
                            Debug.LogErrorFormat($"Failed to get table data from DiceInfo. ID:{tDataItemList.id}");
                            return;
                        }

                        RandomWarsResource.Data.TDataDiceLevelInfo dataDiceLevelInfo;
                        if (TableManager.Get().DiceLevelInfo.GetData(tDataDiceInfo.grade, out dataDiceLevelInfo) == false)
                        {
                            return;
                        }

                        count = msg[i].Value;
                        UserInfoManager.Get().GetUserInfo().dicGettedDice.Add(msg[i].ItemId, new int[] { dataDiceLevelInfo.baseLevel, count });
                    }
                    else
                    {
                        count += msg[i].Value;
                        UserInfoManager.Get().GetUserInfo().dicGettedDice[msg[i].ItemId][1] = count;
                    }
                }
                break;
            }
        }
        
        UI_Main.Get().RefreshUserInfoUI();
        UI_Main.Get().panel_Dice.RefreshGettedDice();
        
        SetShowItems();
    }
    
    private void SetShowItems()
    {
        image_Blind.gameObject.SetActive(true);
        image_Blind.raycastTarget = true;
        image_Blind.DOFade(1f, 0.5f);
        image_Pattern.DOFade(0.007843f, 0.5f);
        
        if (isBoxOpen)
        {
            ani_Box.gameObject.SetActive(true);
            RectTransform rts = ((RectTransform) ani_Box.transform);
            rts.DOAnchorPosY(-500f, 0.5f).SetEase(Ease.OutQuint);
            rts.DOScale(1.4f, 0.5f);
        }
        else
        {
            ani_Box.gameObject.SetActive(false);
            Click_NextButton();
        }
    }
    
    public void Click_NextButton()
    {
        if (_currentAudio != null)
        {
            SoundManager.instance.Stop(_currentAudio);
            _currentAudio = null;
        }
        
        if (msg != null && openCount < msg.Length)
        {
            if (openCount == 0 && isBoxOpen)
            {
                //btn_Blind.interactable = false;
                obj_BoxOpenParticle.SetActive(true);

                Invoke("ItemAnimation", 1f);
                SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_COMMON_OPEN);
            }
            else
            {
                //btn_Blind.interactable = false;
                ItemAnimation();
                SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_COMMON_OPEN_REPEAT);
            }
        }
        else if (openCount == msg.Length)
        {
            if (ani_Item.GetCurrentAnimatorStateInfo(0).IsName("UI_getdice")
                || ani_Item.GetCurrentAnimatorStateInfo(0).IsName("UI_getdice_legend"))
            {
                ItemAnimation();
                return;
            }

            ani_Item.gameObject.SetActive(false);

            if (isShowEndResult)
            {
                btn_Blind.interactable = false;
                StartCoroutine(ShowResultCoroutine());
            }
            else
            {
                gameObject.SetActive(false);
                ani_Item.gameObject.SetActive(false);
                image_Blind.gameObject.SetActive(false);
            }
        }
        else
        {
            gameObject.SetActive(false);
            ani_Item.gameObject.SetActive(false);
            image_Blind.gameObject.SetActive(false);
        }
    }
    
    private void ItemAnimation()
    {
        MsgReward reward = msg[Mathf.Clamp(openCount, 0, msg.Length - 1)];

        RandomWarsResource.Data.TDataItemList tDataItemList;
        if (TableManager.Get().ItemList.GetData(reward.ItemId, out tDataItemList) == false)
        {
            Debug.LogErrorFormat($"Failed to get table data from ItemList. ID:{reward.ItemId}");
            return;
        }
        
        if (crt_TextCount != null) StopCoroutine(crt_TextCount);
        
        if (ani_Item.GetCurrentAnimatorStateInfo(0).IsName("UI_getdice")
        || ani_Item.GetCurrentAnimatorStateInfo(0).IsName("UI_getdice_legend"))
        {
            ani_Item.SetTrigger("End");
            iconChangeDelay = 0;

            if ((ITEM_TYPE) tDataItemList.itemType == ITEM_TYPE.DICE)
            {
                // dice
                RandomWarsResource.Data.TDataDiceInfo dataDiceInfo;
                if (TableManager.Get().DiceInfo.GetData(reward.ItemId, out dataDiceInfo) == false)
                {
                    return;
                }
                int level = 0;
                if (UserInfoManager.Get().GetUserInfo().dicGettedDice.ContainsKey(reward.ItemId))
                {
                    level = UserInfoManager.Get().GetUserInfo().dicGettedDice[reward.ItemId][0];
                }
                RandomWarsResource.Data.TDataDiceUpgrade dataDiceUpgrade;
                if (TableManager.Get().DiceUpgrade.GetData(x => x.diceLv == level + 1 && x.diceGrade == dataDiceInfo.grade, out dataDiceUpgrade) == false)
                {
                    return;
                }

                int needDiceCount = dataDiceUpgrade.needCard;
                int current = UserInfoManager.Get().GetUserInfo().dicGettedDice[reward.ItemId][1];
                
                text_ItemGuageCount.text = $"{current}/{needDiceCount}";
                image_ItemGuage.fillAmount = current / (float) needDiceCount;

                image_ItemGuage.color = current >= needDiceCount ? Color.green : UnityUtil.HexToColor("6AD3E5");
                if (image_UpgradeIcon.gameObject.activeSelf == false && current >= needDiceCount)
                {
                    image_UpgradeIcon.gameObject.SetActive(true);
                    image_UpgradeIcon.rectTransform.localScale = Vector3.zero;
                    image_UpgradeIcon.rectTransform.DOScale(1f, 0.25f);
                }
            }
            
            // 애니메이션
            RectTransform _rts = (RectTransform) ani_Item.transform;
            _rts.DOKill();
            _rts.DOAnchorPosY(350f, 0f);
            _rts.DOScale(1.4f, 0f);

            return;
        }
        
        ani_Box.SetTrigger("Open");

        if (crt_IconChange != null) StopCoroutine(crt_IconChange);
        ani_Item.gameObject.SetActive(true);
        
        // 보상내용 세팅
        switch ((ITEM_TYPE)tDataItemList.itemType)
        {
            case ITEM_TYPE.GOLD:
                image_ItemIcon.sprite = sprite_Gold;
                image_ItemIcon.SetNativeSize();
                iconChangeDelay = 0.6f;
                crt_IconChange = StartCoroutine(IconChangeCoroutine(sprite_Gold));
                ani_Item.SetTrigger("Get");
                text_ItemName.text = LocalizationManager.GetLangDesc(tDataItemList.itemName_langId);
                text_ItemCount.text = $"x{reward.Value}";
                obj_Guage.SetActive(false);
                SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_COMMON_GET_GOLD, 0.5f);
                break;
            case ITEM_TYPE.DIAMOND:
                image_ItemIcon.sprite = sprite_Diamond;
                image_ItemIcon.SetNativeSize();
                iconChangeDelay = 0.6f;
                crt_IconChange = StartCoroutine(IconChangeCoroutine(sprite_Diamond));
                ani_Item.SetTrigger("Get");
                text_ItemName.text = LocalizationManager.GetLangDesc(tDataItemList.itemName_langId);
                text_ItemCount.text = $"x{reward.Value}";
                obj_Guage.SetActive(false);
                SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_COMMON_GET_DIAMOND, 0.5f);
                break;
            case ITEM_TYPE.DICE:
            {
                obj_Guage.SetActive(true);
                for (int i = 0; i < arrPs_ItemNormal.Length; i++)
                {
                    var module = arrPs_ItemNormal[i].main;
                    module.startColor = UnityUtil.HexToColor(Global.g_gradeColor[0]);
                }

                image_ItemIcon.sprite = arrSprite_UnknownDiceIcon[0];

                RandomWarsResource.Data.TDataDiceInfo dataDiceInfo;
                if (TableManager.Get().DiceInfo.GetData(reward.ItemId, out dataDiceInfo) == false)
                {
                    return;
                }
                

                if ((DICE_GRADE) dataDiceInfo.grade == DICE_GRADE.LEGEND)
                {
                    _currentAudio = SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_COMMON_GET_DICE_LEGEND);
                    ani_Item.SetTrigger("GetLegend");
                    iconChangeDelay = 1.6f;
                }
                else
                {
                    SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_COMMON_GET_DICE, 0.5f);
                    ani_Item.SetTrigger("Get");
                    iconChangeDelay = 0.6f;
                    
                }
                
                crt_IconChange = StartCoroutine(IconChangeCoroutine(FileHelper.GetIcon(dataDiceInfo.iconName)));

                image_ItemIcon.SetNativeSize();
                text_ItemName.text = LocalizationManager.GetLangDesc(tDataItemList.itemName_langId);
                text_ItemCount.text = $"x{reward.Value}";
                int level = 0;
                if (UserInfoManager.Get().GetUserInfo().dicGettedDice.ContainsKey(reward.ItemId))
                {
                    level = UserInfoManager.Get().GetUserInfo().dicGettedDice[reward.ItemId][0];
                }

                RandomWarsResource.Data.TDataDiceUpgrade dataDiceUpgrade;
                if (TableManager.Get().DiceUpgrade.GetData(x => x.diceLv == level + 1 && x.diceGrade == dataDiceInfo.grade, out dataDiceUpgrade) == false)
                {
                    return;
                }

                int needDiceCount = dataDiceUpgrade.needCard;
                crt_TextCount = StartCoroutine(TextCountCoroutine(
                    UserInfoManager.Get().GetUserInfo().dicGettedDice[reward.ItemId][1],
                    reward.Value, needDiceCount, 1.2f));
                
            }
                break;
        }
        
        // 애니메이션
        RectTransform rts = (RectTransform) ani_Item.transform;
        rts.DOKill();
        rts.anchoredPosition = new Vector2(0, -250);
        rts.DOAnchorPosY(350f, 0.5f);
        rts.localScale = Vector3.zero;
        rts.DOScale(1.4f, 0.5f);

        SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_COMMON_ITEM_APPEAR);

        openCount++;
    }

    private Coroutine crt_IconChange;
    IEnumerator IconChangeCoroutine(Sprite icon)
    {
        while (iconChangeDelay > 0)
        {
            iconChangeDelay -= Time.deltaTime;
            yield return null;
        }

        btn_Blind.interactable = true;
        image_ItemIcon.sprite = icon;
        image_ItemIcon.SetNativeSize();
    }

    private Coroutine crt_TextCount;
    IEnumerator TextCountCoroutine(int current, int add, int max, float delay)
    {
        int before = current - add;
        text_ItemGuageCount.text = $"{before}/{max}";

        image_ItemGuage.fillAmount = before / (float)max;
        image_ItemGuage.color = before >= max ? Color.green : UnityUtil.HexToColor("6AD3E5");
        image_UpgradeIcon.gameObject.SetActive(before >= max);
        
        yield return new WaitForSeconds(delay);

        float t = 0;
        float timeMax = Mathf.Clamp(0.1f * add, 0, 0.5f);
        while (t < timeMax)
        {
            float v = Mathf.Lerp(before, current, t / timeMax);
            text_ItemGuageCount.text = $"{(int)v}/{max}";
            image_ItemGuage.fillAmount = v / (float)max;
            
            image_ItemGuage.color = (int)v >= max ? Color.green : UnityUtil.HexToColor("6AD3E5");
            if (image_UpgradeIcon.gameObject.activeSelf == false && (int) v >= max)
            {
                image_UpgradeIcon.gameObject.SetActive(true);
                image_UpgradeIcon.rectTransform.localScale = Vector3.zero;
                image_UpgradeIcon.rectTransform.DOScale(1f, 0.25f);
            }

            t += Time.deltaTime;
            yield return null;
            
        }
        
        btn_Blind.interactable = true;
        
        text_ItemGuageCount.text = $"{current}/{max}";
        image_ItemGuage.fillAmount = current / (float)max;
        
        image_ItemGuage.color = current >= max ? Color.green : UnityUtil.HexToColor("6AD3E5");
        if (image_UpgradeIcon.gameObject.activeSelf == false && current >= max)
        {
            image_UpgradeIcon.gameObject.SetActive(true);
            image_UpgradeIcon.rectTransform.localScale = Vector3.zero;
            image_UpgradeIcon.rectTransform.DOScale(1f, 0.25f);
        }
    }
    
    
    IEnumerator ShowResultCoroutine()
    {
        SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_COMMON_RESULT);
        obj_Result.SetActive(true);
        ani_Box.gameObject.SetActive(false);
        UI_Main.Get().boxOpenPopup.Close();
        
        List<MsgReward> list = new List<MsgReward>(msg);
        
        var gold = list.Find(m => m.ItemId == (int)RandomWarsResource.Data.EItemListKey.gold);
        text_ResultGold.text = $"{gold?.Value ?? 0}";
        
        var diamond = list.Find(m => m.ItemId == (int)RandomWarsResource.Data.EItemListKey.dia);
        text_ResultDiamond.text = $"{diamond?.Value ?? 0}";

        int childCount = rts_ResultDiceParent.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(rts_ResultDiceParent.GetChild(i).gameObject);
        }

        int loopCount = 0;
        foreach (var msgReward in list)
        {
            RandomWarsResource.Data.TDataItemList tDataItemList;
            if (TableManager.Get().ItemList.GetData(msgReward.ItemId, out tDataItemList) == true)
            {
                if (tDataItemList.itemType == (int)ITEM_TYPE.DICE)
                {
                    RandomWarsResource.Data.TDataDiceInfo dataDiceInfo;
                    if (TableManager.Get().DiceInfo.GetData(msgReward.ItemId, out dataDiceInfo) == false)
                    {
                        break;
                    }

                    var dice = Instantiate(pref_ResultDice, rts_ResultDiceParent);
                    dice.GetComponent<Image>().sprite = FileHelper.GetIcon(dataDiceInfo.iconName);
                    dice.transform.GetChild(0).GetComponent<Text>().text = LocalizationManager.GetLangDesc((int)LANG_ENUM.DICE_NAME + dataDiceInfo.id);
                    dice.transform.GetChild(1).GetComponent<Text>().text = $"x{msgReward.Value}";
                    dice.transform.localScale = Vector3.zero;
                    dice.transform.DOScale(Vector3.one, 0.2f).SetDelay(0.05f * loopCount++).SetEase(Ease.OutBack)
                        .OnComplete(() =>
                        {
                            SoundManager.instance.Play(Global.E_SOUND.SFX_UI_BOX_COMMON_RESULT_ITEM);
                        });
                    
                }
            }
        }

        openCount++;
        yield return new WaitForSeconds(1.5f);
        btn_Blind.interactable = true;
    }
}
