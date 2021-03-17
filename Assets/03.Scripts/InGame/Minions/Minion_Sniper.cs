using System;
using System.Collections;
using System.Collections.Generic;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using UnityEngine;

namespace ED
{
    public class Minion_Sniper : Minion
    {
        public float bulletMoveSpeed = 6f;
        public ParticleSystem ps_Fire;
        public Light light_Fire;
        public LineRenderer lr;

        [Header("AudioClip")]
        public AudioClip clip_Shot;
        
        protected override void Start()
        {
            base.Start();

            var ae = animator.GetComponent<MinionAnimationEvent>();
            ae.event_FireArrow += FireArrow;
            ae.event_FireLight += FireLightOn;
        }

        public override void Initialize()
        {
            base.Initialize();
            
            lr.gameObject.SetActive(false);
            light_Fire.enabled = false;
        }

        public override IEnumerator Attack()
        {
            if (ActorProxy.isPlayingAI)
            {
                ActorProxy.PlayAnimationWithRelay(AnimationHash.AttackReady, target);
            }

            //yield return new WaitForSeconds(0.5f);
            var aimingAction = new SniperAimingAction();
            RunningAction = aimingAction;
            yield return aimingAction.ActionWithSync(ActorProxy, target.ActorProxy);
            RunningAction = null;

            if (ActorProxy.isPlayingAI)
            {
                if (IsCanAiming())
                {
                    ActorProxy.PlayAnimationWithRelay(AnimationHash.Attack, target);
                    yield return new WaitForSeconds(0.5f);
                }
                else
                {
                    ActorProxy.PlayAnimationWithRelay(AnimationHash.Idle, target);
                }
            }
        }

        public bool IsCanAiming()
        {
            if (target == null || target.isAlive == false) return false;
            if (IsTargetInnerRange() == false) return false;

            return true;
        }

        public override void Death()
        {
            base.Death();

            lr.gameObject.SetActive(false);
            light_Fire.enabled = false;
        }

        public void Aiming()
        {
            StartCoroutine(AimingCoroutine());
        }

        public void StopAiming()
        {
            StopAllCoroutines();
            lr.gameObject.SetActive(false);
            animator.SetTrigger(AnimationHash.Idle);
        }
        
        IEnumerator AimingCoroutine()
        {
            float t = 0;
            while (t < attackSpeed - 1.5f && target != null)
            {
                if (target != null)
                {
                    lr.gameObject.SetActive(true);
                    ActorProxy.transform.LookAt(target.transform);
                    lr.SetPositions(new Vector3[2] {ts_ShootingPos.position, target.ts_HitPos.position});
                }
                else
                {
                    break;
                }

                t += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            
            lr.gameObject.SetActive(false);
        }

        public void FireArrow()
        {
            SoundManager.instance.Play(clip_Shot);
            
            if (target != null)
            {
                if (ActorProxy.isPlayingAI)
                {
                    ActorProxy.FireBulletWithRelay(E_BulletType.ARROW, target, power, bulletMoveSpeed);
                }
            }
        }

        public void FireLightOn()
        {
            lr.gameObject.SetActive(false);
            if (target == null || IsTargetInnerRange() == false)
            {
                return;
            }

            if (ps_Fire != null)
            {
                ps_Fire.Play();
            }

            if (light_Fire != null)
            {
                light_Fire.enabled = true;
                Invoke("FireLightOff", 0.15f);
            }
        }

        private void FireLightOff()
        {
            if (light_Fire != null)
            {
                light_Fire.enabled = false;
            }
        }

        public override void EndGameUnit()
        {
            base.EndGameUnit();
            StopAiming();
        }
    }
    
    public class SniperAimingAction : SyncActionWithTarget
    {
        public override IEnumerator Action(ActorProxy actorProxy, ActorProxy targetActorProxy)
        {
            var sniper = actorProxy.baseStat as Minion_Sniper;

            float t = 0f;
            sniper.lr.gameObject.SetActive(true);
            while (t < 4f)
            {
                if (sniper.IsCanAiming() == false)
                {
                    break;
                }
                
                actorProxy.transform.LookAt(targetActorProxy.transform);
                sniper.lr.SetPositions(new Vector3[] { sniper.ts_ShootingPos.position, targetActorProxy.baseStat.ts_HitPos.position });

                t += Time.deltaTime;
                yield return null;
            }
            
            sniper.lr.gameObject.SetActive(false);
        }

        public override void OnActionCancel(ActorProxy actorProxy)
        {
            base.OnActionCancel(actorProxy);
            var sniper = actorProxy.baseStat as Minion_Sniper;
            sniper.lr.gameObject.SetActive(false);
        }
    }
}
