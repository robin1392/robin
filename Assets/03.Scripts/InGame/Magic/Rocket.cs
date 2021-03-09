using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Rocket : Magic
    {
        public ParticleSystem ps_Tail;
        public ParticleSystem ps_BombEffect;

        private bool isBombed;

        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);

            isBombed = false;
            transform.localScale = Vector3.one * Mathf.Lerp(1f, 3f, (eyeLevel - 1) / 5f);
        }

        public override void SetTarget()
        {
            target = controller.targetPlayer;
            SetColor();
            StartCoroutine(Move());
        }

        protected override IEnumerator Move()
        {
            var startPos = transform.position;
            while (target == null) { yield return null; }
            var endPos = target.transform.position;
            var distance = Vector3.Distance(startPos, endPos);
            var moveTime = distance / moveSpeed;
            transform.LookAt(target.transform);

            float t = 0;

            while (t < moveTime)
            {
                if (target != null && target.isAlive)
                {
                    endPos = target.transform.position;
                    transform.position = Vector3.Lerp(startPos, target.transform.position, t / moveTime);
                }
                else
                {
                    transform.position = Vector3.Lerp(startPos, endPos, t / moveTime);
                }

                t += Time.deltaTime;
                yield return null;
            }

            if (isBombed == false)
            {
                isBombed = true;
                
                //if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount > 1 && isMine)
                if(InGameManager.IsNetwork && (isMine || controller.isPlayingAI))
                {
                    if (target != null)
                        controller.AttackEnemyMinionOrMagic(target.UID, target.id, power, 0f);
                    
                    //controller.HitMinionDamage( true , target.id , power, 0f);
                    //controller.targetPlayer.SendPlayer(RpcTarget.Others, E_PTDefine.PT_HITMINIONANDMAGIC, target.id, power, 0f);
                    //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_ROCKETBOMB, id);
                    controller.ActionRocketBomb(id);
                }
                //else if (PhotonNetwork.IsConnected == false)
                else if(InGameManager.IsNetwork == false)
                {
                    if (target != null)
                    {
                        controller.AttackEnemyMinionOrMagic(target.UID, target.id, power, 0f);
                        //controller.HitMinionDamage( true , target.id , power, 0f);
                    }

                    Bomb();
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (InGameManager.Get().isGamePlaying == false || destroyRoutine != null || isBombed) return;

            if (target != null && other.gameObject == target.gameObject || other.gameObject.layer == LayerMask.NameToLayer("Map"))
            {
                StopAllCoroutines();
                isBombed = true;

                //if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount > 1 && isMine)
                if(InGameManager.IsNetwork && (isMine || controller.isPlayingAI))
                {
                    if (target != null)
                        controller.AttackEnemyMinionOrMagic(target.UID, target.id, power, 0f);
                    
                    //controller.HitMinionDamage( true , target.id , power, 0f);
                    //controller.targetPlayer.SendPlayer(RpcTarget.Others , E_PTDefine.PT_HITMINIONANDMAGIC , target.id, power, 0f);
                    //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_ROCKETBOMB ,id);
                    controller.ActionRocketBomb(id);
                }
                //else if (PhotonNetwork.IsConnected == false)
                else if(InGameManager.IsNetwork == false)
                {
                    if (target != null)
                    {
                        controller.AttackEnemyMinionOrMagic(target.UID, target.id, power, 0f);
                        //controller.HitMinionDamage( true , target.id , power, 0f);
                        //controller.targetPlayer.HitDamageMinionAndMagic(target.id, power, 0f);
                    }

                    Bomb();
                }
            }
        }

        public void Bomb()
        {
            ps_Tail.Stop();
            ps_BombEffect.Play();

            Destroy(1.1f);
        }
    }
}