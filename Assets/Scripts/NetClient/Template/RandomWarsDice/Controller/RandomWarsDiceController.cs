using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Template.Item.RandomWarsDice.Common;

namespace Template.Item.RandomWarsDice
{
    public partial class RandomWarsDiceTemplate
    {
        bool OnUpdateDeckController(ERandomWarsDiceErrorCode errorCode, int deckIndex, int[] deckInfo)
        {
            UserInfoManager.Get().GetUserInfo().SetDeck(deckIndex, deckInfo);

            ED.UI_Panel_Dice panelDice = GameObject.FindObjectOfType<ED.UI_Panel_Dice>();
            panelDice.CallBackDeckUpdate();
            UnityUtil.Print("RECV DECK UPDATE => userid", string.Format("index:{0}, deck:[{1}]", deckIndex, string.Join(",", deckInfo)), "green");

            return true;
        }


        bool OnLevelupDiceController(ERandomWarsDiceErrorCode errorCode, int diceId, short level, short count, int gold)
        {
            ED.UI_Popup_Dice_Info panelDiceInfo = GameObject.FindObjectOfType<ED.UI_Popup_Dice_Info>();
            panelDiceInfo.DiceUpgradeCallback(errorCode, diceId, level, count, gold);  
            return true;
        }


        bool OnOpenBoxController(ERandomWarsDiceErrorCode errorCode, MsgOpenBoxReward[] rewardInfo)
        {
            UI_BoxOpenPopup panelBoxOpen = GameObject.FindObjectOfType<UI_BoxOpenPopup>();
            panelBoxOpen.Callback_BoxOpen(rewardInfo);
            return true;
        }
    }
}
