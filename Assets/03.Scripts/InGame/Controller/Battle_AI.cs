using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.BehaviourTrees;
using Random = UnityEngine.Random;

namespace ED
{
    public class Battle_AI : PlayerController
    {
        public int[] deck;
        public override int sp
        {
            get => _sp;
            protected set { /*Debug.LogFormat("SP:{0}, Value:{1}", _sp, value);*/ _sp = value; }
        }

        public int getDiceCost => 10 + getDiceCount * 10;
        public int getDiceCount;

        protected override void Start()
        {
            // throw new NotImplementedException();
            //
            // // _myUID = NetworkManager.Get().OtherUID;
            // // id = myUID * 10000;
            //
            // sp = 200;
            // maxHealth = ConvertNetMsg.MsgIntToFloat(isMine ? NetworkManager.Get().GetNetInfo().playerInfo.TowerHp : NetworkManager.Get().GetNetInfo().otherInfo.TowerHp);
            // currentHealth = maxHealth;
            // //Mirage => PlayerState Field
            // // _arrDice = new Dice[15];
            // //
            // // for (var i = 0; i < arrDice.Length; i++)
            // // {
            // //     arrDice[i] = new Dice {diceFieldNum = i};
            // // }
            //
            // InGameManager.Get().AddPlayerUnit(isBottomPlayer, this);
            //
            // targetPlayer = InGameManager.Get().playerController;
            // InGameManager.Get().playerController.targetPlayer = this;
            //
            // //AI_SetRandomDeck();
            //
            // NetworkManager.Get().GetNetInfo().otherInfo.DiceLevelArray = new short[5];
            //
            // SetColor(E_MaterialType.TOP);
            //
            // if (TutorialManager.isTutorial)
            // {
            //     SetDiceFieldOnTutorial(0);
            // }
        }

        //KZSEE: 에이아이 스크립트에 적용할 것
        public void AI_SetRandomDeck()
        {
            // _arrUpgradeLevel = new int[5];
            // // _arrDiceDeck = new RandomWarsResource.Data.TDataDiceInfo[5];
            //
            // if (deck == null || deck.Length == 0)
            // {
            //     var listDeck = new List<int>();
            //     if (TutorialManager.isTutorial)
            //     {
            //         listDeck.Add(1000);    // 궁수
            //         listDeck.Add(1002);    // 해골
            //         listDeck.Add(3011);    // 전사
            //         listDeck.Add(3003);    // 방패병
            //         listDeck.Add(3005);    // 화염술사
            //
            //         GetComponent<BehaviourTreeOwner>().behaviour.Pause();
            //     }
            //     else
            //     {
            //         for (var i = 0; i < _arrDiceDeck.Length; i++)
            //         {
            //             var rndDiceNum = 0;
            //             List<int> keyList = InGameManager.Get().data_DiceInfo.Keys;
            //             do
            //             {
            //                 var rndNum = keyList[Random.Range(0, keyList.Count)];
            //
            //                 if (InGameManager.Get().data_DiceInfo.KeyValues[rndNum].enableDice == false) continue;
            //
            //                 rndDiceNum = InGameManager.Get().data_DiceInfo.KeyValues[rndNum].id;
            //
            //             } while (listDeck.Contains(rndDiceNum) || rndDiceNum == 0);
            //
            //             listDeck.Add(rndDiceNum);
            //         }
            //     }
            //
            //     deck = listDeck.ToArray();
            // }
            //
            // NetworkManager.Get().GetNetInfo().otherInfo.DiceIdArray = deck;
            // SetDeck(deck);
        }

        public void AI_GetDice()
        {
            if (sp >= getDiceCost)
            {
                sp -= getDiceCost;
                getDiceCount++;

                GetDice();
            }
        }

        //KZSee: 에이에이 레벨업에 적용
        public void AI_LevelUpDice()
        {
            // if (InGameManager.Get().time > 5f)
            // {
            //     var arr = new int[arrDice.Length];
            //     for (var i = 0; i < arr.Length; i++) arr[i] = i;
            //     ShuffleIntArray(ref arr, 2);
            //
            //
            //     for (var i = 0; i < arrDice.Length; i++)
            //     {
            //         //if (arrDice[arr[i]].data != null)
            //         if (arrDice[arr[i]].diceData != null)
            //         {
            //             var data = arrDice[arr[i]].diceData;
            //             var level = arrDice[arr[i]].eyeLevel;
            //
            //             for (var j = 0; j < arrDice.Length; j++)
            //             {
            //                 if (arr[i] == j) continue;
            //
            //                 if (data == arrDice[j].diceData && level == arrDice[j].eyeLevel)
            //                 {
            //                     // Upgrade
            //                     arrDice[j].LevelUp(arrDiceDeck);
            //                     arrDice[arr[i]].Reset();
            //
            //                     return;
            //                 }
            //             }
            //         }
            //     }
            // }
        }
        
        
        //KZSee: AI 업그레이드에 적용
        private readonly int[] _arrPrice = { 100, 200, 400, 700, 1100 };
        public void AI_UpgradeDice()
        {
            // var arr = new int[arrDiceDeck.Length];
            // for (var i = 0; i < arr.Length; i++) arr[i] = i;
            // ShuffleIntArray(ref arr);
            //
            // for (var i = 0; i < arrDiceDeck.Length; i++)
            // {
            //     if (arrUpgradeLevel[arr[i]] < 5 && sp >= _arrPrice[arrUpgradeLevel[arr[i]]])
            //     {
            //         DiceUpgrade(arr[i]);
            //         AddSp(-_arrPrice[arrUpgradeLevel[arr[i]] - 1]);
            //     }
            // }
            //
            // UI_InGame.Get().SetEnemyUpgrade();
        }

        private static void ShuffleIntArray(ref int[] arr, int shuffleCount = 1)
        {
            for (var k = 0; k < shuffleCount; k++)
            {
                for (var i = 0; i < arr.Length; i++)
                {
                    var j = Random.Range(0, arr.Length);
                    var temp = arr[j];
                    arr[j] = arr[i];
                    arr[i] = temp;
                }
            }
        }

        public void SetDiceFieldOnTutorial(int phase)
        {
            switch (phase)
            {
                case 0:
                    // 궁수, 해골, 전사, 방패, 화염
                    GetDice(3, 1);
                    GetDice(1, 2);
                    GetDice(3, 3);
                    GetDice(2, 5);
                    GetDice(4, 7);
                    GetDice(2, 9);
                    GetDice(0, 11);
                    GetDice(0, 13);
                    break;
                case 1:
                    break;
                case 2:
                    break;
            }
        }
    }
}
