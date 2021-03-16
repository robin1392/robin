using System;
using Mirage;

namespace ED
{
    public class Installation : Magic
    {
        protected float spawnTime;

        protected float elapsedTime
        {
            get
            {
                if (ActorProxy == null)
                {
                    return 0;
                }
                
                return (float)ActorProxy.NetworkTime.Time - spawnTime;        
            }
        }
        
        protected float lifeTimeFactor;

        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);
            spawnTime = ActorProxy.spawnTime;
            lifeTimeFactor = ActorProxy.maxHealth / InGameManager.Get().spawnTime;
        }
    }
}