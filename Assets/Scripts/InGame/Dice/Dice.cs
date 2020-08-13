using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    [System.Serializable]
    public class Dice
    {
        public int diceFieldNum;
        public Data_Dice data;
        
        public int id => (data != null && data.id >= 0) ? data.id : -1;
        public int level;


        public DiceInfoData diceData;

        public Sprite GetIcon()
        {
            return data == null ? null : data.icon;
        }

        public void Reset()
        {
            this.data = null;
            this.level = 0;
        }

        public void Set(Data_Dice pData, int pLevel = 0)
        {
            this.data = pData;
            this.level = pLevel;
        }

        public bool LevelUp(Data_Dice[] deck)
        {
            if (level < 5)
            {
                level++;
                var rndNum = Random.Range(0, deck.Length);
                Set(deck[rndNum], level);

                return true;
            }

            return false;
        }
    }
}