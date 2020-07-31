using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ED
{
    public class UI_DiceField : MonoBehaviour
    {
        public UI_BattleFieldDiceSlot[] arrSlot;

        private InGameManager _ingameManager;

        private void Awake()
        {
            _ingameManager = FindObjectOfType<InGameManager>();
        }

        public void SetField(Dice[] arrDice)
        {
            for (var i = 0; i < arrSlot.Length; i++)
            {
                arrSlot[i].SetDice(arrDice[i]);
            }
        }

        public void RefreshField(float alpha = 1f)
        {
            for (var i = 0; i < arrSlot.Length; i++)
            {
                arrSlot[i].SetIcon(alpha);
            }
        }

        public void Click_GetDiceButton()
        {
            if (_ingameManager.playerController.sp >= _ingameManager.getDiceCost && _ingameManager.playerController.GetDiceFieldEmptySlotCount() > 0)
            {
                _ingameManager.GetDice();
                _ingameManager.playerController.GetDice();
                RefreshField();
            }
        }
    }
}