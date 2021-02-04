using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Percent.Platform
{
    public enum ItemType
    {
        gold = 1,
        dia = 2,
        key = 11,
        dice_1000 = 1000,
        dice_1001 = 1001,
        dice_1002 = 1002,
        dice_1003 = 1003,
        dice_1004 = 1004,
        dice_1005 = 1005,
        dice_1006 = 1006,
        dice_1007 = 1007,
        dice_1008 = 1008,
        dice_1009 = 1009,
        dice_1010 = 1010
    }

    //리스트로 받게 될 상점 아이템 단위와 매칭되는 정보
    [CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
    public class ScriptableObjectItem : ScriptableObject
    {
        //ID
        public int id;
        public string itemName;
        public ItemType itemType;
        public string itemIcon;
    }
    
}
