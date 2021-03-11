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

        [Header("AudioClip")]
        public AudioClip clip_BabyAttack;

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

            ae.event_Attack += AttackEvent;
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
            originRange = range;
            //KZSee:
            //range = 0.7f;
            StartCoroutine(PolymorphCoroutine());
        }

        public void AttackEvent()
        {
            SoundManager.instance?.Play(clip_BabyAttack);
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

            if (ActorProxy.isPlayingAI)
            {
                ActorProxy.FireBulletWithRelay(E_BulletType.BABYDRAGON, target, power,
                    ani_Baby.gameObject.activeSelf ? bulletMoveSpeedBaby : bulletMoveSpeedDragon);
            }
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
            // range = originRange;
            
            ps_Smoke.Play();
            //KZSee:
            // power = effect;
            // maxHealth = effectDuration + (effectCooltime * ingameUpgradeLevel);
            // currentHealth = maxHealth;
            RefreshHealthBar();
        }
    }
}