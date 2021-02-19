using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using ED;



public class UI_InGame : SingletonDestroy<UI_InGame>
{
    
    
    
    #region ui element variable
    public UI_UpgradeButton[] arrUpgradeButtons;
    public Text text_SP;

    public Button btn_Exit;
    
    public Text text_GetDiceButton;
    public UI_GetDiceButton btn_GetDice;
    
    

    public GameObject obj_ViewTargetDiceField;
    
    public Button button_SP_Upgrade;
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
    
    
    #endregion

    
    
    #region unity base

    public override void Awake()
    {
        base.Awake();

        InitUIElement();
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
    public void SetArrayDeck(RandomWarsResource.Data.TDataDiceInfo[] deckDice, int[] arrUpgradeLv)
    {
        for (var i = 0; i < arrUpgradeButtons.Length; i++)
        {
            arrUpgradeButtons[i].Initialize( deckDice[i], arrUpgradeLv[i]);
        }
    }

    public void SetEnemyArrayDeck()
    {
        RandomWarsResource.Data.TDataDiceInfo[] deckDice = InGameManager.Get().playerController.targetPlayer.arrDiceDeck;
        int[] arrUpgradeLv = InGameManager.Get().playerController.targetPlayer.arrUpgradeLevel;
        
        for (int i = 0; i < deckDice.Length; i++)
        {
            arrImage_EnemyUpgradeIcon[i].sprite = FileHelper.GetIcon(deckDice[i].iconName);
        }
    }

    public void SetEnemyUpgrade()
    {
        int[] arrUpgradeLv = InGameManager.Get().playerController.targetPlayer.arrUpgradeLevel;

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

    public void SetSP(int sp)
    {
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

    public void SetNickName(string myName, string enemyName)
    {
        text_MyNickname.text = myName;
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
        UI_InGamePopup.Get().ViewLowHP(false);
    }
    
    #endregion


    #region system

    public void SetSPUpgrade(int upgradeLv , int sp)
    {
        //button_SP_Upgrade.interactable = (upgradeLv + 1) * 500 <= sp;
        if (upgradeLv < 5)
        {
            text_SP_Upgrade.text = $"Lv.{upgradeLv + 1}";
            text_SP_Upgrade_Price.text = $"{(upgradeLv + 1) * 100}";
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
    
    
    
    #endregion
    
    
}
