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
                controller.GetRandomMinion().Invincibility(1);
            }
            
            Destroy();
        }

        public override void SetTarget() { }
    }
}
