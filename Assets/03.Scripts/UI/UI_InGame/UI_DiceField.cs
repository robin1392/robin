using System.Collections;
using System.Collections.Generic;
using MirageTest.Scripts;
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
            var localPlayerState = RWNetworkClient.instance.GetLocalPlayerState();
            var diceCost = localPlayerState.GetDiceCost();
            if (localPlayerState == null)
            {
                return;
            }
            
            if (localPlayerState.sp >= diceCost && localPlayerState.GetEmptySlotCount() > 0)
            {
                if (TutorialManager.isTutorial)
                {
                    Debug.Log($"GetDiceCount: {TutorialManager.getDiceCount}");
                    switch (TutorialManager.getDiceCount)
                    {
                        case 0:
                            InGameManager.Get().playerController.GetDice(2, 0);
                            break;
                        case 1:
                            InGameManager.Get().playerController.GetDice(2, 1);
                            break;
                        case 2:
                            InGameManager.Get().playerController.GetDice(2, 3);
                            break;
                        case 3:
                            InGameManager.Get().playerController.GetDice(0, 6);
                            break;
                        case 4:
                            InGameManager.Get().playerController.GetDice(0, 8);
                            break;
                        case 5:
                            InGameManager.Get().playerController.GetDice(2, 4);
                            break;
                        case 6:
                            InGameManager.Get().playerController.GetDice(2, 2);
                            break;
                        case 7:
                            InGameManager.Get().playerController.GetDice(2, 5);
                            break;
                        case 8:
                            InGameManager.Get().playerController.GetDice(2, 7);
                            break;
                        case 9:
                            InGameManager.Get().playerController.GetDice(2, 9);
                            break;
                        case 10:
                            InGameManager.Get().playerController.GetDice(2, 10);
                            break;
                        case 11:
                            InGameManager.Get().playerController.GetDice(0, 11);
                            break;
                        case 12:
                            InGameManager.Get().playerController.GetDice(3, 12);
                            break;
                        case 13:
                            InGameManager.Get().playerController.GetDice(0, 13);
                            break;
                        case 14:
                            InGameManager.Get().playerController.GetDice(2, 14);
                            break;
                        default:
                            InGameManager.Get().playerController.GetDice();
                            break;
                    }

                    InGameManager.Get().playerController.uiDiceField.RefreshField();
                }
                else
                {
                    UI_InGame.Get().ControlGetDiceButton(false);
                    var playerProxy = RWNetworkClient.instance.GetLocalPlayerProxy();
                    playerProxy.GetDice();
                }
            }
        }
    }
}