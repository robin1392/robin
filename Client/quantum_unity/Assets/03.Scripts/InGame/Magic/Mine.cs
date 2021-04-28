#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ED
{
    public class Mine : Installation
    {
        public ParticleSystem ps_Bomb;

        [Header("AudioClip")] public AudioClip clip_Set;
        public AudioClip clip_Explosion;

        private bool isArrivedAtTargetPosition;
        

        protected float durationToTarget;
        private bool isBombed = false;


        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);

            isCanBeTarget = false;
            image_HealthBar.transform.parent.gameObject.SetActive(false);
            collider.enabled = true;
            animator.gameObject.SetActive(true);
            animator.transform.localScale = Vector3.one * Mathf.Lerp(1f, 1.5f, (eyeLevel - 1) / 5f);
            ps_Bomb.transform.localScale = Vector3.one * Mathf.Pow(1.5f, eyeLevel - 1);

            var distance = Vector3.Distance(startPos, targetPos);
            durationToTarget = distance / moveSpeed;
            isArrivedAtTargetPosition = false;
            isBombed = false;
            collider.enabled = false;
        }

        protected void Update()
        {
            if (ActorProxy == null)
            {
                return;
            }

            var actorTransform = ActorProxy.transform;
            var elapsedTime = this.elapsedTime;
            actorTransform.position = Vector3.Lerp(startPos, targetPos, elapsedTime / durationToTarget);

            if (isArrivedAtTargetPosition == false)
            {
                if (elapsedTime >= durationToTarget)
                {
                    EndMove();
                }
            }
            
            var damageByTime = lifeTimeFactor * (elapsedTime - durationToTarget);
            var currentHealth = ActorProxy.currentHealth - damageByTime; 
            
            if (image_HealthBar != null)
            {
                image_HealthBar.fillAmount = currentHealth / ActorProxy.maxHealth;
            }

            if (currentHealth <= 0 && isBombed == false)
            {
                Bomb();
            }
        }

        private void EndMove()
        {
            isArrivedAtTargetPosition = true;
            image_HealthBar.transform.parent.gameObject.SetActive(true);
            SoundManager.instance.Play(clip_Set);
            animator.SetTrigger(AnimationHash.Set);
            collider.enabled = true;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if ((1 << collision.gameObject.layer) == targetLayer)
            {
                Bomb();
            }
        }

        public void Bomb()
        {
            if (ActorProxy.isPlayingAI == false)
            {
                return;
            }
            
            if (isBombed)
            {
                return;
            }

            isBombed = true;
            ActorProxy.Destroy();
        }

        public override void OnBaseEntityDestroyed()
        {
            if (ActorProxy.isPlayingAI)
            {
                var cols = Physics.OverlapSphere(transform.position, range * Mathf.Pow(1.5f, eyeLevel - 1), targetLayer);
                foreach (var col in cols)
                {
                    var minion = col.GetComponentInParent<Minion>();
                    if(minion != null)
                    {
                        minion.ActorProxy.HitDamage(power);
                    }
                }   
            }

            //ActorProxy가 Destroy 될 때 파괴 이펙트 재생을 위해 ActorProxy 밖으로 빼놓는다.
            transform.SetParent(null);
            StartCoroutine(MineDestroyDelayed());
        }

        IEnumerator MineDestroyDelayed()
        {
            collider.enabled = false;
            image_HealthBar.transform.parent.gameObject.SetActive(false);
            animator.gameObject.SetActive(false);
            SoundManager.instance.Play(clip_Explosion);
            ps_Bomb.Play();

            yield return new WaitForSeconds(2.0f);
            
            _poolObjectAutoDeactivate.Deactive();
        }
    }
}