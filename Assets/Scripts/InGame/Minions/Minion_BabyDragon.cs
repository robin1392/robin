using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ED
{
    public class Minion_BabyDragon : Minion
    {
        public Animator ani_Baby;
        public Animator ani_Dragon;
        public Transform ts_HitPosBaby;
        public Transform ts_HitPosDragon;
        public Transform ts_HPBarParent;
        public Transform ts_BabyHPBarPoint;
        public Transform ts_DragonHPBarPoint;
        public ParticleSystem ps_Smoke;
        public float polymophCooltime = 20f;

        public float bulletMoveSpeedBaby = 6f;
        public float bulletMoveSpeedDragon = 10f;

        [Header("Prefab")] 
        public GameObject pref_Fireball;

        private float originRange;
        private readonly string strTagGround = "Minion_Ground";
        private readonly string strTagFlying = "Minion_Flying";

        protected override void Awake()
        {
            base.Awake();
            
            PoolManager.instance.AddPool(pref_Fireball, 1);
        }

        protected override void Start()
        {
            base.Start();

            var ae = ani_Dragon.GetComponent<MinionAnimationEvent>();
            ae.event_FireSpear += FireSpear;
        }

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);

            gameObject.tag = strTagGround;
            ani_Baby.gameObject.SetActive(true);
            ani_Dragon.gameObject.SetActive(false);
            animator = ani_Baby;
            ts_HitPos = ts_HitPosBaby;
            ts_HPBarParent.localPosition = ts_BabyHPBarPoint.localPosition;
            agent.baseOffset = 0;
            agent.areaMask = 1 << NavMesh.GetAreaFromName("Walkable");
            originRange = range;
            range = 0.7f;
            StartCoroutine(PolymorphCoroutine());
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

        public void FireSpear()
        {
            if (target == null)
            {
                return;
            }
            else if (IsTargetInnerRange() == false)
            {
                animator.SetTrigger(_animatorHashIdle);
                return;
            }

            if( (InGameManager.IsNetwork && isMine) || InGameManager.IsNetwork == false || controller.isPlayingAI )
            {
                controller.ActionFireBullet(E_BulletType.BABYDRAGON , ts_ShootingPos.position, target.id, power, ani_Baby.gameObject.activeSelf ? bulletMoveSpeedBaby : bulletMoveSpeedDragon);
            }

            /*//if (PhotonNetwork.IsConnected && isMine)
            if( InGameManager.IsNetwork && isMine )
            {
                controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREBULLET, E_BulletType.BABYDRAGON, ts_ShootingPos.position, target.id, power, ani_Baby.gameObject.activeSelf ? bulletMoveSpeedBaby : bulletMoveSpeedDragon);
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIRESPEAR, ts_ShootingPos.position, target.id, power, ani_Baby.gameObject.activeSelf ? bulletMoveSpeedBaby : bulletMoveSpeedDragon);
                controller.ActionFireSpear(ts_ShootingPos.position, target.id, power , ani_Baby.gameObject.activeSelf ? bulletMoveSpeedBaby : bulletMoveSpeedDragon);
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.IsNetwork == false )
            {
                controller?.FireBullet(E_BulletType.BABYDRAGON, ts_ShootingPos.position, target.id, power, ani_Baby.gameObject.activeSelf ? bulletMoveSpeedBaby : bulletMoveSpeedDragon);
            }
            */
            
        }

        IEnumerator PolymorphCoroutine()
        {
            yield return new WaitForSeconds(polymophCooltime);

            gameObject.tag = strTagFlying;
            gameObject.layer =
                LayerMask.NameToLayer(string.Format("{0}Flying", isBottomPlayer ? "BottomPlayer" : "TopPlayer"));
            targetMoveType = DICE_MOVE_TYPE.ALL;
            ani_Baby.gameObject.SetActive(false);
            ani_Dragon.gameObject.SetActive(true);
            animator = ani_Dragon;
            ts_HitPos = ts_HitPosDragon;
            ts_HPBarParent.localPosition = ts_DragonHPBarPoint.localPosition;
            agent.baseOffset = -2f;
            agent.areaMask = 1 << NavMesh.GetAreaFromName("Fly");
            range = originRange;
            
            ps_Smoke.Play();
            power = effect;
            maxHealth = effectDuration + (effectCooltime * upgradeLevel);
            currentHealth = maxHealth;
            RefreshHealthBar();
        }
    }
}