namespace Quantum.RandomWars.Systems.Combat
{
    public class ProjectileCreationSystem
    {
        
    }
}

namespace Quantum.Actors
{
    public unsafe struct ProjectileCreationFilter
    {
        public EntityRef EntityRef;
        public ProjectileSpec* Spec;
        public ProjectileCreation* Creation;
    }

    public unsafe class ProjectileCreationSystem : SystemMainThreadFilter<ProjectileCreationFilter>
    {
        private static readonly string PROJECTILE_PROTOTYPE = "Resources/DB/EntityPrototypes/Projectile|EntityPrototype";
        
        public override void Update(Frame f, ref ProjectileCreationFilter filter)
        {
            if (f.IsVerified == false)
            {
                return;
            }

            var spec = filter.Spec;
            var prototype = f.FindAsset<EntityPrototype>(PROJECTILE_PROTOTYPE);
            var projectileCreation = f.Create(prototype);
            f.Add<ProjectileSpec>(projectileCreation);
            var projectile = f.Unsafe.GetPointer<ProjectileSpec>(projectileCreation);
            projectile->Owner = spec->Owner;
            projectile->Attacker = spec->Attacker;
            projectile->Defender = spec->Defender;
            projectile->Power = spec->Power;
            projectile->Team = spec->Team;
            projectile->Model = spec->Model;
            projectile->HitEffect = spec->HitEffect;
            projectile->Debuff = spec->Debuff;
            projectile->DebuffDuration = spec->DebuffDuration;
            projectile->HitTime = spec->HitTime;
            
            f.Destroy(filter.EntityRef);
        }
    }
}