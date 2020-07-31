using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Invincibility : Magic
    {
        public override void Initialize(bool pIsBottomPlayer, float pDamage, float pMoveSpeed = 1)
        {
            base.Initialize(pIsBottomPlayer, pDamage, pMoveSpeed);

            if (isMine)
            {
                controller.GetRandomMinion().Invincibility(1);
            }
            
            Destroy();
        }

        public override void SetTarget() { }
    }
}
