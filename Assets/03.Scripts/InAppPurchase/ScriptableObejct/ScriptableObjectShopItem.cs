using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Percent.Platform
{
    public enum BuyType
    {
        gold = 1,
        dia = 2,
        cash = 3,
        free = 4,
        ad = 5
    }

    //리스트로 받게 될 상점 아이템 단위와 매칭되는 정보
    [CreateAssetMenu(fileName = "Item", menuName = "ScriptableObjects/Item", order = 1)]
    public class ScriptableObjectShopItem : ScriptableObject
    {
        public int id;
        public string itemName;
        public BuyType buyType;
        public string shopImage;
        public int buyPrice;
    }
    
}
