using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    [System.Serializable]
    public class Dice
    {
        public int diceFieldNum;
        //public Data_Dice data;
        
        //public int id => (data != null && data.id >= 0) ? data.id : -1;
        public int id => (diceData != null && diceData.id >= 0) ? diceData.id : -1;
        public int eyeLevel;


        public DiceInfoData diceData;

        public Sprite GetIcon()
        {
            //return data == null ? null : data.icon;
            return diceData == null ? null : FileHelper.GetIcon(diceData.iconName);
        }

        public void Reset()
        {
            //this.data = null;
            this.eyeLevel = 0;

            this.diceData = null;
        }

        /*public void Set(Data_Dice pData, int pLevel = 0)
        {
            this.data = pData;
            this.level = pLevel;
        }*/

        // new dice
        public void Set(DiceInfoData pData, int pLevel = 0)
        {
            this.diceData = pData;
            this.eyeLevel = pLevel;
        }

        /*public bool LevelUp(Data_Dice[] deck)
        {
            if (level < 5)
            {
                level++;
                var rndNum = Random.Range(0, deck.Length);
                Set(deck[rndNum], level);

                return true;
            }

            return false;
        }*/
        
        public bool LevelUp(DiceInfoData[] deck)
        {
            if (eyeLevel < 5)
            {
                eyeLevel++;
                var rndNum = Random.Range(0, deck.Length);
                Set(deck[rndNum], eyeLevel);

                return true;
            }

            return false;
        }
        
    }
}