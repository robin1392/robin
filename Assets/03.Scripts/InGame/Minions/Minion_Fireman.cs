using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Minion_Fireman : Minion
    {
        public ParticleSystem ps_Fire;
        public Light light;

        public bool isFire;

        [Header("AudioClip")]
        public AudioClip clip_Flame;

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);

            _aiPath.canMove = true;
            ps_Fire.Stop();
            light.enabled = false;
            isFire = false;
        }

        public override void Death()
        {
            base.Death();

            isFire = false;
            light.enabled = false;
            ps_Fire.Stop();
        }

        public override void Sturn(float duration)
        {
            base.Sturn(duration);
            
            isFire = false;
            light.enabled = false;
            ps_Fire.Stop();
        }

        public override IEnumerator Attack()
        {
            //if (PhotonNetwork.IsConnected && isMine)
            if( InGameManager.IsNetwork && (isMine || controller.isPlayingAI) )
            {
                base.Attack();
                
                //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
                
                ActorProxy.PlayAnimationWithRelay(_animatorHashAttack, target);

                //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_FIREMANFIRE , id);
                controller.ActionFireManFire(id);
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false)
            {
                base.Attack();
                animator.SetTrigger(_animatorHashAttack);
                controller.FiremanFire(id);
            }

            yield return null;
        }

        public void MFire(GameObject target)
        {
            if (target != null) transform.LookAt(target.transform);
            animator.SetTrigger("Attack");
            Fire();
        }
        

        public void Fire()
        {
            if (isFire == false)
            {
                StartCoroutine(FireCoroutine());
            }
        }
        
        public override void SetVelocityTarget()
        {
            if (controller.isMinionAgentMove && isFire == false)
            {
                if (target != null && isAlive)
                {
                    Vector3 targetPos = target.transform.position + (target.transform.position - transform.position).normalized * range;
                    _seeker.StartPath(transform.position, targetPos);
                }
            }
        }

        IEnumerator FireCoroutine()
        {
            SoundManager.instance?.Play(clip_Flame);
            
            SetControllEnable(false);
            _aiPath.canMove = false;
            isFire = true;
            ps_Fire.Play();
            light.enabled = true;
            var t = 0f;
            var tick = 0f;
            while (t < 0.95f)
            {
                t += Time.deltaTime;
                
                if (t >= tick)
                {
                    tick += attackSpeed;
                    
                    //if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
                    if(controller.isPlayingAI)
                    {
                        var cols = Physics.RaycastAll(transform.position + Vector3.up * 0.1f, transform.forward, range,
                            targetLayer);
                        foreach (var col in cols)
                        {
                            var bs = col.transform.GetComponentInParent<BaseStat>();

                            DamageToTarget(bs);
                        }
                    }
                    
                }
                
                yield return null;
            }

            ps_Fire.Stop();
            light.enabled = false;
            isFire = false;
            _aiPath.canMove = true;
            SetControllEnable(true);
        }

        public override void EndGameUnit()
        {
            base.EndGameUnit();
            
            ps_Fire.Stop();
        }
    }
}
