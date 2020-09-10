using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace  ED
{
    public class Minion_Magician : Minion
    {
        public float bulletMoveSpeed = 6f;
        
        [SerializeField] private readonly float _skillCooltime = 10f;
        private int _skillCastedCount;

        public override void Initialize(DestroyCallback destroy)
        {
            base.Initialize(destroy);
            _skillCastedCount = 0;
        }

        public override void Attack()
        {
            if (target == null) return;
            
            //if (PhotonNetwork.IsConnected && isMine)
            if( InGameManager.Get().IsNetwork() && isMine )
            {
                base.Attack();
                //controller.SendPlayer(RpcTarget.All , E_PTDefine.PT_MINIONANITRIGGER , id , "Attack");
                controller.MinionAniTrigger(id, "Attack");
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.Get().IsNetwork() == false )
            {
                base.Attack();
                animator.SetTrigger(_animatorHashAttack);
            }
        }

        public void FireArrow()
        {
            //if (PhotonNetwork.IsConnected && isMine)
            if( InGameManager.Get().IsNetwork() && isMine )
            {
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_FIREARROW , ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
                controller.ActionFireArrow(ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
            }
            //else if (PhotonNetwork.IsConnected == false)
            else if(InGameManager.Get().IsNetwork() == false )
            {
                controller.FireArrow(ts_ShootingPos.position, target.id, power, bulletMoveSpeed);
            }
        }

        public void Skill()
        {
            if (_spawnedTime >= _skillCooltime * _skillCastedCount)
            {
                Polymorph();
            }
        }

        private void Polymorph()
        {
            var cols = Physics.OverlapSphere(transform.position, searchRange, targetLayer);
            var list = new List<int>();

            foreach (var col in cols)
            {
                var bs = col.GetComponentInParent<BaseStat>();
                if (bs != null && bs.id > 0 && bs.isFlying == false && ((Minion)bs).isPolymorph == false)
                {
                    list.Add(bs.id);
                }
            }

            if (list.Count > 0)
            {
                _skillCastedCount++;
                //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONANITRIGGER, id, "Skill");
                controller.MinionAniTrigger(id, "Skill");
                
                controller.targetPlayer.SendPlayer(RpcTarget.All,
                    E_PTDefine.PT_SCARECROW,
                    list[Random.Range(0, list.Count)],
                    (float) eyeLevel);
            }
        }
    }
}
