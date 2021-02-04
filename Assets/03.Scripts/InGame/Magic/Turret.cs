#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Turret : Magic
    {
        public ParticleSystem ps_Fire;
        public Light light_Fire;

        public Transform ts_Head;
        public Transform ts_ShootPoint;
        //public float lifeTime = 20f;
        public Minion flyingTarget;
        public float bulletMoveSpeed = 6f;
        public float shootTime = 0;

        public Transform[] arrTs_Parts;

        [Header("AudioClip")]
        public AudioClip clip_Fire;

        protected override void Start()
        {
            base.Start();

            var ae = animator.GetComponent<MinionAnimationEvent>();
            ae.event_FireArrow += FireArrow;
            ae.event_FireLight += FireLightOn;
        }

        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);

            transform.position = controller.transform.parent.GetChild(diceFieldNum).position;
            shootTime = 0;
            SetColor();

            //if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
            if( (InGameManager.IsNetwork && isMine) || InGameManager.IsNetwork == false || controller.isPlayingAI)
            {
                StartCoroutine(AttackCoroutine());
            }
            
            SetParts();
        }

        private void SetParts()
        {
            for (int i = 0; i < arrTs_Parts.Length; i++)
            {
                arrTs_Parts[i].localScale = i + 1 < eyeLevel ? Vector3.one : Vector3.zero;
            }

            animator.transform.localScale = Vector3.one * Mathf.Lerp(1f, 1.5f, (eyeLevel - 1) / 5f);
            ts_Head.rotation = isBottomPlayer ? Quaternion.identity : Quaternion.Euler(0, 180f, 0);
        }

        private IEnumerator AttackCoroutine()
        {
            var t = 0f;
            var lifeTime = InGameManager.Get().spawnTime;

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
            if ((InGameManager.IsNetwork && isMine) || InGameManager.IsNetwork == false || controller.isPlayingAI)
            {
                controller.ActionFireBullet(E_BulletType.ARROW , id, flyingTarget.id, power, bulletMoveSpeed);
            }
            
            /*//if (PhotonNetwork.IsConnected && isMine)
            if(InGameManager.IsNetwork && isMine)
            {
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREBULLET, E_BulletType.ARROW, ts_ShootPoint.position, flyingTarget.id, power, bulletMoveSpeed);
                //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_FIREARROW , ts_ShootPoint.position, flyingTarget.id, power, bulletMoveSpeed);
                controller.ActionFireArrow(ts_ShootPoint.position, flyingTarget.id, power, bulletMoveSpeed);
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false)
            {
                controller.FireBullet(E_BulletType.ARROW, ts_ShootPoint.position, flyingTarget.id, power, bulletMoveSpeed);
            }*/
        }

        private void SetFlyingTarget()
        {
            var distance = float.MaxValue;
            var cols = Physics.OverlapSphere(transform.position, range, targetLayer);
            //Collider colTarget = null;

            foreach (var col in cols)
            {
                if (col.CompareTag("Player")) continue;
                
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
                controller.ActionSendMsg(id, "LookAndAniTrigger", flyingTarget.id);
            }
        }

        public void LookAndAniTrigger(int targetID)
        {
            flyingTarget = InGameManager.Get().GetBaseStatFromId(targetID) as Minion;
            if (flyingTarget)
            {
                StartCoroutine(LookAtTargetCoroutine());
            }
        }

        private IEnumerator LookAtTargetCoroutine()
        {
            float t = 0;
            while (t < 0.5f)
            {
                ts_Head.rotation = Quaternion.RotateTowards(ts_Head.rotation,
                    Quaternion.LookRotation((flyingTarget.transform.position - ts_Head.position).normalized),
                    Time.deltaTime * 540f);
                t += Time.deltaTime;
                yield return null;
            }
            animator.SetTrigger("Attack");
        }

        public void EndGameUnit()
        {
            StopAllCoroutines();
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
