using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Mirage;
using MirageTest.Scripts;
using MirageTest.Scripts.Messages;
using UnityEngine;

namespace ED
{
    public class Minion_Zombie : Minion
    {
        [Header("AudioClip")]
        public AudioClip clip_Blade;
        public AudioClip clip_Poison;
        
        protected override void Start()
        {
            base.Start();

            _animationEvent.event_Attack += AttackEvent;
        }

        public void AttackEvent()
        {
            SoundManager.instance.Play(clip_Blade);
        }

        // public override void Death()
        // {
        //     if (_reviveCount > 0)
        //     {
        //         StartCoroutine(ReviveCoroutine());
        //         
        //         SoundManager.instance.Play(Global.E_SOUND.SFX_MINION_DEATH);
        //     }
        //     else
        //     {
        //         base.Death();
        //     }
        // }
        
        // public override void SetColor(E_MaterialType type, bool isAlly)
        // {
        //     var mat = arrMaterial[isAlly ? 0 : 1];
        //
        //     arrMeshRenderer = GetComponentsInChildren<MeshRenderer>();
        //     
        //     foreach (var m in arrMeshRenderer)
        //     {
        //         m.material = mat;
        //         switch (type)
        //         {
        //             case E_MaterialType.BOTTOM:
        //             case E_MaterialType.TOP:
        //                 Color c = m.material.color;
        //                 c.a = 1f;
        //                 m.material.color = c;
        //                 break;
        //             case E_MaterialType.HALFTRANSPARENT:
        //                 c = m.material.color;
        //                 c.a = 0.2f;
        //                 m.material.color = c;
        //                 break;
        //             case E_MaterialType.TRANSPARENT:
        //                 c = m.material.color;
        //                 c.a = 0.1f;
        //                 m.material.color = c;
        //                 break;
        //         }
        //     }
        //
        //     arrSkinnedMeshRenderer = GetComponentsInChildren<SkinnedMeshRenderer>();
        //
        //     foreach (var m in arrSkinnedMeshRenderer)
        //     {
        //         m.material = mat;
        //         switch (type)
        //         {
        //             case E_MaterialType.BOTTOM:
        //             case E_MaterialType.TOP:
        //                 Color c = m.material.color;
        //                 c.a = 1f;
        //                 m.material.color = c;
        //                 break;
        //             case E_MaterialType.HALFTRANSPARENT:
        //                 c = m.material.color;
        //                 c.a = 0.2f;
        //                 m.material.color = c;
        //                 break;
        //             case E_MaterialType.TRANSPARENT:
        //                 c = m.material.color;
        //                 c.a = 0.1f;
        //                 m.material.color = c;
        //                 break;
        //         }
        //     }
        // }

        public override void OnBaseStatDestroyed()
        {
            var rwClient = ActorProxy.Client as RWNetworkClient;
            // 독구름
            rwClient.Send(new CreateActorMessage()
            {
                diceId = 3014,
                ownerTag = ActorProxy.ownerTag,
                team = ActorProxy.team,
                inGameLevel = ActorProxy.ingameUpgradeLevel,
                outGameLevel = ActorProxy.outgameUpgradeLevel,
                positions = new Vector3[] { transform.position },
                delay = 0f,
            });
            
            // 부활
            rwClient.Send(new CreateActorMessage()
            {
                diceId = 3015,
                ownerTag = ActorProxy.ownerTag,
                team = ActorProxy.team,
                inGameLevel = ActorProxy.ingameUpgradeLevel,
                outGameLevel = ActorProxy.outgameUpgradeLevel,
                positions = new Vector3[] { transform.position },
                delay = 2f,
            });
            
            base.OnBaseStatDestroyed();
        }

        // IEnumerator ReviveCoroutine()
        // {
        //     collider.enabled = false;
        //     animator.gameObject.SetActive(false);
        //
        //     StartCoroutine(PoisonCoroutine(2f));
        //     yield return new WaitForSeconds(2f);
        //
        //     //KZSee:
        //     // currentHealth = (eyeLevel * 15) * 0.01f * maxHealth;
        //     RefreshHealthBar();
        //     SetColor(isBottomCamp ? E_MaterialType.BOTTOM : E_MaterialType.TOP, ActorProxy.IsLocalPlayerAlly());
        //     collider.enabled = true;
        // }

        // IEnumerator PoisonCoroutine(float duration)
        // {
        //     SoundManager.instance.Play(clip_Poison);
        //     PoolManager.instance.ActivateObject("Effect_Poison", transform.position);
        //     float t = 0;
        //     float tick = 0.1f;
        //     
        //     while (t < duration)
        //     {
        //         var cols = Physics.OverlapSphere(transform.position, 1f, targetLayer);
        //         foreach (var col in cols)
        //         {
        //             var bs = col.GetComponentInParent<BaseStat>();
        //             if (bs != null && bs.id > 0 && bs.isFlying == false && bs.isAlive)
        //             {
        //                 bs.ActorProxy.HitDamage(effect);
        //             }
        //         }
        //         
        //         t += tick;
        //         yield return new WaitForSeconds(tick);
        //     }
        // }
    }
}