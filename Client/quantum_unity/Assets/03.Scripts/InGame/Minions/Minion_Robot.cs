#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using Mirage;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;

namespace ED
{
    public class Minion_Robot : Minion
    {
        public Transform[] arrTs_Parts;
        public int pieceID;
        
        public override void Initialize()
        {
            base.Initialize();

            collider.enabled = false;
            var rwClient = ActorProxy.Client as RWNetworkClient;
            var robots = rwClient.ActorProxies.Where(actor => actor.dataId == ActorProxy.dataId && actor.team == ActorProxy.team);
            pieceID = robots.Count();
            
            SetParts();
        }
        
        protected override IEnumerator Root()
        {
            yield break;
        }

        public void SetParts()
        {
            if (pieceID < 4)
            {
                for (int i = 0; i < arrTs_Parts.Length; i++)
                {
                    arrTs_Parts[i].gameObject.SetActive(i == pieceID);
                }
            }
            else
            {
                int rnd = Random.Range(0, arrTs_Parts.Length);
                for (int i = 0; i < arrTs_Parts.Length; i++)
                {
                    arrTs_Parts[i].gameObject.SetActive(i == rnd);
                }
            }
        }
    }
}