using System;

namespace ED
{
    public class FixedPositionInstallation : Installation
    {
        private bool isDestroyRequested = false;

        public override void Initialize(bool pIsBottomPlayer)
        {
            base.Initialize(pIsBottomPlayer);
            isDestroyRequested = false;
        }

        protected void Update()
        {
            if (ActorProxy == null)
            {
                return;
            }

            var damageByTime = lifeTimeFactor * elapsedTime;
            var currentHealth = ActorProxy.currentHealth - damageByTime; 
            
            if (image_HealthBar != null)
            {
                image_HealthBar.fillAmount = currentHealth / ActorProxy.maxHealth;
            }

            if (currentHealth <= 0)
            {
                Destroy();
            }
        }

        private void Destroy()
        {
            if (isDestroyRequested)
            {
                return;
            }

            if (ActorProxy.isPlayingAI)
            {
                isDestroyRequested = true;
                ActorProxy.Destroy();
            }
        }
    }
}