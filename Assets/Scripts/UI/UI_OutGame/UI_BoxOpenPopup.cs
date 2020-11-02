#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ED;
using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using UnityEngine;
using UnityEngine.UI;

public class UI_BoxOpenPopup : UI_Popup
{
    public enum COST_TYPE
    {
        KEY,
        GOLD,
        DIAMOND
    }

    [Header("Link")]
    public Button btn_Open;
    public Image image_CostIcon;
    public Text text_Cost;

    [Header("Box")]
    public Animator ani_Box;
    public Image image_Box;
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

    private int boxID;
    private COST_TYPE costType;
    private int cost;
    private int openCount;
    private MsgOpenBoxAck msg;
    
    public void Initialize(int id, COST_TYPE costType, int cost)
    {
        this.boxID = id;
        this.costType = costType;
        this.cost = cost;
        
        var data = JsonDataManager.Get().dataBoxInfo.GetData(id);
        var classData = new List<RewardData>(data.classRewards[UserInfoManager.Get().GetUserInfo().nClass]);
        
        // Gold
        var goldData = classData.Find(d => d.rewardType == REWARD_TYPE.GOLD);
        obj_Gold.SetActive(goldData != null);
        if (goldData != null)
        {
            text_Gold.text = goldData.value.ToString();
        }
        
        int totalDiceCount = 0;
        // Normal Dice
        var normalDiceData = classData.Find(d => d.rewardType == REWARD_TYPE.DICE_NORMAL);
        if (normalDiceData != null) totalDiceCount += normalDiceData.value;
        
        // Magic Dice
        var magicDiceData = classData.Find(d => d.rewardType == REWARD_TYPE.DICE_MAGIC);
        if (magicDiceData != null) totalDiceCount += magicDiceData.value;

        // Epic Dice
        var epicDiceData = classData.Find(d => d.rewardType == REWARD_TYPE.DICE_EPIC);
        if (epicDiceData != null) totalDiceCount += epicDiceData.value;

        // Legend Dice
        var legendDiceData = classData.Find(d => d.rewardType == REWARD_TYPE.DICE_LEGEND);
        if (legendDiceData != null) totalDiceCount += legendDiceData.value;
        
        // Total Dice
        obj_Dice.SetActive(totalDiceCount > 0);
        if (totalDiceCount > 0) text_Dice.text = $"x{totalDiceCount}";
        
        // Diamond
        var diamondData = classData.Find(d => d.rewardType == REWARD_TYPE.DIAMOND);
        obj_Diamond.SetActive(diamondData != null);
        if (diamondData != null)
        {
            text_Diamond.text = diamondData.value.ToString();
        }
        
        // Include Magic Dice
        obj_MagicDice.SetActive(magicDiceData.value > 0);
        if (magicDiceData.value > 0) text_MagicDice.text = $"x{magicDiceData.value}";
        
        // Include Epic Dice
        obj_EpicDice.SetActive(epicDiceData.value > 0);
        if (epicDiceData.value > 0) text_EpicDice.text = $"x{epicDiceData.value}";
        
        // Include Legend Dice
        obj_LegendDice.SetActive(legendDiceData.value > 0);
        if (legendDiceData.value > 0) text_LegendDice.text = $"x{legendDiceData.value}";

        image_CostIcon.sprite = arrSprite_CostType[(int)costType];
        text_Cost.text = cost.ToString();
        obj_BoxOpenParticle.SetActive(false);
        openCount = 0;
        
        RectTransform rts = ((RectTransform) ani_Box.transform); 
        rts.DOAnchorPosY(400f, 0);
        rts.DOScale(1f, 0);

        image_Blind.raycastTarget = false;
        image_Blind.DOFade(0, 0);
        image_Pattern.DOFade(0, 0);
        ani_Item.gameObject.SetActive(false);
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
        
        for (int i = 0; i < msg.BoxReward.Length; i++)
        {
            Debug.Log($"Reward   ID:{msg.BoxReward[i].Id} , Type:{msg.BoxReward[i].RewardType.ToString()}, Count:{msg.BoxReward[i].Value}");

            var level = 0;
            var count = 0;
            if (UserInfoManager.Get().GetUserInfo().dicGettedDice.ContainsKey(msg.BoxReward[i].Id))
            {
                level = UserInfoManager.Get().GetUserInfo().dicGettedDice[msg.BoxReward[i].Id][0];
                count = UserInfoManager.Get().GetUserInfo().dicGettedDice[msg.BoxReward[i].Id][1];
            }

            switch (msg.BoxReward[i].RewardType)
            {
                case REWARD_TYPE.TROPHY:
                    UserInfoManager.Get().GetUserInfo().trophy += msg.BoxReward[i].Value;
                    break;
                case REWARD_TYPE.GOLD:
                    UserInfoManager.Get().GetUserInfo().gold += msg.BoxReward[i].Value;
                    break;
                case REWARD_TYPE.DIAMOND:
                    UserInfoManager.Get().GetUserInfo().diamond += msg.BoxReward[i].Value;
                    break;
                case REWARD_TYPE.KEY:
                    UserInfoManager.Get().GetUserInfo().key += msg.BoxReward[i].Value;
                    break;
                case REWARD_TYPE.BOX:
                    if (UserInfoManager.Get().GetUserInfo().dicBox.ContainsKey(msg.BoxReward[i].Id))
                    {
                        UserInfoManager.Get().GetUserInfo().dicBox[msg.BoxReward[i].Id] += msg.BoxReward[i].Value;
                    }
                    else
                    {
                        UserInfoManager.Get().GetUserInfo().dicBox.Add(msg.BoxReward[i].Id, msg.BoxReward[i].Value);
                    }
                    break;
                case REWARD_TYPE.DICE_NORMAL:
                {
                    if (level == 0)
                    {
                        level = JsonDataManager.Get().dataGlobalDataInfo.GetData(GLOBAL_DATA_KEY.DICE_START_LEVEL_NORMAL).value;
                        count = msg.BoxReward[i].Value;
                        UserInfoManager.Get().GetUserInfo().dicGettedDice.Add(msg.BoxReward[i].Id, new int[] { level, count });
                    }
                    else
                    {
                        count += msg.BoxReward[i].Value;
                        UserInfoManager.Get().GetUserInfo().dicGettedDice[msg.BoxReward[i].Id][1] = count;
                    }
                }
                    break;
                case REWARD_TYPE.DICE_MAGIC:
                {
                    if (level == 0)
                    {
                        level = JsonDataManager.Get().dataGlobalDataInfo.GetData(GLOBAL_DATA_KEY.DICE_START_LEVEL_MAGIC).value;
                        count = msg.BoxReward[i].Value;
                        UserInfoManager.Get().GetUserInfo().dicGettedDice.Add(msg.BoxReward[i].Id, new int[] { level, count });
                    }
                    else
                    {
                        count += msg.BoxReward[i].Value;
                        UserInfoManager.Get().GetUserInfo().dicGettedDice[msg.BoxReward[i].Id][1] = count;
                    }
                }
                    break;
                case REWARD_TYPE.DICE_EPIC:
                {
                    if (level == 0)
                    {
                        level = JsonDataManager.Get().dataGlobalDataInfo.GetData(GLOBAL_DATA_KEY.DICE_START_LEVEL_EPIC).value;
                        count = msg.BoxReward[i].Value;
                        UserInfoManager.Get().GetUserInfo().dicGettedDice.Add(msg.BoxReward[i].Id, new int[] { level, count });
                    }
                    else
                    {
                        count += msg.BoxReward[i].Value;
                        UserInfoManager.Get().GetUserInfo().dicGettedDice[msg.BoxReward[i].Id][1] = count;
                    }
                }
                    break;
                case REWARD_TYPE.DICE_LEGEND:
                {
                    if (level == 0)
                    {
                        level = JsonDataManager.Get().dataGlobalDataInfo.GetData(GLOBAL_DATA_KEY.DICE_START_LEVEL_LEGEND).value;
                        count = msg.BoxReward[i].Value;
                        UserInfoManager.Get().GetUserInfo().dicGettedDice.Add(msg.BoxReward[i].Id, new int[] { level, count });
                    }
                    else
                    {
                        count += msg.BoxReward[i].Value;
                        UserInfoManager.Get().GetUserInfo().dicGettedDice[msg.BoxReward[i].Id][1] = count;
                    }
                }
                    break;
            }
        }
        
        UI_Main.Get().boxPopup.RefreshBox();
        UI_Main.Get().RefreshUserInfoUI();
        UI_Main.Get().panel_Dice.RefreshGettedDice();
        
        SetShowItems();
    }

    private void SetShowItems()
    {
        RectTransform rts = ((RectTransform) ani_Box.transform); 
        rts.DOAnchorPosY(-500f, 0.5f).SetEase(Ease.OutQuint);
        rts.DOScale(1.4f, 0.5f);

        image_Blind.raycastTarget = true;
        image_Blind.DOFade(1f, 0.5f);
        image_Pattern.DOFade(0.007843f, 0.5f);
    }

    public void Click_NextButton()
    {
        Debug.Log("Click NextButotn");

        if (msg != null && openCount < msg.BoxReward.Length)
        {
            ani_Box.SetTrigger("Open");

            if (openCount == 0)
            {
                obj_BoxOpenParticle.SetActive(true);

                Invoke("ItemAnimation", 0.6666f);
            }
            else
            {
                ItemAnimation();
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void ItemAnimation()
    {
        ani_Item.gameObject.SetActive(true);

        MsgReward reward = msg.BoxReward[openCount];
        
        // 보상내용 세팅
        switch (reward.RewardType)
        {
            case REWARD_TYPE.GOLD:
                image_ItemIcon.sprite = sprite_Gold;
                image_ItemIcon.SetNativeSize();
                ani_Item.SetTrigger("Get");
                text_ItemName.text = LocalizationManager.GetLangDesc((int)LANG_ENUM.GOLD);
                text_ItemCount.text = $"x{reward.Value}";
                break;
            case REWARD_TYPE.DIAMOND:
                image_ItemIcon.sprite = sprite_Diamond;
                image_ItemIcon.SetNativeSize();
                ani_Item.SetTrigger("Get");
                text_ItemName.text = LocalizationManager.GetLangDesc((int)LANG_ENUM.DIAMOND);
                text_ItemCount.text = $"x{reward.Value}";
                break;
            case REWARD_TYPE.DICE_NORMAL:
                for (int i = 0; i < arrPs_ItemNormal.Length; i++)
                {
                    var module = arrPs_ItemNormal[i].main;
                    module.startColor = UnityUtil.HexToColor(Global.g_gradeColor[0]);
                }
                image_ItemIcon.sprite = FileHelper.GetIcon(JsonDataManager.Get().dataDiceInfo.dicData[reward.Id].iconName);
                ani_Item.SetTrigger("Get");
                image_ItemIcon.SetNativeSize();
                text_ItemName.text = LocalizationManager.GetLangDesc((int)LANG_ENUM.DICE_NAME + reward.Id);
                text_ItemCount.text = $"x{reward.Value}";
                StartCoroutine(TextCountCoroutine(UserInfoManager.Get().GetUserInfo().dicGettedDice[reward.Id][1],
                    reward.Value,
                    Global.g_needDiceCount[UserInfoManager.Get().GetUserInfo().dicGettedDice[reward.Id][0]]));
                break;
            case REWARD_TYPE.DICE_MAGIC:
                for (int i = 0; i < arrPs_ItemNormal.Length; i++)
                {
                    var module = arrPs_ItemNormal[i].main;
                    module.startColor = UnityUtil.HexToColor(Global.g_gradeColor[1]);
                }
                image_ItemIcon.sprite = FileHelper.GetIcon(JsonDataManager.Get().dataDiceInfo.dicData[reward.Id].iconName);
                ani_Item.SetTrigger("Get");
                image_ItemIcon.SetNativeSize();
                text_ItemName.text = LocalizationManager.GetLangDesc((int)LANG_ENUM.DICE_NAME + reward.Id);
                text_ItemCount.text = $"x{reward.Value}";
                StartCoroutine(TextCountCoroutine(UserInfoManager.Get().GetUserInfo().dicGettedDice[reward.Id][1],
                    reward.Value,
                    Global.g_needDiceCount[UserInfoManager.Get().GetUserInfo().dicGettedDice[reward.Id][0]]));
                break;
            case REWARD_TYPE.DICE_EPIC:
                for (int i = 0; i < arrPs_ItemNormal.Length; i++)
                {
                    var module = arrPs_ItemNormal[i].main;
                    module.startColor = UnityUtil.HexToColor(Global.g_gradeColor[2]);
                }
                image_ItemIcon.sprite = FileHelper.GetIcon(JsonDataManager.Get().dataDiceInfo.dicData[reward.Id].iconName);
                ani_Item.SetTrigger("Get");
                image_ItemIcon.SetNativeSize();
                text_ItemName.text = LocalizationManager.GetLangDesc((int)LANG_ENUM.DICE_NAME + reward.Id);
                text_ItemCount.text = $"x{reward.Value}";
                StartCoroutine(TextCountCoroutine(UserInfoManager.Get().GetUserInfo().dicGettedDice[reward.Id][1],
                    reward.Value,
                    Global.g_needDiceCount[UserInfoManager.Get().GetUserInfo().dicGettedDice[reward.Id][0]]));
                break;
            case REWARD_TYPE.DICE_LEGEND:
                image_ItemIcon.sprite = FileHelper.GetIcon(JsonDataManager.Get().dataDiceInfo.dicData[reward.Id].iconName);
                image_ItemIcon.SetNativeSize();
                ani_Item.SetTrigger("GetLegend");
                text_ItemName.text = LocalizationManager.GetLangDesc((int)LANG_ENUM.DICE_NAME + reward.Id);
                text_ItemCount.text = $"x{reward.Value}";
                StartCoroutine(TextCountCoroutine(UserInfoManager.Get().GetUserInfo().dicGettedDice[reward.Id][1],
                    reward.Value,
                    Global.g_needDiceCount[UserInfoManager.Get().GetUserInfo().dicGettedDice[reward.Id][0]]));
                btn_Blind.interactable = false;
                break;
        }
        
        // 애니메이션
        RectTransform rts = (RectTransform) ani_Item.transform;
        rts.anchoredPosition = new Vector2(0, -250);
        rts.DOAnchorPosY(350f, 0.5f);
        rts.localScale = Vector3.zero;
        rts.DOScale(1.4f, 0.5f);

        openCount++;
    }

    IEnumerator TextCountCoroutine(int current, int add, int max)
    {
        int before = current - add;
        text_ItemGuageCount.text = $"{before}/{max}";

        image_ItemGuage.fillAmount = before / (float)max;
        image_ItemGuage.color = before >= max ? Color.green : UnityUtil.HexToColor("6AD3E5");
        image_UpgradeIcon.gameObject.SetActive(before >= max);
        
        yield return new WaitForSeconds(1.3f);

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
}
