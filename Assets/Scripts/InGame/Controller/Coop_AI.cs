#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace ED
{
    public class Coop_AI : BaseStat, IPunObservable
    {
        protected override void Start()
        {
            Debug.Log("COOP_AI instantiated !!");
            currentHealth = maxHealth;
            InGameManager.Get().AddPlayerUnit(false, this);
        }

        //[PunRPC]
        public override void HitDamage(float damage)
        {
            if (currentHealth > 0)
            {
                currentHealth -= damage;

                if (currentHealth <= 0)
                {
                    Death();
                }
            }

            RefreshHealthBar();
        }

        public void Death()
        {
            //InGameManager.Get().photonView.RPC("EndGame", RpcTarget.All);
            InGameManager.Get().SendBattleManager(RpcTarget.All , E_PTDefine.PT_ENDGAME);

            if (photonView.IsMine)
            {
                PhotonNetwork.Destroy(photonView);
            }
        }

        public void RefreshHealthBar()
        {
            image_HealthBar.fillAmount = currentHealth / maxHealth;
        }

        public void Spawn()
        {
            //float magicCastDelay = 0.1f;
            //for (int i = 0; i < arrDice.Length; i++)
            //{
            //    if (arrDice[i].type != DICE_TYPE.NONE)
            //    {
            //        Transform ts = transform.parent.GetChild(i);

            //        switch (arrDice[i].data.castType)
            //        {
            //            case DICE_CAST_TYPE.MINION:
            //                arrDice[i].SpawnMinion(ts.position, isBottomPlayer);
            //                break;
            //            case DICE_CAST_TYPE.MAGIC:
            //                for (int j = 0; j < ((arrDice[i].level + 1) * arrDice[i].data.spawnMultiply); j++)
            //                {
            //                    StartCoroutine(CastMagicCoroutine(arrDice[i].data, magicCastDelay));
            //                    magicCastDelay += 0.1f;
            //                }
            //                break;
            //            default:
            //                break;
            //        }
            //    }
            //}
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            
        }
    }
}