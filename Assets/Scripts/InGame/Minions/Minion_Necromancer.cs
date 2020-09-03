#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace ED
{
    public class Minion_Necromancer : Minion
    {
        [Header("Prefab")]
        public GameObject pref_Bullet;
        public GameObject pref_Skeleton;
        [Space]
        public float bulletMoveSpeed = 6f;
        public Transform[] arrSpawnPos;
        //public Data_Dice spawnDiceData;
        public ParticleSystem[] arrPs_Spawn;

        private readonly float _skillCooltime = 10f;
        private float _skillCastedTime;
        private bool _isSkillCasting;

        protected override void Awake()
        {
            base.Awake();
            
            PoolManager.instance.AddPool(pref_Bullet, 1);
        }

        private void Update()
        {
            if (isPlayable && _spawnedTime >= _skillCastedTime + _skillCooltime)
            {
                _skillCastedTime = _spawnedTime;
                Summon();
            }
        }

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);
            _skillCastedTime = -_skillCooltime;
        }

        public override void Attack()
        {
            if (PhotonNetwork.IsConnected && isMine)
            {
                base.Attack();
                controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                base.Attack();
                animator.SetTrigger(_animatorHashAttack);
            }
        }
        
        public void FireArrow()
        {
            if (target == null)
            {
                return;
            }
            else if (IsTargetInnerRange() == false)
            {
                animator.SetTrigger(_animatorHashIdle);
                isAttacking = false;
                SetControllEnable(true);
                return;
            }

            if (PhotonNetwork.IsConnected && isMine)
            {
                controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREBULLET, pref_Bullet.name, ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
            }
            else if (PhotonNetwork.IsConnected == false)
            {
                controller.FireBullet(pref_Bullet.name, ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
            }
        }
        
        public void Skill()
        {
            // if (_spawnedTime >= _skillCastedTime + _skillCooltime)
            // {
            //     _skillCastedTime = _spawnedTime;
            //     //StartCoroutine(SkillCoroutine());
            //     controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_SENDMESSAGEVOID, id, "Summon");
            // }
        }

        public void Summon()
        {
            StartCoroutine(SummonCoroutine());
        }

        IEnumerator SummonCoroutine()
        {
            SetControllEnable(false);
            animator.SetTrigger("Skill");
            _isSkillCasting = true;
            
            yield return new WaitForSeconds(0.4f);

            for (int i = 0; i < arrSpawnPos.Length; i++)
            {
                arrPs_Spawn[i].Play();
                
                // if (PhotonNetwork.IsConnected && isMine)
                // {
                //     //controller.photonView.RPC("SpawnSkeleton", RpcTarget.All, arrSpawnPos[i].position);
                //     controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_SPAWNSKELETON , arrSpawnPos[i].position);
                // }
                // else
                // {
                //     controller.SpawnSkeleton(arrSpawnPos[i].position);
                // }
                
                var m = controller.CreateMinion(pref_Skeleton,
                    arrSpawnPos[i].position, 1, 0);

                m.targetMoveType = DICE_MOVE_TYPE.GROUND;
                m.ChangeLayer(isBottomPlayer);
                m.power = 10f + (6f * upgradeLevel);
                m.maxHealth = 40f + (12f * upgradeLevel);
                m.attackSpeed = 0.8f;
                m.moveSpeed = 1.2f;
                m.range = 0.7f;
                m.eyeLevel = eyeLevel;
                m.upgradeLevel = upgradeLevel;
                m.Initialize(destroyCallback);
            }
            
            yield return new WaitForSeconds(0.3f);
            
            SetControllEnable(true);
            _isSkillCasting = false;
        }
    }
}
