using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace ED
{
    public class Iceball : Magic
    {
        public ParticleSystem ps_Tail;
        public ParticleSystem ps_BombEffect;

        private float sturnTime => effect + effectDuration * eyeLevel;
        private bool isBombed = false;

        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);

            isBombed = false;
            transform.localScale = Vector3.one * Mathf.Lerp(1f, 1.5f, (eyeLevel - 1) / 5f);
        }

        public override void SetTarget()
        {
            StartCoroutine(Move());
        }

        protected override IEnumerator Move()
        {
            var startPos = transform.position;
            while (target == null)
            {
                yield return null;
                
                target = InGameManager.Get().GetRandomPlayerUnitHighHealth(!isBottomPlayer);
                if (target != null)
                {
                    //controller.SendPlayer(RpcTarget.Others, E_PTDefine.PT_SETMAGICTARGET, id, target.id);
                    controller.ActionSetMagicTarget(id, target.id);
                }
            }
            var endPos = target.ts_HitPos.position;
            var distance = Vector3.Distance(startPos, endPos);
            var moveTime = distance / moveSpeed;
            var moveDistance = 0f;

            float t = 0;
            
            while (true)
            {
                if (target != null && target.isAlive)
                {
                    endPos = target.ts_HitPos.position;
                }
                //rb.position = Vector3.Lerp(startPos, endPos, t / moveTime);
                rb.position += (endPos - transform.position).normalized * (moveSpeed * Time.deltaTime);

                if (Vector3.Distance(transform.position, endPos) < 0.4f)
                {
                    break;
                }

                t += Time.deltaTime;
                yield return null;
            }

            if ((InGameManager.IsNetwork && isMine || InGameManager.IsNetwork == false || controller.isPlayingAI) && isBombed == false)
            {
                isBombed = true;
                
                //if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount > 1 && isMine)
                if(InGameManager.IsNetwork && isMine)
                {
                    if (target != null)
                    {

                        controller.AttackEnemyMinionOrMagic(target.id, power, 0f);
                        //controller.AttackEnemyMinion(target.id, power, 0f);
                        //controller.targetPlayer.SendPlayer(RpcTarget.All, E_PTDefine.PT_STURNMINION, target.id, sturnTime);
                        controller.ActionSturn(true , target.id , sturnTime);
                    }

                    //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_ICEBALLBOMB, id);
                    controller.ActionIceBallBomb(id);
                }
                //else if (PhotonNetwork.IsConnected == false)
                else if(InGameManager.IsNetwork == false || controller.isPlayingAI)
                {
                    if (target != null)
                    {
                        controller.AttackEnemyMinionOrMagic(target.id, power, 0f);
                        controller.targetPlayer.SturnMinion(target.id, sturnTime);
                    }

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
                isBombed = true;
                rb.velocity = Vector3.zero;

                //if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount > 1 && isMine)
                if(InGameManager.IsNetwork && isMine)
                {
                    if (target != null)
                    {
                        controller.AttackEnemyMinionOrMagic(target.id, power, 0f);
                        //controller.targetPlayer.SendPlayer(RpcTarget.All , E_PTDefine.PT_STURNMINION , target.id, sturnTime);
                        controller.ActionSturn(true , target.id , sturnTime);
                    }
                    
                    //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_ICEBALLBOMB , id);
                    controller.ActionIceBallBomb(id);
                }
                //else if (PhotonNetwork.IsConnected == false)
                else if(InGameManager.IsNetwork == false || controller.isPlayingAI)
                {
                    if (target != null)
                    {
                        controller.AttackEnemyMinionOrMagic(target.id, power, 0f);
                        controller.targetPlayer.SturnMinion(target.id, sturnTime);
                    }

                    Bomb();
                }
            }
        }

        public void Bomb()
        {
            rb.velocity = Vector3.zero;
            ps_Tail.Stop();
            ps_BombEffect.Play();

            Destroy(1.1f);
        }
    }
}
