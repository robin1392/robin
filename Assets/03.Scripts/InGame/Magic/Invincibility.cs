using System.Collections;
using System.Collections.Generic;
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
                for (int i = 0; i < eyeLevel; i++)
                {
                    //KZSee:
                    // var m = controller.GetRandomMinion();
                    // if (m != null)
                    // {
                    //     //controller.SendPlayer(RpcTarget.All, E_PTDefine.PT_MINIONINVINCIBILITY, m.id, effect);
                    //     // add nev
                    //     controller.ActionInvincibility(m.id, effect);
                    // }
                }
            }
            
            //Destroy();
        }
    }
}
