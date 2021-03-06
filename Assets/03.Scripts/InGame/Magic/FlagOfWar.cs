#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class FlagOfWar : Magic
    {
        public ParticleSystem ps;
        public ParticleSystem ps_NotIsMine;

        [Header("AudioClip")]
        public AudioClip clip_Summon;

        private bool _isTriggerOn;
        private List<uint> _listAttackSpeedUp = new List<uint>();
        
        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);

            collider.enabled = false;
            _hitCollider.enabled = false;
            _listAttackSpeedUp.Clear();
            //KZSee:
            // SetColor();
            if (isMine) ps.Play(); else ps_NotIsMine.Play();
            
            animator.transform.localScale = Vector3.one * Mathf.Lerp(1f, 1.5f, (eyeLevel - 1) / 5f);
            ps.transform.parent.localScale = animator.transform.localScale;
            ((SphereCollider)collider).radius = Mathf.Lerp(1.5f, 2.25f, (eyeLevel - 1) / 5f);
        }

        //KZSee:
        // protected override IEnumerator Activate()
        // {
        //     SoundManager.instance.Play(clip_Summon);
        //     var startPos = transform.position;
        //     var endPos = targetPos;
        //     var distance = Vector3.Distance(startPos, endPos);
        //     var max = distance / moveSpeed;
        //
        //     throw new NotImplementedException("정해진 위치로 이동시킨다.");
        //     // transform.velocity = (endPos - startPos).normalized * moveSpeed;
        //     yield return new WaitForSeconds(max);
        //     // rb.velocity = Vector3.zero;
        //     transform.position = endPos;
        //
        //     EndMove();
        // }

        private void EndMove()
        {
            _isTriggerOn = true;
            collider.enabled = true;
            _hitCollider.enabled = true;
            StartCoroutine(LifetimeCoroutine());
        }

        private IEnumerator LifetimeCoroutine()
        {
            yield return new WaitForSeconds(magicLifeTime - 1.5f);

            _isTriggerOn = false;

            foreach (var baseStatId in _listAttackSpeedUp)
            {
                //if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
                if(ActorProxy.isPlayingAI)
                {
                    //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONFOGOFWAR ,baseStatId, false, effect);
                    // controller.ActionFlagOfWar(baseStatId, false, effect);
                }
            }

            ps.Stop();
            ps_NotIsMine.Stop();
            yield return new WaitForSeconds(1.5f);
            
            // Destroy();
        }

        private void OnTriggerEnter(Collider collision)
        {
            //if (_isTriggerOn && ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false) && IsFriendlyLayer(collision.gameObject))
            if (_isTriggerOn && ActorProxy.isPlayingAI && IsFriendlyLayer(collision.gameObject))
            {
                var m = collision.GetComponentInParent<Minion>();

                if (m == null) return;

                if (_listAttackSpeedUp.Contains(m.id) == false)
                {
                    _listAttackSpeedUp.Add(m.id);
                    
                    //if ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false)
                    if( ActorProxy.isPlayingAI)
                    {
                        //KZSee:
                        // controller.ActionFlagOfWar(m.id, true, effect);
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            
            //if (_isTriggerOn && ((PhotonNetwork.IsConnected && isMine) || PhotonNetwork.IsConnected == false) && IsFriendlyLayer(other.gameObject))
            if (ActorProxy.isPlayingAI && IsFriendlyLayer(other.gameObject))
            {
                var m = other.GetComponentInParent<Minion>();

                if (m == null) return;

                if (_listAttackSpeedUp.Contains(m.id))
                {
                    _listAttackSpeedUp.Remove(m.id);
                    
                    if(ActorProxy.isPlayingAI )
                    {
                        //KZSee:
                        //controller.ActionFlagOfWar(m.id, false, effect);
                    }
                }
            }
        }
    }
}
