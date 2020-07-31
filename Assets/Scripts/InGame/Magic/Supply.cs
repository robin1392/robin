using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Supply : Magic
    {
        public override void Initialize(bool pIsBottomPlayer, float pDamage, float pMoveSpeed = 1)
        {
            base.Initialize(pIsBottomPlayer, pDamage, pMoveSpeed);

            if (isMine)
            {
                controller.AddSp((int)pDamage);
            }
            
            Destroy();
        }

        public override void SetTarget() { }
    }
}
