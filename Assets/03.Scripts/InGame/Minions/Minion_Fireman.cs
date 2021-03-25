using System.Collections;
using System.Collections.Generic;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using UnityEngine;

namespace ED
{
    public class Minion_Fireman : Minion
    {
        public ParticleSystem ps_Fire;
        public Light light;

        [Header("AudioClip")]
        public AudioClip clip_Flame;

        public override void Initialize()
        {
            base.Initialize();
            
            ps_Fire.Stop();
            light.enabled = false;
        }

        public override IEnumerator Attack()
        {
            var fireAction = new FireManAction();
            yield return fireAction.ActionWithSync(ActorProxy, target.ActorProxy);
        }
    }
    
    public class FireManAction : SyncActionWithTarget
    {
        public override IEnumerator Action(ActorProxy actorProxy, ActorProxy targetActorProxy)
        {
            var fireMan = actorProxy.baseStat as Minion_Fireman;
            SoundManager.instance.Play(fireMan.clip_Flame);
            
            fireMan.animator.SetTrigger(AnimationHash.Attack);
            
            actorProxy.transform.LookAt(targetActorProxy.transform);
            
            fireMan.ps_Fire.Play();
            fireMan.light.enabled = true;
            var t = 0f;
            var tick = 0f;
            while (t < 0.95f)
            {
                t += Time.deltaTime;
                
                if (t >= tick)
                {
                    tick += fireMan.attackSpeed;
                    if(actorProxy.isPlayingAI)
                    {
                        var actorTransform = fireMan.transform;
                        var cols = Physics.RaycastAll(actorTransform.position + Vector3.up * 0.1f, actorTransform.forward, fireMan.range,
                            fireMan.targetLayer);
                        foreach (var col in cols)
                        {
                            var bs = col.transform.GetComponentInParent<BaseStat>();
                            if (bs != null && bs.isAlive)
                            {
                                bs.ActorProxy.HitDamage(actorProxy.power);    
                            }
                        }
                    }
                }
                
                yield return null;
            }

            FireEffectOff(actorProxy);
        }

        public override void OnActionCancel(ActorProxy actorProxy)
        {
            FireEffectOff(actorProxy);
        }

        void FireEffectOff(ActorProxy actorProxy)
        {
            var fireMan = actorProxy.baseStat as Minion_Fireman;
            fireMan.ps_Fire.Stop();
            fireMan.light.enabled = false;
        }
    }
}
