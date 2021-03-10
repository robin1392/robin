using System;
using System.Collections;
using System.Collections.Generic;
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
        
        private static readonly int AttackReady = Animator.StringToHash("AttackReady");

        protected override void Start()
        {
            base.Start();

            var ae = animator.GetComponent<MinionAnimationEvent>();
            ae.event_FireArrow += FireArrow;
            ae.event_FireLight += FireLightOn;
        }

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);
            
            lr.gameObject.SetActive(false);
            light_Fire.enabled = false;
        }

        public override IEnumerator Attack()
        {
            ActorProxy.PlayAnimationWithRelay(_animatorHashAttackReady , target);

            yield return new WaitForSeconds(0.5f);

            isPushing = false;
            float t = 0;
            if (CanAttackTarget())
            {
                yield break;
            }
            
            lr.gameObject.SetActive(true);
            while (t < attackSpeed - 1.5f)
            {
                
                
                var m = target as Minion;
                target = SetTarget();
                if (target == null || target.isAlive == false || (target.isAlive && m != null && !m.isCloacking))
                {
                    

                    if (target != null)
                    {
                        //controller.SendPlayer(RpcTarget.Others, E_PTDefine.PT_SETMINIONTARGET, id, target.id);
                        controller.ActionMinionTarget(id, target.id);
                    }
                    else if (target == null || IsTargetInnerRange() == false)
                    {
                        //controller.SendPlayer(RpcTarget.Others, E_PTDefine.PT_SENDMESSAGEVOID, id, "StopAiming");
                        controller.ActionSendMsg(id, "StopAiming");
                        break;
                    }
                }

                if (target != null && IsTargetInnerRange())
                {
                    transform.LookAt(target.transform);
                    lr.SetPositions(new Vector3[2] {ts_ShootingPos.position, target.ts_HitPos.position});
                }
                else if (target == null || IsTargetInnerRange() == false)
                {

                    //controller.SendPlayer(RpcTarget.Others, E_PTDefine.PT_SENDMESSAGEVOID, id, "StopAiming");
                    controller.ActionSendMsg(id, "StopAiming");
                    yield break;
                }

                t += Time.deltaTime;
                yield return null;
            }
            
            lr.gameObject.SetActive(false);

            if (ActorProxy.isPlayingAI)
            {
                ActorProxy.PlayAnimationWithRelay(_animatorHashAttack, target);
            }
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
            SetControllEnable(true);
            lr.gameObject.SetActive(false);
            animator.SetTrigger(_animatorHashIdle);
        }
        
        IEnumerator AimingCoroutine()
        {
            float t = 0;
            while (t < attackSpeed - 1.5f && target != null)
            {
                if (target != null)
                {
                    lr.gameObject.SetActive(true);
                    transform.LookAt(target.transform);
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
            SoundManager.instance?.Play(clip_Shot);
            
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


            /*if (target != null)
            {
                //if (PhotonNetwork.IsConnected && isMine)
                if( InGameManager.IsNetwork && isMine )
                {
                    //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREARROW, ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
                    controller.ActionFireArrow(ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
                }
                //else if (PhotonNetwork.IsConnected == false)
                else if(InGameManager.IsNetwork == false)
                {
                    controller.FireArrow(ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
                }
            }
            */
            

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
}
