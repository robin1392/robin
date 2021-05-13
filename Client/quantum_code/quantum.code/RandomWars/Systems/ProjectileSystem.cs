using Photon.Deterministic;
using Quantum.Actors;

namespace Quantum
{
    public unsafe struct ProjectileFilter
    {
        public EntityRef Entity;
        public Projectile* Projectile;
        public ProjectileSpec* Spec;
    }
    
    public unsafe class ProjectileSystem : SystemMainThreadFilter<ProjectileFilter>
    {
        public override void Update(Frame f, ref ProjectileFilter filter)
        {
            var currentTime = f.DeltaTime * f.Number;
            var projectile = filter.Spec;
            if (projectile->HitTime <= currentTime)
            {
                if (f.Exists(filter.Spec->Defender))
                {
                    var defenderEntity = filter.Spec->Defender;
                    var defender = f.Unsafe.GetPointer<Health>(defenderEntity);
                    defender->Value -= filter.Spec->Power;
                    if (f.Has<NoCC>(defenderEntity) == false &&
                        projectile->Debuff == DebuffType.Frozen)
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

                f.Add<Destroy>(filter.Entity);
            }
        }
    }
}