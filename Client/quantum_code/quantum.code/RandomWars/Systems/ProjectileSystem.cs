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
            if (filter.Projectile->HitTime <= currentTime)
            {
                if (f.Exists(filter.Projectile->Defender))
                {
                    var defender = f.Unsafe.GetPointer<Actor>(filter.Projectile->Defender);
                    defender->Health -= filter.Projectile->Power;
                }
                
                f.Destroy(filter.Entity);
            }
        }
    }
}