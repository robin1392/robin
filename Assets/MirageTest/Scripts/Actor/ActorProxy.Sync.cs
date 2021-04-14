using ED;
using Mirage;
using UnityEngine;
using Debug = ED.Debug;

namespace MirageTest.Scripts
{
    public partial class ActorProxy 
    {
        void RelayPlayAnimation(int aniHash, BaseStat target)
        {
            if (Client.IsConnected == false)
            {
                return;
            }
            
            uint targetId = 0;
            if (target != null)
            {
                targetId = target.id;
            }
            PlayAnimationRelayServer(Client.Player.Identity.NetId, aniHash, targetId);
        }
        
        [ServerRpc(requireAuthority = false)]
        public void PlayAnimationRelayServer(uint senderNetId, int aniHash, uint targetID)
        {
            foreach (var player in Server.Players)
            {
                if (player == null)
                {
                    continue;
                }

                if (player.Identity == null)
                {
                    continue;
                }
                
                if (senderNetId == player.Identity.NetId)
                {
                    continue;
                }

                RelayPlayAnimationOnClient(player, aniHash, targetID);
            }
        }

        [ClientRpc(target = Mirage.Client.Player)]
        public void RelayPlayAnimationOnClient(INetworkPlayer con, int aniHash, uint targetID)
        {
            var target = GetBaseStatWithNetId(targetID);
            if (target == null)
            {
                return;
            }
            
            PlayAnimationInternal(aniHash, target);
        }
        
        public void PlayAnimationWithRelay(int hash, BaseStat target)
        {
            PlayAnimationInternal(hash, target);
            RelayPlayAnimation(hash, target);
        }
        
        void PlayAnimationInternal(int hash, BaseStat target)
        {
            if (target != null) transform.LookAt(target.ActorProxy.transform);

            baseStat.animator.SetFloat(AnimationHash.MoveSpeed, 0);
            baseStat.animator.SetTrigger(hash);
        }
        
        public void FireBulletWithRelay(E_BulletType bulletType, BaseStat target, float damage, float moveSpeed)
        {
            FireBulletInternal(bulletType, target, damage, moveSpeed);
            RelayFireBullet(bulletType, target, damage, moveSpeed);
        }
        
        void RelayFireBullet(E_BulletType arrow, BaseStat target, float f, float bulletMoveSpeed)
        {
            if (Client.IsConnected == false)
            {
                return;
            }
            
            uint targetId = 0;
            if (target != null)
            {
                targetId = target.id;
            }
            
            RelayFireBulletOnServer(Client.Player.Identity.NetId, arrow, targetId, f, bulletMoveSpeed);
        }
        
        [ServerRpc(requireAuthority = false)]
        public void RelayFireBulletOnServer(uint senderNetId, E_BulletType arrow, uint targetID, float f, float bulletMoveSpeed)
        {
            foreach (var player in Server.Players)
            {
                if (player == null)
                {
                    continue;
                }

                if (player.Identity == null)
                {
                    continue;
                }
                
                if (senderNetId == player.Identity.NetId)
                {
                    continue;
                }

                RelayFireBulletOnClient(player, arrow, targetID, f, bulletMoveSpeed);
            }
        }

        [ClientRpc(target = Mirage.Client.Player)]
        public void RelayFireBulletOnClient(INetworkPlayer con, E_BulletType arrow, uint targetID, float f, float bulletMoveSpeed)
        {
            var target = GetBaseStatWithNetId(targetID);
            if (target == null)
            {
                return;
            }
            
            FireBulletInternal(arrow, target, f, bulletMoveSpeed);
        }
        
        public void FireBulletWithRelay(E_BulletType bulletType, BaseStat target, float damage, float moveSpeed, float effect)
        {
            FireBulletInternal(bulletType, target, damage, moveSpeed, effect);
            RelayFireBulletWithEffect(bulletType, target, damage, moveSpeed, effect);
        }

        void RelayFireBulletWithEffect(E_BulletType arrow, BaseStat target, float f, float bulletMoveSpeed, float effect)
        {
            uint targetId = 0;
            if (target != null)
            {
                targetId = target.id;
            }
            
            RelayFireBulletWithEffectOnServer(Client.Player.Identity.NetId, arrow, targetId, f, bulletMoveSpeed, effect);
        }
        
        [ServerRpc(requireAuthority = false)]
        public void RelayFireBulletWithEffectOnServer(uint senderNetId, E_BulletType arrow, uint targetID, float f, float bulletMoveSpeed, float effect)
        {
            foreach (var player in Server.Players)
            {
                if (player == null)
                {
                    continue;
                }

                if (player.Identity == null)
                {
                    continue;
                }
                
                if (senderNetId == player.Identity.NetId)
                {
                    continue;
                }

                RelayFireBulletWithEffectOnClient(player, arrow, targetID, f, bulletMoveSpeed, effect);
            }
        }

        [ClientRpc(target = Mirage.Client.Player)]
        public void RelayFireBulletWithEffectOnClient(INetworkPlayer con, E_BulletType arrow, uint targetID, float f, float bulletMoveSpeed, float effect)
        {
            var target = GetBaseStatWithNetId(targetID);
            if (target == null)
            {
                return;
            }
            
            FireBulletInternal(arrow, target, f, bulletMoveSpeed, effect);
        }

        void FireBulletInternal(E_BulletType bulletType, BaseStat target, float damage, float moveSpeed, float effect = 0)
        {
            if (baseStat == null)
            {
                return;
            }

            Vector3 startPos = baseStat.ts_ShootingPos.position;
            Bullet bullet = null;
            switch (bulletType)
            {
                case E_BulletType.ARROW_BULLET:
                    bullet = PoolManager.instance.ActivateObject<Bullet>("Arrow", startPos);
                    break;
                case E_BulletType.SPEAR_BULLET:
                    bullet = PoolManager.instance.ActivateObject<Bullet>("Spear", startPos);
                    SoundManager.instance.Play(Global.E_SOUND.SFX_INGAME_MISSILE_SPEAR);
                    break;
                case E_BulletType.NECROMANCER_BULLET:
                    bullet = PoolManager.instance.ActivateObject<Bullet>("Necromancer_Bullet", startPos);
                    break;
                case E_BulletType.MAGICIAN_BULLET:
                    bullet = PoolManager.instance.ActivateObject<Bullet>("Magician_Bullet", startPos);
                    break;
                case E_BulletType.ARBITER_BULLET:
                    bullet = PoolManager.instance.ActivateObject<Bullet>("Arbiter_Bullet", startPos);
                    break;
                case E_BulletType.BABYDRAGON_BULLET:
                    bullet = PoolManager.instance.ActivateObject<Bullet>("Babydragon_Bullet", startPos);
                    break;
                case E_BulletType.VALLISTA_SPEAR:
                    bullet = PoolManager.instance.ActivateObject<Bullet>("Vallista_Spear", startPos);
                    break;
                case E_BulletType.GUARDIAN3_BULLET:
                    bullet = PoolManager.instance.ActivateObject<Bullet>("Guardian3_Bullet", startPos);
                    break;
                case E_BulletType.POSU_BULLET:
                    bullet = PoolManager.instance.ActivateObject<Bullet>("Posu_Bullet", startPos);
                    break;
                case E_BulletType.TURRET_BULLET:
                    bullet = PoolManager.instance.ActivateObject<Bullet>("Turret_Bullet", startPos);
                    break;
                case E_BulletType.BOSS5_BULLET:
                    bullet = PoolManager.instance.ActivateObject<Bullet>("Boss_05_Bullet", startPos);
                    break;
                case E_BulletType.ICE_NORMAL_BULLET:
                case E_BulletType.ICE_FREEZE_BULLET:
                    bullet = PoolManager.instance.ActivateObject<Bullet>("Ice_Bullet", startPos);
                    break;
                case E_BulletType.WIND_BULLET:
                    bullet = PoolManager.instance.ActivateObject<Bullet>("Wind_Bullet", startPos);
                    break;
            }

            if (bullet != null)
            {
                var rwClient = Client as RWNetworkClient;
                bullet.transform.rotation = Quaternion.identity;
                bullet.client = rwClient;
                bullet.moveSpeed = moveSpeed;
                bullet.Initialize(bulletType, target, damage, 0, rwClient.IsPlayingAI, IsBottomCamp(), effect);
            }
        }
        
        public void FireCannonBallWithRelay(E_CannonType cannonType, Vector3 targetPosition)
        {
            FireCannonBallInternal(cannonType, targetPosition);
            RelayFireCannonBall(cannonType, targetPosition);
        }
        
        void RelayFireCannonBall(E_CannonType bullet, Vector3 targetPosition)
        {
            RelayFireCannonBallOnServer(Client.Player.Identity.NetId, bullet, targetPosition);
        }
        
        [ServerRpc(requireAuthority = false)]
        public void RelayFireCannonBallOnServer(uint senderNetId, E_CannonType cannonType, Vector3 targetPosition)
        {
            foreach (var player in Server.Players)
            {
                if (player == null)
                {
                    continue;
                }

                if (player.Identity == null)
                {
                    continue;
                }
                
                if (senderNetId == player.Identity.NetId)
                {
                    continue;
                }

                RelayFireCannonBallOnClient(player, cannonType, targetPosition);
            }
        }

        [ClientRpc(target = Mirage.Client.Player)]
        public void RelayFireCannonBallOnClient(INetworkPlayer con, E_CannonType cannonType, Vector3 targetPosition)
        {
            FireCannonBallInternal(cannonType, targetPosition);
        }

        void FireCannonBallInternal(E_CannonType cannonType, Vector3 targetPosition)
        {
            if (baseStat == null)
            {
                return;
            }

            Vector3 startPos = baseStat.ts_ShootingPos.position;
            CannonBall cannonBall = null;
            switch (cannonType)
            {
                case E_CannonType.DEFAULT:
                    cannonBall = PoolManager.instance.ActivateObject<CannonBall>("CannonBall", startPos);
                    SoundManager.instance.Play(Global.E_SOUND.SFX_INGAME_MORTAR_SHOT);
                    SoundManager.instance.Play(Global.E_SOUND.SFX_INGAME_MORTAR_MISSILE);
                    break;
                case E_CannonType.BOMBER:
                    cannonBall = PoolManager.instance.ActivateObject<CannonBall>("Bomber_Bullet", startPos);
                    break;
            }

            if (cannonBall != null)
            {
                cannonBall.transform.rotation = Quaternion.identity;
                cannonBall.client = Client as RWNetworkClient;
                cannonBall.Initialize(targetPosition, power, baseStat.range, IsLocalPlayerActor, IsBottomCamp());
            }
        }

        public void SyncActionWithTarget(uint senderNetId, string actionTypeHash, uint targetNetId)
        {
            SyncActionWithTargetOnServer(senderNetId, actionTypeHash, targetNetId);
        }
        
        [ServerRpc(requireAuthority =  false)]
        public void SyncActionWithTargetOnServer(uint senderNetId, string actionTypeHash, uint targetNetId)
        {
            foreach (var player in Server.Players)
            {
                if (player == null)
                {
                    continue;
                }

                if (player.Identity == null)
                {
                    continue;
                }
                
                if (senderNetId == player.Identity.NetId)
                {
                    continue;
                }

                SyncActionWithTargetOnClient(player, actionTypeHash, targetNetId);
            }
        }
        
        [ClientRpc(target = Mirage.Client.Player)]
        public void SyncActionWithTargetOnClient(INetworkPlayer con, string actionTypeHash, uint targetNetId)
        {
            if (baseStat == null)
            {
                return;
            }

            var target = ClientObjectManager[targetNetId];
            if (target == null)
            {
                return;
            }

            baseStat.SyncActionWithTarget(actionTypeHash, target.GetComponent<ActorProxy>());
        }
        
        public void SyncActionWithoutTarget(uint senderNetId, string actionTypeHash)
        {
            SyncActionWithoutTargetOnServer(senderNetId, actionTypeHash);
        }
        
        [ServerRpc(requireAuthority =  false)]
        public void SyncActionWithoutTargetOnServer(uint senderNetId, string actionTypeHash)
        {
            foreach (var player in Server.Players)
            {
                if (player == null)
                {
                    continue;
                }

                if (player.Identity == null)
                {
                    continue;
                }
                
                if (senderNetId == player.Identity.NetId)
                {
                    continue;
                }

                SyncActionWithoutTargetOnClient(player, actionTypeHash);
            }
        }
        
        [ClientRpc(target = Mirage.Client.Player)]
        public void SyncActionWithoutTargetOnClient(INetworkPlayer con, string actionTypeHash)
        {
            if (baseStat == null)
            {
                return;
            }

            baseStat.SyncActionWithoutTarget(actionTypeHash);
        }

        public void SyncMultiTarget(uint senderNetId, uint[] targetNetIds)
        {
            SyncMultiTargetOnServer(senderNetId, targetNetIds);
        }

        [ServerRpc(requireAuthority = false)]
        public void SyncMultiTargetOnServer(uint senderNetId, uint[] targetNetIds)
        {
            foreach (var player in Server.Players)
            {
                if (player == null)
                {
                    continue;
                }

                if (player.Identity == null)
                {
                    continue;
                }
                
                if (senderNetId == player.Identity.NetId)
                {
                    continue;
                }

                SyncMultiTargetOnClient(player, targetNetIds);
            }
        }
        
        [ClientRpc(target = Mirage.Client.Player)]
        public void SyncMultiTargetOnClient(INetworkPlayer player, uint[] targetNetIds)
        {
            if (baseStat == null)
            {
                return;
            }

            ((Minion_Layzer)baseStat).SetTargetList(targetNetIds);
        }
    }
}