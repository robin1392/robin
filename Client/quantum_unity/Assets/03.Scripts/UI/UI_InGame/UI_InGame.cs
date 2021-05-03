using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CodeStage.AntiCheat.ObscuredTypes;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;
using ED;
using MirageTest.Scripts;
using Quantum;
using RandomWarsResource.Data;
using Button = UnityEngine.UI.Button;


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

    private TweenerCore<int, int, NoOptions> spIncreaseTween;

    public override void Awake()
    {
        base.Awake();

        InitUIElement();
        image_TopTowerHealthBar.transform.parent.gameObject.SetActive(false);
        image_BottomTowerHealthBar.transform.parent.gameObject.SetActive(false);
        startAnimator.gameObject.SetActive(false);
        startFight.gameObject.SetActive(false);
        
        QuantumEvent.Subscribe<EventPlayerInitialized>(listener: this, handler: OnPlayerInitialized);
        QuantumEvent.Subscribe<EventPoweredDeckDiceUp>(listener: this, handler: OnPoweredDeckDiceUp);
        
        QuantumEvent.Subscribe<EventSpIncreased>(listener: this, handler: OnSpIncreased);
        QuantumEvent.Subscribe<EventSpDecreased>(listener: this, handler: OnSpDecreased);
        QuantumEvent.Subscribe<EventCommingSpGradeUpgraded>(listener: this, handler: OnCommingSpGradeUpgraded);
        QuantumEvent.Subscribe<EventCommingSpChanged>(listener: this, handler: OnCommingSpChanged);
        QuantumEvent.Subscribe<EventCommingSpGradeChanged>(listener: this, handler: OnCommingSpGradeChanged);
        QuantumEvent.Subscribe<EventDiceCreationCountChanged>(listener: this, handler: OnDiceCreationCountChanged);
    }

    private void OnDiceCreationCountChanged(EventDiceCreationCountChanged callback)
    {
        var frame = callback.Game.Frames.Predicted;
        SetDiceButtonText(frame.CreateFieldDiceCost(callback.Player));
    }

    private unsafe void OnCommingSpGradeChanged(EventCommingSpGradeChanged callback)
    {
        var frame = callback.Game.Frames.Predicted;
        var player = frame.Global->Players[callback.Player];
        var sp = frame.Get<Sp>(player.EntityRef);
        
        SetSPUpgrade(sp.CommingSpGrade, frame.SpUpgradeCost(sp.CommingSpGrade));
    }

    private unsafe void OnCommingSpChanged(EventCommingSpChanged callback)
    {
        var frame = callback.Game.Frames.Predicted;
        var player = frame.Global->Players[callback.Player];
        var sp = frame.Get<Sp>(player.EntityRef);
        
        SetComingSp(sp.CommingSp);
    }

    private void OnCommingSpGradeUpgraded(EventCommingSpGradeUpgraded callback)
    {
        SoundManager.instance.Play(Global.E_SOUND.SFX_INGAME_UI_SP_LEVEL_UP);
        spUpgradeAnimator.SetTrigger("Fx_SP_Upgrade");
    }

    private unsafe void OnSpDecreased(EventSpDecreased callback)
    {
        var frame = callback.Game.Frames.Predicted;
        var player = frame.Global->Players[callback.Player];
        var sp = frame.Get<Sp>(player.EntityRef);
        
        SetSP(sp.CurrentSp);
        UpdateButtons(sp.CurrentSp, sp.CommingSpGrade, frame, callback.Player);
    }

    private unsafe void OnSpIncreased(EventSpIncreased callback)
    {
        var frame = callback.Game.Frames.Predicted;
        var player = frame.Global->Players[callback.Player];
        var sp = frame.Get<Sp>(player.EntityRef);
        
        addSpAnimator.SetTrigger("Sp_Get");
        SetSPGradually(sp.CurrentSp);
        UpdateButtons(sp.CurrentSp, sp.CommingSpGrade, frame, callback.Player);
    }

    void UpdateButtons(int currentSp, int spGrade, Frame frame, PlayerRef playerRef)
    {
        foreach (var upgradeButton in arrUpgradeButtons)
        {
            upgradeButton.EditSpCallback(currentSp);
        }
        
        btn_GetDice.EditSpCallback(currentSp >= frame.CreateFieldDiceCost(playerRef));
        button_SP_Upgrade.EditSpCallback(currentSp >= frame.SpUpgradeCost(playerRef) && spGrade < GameConstants_Mirage.MaxSpUpgradeLevel);
    }

    private unsafe void OnPoweredDeckDiceUp(EventPoweredDeckDiceUp callback)
    {
        var frame = callback.Game.Frames.Verified;
        var player = frame.Global->Players[callback.Player];
        var deck = frame.Get<Deck>(player.EntityRef);
        
        var deckDice = deck.Dices[callback.DeckIndex];
        TableManager.Get().DiceInfo.GetData(deckDice.DiceId, out var diceInfo);
        arrUpgradeButtons[callback.DeckIndex].Initialize( diceInfo, deckDice.inGameLevel, callback.DeckIndex);
        
        arrUpgradeButtons[callback.DeckIndex].OnPowerUp();
        
        SoundManager.instance.Play(Global.E_SOUND.SFX_INGAME_UI_DICE_LEVEL_UP);
    }

    private unsafe void OnPlayerInitialized(EventPlayerInitialized callback)
    {
        var frame = callback.Game.Frames.Verified;
        var player = frame.Global->Players[callback.Player];
        var deck = frame.Get<Deck>(player.EntityRef);
        for (var i = 0; i < deck.Dices.Length; ++i)
        {
            var deckDice = deck.Dices[i];
            TableManager.Get().DiceInfo.GetData(deckDice.DiceId, out var diceInfo);
            arrUpgradeButtons[i].Initialize( diceInfo, deckDice.inGameLevel, i);
        }
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
        if (spIncreaseTween != null)
        {
            spIncreaseTween.Kill();
        }
        spIncreaseTween = DOTween.To(() => lastSetSp, x => SetSP(x), sp, 5.0f/ 60.0f);
    }

    
    public void SetSP(int sp)
    {
        if (spIncreaseTween != null)
        {
            spIncreaseTween.Kill();
            spIncreaseTween = null;
        }
        
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

