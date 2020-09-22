using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace ED
{
    public class Invincibility : Magic
    {
        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);

            if (isMine)
            {
                //controller.GetRandomMinion()?.Invincibility(1);
                for (int i = 0; i < eyeLevel; i++)
                {
                    var m = controller.GetRandomMinion();
                    if (m != null)
                    {
                        //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONINVINCIBILITY, m.id, effect);
                        // add nev
                    }
                }
            }
            
            Destroy();
        }

        public override void SetTarget() { }
    }
}
