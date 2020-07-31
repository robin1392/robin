using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public enum DICE_CAST_TYPE
    {
        MINION,
        MAGIC,
        INSTALLATION,
        HERO,
    }

    public enum DICE_GRADE
    {
        NORMAL,
        MAGIC,
        HEROIC,
        LEGENDARY,
    }

    public enum DICE_MOVE_TYPE
    {
        GROUND,
        FLYING,
        ALL,
    }
    
    [CreateAssetMenu()]
    [System.Serializable]
    public class Data_Dice : ScriptableObject
    {
        public int id;
        public DICE_GRADE grade;
        public DICE_CAST_TYPE castType;
        public DICE_MOVE_TYPE moveType;
        public DICE_MOVE_TYPE targetMoveType;
        public GameObject prefab;
        public int spawnMultiply = 1;
        public Sprite icon;
        public Sprite card;
        public Color color;

        [Header("Stat")]
        public float power;
        public float powerUpByUpgrade;
        public float powerUpByInGameUp;
        public float maxHealth;
        public float maxHealthUpByUpgrade;
        public float maxHealthUpByInGameUp;
        public float effect;
        public float effectUpByUpgrade;
        public float effectUpByInGameUp;
        public float attackSpeed;
        public float moveSpeed;
        public float range;
    }
}