using System.Collections;
using System.Collections.Generic;
using MirageTest.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace ED
{
    public class UI_DiceField : SingletonDestroy<UI_DiceField>
    {
        public UI_BattleFieldDiceSlot[] arrSlot;

        private InGameManager _ingameManager;

        private RWNetworkClient _client;

        private void Awake()
        {
            _ingameManager = FindObjectOfType<InGameManager>();
        }

        public void InitClient(RWNetworkClient client)
        {
            _client = client;
            foreach (var slot in arrSlot)
            {
                slot.InitClient(client);
            }
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
            var localPlayerState = _client.GetLocalPlayerState();
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
                            _client.GetLocalPlayerProxy().GetDice(2, 0);
                            break;
                        case 1:
                            _client.GetLocalPlayerProxy().GetDice(2, 1);
                            break;
                        case 2:
                            _client.GetLocalPlayerProxy().GetDice(2, 3);
                            break;
                        case 3:
                            _client.GetLocalPlayerProxy().GetDice(0, 6);
                            break;
                        case 4:
                            _client.GetLocalPlayerProxy().GetDice(0, 8);
                            break;
                        case 5:
                            _client.GetLocalPlayerProxy().GetDice(2, 4);
                            break;
                        case 6:
                            _client.GetLocalPlayerProxy().GetDice(2, 2);
                            break;
                        case 7:
                            _client.GetLocalPlayerProxy().GetDice(2, 5);
                            break;
                        case 8:
                            _client.GetLocalPlayerProxy().GetDice(2, 7);
                            break;
                        case 9:
                            _client.GetLocalPlayerProxy().GetDice(2, 9);
                            break;
                        case 10:
                            _client.GetLocalPlayerProxy().GetDice(2, 10);
                            break;
                        case 11:
                            _client.GetLocalPlayerProxy().GetDice(0, 11);
                            break;
                        case 12:
                            _client.GetLocalPlayerProxy().GetDice(3, 12);
                            break;
                        case 13:
                            _client.GetLocalPlayerProxy().GetDice(0, 13);
                            break;
                        case 14:
                            _client.GetLocalPlayerProxy().GetDice(2, 14);
                            break;
                        default:
                            _client.GetLocalPlayerProxy().GetRandomDice();
                            break;
                    }

                    UI_DiceField.Get().RefreshField();
                }
                else
                {
                    UI_InGame.Get().ControlGetDiceButton(false);
                    var playerProxy = _client.GetLocalPlayerProxy();
                    playerProxy.GetRandomDice();
                }
            }
        }
    }
}