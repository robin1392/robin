using System.Collections;
using System.Collections.Generic;
using ED;
using UnityEngine;

namespace ED
{
    public class Shield : MonoBehaviour
    {
        public MeshRenderer mr;
        public Material[] arrMaterials;
        public PoolObjectAutoDeactivate poad;

        public void Initialize(bool isMine)
        {
            mr.material = arrMaterials[isMine ? 0 : 1];
        }

        public void Deactive()
        {
            poad.Deactive();
        }
    }
}
