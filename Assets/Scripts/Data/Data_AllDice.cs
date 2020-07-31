using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    [CreateAssetMenu()]
    [System.Serializable]
    public class Data_AllDice : ScriptableObject
    {
        public List<Data_Dice> listDice;
    }
}