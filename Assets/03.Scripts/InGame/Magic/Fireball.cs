#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using RandomWarsProtocol;

namespace ED
{
    public class Fireball : Magic
    {
        public Light light;
        public ParticleSystem ps_Tail;
        public ParticleSystem ps_BombEffect;

        [Header("AudioClip")]
        public AudioClip clip_Fire;
        public AudioClip clip_Explosion;
        
        private bool isBombed = false;

        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);
            
            isBombed = false;
            ps_Tail.transform.localScale = Vector3.one * Mathf.Lerp(1f, 3f, (eyeLevel - 1) / 5f);
            ps_BombEffect.transform.localScale = Vector3.one * Mathf.Lerp(0.7f, 1f, (eyeLevel - 1) / 5f);
            
            ps_Tail.Clear();
            ps_BombEffect.Clear();

            SoundManager.instance?.Play(Global.E_SOUND.SFX_FIREBALL_FIRE);
        }

        protected override IEnumerator Move()
        {
            SoundManager.instance?.Play(clip_Fire);
            
            light.enabled = true;
            var startPos = transform.position;
            while (target == null) { yield return null; }
            var endPos = target.ts_HitPos.position;
            var distance = Vector3.Distance(startPos, endPos);
            var moveTime = distance / moveSpeed;

            float t = 0;
            
            while (t < moveTime)
            {
                if (target != null && target.isAlive)
                {
                    endPos = target.ts_HitPos.position;
                }
                rb.position = Vector3.Lerp(startPos, endPos, t / moveTime);

                t += Time.deltaTime;
                yield return null;
            }

            if ((InGameManager.IsNetwork && isMine || InGameManager.IsNetwork == false || controller.isPlayingAI) && isBombed == false)
            {
                isBombed = true;
                
                //if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount > 1 && isMine)
                if(InGameManager.IsNetwork && (isMine || controller.isPlayingAI))
                {
                    SplashDamage();
                    //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREBALLBOMB, id);
                    controller.ActionFireBallBomb(id);
                }
                //else if (PhotonNetwork.IsConnected == false)
                else if(InGameManager.IsNetwork == false)
                {
                    SplashDamage();
                    Bomb();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (InGameManager.Get().isGamePlaying == false || destroyRoutine != null || isBombed) return;

            if ((InGameManager.IsNetwork && isMine || InGameManager.IsNetwork == false || controller.isPlayingAI) &&
                target != null && other.gameObject == target.gameObject ||
                other.gameObject.layer == LayerMask.NameToLayer("Map"))
            {
                rb.velocity = Vector3.zero;
                isBombed = true;

                //if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount > 1 && isMine)
                if(InGameManager.IsNetwork && (isMine || controller.isPlayingAI) )
                {
                    SplashDamage();
                    //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_FIREBALLBOMB ,id);
                    controller.ActionFireBallBomb(id);
                }
                //else if (PhotonNetwork.IsConnected == false)
                else if(InGameManager.IsNetwork == false)
                {
                    SplashDamage();
                    Bomb();
                }
            }
        }

        private void SplashDamage()
        {
            Vector3 pos = transform.position;
            pos.y = 0;
            var cols = Physics.OverlapSphere(pos, range * Mathf.Pow(1.5f, eyeLevel - 1), targetLayer);
            foreach (var col in cols)
            {
                //controller.targetPlayer.SendPlayer(RpcTarget.Others , E_PTDefine.PT_HITMINIONANDMAGIC , col.GetComponentInParent<BaseStat>().id, power, 0f);
                var bs = col.GetComponentInParent<BaseStat>();
                if (bs != null)
                {
                    controller.AttackEnemyMinionOrMagic(bs.UID, bs.id, power, 0f);
                }
                //controller.HitMinionDamage( true , col.GetComponentInParent<BaseStat>().id , power, 0f);
            }
        }

        public void Bomb()
        {
            light.enabled = false;
            rb.velocity = Vector3.zero;
            ps_Tail.Stop();
            ps_BombEffect.Play();
            
            SoundManager.instance?.Play(Global.E_SOUND.SFX_INGAME_COMMON_EXPLOSION);

            Destroy(1.1f);
        }
    }
}