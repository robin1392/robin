using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.ObscuredTypes;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using ED;
using MirageTest.Scripts;
using RandomWarsResource.Data;


public class UI_InGame : SingletonDestroy<UI_InGame>
{
    private int lastSetSp;
    
    #region ui element variable
    public UI_UpgradeButton[] arrUpgradeButtons;
    public Text text_SP;

    public Button btn_Exit;
    
    public Text text_GetDiceButton;
    public UI_GetDiceButton btn_GetDice;
    
    public GameObject obj_ViewTargetDiceField;
    
    public UI_SPUpgradeButton button_SP_Upgrade;
    public Text text_SP_Upgrade;
    public Text text_SP_Upgrade_Price;

    [Header("Nickname")] 
    public Text text_MyNickname;
    public Text text_EnemyNickname;
    
    [Header("DEV UI")] 
    public GameObject viewTargetDiceField;
    public Text textUnitCount;

    [Header("Enemy Upgrade")] 
    public Image[] arrImage_EnemyUpgradeIcon;
    public Text[] arrText_EnemyUpgrade;

    [Header("SP upgrade message")] 
    public CanvasGroup cg_SpUpgradeMessage;
    public Text text_SpUpgradeMessage;
    
    public Image image_TopTowerHealthBar;
    public Text text_TopTowerHealthBar;
    public Image image_BottomTowerHealthBar;
    public Text text_BottomTowerHealthBar;
    
    public Animator startAnimator;
    public Text startNumber;
    public GameObject startFight;
    public GameObject suddenDeath;
    public Slider sliderWave;
    public RawImageFlow waveFlow;
    public Texture suddenDeathTexture;
    public Animator waveAnimator;
    public Text textComingSP;
    public Animator addSpAnimator;
    public Animator spUpgradeAnimator;
    #endregion

    #region unity base

    public override void Awake()
    {
        base.Awake();

        InitUIElement();
        image_TopTowerHealthBar.transform.parent.gameObject.SetActive(false);
        image_BottomTowerHealthBar.transform.parent.gameObject.SetActive(false);
        startAnimator.gameObject.SetActive(false);
        startFight.gameObject.SetActive(false);
    }
    
    private RWNetworkClient _client;
    public void InitClient(RWNetworkClient client)
    {
        _client = client;
        foreach (var button in arrUpgradeButtons)
        {
            button.InitClient(client);
        }
    }

    public override void OnDestroy()
    {
        DestroyElement();
        
        base.OnDestroy();
    }

    #endregion
    
    #region init destroy

    public void InitUIElement()
    {
        
    }

    public void DestroyElement()
    {
        
    }
    #endregion

    
    
    #region get set

    /// <summary>
    /// 덱 주사위 셋팅
    /// </summary>
    /// <param name="deckDice"></param>
    /// <param name="arrUpgradeLv"></param>
    //public void SetArrayDeck(Data_Dice[] deckDice, int[] arrUpgradeLv)
    public void SetArrayDeck((TDataDiceInfo diceInfo, byte inGameLevel)[] deckDice)
    {
        for (var i = 0; i < arrUpgradeButtons.Length; i++)
        {
            arrUpgradeButtons[i].Initialize( deckDice[i].diceInfo, deckDice[i].inGameLevel, i);
        }
    }

    public void SetEnemyArrayDeck()
    {
        var diceInfos = TableManager.Get().DiceInfo;
        var deck = _client.GetEnemyPlayerState().Deck;
        TDataDiceInfo[] deckDice = deck.Select(d =>
        {
            diceInfos.GetData(d.diceId, out var diceInfo);
            return diceInfo;
        }).ToArray();

        for (int i = 0; i < deckDice.Length; i++)
        {
            arrImage_EnemyUpgradeIcon[i].sprite = FileHelper.GetIcon(deckDice[i].iconName);
        }
    }

    public void SetEnemyUpgrade()
    {
        var diceInfos = TableManager.Get().DiceInfo;
        var arrUpgradeLv = _client.GetEnemyPlayerState().Deck.Select(d =>
        {
            return d.inGameLevel;
        }).ToArray();

        for (int i = 0; i < arrUpgradeLv.Length; i++)
        {
            arrText_EnemyUpgrade[i].text = $"Lv.{(arrUpgradeLv[i] < 5 ? (arrUpgradeLv[i] + 1).ToString() : "MAX")}";
            arrText_EnemyUpgrade[i].color = arrUpgradeLv[i] < 5 ? Color.white : Color.red;
        }
    }

    public void SetDeckRefresh(int diceId , int upgradeLv)
    {
        for (var i = 0; i < arrUpgradeButtons.Length; i++)
        {
            if (arrUpgradeButtons[i].GetDeckDiceId() == diceId)
            {
                arrUpgradeButtons[i].RefreshLevel(upgradeLv);
                arrUpgradeButtons[i].Refresh();
                break;
            }
        }
    }

    public void SetTargetDiceView(bool view)
    {
        viewTargetDiceField.SetActive(view);
    }
    
    public void SetSPGradually(int sp)
    {
        text_SP.text = sp.ToString();
        DOTween.To(() => lastSetSp, x => SetSP(x), sp, 5.0f/ 60.0f);
    }

    
    public void SetSP(int sp)
    {
        lastSetSp = sp;
        text_SP.text = sp.ToString();
    }

    public void SetUnitCount(int count)
    {
        //textUnitCount.text = count.ToString();
        textUnitCount.text = $"총 유닛수: {count}";
    }

    public void SetDiceButtonText(int diceCost)
    {
        text_GetDiceButton.text = $"{diceCost}";
    }

    public void ViewTargetDice(bool view)
    {
        obj_ViewTargetDiceField.SetActive(view);
    }

    public void SetMyNickName(string myName)
    {
        text_MyNickname.text = myName;
    }
    
    public void SetEnemyNickName(string enemyName)
    {
        text_EnemyNickname.text = enemyName;
    }

    public void ControlGetDiceButton(bool inter)
    {
        btn_GetDice.SetInteracterButton(inter);
    }

    public void ClearUI()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
        UI_InGamePopup.Get().ShowLowHPEffect(false);
        var card = FindObjectOfType<UI_Card>();
        if (card != null) card.Off();
    }
    
    #endregion


    #region system

    public void SetSPUpgrade(int currentLv , int cost)
    {
        //button_SP_Upgrade.interactable = (upgradeLv + 1) * 500 <= sp;
        if (currentLv < GameConstants.MaxSpUpgradeLevel)
        {
            text_SP_Upgrade.text = $"Lv.{currentLv}";
            text_SP_Upgrade_Price.text = $"{cost}";
        }
        else
        {
            text_SP_Upgrade.text = $"MAX";
            text_SP_Upgrade_Price.text = string.Empty;
        }
    }

    public void ShowSpUpgradeMessage()
    {
        //text_SpUpgradeMessage.text = $"Bonus {5 * InGameManager.Get().playerController.spUpgradeLevel} SP by wave";
        text_SpUpgradeMessage.text = $"Bonus SP by wave";
        LayoutRebuilder.ForceRebuildLayoutImmediate(text_SpUpgradeMessage.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)cg_SpUpgradeMessage.transform);
        cg_SpUpgradeMessage.DOKill();
        cg_SpUpgradeMessage.DOFade(1f, 0.3f).OnComplete(() =>
        {
            cg_SpUpgradeMessage.DOFade(0f, 0.25f).SetDelay(1f);
        });
    }
    
    #endregion
    
    
    #region event
    
    public void SetSpawnTime(float amount)
    {
        sliderWave.value = amount;
    }
    
    #endregion

    public void EnableSuddenDeath()
    {
        suddenDeath.gameObject.SetActive(true);
        waveFlow.image.texture = suddenDeathTexture;
    }
    
    public void SetComingSp(int addSP)
    {
        textComingSP.text = $"{addSP}";
    }
}
