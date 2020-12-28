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
            if (InGameManager.Get().playerController.sp >= InGameManager.Get().GetDiceCost() && InGameManager.Get().playerController.GetDiceFieldEmptySlotCount() > 0)
            {
                if (InGameManager.IsNetwork == true)
                {
                    UI_InGame.Get().ControlGetDiceButton(false);
                    InGameManager.Get().NetGetDice();
                }
                else
                {
                    InGameManager.Get().GetDice();
                    if (TutorialManager.isTutorial)
                    {
                        Debug.Log($"GetDiceCount: {TutorialManager.getDiceCount}");
                        switch (TutorialManager.getDiceCount)
                        {
                            case 0:
                                InGameManager.Get().playerController.GetDice(0, 10);
                                break;
                            case 1:
                                InGameManager.Get().playerController.GetDice(2, 6);
                                break;
                            case 2:
                                InGameManager.Get().playerController.GetDice(2, 7);
                                break;
                            case 3:
                                InGameManager.Get().playerController.GetDice(2, 8);
                                break;
                            case 4:
                                InGameManager.Get().playerController.GetDice(0, 14);
                                break;
                            default:
                                InGameManager.Get().playerController.GetDice();
                                break;
                        }
                        InGameManager.Get().playerController.uiDiceField.RefreshField();
                    }
                    else
                    {
                        InGameManager.Get().playerController.GetDice();
                    }
                    RefreshField();
                }
            }
        }
    }
}