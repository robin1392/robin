using System.Collections;
using System.Collections.Generic;
using MirageTest.Scripts;
using Quantum;
using Quantum.Commands;
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

            QuantumEvent.Subscribe<EventFieldDiceCreated>(listener: this, handler: OnFieldDiceCreated);
            QuantumEvent.Subscribe<EventFieldDiceMerged>(listener: this, handler: OnFieldDiceMerged);
        }

        private void OnFieldDiceMerged(EventFieldDiceMerged callback)
        {
            if(TutorialManager.isTutorial)
            {
                TutorialManager.MergeComplete();
            }
            
            UpdateSlot(callback.SourceFieldIndex, callback.Game.Frames.Predicted, callback.Player);
            UpdateSlot(callback.TargetFieldIndex, callback.Game.Frames.Predicted, callback.Player);
            
            SoundManager.instance.Play(Global.E_SOUND.SFX_INGAME_UI_DICE_MERGE);
        }

        void UpdateSlot(int fieldIndex, Frame frame, PlayerRef playerRef)
        {
            var slot = arrSlot[fieldIndex];

            if (frame.TryGetFieldDiceInfo(playerRef, fieldIndex, out var diceId, out var diceScale))
            {
                TableManager.Get().DiceInfo.GetData(diceId, out var diceInfo);    
                slot.SetDice(new Dice()
                {
                    diceFieldNum = fieldIndex,
                    diceData = diceInfo,
                    eyeLevel = diceScale
                });
            
                slot.SetIcon();
                slot.BBoing();
            }
            else
            {
                slot.SetDice(new Dice()
                {
                    diceFieldNum = fieldIndex,
                    diceData = null,
                    eyeLevel = 0
                });
            
                slot.SetIcon();
            }
        }

        private void OnFieldDiceCreated(EventFieldDiceCreated callback)
        {
            var fieldIndex = callback.FieldIndex;
            UpdateSlot(fieldIndex, callback.Game.Frames.Predicted, callback.Player);

            SoundManager.instance.Play(Global.E_SOUND.SFX_INGAME_UI_GET_DICE);
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
            var predicedFrame = QuantumRunner.Default.Game.Frames.Predicted;
            var localPlayer = QuantumRunner.Default.Game.GetLocalPlayers()[0];
            
            if (predicedFrame.IsFieldFull(localPlayer) == false && predicedFrame.HasEnouphSpToCreateFieldDice(localPlayer))
            {
                if (TutorialManager.isTutorial)
                {
                    Debug.Log($"GetDiceCount: {TutorialManager.getDiceCount}");
                    switch (TutorialManager.getDiceCount)
                    {
                        case 0:
                            CreateFieldDice(2, 0);
                            break;
                        case 1:
                            CreateFieldDice(2, 1);
                            break;
                        case 2:
                            CreateFieldDice(2, 3);
                            break;
                        case 3:
                            CreateFieldDice(0, 6);
                            break;
                        case 4:
                            CreateFieldDice(0, 8);
                            break;
                        case 5:
                            CreateFieldDice(2, 4);
                            break;
                        case 6:
                            CreateFieldDice(2, 2);
                            break;
                        case 7:
                            CreateFieldDice(2, 5);
                            break;
                        case 8:
                            CreateFieldDice(2, 7);
                            break;
                        case 9:
                            CreateFieldDice(2, 9);
                            break;
                        case 10:
                            CreateFieldDice(2, 10);
                            break;
                        case 11:
                            CreateFieldDice(0, 11);
                            break;
                        case 12:
                            CreateFieldDice(3, 12);
                            break;
                        case 13:
                            CreateFieldDice(0, 13);
                            break;
                        case 14:
                            CreateFieldDice(2, 14);
                            break;
                        default:
                            CreateRandomFieldDice();
                            break;
                    }

                    RefreshField();
                }
                else
                {
                    CreateRandomFieldDice();
                }
            }
        }

        void CreateRandomFieldDice()
        {
            var command = new CreateRandomFieldDiceCommand();
            QuantumRunner.Default.Game.SendCommand(command);
        }
        
        void CreateFieldDice(int deckIndex, int fieldIndex)
        {
            var command = new CreateFieldDiceCommand();
            command.DeckIndex = deckIndex;
            command.FieldIndex = fieldIndex;
            QuantumRunner.Default.Game.SendCommand(command);
        }
    }
}