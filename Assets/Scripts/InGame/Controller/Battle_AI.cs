﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
            _myUID = NetworkManager.Get().OtherUID;
            id = myUID * 10000;
            
            sp = 200;
            maxHealth = ConvertNetMsg.MsgIntToFloat(isMine ? NetworkManager.Get().GetNetInfo().playerInfo.TowerHp : NetworkManager.Get().GetNetInfo().otherInfo.TowerHp);
            currentHealth = maxHealth;
            _arrDice = new Dice[15];
            
            //_arrDeck = new Data_Dice[5];
            _arrDiceDeck = new DiceInfoData[5];
            
            _arrUpgradeLevel = new int[5];
            for (var i = 0; i < arrDice.Length; i++)
            {
                arrDice[i] = new Dice {diceFieldNum = i};
            }

            //image_HealthBar = isBottomPlayer ? InGameManager.Get().image_BottomHealthBar : InGameManager.Get().image_TopHealthBar;
            //text_Health = isBottomPlayer ? InGameManager.Get().text_BottomHealth : InGameManager.Get().text_TopHealth;
            
            // image_HealthBar = WorldUIManager.Get().GetHealthBar(isBottomPlayer);
            // text_Health = WorldUIManager.Get().GetHealthText(isBottomPlayer);
            //
            // text_Health.text = $"{Mathf.CeilToInt(currentHealth)}";

            InGameManager.Get().AddPlayerUnit(isBottomPlayer, this);

            targetPlayer = InGameManager.Get().playerController;
            InGameManager.Get().playerController.targetPlayer = this;

            if (deck == null || deck.Length == 0)
            {
                var listDeck = new List<int>();
                for (var i = 0; i < _arrDiceDeck.Length; i++)
                {
                    var rndDiceNum = 0;
                    List<int> keyList = InGameManager.Get().data_DiceInfo.dicData.Keys.ToList();
                    do
                    {
                        var rndNum = keyList[Random.Range(0, keyList.Count)];
                        

                        //var rndNum = Random.Range(0, keyList.Count - 10);
                        //var rndNum = DataPatchManager.Get().
                        //rndDiceNum = keyList[rndNum];
                        //var rndNum = Random.Range(0, InGameManager.Get().data_AllDice.listDice.Count);
                        
                        if (InGameManager.Get().data_DiceInfo.dicData[rndNum].enableDice == false) continue;
                        
                        rndDiceNum = InGameManager.Get().data_DiceInfo.dicData[rndNum].id;

                    } while (listDeck.Contains(rndDiceNum) || rndDiceNum == 0);

                    listDeck.Add(rndDiceNum);
                }

                //deck = $"{listDeck[0]}/{listDeck[1]}/{listDeck[2]}/{listDeck[3]}/{listDeck[4]}";
                deck = listDeck.ToArray();
            }

            NetworkManager.Get().GetNetInfo().otherInfo.DiceIdArray = deck;
            NetworkManager.Get().GetNetInfo().otherInfo.DiceLevelArray = new short[5];

            SetDeck(NetworkManager.Get().GetNetInfo().otherInfo.DiceIdArray);
            SetColor(E_MaterialType.TOP);
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

        public void AI_LevelUpDice()
        {
            if (InGameManager.Get().time > 5f)
            {
                var arr = new int[arrDice.Length];
                for (var i = 0; i < arr.Length; i++) arr[i] = i;
                ShuffleIntArray(ref arr, 2);


                for (var i = 0; i < arrDice.Length; i++)
                {
                    //if (arrDice[arr[i]].data != null)
                    if (arrDice[arr[i]].diceData != null)
                    {
                        var data = arrDice[arr[i]].diceData;
                        var level = arrDice[arr[i]].eyeLevel;

                        for (var j = 0; j < arrDice.Length; j++)
                        {
                            if (arr[i] == j) continue;

                            if (data == arrDice[j].diceData && level == arrDice[j].eyeLevel)
                            {
                                // Upgrade
                                arrDice[j].LevelUp(arrDiceDeck);
                                arrDice[arr[i]].Reset();

                                return;
                            }
                        }
                    }
                }
            }
        }

        private readonly int[] _arrPrice = { 100, 200, 400, 700, 1100 };
        public void AI_UpgradeDice()
        {
            var arr = new int[arrDiceDeck.Length];
            for (var i = 0; i < arr.Length; i++) arr[i] = i;
            ShuffleIntArray(ref arr);

            for (var i = 0; i < arrDiceDeck.Length; i++)
            {
                if (arrUpgradeLevel[arr[i]] < 5 && sp >= _arrPrice[arrUpgradeLevel[arr[i]]])
                {
                    DiceUpgrade(arr[i]);
                    AddSp(-_arrPrice[arrUpgradeLevel[arr[i]] - 1]);
                }
            }
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
    }
}
