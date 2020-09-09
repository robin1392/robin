using System.Collections.Generic;
using UnityEngine;
using System;

namespace ED
{
    [Serializable]
    public class Pool
    {
        public GameObject obj;
        public int count;
    }

    [CreateAssetMenu]
    public class PoolData : ScriptableObject {

        public List<Pool> listPool = new List<Pool>();

    }
}