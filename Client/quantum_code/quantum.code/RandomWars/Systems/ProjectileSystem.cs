using Photon.Deterministic;
using Quantum.Actors;

namespace Quantum
{
    public unsafe struct ProjectileFilter
    {
        public EntityRef Entity;
        public Projectile* Projectile;
    }
    
    public unsafe class ProjectileSystem : SystemMainThreadFilter<ProjectileFilter>
    {
        public override void Update(Frame f, ref ProjectileFilter filter)
        {
            if (f.IsVerified == false)
            {
                return;
            }
            
            var currentTime = f.DeltaTime * f.Number;
            var projectile = filter.Projectile;
            if (projectile->HitTime <= currentTime)
            {
                if (f.Exists(filter.Projectile->Defender))
                {
                    var defenderEntity = filter.Projectile->Defender;
                    var defender = f.Unsafe.GetPointer<Hittable>(defenderEntity);
                    defender->Health -= filter.Projectile->Power;
                    if (f.Has<NoCC>(defenderEntity) == false &&
                        projectile->Debuff == DebuffType.Freeze)
                    {
                        if (f.Has<Frozen>(defenderEntity) == false)
                        {
                            f.Add<Frozen>(defenderEntity);
                        }
                        
                        var freeze = f.Unsafe.GetPointer<Frozen>(defenderEntity);
                        var endTimePrev = freeze->EndTime;
                        var endTime = currentTime + projectile->DebuffDuration;
                        if (endTimePrev < endTime)
                        {
                            freeze->EndTime = endTime;
                        }
                    }
                }
                
                f.Destroy(filter.Entity);
            }
        }
    }
}