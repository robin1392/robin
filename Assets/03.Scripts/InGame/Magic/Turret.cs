#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Turret : FixedPositionInstallation
    {
        public ParticleSystem ps_Fire;
        public Light light_Fire;

        public Transform ts_Head;
        public Transform ts_ShootPoint;
        //public float lifeTime = 20f;
        public BaseEntity flyingTarget;
        public float bulletMoveSpeed = 6f;
        public float shootTime = 0;

        //public Transform[] arrTs_Parts;
        public GameObject pref_Bullet;

        [Header("AudioClip")]
        public AudioClip clip_Fire;

        protected override void Start()
        {
            base.Start();

            var ae = animator.GetComponent<MinionAnimationEvent>();
            ae.event_FireArrow -= FireArrow;
            ae.event_FireLight -= FireLightOn;
            ae.event_FireArrow += FireArrow;
            ae.event_FireLight += FireLightOn;
            PoolManager.instance.AddPool(pref_Bullet, 1);
        }

        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);
            
            shootTime = 0;
            
            SetParts();
        }

        private void SetParts()
        {
            animator.transform.localScale = Vector3.one * Mathf.Lerp(1f, 1.5f, (eyeLevel - 1) / 5f);
        }
        
        protected override IEnumerator Cast()
        {
            return base.Cast();
        }

        private IEnumerator AttackCoroutine()
        {
            var t = 0f;
            var lifeTime = magicLifeTime;

            while (t < lifeTime)
            {
                t += Time.deltaTime;
                if (shootTime + attackSpeed <= Time.time)
                {
                    SetFlyingTarget();
                }
                
                yield return null;
            }
            
            //Destroy();
        }

        public void FireArrow()
        {
            if (ActorProxy.isPlayingAI)
            {
                ActorProxy.FireBulletWithRelay(E_BulletType.TURRET_BULLET, flyingTarget, power, bulletMoveSpeed);
            }
        }

        private void SetFlyingTarget()
        {
            var distance = float.MaxValue;
            var cols = Physics.OverlapSphere(transform.position, range, targetLayer);
            //Collider colTarget = null;

            foreach (var col in cols)
            {
                var dis = Vector3.SqrMagnitude(transform.position - col.transform.position);
                var m = col.GetComponentInParent<Minion>();
                if (dis < distance && m != null && m.isCloacking == false)
                {
                    distance = dis;
                    //colTarget = col;
                    flyingTarget = m;
                }
            }

            if (flyingTarget != null)
            {
                shootTime = Time.time;
                //flyingTarget = colTarget.GetComponentInParent<Minion>();
                
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_SENDMESSAGEPARAM1, id, "LookAndAniTrigger", flyingTarget.id);
                // controller.ActionSendMsg(id, "LookAndAniTrigger", flyingTarget.id);
            }
        }

        public void LookAndAniTrigger(uint targetId)
        {
            flyingTarget = ActorProxy.GetBaseStatWithNetId(targetId);
            if (flyingTarget)
            {
                StartCoroutine(LookAtTargetCoroutine());
            }
        }

        private IEnumerator LookAtTargetCoroutine()
        {
            float t = 0;
            animator.SetBool("Walk", true);
            while (t < 0.5f)
            {
                ts_Head.rotation = Quaternion.RotateTowards(ts_Head.rotation,
                    Quaternion.LookRotation((flyingTarget.transform.position - ts_Head.position).normalized),
                    Time.deltaTime * 540f);
                t += Time.deltaTime;
                yield return null;
            }
            animator.SetBool("Walk", false);
            animator.SetTrigger("Attack");
        }

        public void FireLightOn()
        {
            if (target == null)
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
            
            SoundManager.instance.Play(clip_Fire);
        }
        
        private void FireLightOff()
        {
            if (light_Fire != null)
            {
                light_Fire.enabled = false;
            }
        }
    }
}
