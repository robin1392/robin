#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.AI;

using RandomWarsProtocol;
using RandomWarsProtocol.Msg;
using RandomWarsResource.Data;
using Template.Shop.GameBaseShop.Table;

//
using UnityEngine.Serialization;
using UnityEngine.UI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using TDataDiceInfo = RandomWarsResource.Data.TDataDiceInfo;


#region photon
//using Photon.Pun;
#endregion

namespace ED
{
    public class ReferenceHolder : MonoBehaviour
    {
        public Animator animator;
        public Material[] arrMaterial;
        
        public GameObject objCollider;
        public ParticleSystem ps_ShieldOff;
        public ParticleSystem ps_Destroy;

        [Header("Positions")] 
        public Transform ts_ShootingPos;
        public Transform ts_HitPos;

        [Header("UI Link")]
        public Image image_HealthBar;
        public Text text_Health;
        
        [Header("AudioClip")]
        public AudioClip clip_TowerFalldown;
        public AudioClip clip_TowerExplosion;
    }
}
