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
            uint targetId = 0;
            if (target != null)
            {
                targetId = target.id;
            }
            PlayAnimationRelayServer(Client.Connection.Identity.NetId, aniHash, targetId);
        }
        
        [ServerRpc(requireAuthority = false)]
        public void PlayAnimationRelayServer(uint senderNetId, int aniHash, uint targetID)
        {
            foreach (var con in Server.connections)
            {
                if (senderNetId == con.Identity.NetId)
                {
                    continue;
                }

                RelayPlayAnimationOnClient(con, aniHash, targetID);
            }
        }

        [ClientRpc(target = Mirage.Client.Connection)]
        public void RelayPlayAnimationOnClient(INetworkConnection con, int aniHash, uint targetID)
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
            if (target != null) baseStat.transform.LookAt(target.transform);
            baseStat.animator.SetTrigger(hash);
        }
        
        
        void RelayFireBullet(E_BulletType arrow, BaseStat target, float f, float bulletMoveSpeed)
        {
            uint targetId = 0;
            if (target != null)
            {
                targetId = target.id;
            }
            
            RelayFireBulletOnServer(Client.Connection.Identity.NetId, arrow, targetId, f, bulletMoveSpeed);
        }
        
        [ServerRpc(requireAuthority = false)]
        public void RelayFireBulletOnServer(uint senderNetId, E_BulletType arrow, uint targetID, float f, float bulletMoveSpeed)
        {
            foreach (var con in Server.connections)
            {
                if (senderNetId == con.Identity.NetId)
                {
                    continue;
                }

                RelayFireBulletOnClient(con, arrow, targetID, f, bulletMoveSpeed);
            }
        }

        [ClientRpc(target = Mirage.Client.Connection)]
        public void RelayFireBulletOnClient(INetworkConnection con, E_BulletType arrow, uint targetID, float f, float bulletMoveSpeed)
        {
            var target = GetBaseStatWithNetId(targetID);
            if (target == null)
            {
                return;
            }
            
            FireBulletInternal(arrow, target, f, bulletMoveSpeed);
        }

        public void FireBulletWithRelay(E_BulletType bulletType, BaseStat target, float damage, float moveSpeed)
        {
            FireBulletInternal(bulletType, target, damage, moveSpeed);
            RelayFireBullet(bulletType, target, damage, moveSpeed);
        }

        void FireBulletInternal(E_BulletType bulletType, BaseStat target, float damage, float moveSpeed)
        {
            if (baseStat == null)
            {
                return;
            }

            Vector3 startPos = baseStat.ts_ShootingPos.position;
            Bullet bullet = null;
            switch (bulletType)
            {
                case E_BulletType.ARROW:
                    bullet = PoolManager.instance.ActivateObject<Bullet>("Arrow", startPos);
                    break;
                case E_BulletType.SPEAR:
                    bullet = PoolManager.instance.ActivateObject<Bullet>("Spear", startPos);
                    SoundManager.instance.Play(Global.E_SOUND.SFX_INGAME_MISSILE_SPEAR);
                    break;
                case E_BulletType.NECROMANCER:
                    bullet = PoolManager.instance.ActivateObject<Bullet>("Necromancer_Bullet", startPos);
                    break;
                case E_BulletType.MAGICIAN:
                    bullet = PoolManager.instance.ActivateObject<Bullet>("Magician_Bullet", startPos);
                    break;
                case E_BulletType.ARBITER:
                    bullet = PoolManager.instance.ActivateObject<Bullet>("Arbiter_Bullet", startPos);
                    break;
                case E_BulletType.BABYDRAGON:
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
            }

            if (bullet != null)
            {
                var rwClient = Client as RWNetworkClient;
                bullet.transform.rotation = Quaternion.identity;
                bullet.client = rwClient;
                bullet.moveSpeed = moveSpeed;
                bullet.Initialize(target, damage, 0, rwClient.IsPlayingAI, IsBottomCamp());
            }
        }

        public void SyncActionWithTarget(uint senderNetId, int actionTypeHash, uint targetNetId)
        {
            SyncActionWithTargetOnServer(senderNetId, actionTypeHash, targetNetId);
        }
        
        [ServerRpc(requireAuthority =  false)]
        public void SyncActionWithTargetOnServer(uint senderNetId, int actionTypeHash, uint targetNetId)
        {
            foreach (var con in Server.connections)
            {
                if (senderNetId == con.Identity.NetId)
                {
                    continue;
                }

                SyncActionWithTargetOnClient(con, actionTypeHash, targetNetId);
            }
        }
        
        [ClientRpc(target = Mirage.Client.Connection)]
        public void SyncActionWithTargetOnClient(INetworkConnection con, int actionTypeHash, uint targetNetId)
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

            baseStat.SyncActionWithTarget(actionTypeHash,this, target.GetComponent<ActorProxy>());
        }
        
        public void SyncActionWithoutTarget(uint senderNetId, int actionTypeHash)
        {
            SyncActionWithoutTargetOnServer(senderNetId, actionTypeHash);
        }
        
        [ServerRpc(requireAuthority =  false)]
        public void SyncActionWithoutTargetOnServer(uint senderNetId, int actionTypeHash)
        {
            foreach (var con in Server.connections)
            {
                if (senderNetId == con.Identity.NetId)
                {
                    continue;
                }

                SyncActionWithoutTargetOnClient(con, actionTypeHash);
            }
        }
        
        [ClientRpc(target = Mirage.Client.Connection)]
        public void SyncActionWithoutTargetOnClient(INetworkConnection con, int actionTypeHash)
        {
            if (baseStat == null)
            {
                return;
            }

            baseStat.SyncActionWithoutTarget(actionTypeHash,this);
        }
    }
}