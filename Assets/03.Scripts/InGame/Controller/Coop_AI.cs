#if UNITY_EDITOR
#define ENABLE_LOG
#endif

using System;
using System.Collections;
using UnityEngine;

namespace ED
{
    public class Coop_AI : PlayerController
    {
        [Header("Egg")] public Transform ts_EggParent;

        public GameObject obj_IncubationParticle;
        public GameObject obj_SummonParticle;

        //private MsgMonster msgBoss;
        

        protected override void StartPlayerControll()
        {
            // throw new NotImplementedException();
            // // _myUID = NetworkManager.Get().CoopUID;
            // // id = myUID * 10000;
            //
            // NetworkManager.Get().event_OtherPause.AddListener(OtherPlayerPauseAI);
            // if (isMine)
            // {
            //     StartCoroutine(HitDamageQueueCoroutine());
            // }
            //
            // isHalfHealth = false;
            //
            // if (InGameManager.IsNetwork == false)
            //     sp = 200;
            //
            // maxHealth = ConvertNetMsg.MsgIntToFloat(NetworkManager.Get().GetNetInfo().coopInfo.TowerHp);
            //
            // currentHealth = maxHealth;
            // RefreshHealthBar();
            //
            // InGameManager.Get().AddPlayerUnit(isBottomPlayer, this);
            //
            // SetColor(isBottomPlayer ? E_MaterialType.BOTTOM : E_MaterialType.TOP);
            //
            // StartSyncMinion();
        }

        private void OtherPlayerPauseAI(bool isPause)
        {
            // SetPlayingAI(isPause);
        }

        public override void SetColor(E_MaterialType type, bool isAlly)
        {
        }

        private void DestroyEgg()
        {
            animator.SetTrigger(AnimationHash.Idle);
            if (ts_EggParent.childCount > 0)
            {
                DestroyImmediate(ts_EggParent.GetChild(0).gameObject);
            }

            obj_IncubationParticle.SetActive(false);
        }
    }
}