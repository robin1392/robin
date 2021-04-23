using System;
using Mirage;

namespace ED
{
    public class Installation : Magic
    {
        
        protected float lifeTimeFactor;

        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);
            lifeTimeFactor = ActorProxy.maxHealth / magicLifeTime;
        }
    }
}