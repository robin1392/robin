#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using MirageTest.Scripts;
using MirageTest.Scripts.SyncAction;
using UnityEngine;

namespace ED
{
    public class Mortar : FixedPositionInstallation
    {
        [Header("Prefab")] public GameObject pref_Cannonball;

        [Space] public ParticleSystem ps_Fire;
        public Light light_Fire;
        public float shootTime = 0;
        public Transform ts_Head;
        public Animator[] arrAnimator;

        [Header("AudioClip")] public AudioClip clip_Missile;
        public AudioClip clip_Shot;

        private Minion longTarget;


        protected override void Awake()
        {
            base.Awake();

            PoolManager.instance.AddPool(pref_Cannonball, 2);
        }

        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);

            shootTime = 0;

            SetParts();
        }

        private void SetParts()
        {
            for (int i = 0; i < arrAnimator.Length; ++i)
                arrAnimator[i].transform.localScale = Vector3.one * Mathf.Lerp(1f, 1.5f, (eyeLevel - 1) / 5f);
        }

        protected override IEnumerator Cast()
        {
            var t = 0f;
            while (true)
            {
                t += Time.deltaTime;
                if (shootTime + attackSpeed <= Time.time)
                {
                    SetLongTarget();

                    if (longTarget != null)
                    {
                        shootTime = Time.time;
                        var action = new MortarAction();
                        yield return action.ActionWithSync(ActorProxy, longTarget.ActorProxy);
                    }
                }

                yield return null;
            }
        }

        private void SetLongTarget()
        {
            var distance = 0f;
            var cols = Physics.OverlapSphere(transform.position, searchRange, targetLayer);
            longTarget = null;

            foreach (var col in cols)
            {
                var dis = Vector3.SqrMagnitude(transform.position - col.transform.position);
                var m = col.GetComponentInParent<Minion>();
                if (dis > distance && m != null && m.CanBeTarget())
                {
                    distance = dis;
                    longTarget = m;
                }
            }
        }

        public void FireLightOn()
        {
            if (light_Fire != null)
            {
                light_Fire.enabled = true;
            }
        }

        public void FireLightOff()
        {
            if (light_Fire != null)
            {
                light_Fire.enabled = false;
            }
        }
    }

    public class MortarAction : SyncActionWithTarget
    {
        public override IEnumerator Action(ActorProxy actorProxy, ActorProxy targetActorProxy)
        {
            var actorTransform = actorProxy.transform;
            var mortar = actorProxy.baseEntity as Mortar;
            float t = 0f;
            Quaternion q = mortar.ts_Head.rotation;
            var targetTransform = targetActorProxy.transform;
            
            while (t < 0.5f)
            {
                if (targetActorProxy == null || 
                    targetActorProxy.baseEntity == null ||
                    targetActorProxy.baseEntity.CanBeTarget() == false)
                {
                    yield break;
                }
                
                t += Time.deltaTime;
                mortar.ts_Head.rotation = Quaternion.Lerp(q,
                    Quaternion.LookRotation((targetTransform.position - actorTransform.position).normalized),
                    t / 0.5f);
                yield return null;
            }
            
            for (int i = 0; i < mortar.arrAnimator.Length; ++i) mortar.arrAnimator[i].SetTrigger(AnimationHash.Shoot);

            if (mortar.ps_Fire != null)
            {
                mortar.ps_Fire.Play();
            }

            mortar.FireLightOn();

            if (actorProxy.isPlayingAI)
            {
                actorProxy.FireCannonBallWithRelay(E_CannonType.DEFAULT, targetActorProxy.transform.position);
            }

            yield return new WaitForSeconds(0.15f);

            mortar.FireLightOff();
        }

        public override void OnActionCancel(ActorProxy actorProxy)
        {
            var mortar = actorProxy.baseEntity as Mortar;
            mortar.FireLightOff();
        }
    }
}