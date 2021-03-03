#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  ED
{
    public class Minion_Magician : Minion
    {
        public float bulletMoveSpeed = 6f;
        public Transform ts_SkillParticlePosition;

        [Header("Effect prefab")] 
        public GameObject pref_Bullet;
        public GameObject pref_SkillEffect;
        public GameObject pref_Scarecrow;
        
        //[SerializeField] private readonly float _skillCooltime = 10f;
        private int _skillCastedCount;

        protected override void Start()
        {
            base.Start();

            var ae = animator.GetComponent<MinionAnimationEvent>();
            ae.event_FireArrow += FireArrow;
        }

        protected override void Awake()
        {
            base.Awake();
            
            PoolManager.instance.AddPool(pref_Bullet, 1);
            PoolManager.instance.AddPool(pref_SkillEffect, 1);
            PoolManager.instance.AddPool(pref_Scarecrow, 1);
        }

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);
            _skillCastedCount = 0;
        }

        public override void Attack()
        {
            if (target == null || target.isAlive == false || IsTargetInnerRange() == false) return;
            
            //if (PhotonNetwork.IsConnected && isMine)
            if( InGameManager.IsNetwork && (isMine || controller.isPlayingAI) )
            {
                base.Attack();
                //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
                controller.MinionAniTrigger(id, "Attack", target.id);
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false)
            {
                base.Attack();
                animator.SetTrigger(_animatorHashAttack);
            }
        }

        public void FireArrow()
        {

            if (target == null || target.isAlive == false || IsTargetInnerRange() == false)
            {
                animator.SetTrigger(_animatorHashIdle);
                isAttacking = false;
                SetControllEnable(true);
                return;
            }
            
            if( (InGameManager.IsNetwork && isMine) || InGameManager.IsNetwork == false || controller.isPlayingAI )
            {
                controller.ActionFireBullet(E_BulletType.MAGICIAN , id, target.id, power, bulletMoveSpeed);
            }
            
            /*//if (PhotonNetwork.IsConnected && isMine)
            if( InGameManager.IsNetwork && isMine )
            {
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREBULLET, E_BulletType.MAGICIAN, ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
                //controller.ActionFireArrow(ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
                controller.ActionFireBullet(E_BulletType.MAGICIAN , ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false )
            {
                controller.FireBullet(E_BulletType.MAGICIAN, ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
            }*/
            
        }

        public void Skill()
        {
            if (_spawnedTime >= effectCooltime * _skillCastedCount)
            {
                Polymorph();
            }
        }

        private void Polymorph()
        {
            var cols = Physics.OverlapSphere(transform.position, searchRange, targetLayer);
            var list = new List<uint>();

            foreach (var col in cols)
            {
                var bs = col.GetComponentInParent<BaseStat>();
                if (bs != null && bs.id > 0 && bs.isFlying == false)
                {
                    var m = bs as Minion;
                    if (m != null && m.isPolymorph == false)
                    {
                        list.Add(bs.id);
                    }
                }
            }

            if (list.Count > 0)
            {
                _skillCastedCount++;

                PoolManager.instance.ActivateObject(pref_SkillEffect.name, ts_SkillParticlePosition.position);

                uint targetId = list[Random.Range(0, list.Count)];
                
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONANITRIGGER, id, "Skill");
                controller.MinionAniTrigger(id, "Skill", targetId);
                
                //controller.targetPlayer.SendPlayer(RpcTarget.All, E_PTDefine.PT_SCARECROW, list[Random.Range(0, list.Count)], effect);
                controller.ActionMinionScareCrow(true, targetId, (float) eyeLevel);

            }
        }
    }
}
