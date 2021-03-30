using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ED
{
    public class Supply : Magic
    {
        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);

            if (isMine)
            {
                // controller.AddSp((int)power);
            }
            
            //KZSee:
           // Destroy();
        }
    }
}
