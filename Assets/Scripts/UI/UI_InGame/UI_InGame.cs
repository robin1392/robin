using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.UI;
using ED;



public class UI_InGame : SingletonDestroy<UI_InGame>
{
    
    
    
    #region ui element variable
    public UI_UpgradeButton[] arrUpgradeButtons;
    public Text text_SP;

    
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
    public void SetArrayDeck(DiceInfoData[] deckDice, int[] arrUpgradeLv)
    {
        for (var i = 0; i < arrUpgradeButtons.Length; i++)
        {
            arrUpgradeButtons[i].Initialize( deckDice[i], arrUpgradeLv[i]);
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

    public void SetNickname(string enemyNickname)
    {   
        text_MyNickname.text = ObscuredPrefs.GetString("Nickname");
        text_EnemyNickname.text = enemyNickname;
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
    #endregion
    
    
    #region event
    
    
    
    #endregion
    
    
}
