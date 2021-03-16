using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using Mirage;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using UnityEngine;

namespace ED
{
    public class Minion_Support : Minion
    {
        public GameObject pref_Dust;

        [Header("AudioClip")] public AudioClip clip_Jump;
        public AudioClip clip_Landing;

        public override void Initialize()
        {
            base.Initialize();

            PoolManager.instance.AddPool(pref_Dust, 1);
        }

        protected override IEnumerator Root()
        {
            var targetMinion = GetLongDistanceFriendlyTarget();
            var action = new JumpAction();
            yield return action.ActionWithSync(ActorProxy, targetMinion.ActorProxy);

            while (isAlive)
            {
                target = SetTarget();
                if (target != null)
                {
                    yield return Combat();
                }

                yield return _waitForSeconds0_1;
            }
        }

        private BaseStat GetLongDistanceFriendlyTarget()
        {
            BaseStat rtn = null;

            var distance = isBottomCamp ? float.MinValue : float.MaxValue;

            var rwClient = ActorProxy.Client as RWNetworkClient;
            var enemies = rwClient.ActorProxies.Where(actor =>
            {
                if (actor.team != ActorProxy.team)
                {
                    return false;
                }

                if (actor.actorType == ActorType.Tower)
                {
                    return false;
                }

                if (actor.baseStat is Magic)
                {
                    return false;
                }

                if (actor == ActorProxy)
                {
                    return false;
                }

                return true;
            });

            var actorProxies = enemies as ActorProxy[] ?? enemies.ToArray();




            foreach (var minion in actorProxies)
            {
                if (((isBottomCamp && minion.transform.position.z > distance) ||
                     (isBottomCamp == false && minion.transform.position.z < distance)))
                {
                    distance = minion.transform.position.z;
                    rtn = minion.baseStat;
                }
            }

            return rtn;
        }
    }

    public class JumpAction : SyncActionWithTarget
    {
        public override IEnumerator Action(ActorProxy actorProxy, ActorProxy targetProxy)
        {
            var m = actorProxy.baseStat as Minion_Support;
            var ts = actorProxy.transform;

            if (targetProxy == null || targetProxy.baseStat.isAlive == false)
            {
                m.collider.enabled = true;
                yield break;
            }

            m.collider.enabled = false;
            ts.LookAt(targetProxy.transform);
            
            yield return null;
            
            SoundManager.instance.Play(m.clip_Jump);
            
            actorProxy.PlayAnimationWithRelay(AnimationHash.Skill, targetProxy.baseStat);

            var startPos = ts.position;
            var targetPos = targetProxy.transform.position;
            var t = 0f;

            float fV_x;
            float fV_y;
            float fV_z;

            float fg;
            float fEndTime;
            float fMaxHeight = 2f;
            float fHeight;
            float fEndHeight;
            float fTime = 0f;
            float fMaxTime = 0.75f;

            fEndHeight = targetPos.y - startPos.y;
            fHeight = fMaxHeight - startPos.y;
            fg = 2 * fHeight / (fMaxTime * fMaxTime);
            fV_y = Mathf.Sqrt(2 * fg * fHeight);

            float a = fg;
            float b = -2 * fV_y;
            float c = 2 * fEndHeight;

            fEndTime = (-b + Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);

            fV_x = -(startPos.x - targetPos.x) / fEndTime;
            fV_z = -(startPos.z - targetPos.z) / fEndTime;

            var currentPos = new Vector3();
            while (t < fEndTime)
            {
                t += Time.deltaTime;

                currentPos.x = startPos.x + fV_x * t;
                currentPos.y = startPos.y + (fV_y * t) - (0.5f * fg * t * t);
                currentPos.z = startPos.z + fV_z * t;

                ts.position = currentPos;
                
                yield return null;
            }

            m.collider.enabled = true;
            var pos = ts.position;
            pos.y = 0.1f;
            
            SoundManager.instance.Play(m.clip_Landing);
            PoolManager.Get().ActivateObject(m.pref_Dust.name, pos);
            yield return null;
        }
    }
}
